#include "pch.h"
#include "TempInspectHikCam.h"

CTempInspectHikCam::CTempInspectHikCam(ITempInspectHikCamToParent* pInterface)
{
	m_pInterface = pInterface;

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_nCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

CTempInspectHikCam::~CTempInspectHikCam()
{
	Destroy();
}

BOOL CTempInspectHikCam::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_INSP_COUNT];

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		CameraInfo* pCamInfo = m_pInterface->GetRecipe(i)->GetCameraInfos();

		USES_CONVERSION;
		char cSensorType[10] = {};
		sprintf_s(cSensorType, "%s", W2A(pCamInfo->m_csSensorType));


		int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
		int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

		grabberParam[i].SetParam_GrabberPort(pCamInfo->m_csSerialNumber);
		grabberParam[i].SetParam_FrameWidth(pCamInfo->m_nFrameWidth);
		grabberParam[i].SetParam_FrameHeight(pCamInfo->m_nFrameHeight);
		grabberParam[i].SetParam_FrameWidthStep(pCamInfo->m_nFrameWidth);
		grabberParam[i].SetParam_FrameDepth(nFrameDepth);
		grabberParam[i].SetParam_FrameChannels(nFrameChannels);
		grabberParam[i].SetParam_FrameCount(MAX_FRAME_COUNT);

		// Buffer
		if (m_pCameraImageBuffer[i] != NULL)
		{
			m_pCameraImageBuffer[i]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[i];
			m_pCameraImageBuffer[i] = NULL;
		}

		DWORD dwFrameWidth = (DWORD)pCamInfo->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)pCamInfo->m_nFrameHeight;
		DWORD dwFrameCount = MAX_FRAME_COUNT;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nFrameChannels;

		m_pCameraImageBuffer[i] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pCameraImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pCameraImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pCameraImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferHik", i);
		m_pCameraImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		// Camera
		m_nCamera[i] = new CFrameGrabber_MVS_GigE(i, this);

		int nRetryCount = 1;
		BOOL bCamConnection = FALSE;

		while (bCamConnection == FALSE)
		{
			if (m_nCamera[i]->Connect(grabberParam[i]) != 1)
				m_bCamera_ConnectStatus[i] = FALSE;

			/*if (m_nCamera[i]->StartGrab() != 1)
				m_bCamera_ConnectStatus[i] = FALSE;*/
			else
				m_bCamera_ConnectStatus[i] = TRUE;

			if (m_bCamera_ConnectStatus[i] == TRUE)
				bCamConnection = TRUE;
			else if (10 < nRetryCount)
				bCamConnection = TRUE;
			else
			{
				CString strMsg;
				strMsg.Format(_T("Align Cam %d Connection Fail.. Retry %d.."), i, nRetryCount++);
				Sleep(3000);
			}
		}


	}

	return TRUE;
}

BOOL CTempInspectHikCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_nCamera[i] != NULL)
		{
			m_nCamera[i]->StopGrab();
			Sleep(1000);
			m_nCamera[i]->Disconnect();
			delete m_nCamera[i], m_nCamera[i] = NULL;
		}

		if (m_pCameraImageBuffer[i] != NULL)
		{
			m_pCameraImageBuffer[i]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[i], m_pCameraImageBuffer[i] = NULL;
		}
	}

	return TRUE;
}

BOOL CTempInspectHikCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CTempInspectHikCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx);
}

BOOL CTempInspectHikCam::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return FALSE;

	if (pBuffer == NULL)
		return FALSE;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return FALSE;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	int nWidth = m_pCameraImageBuffer[nCamIdx]->GetFrameWidth();
	int nHeight = m_pCameraImageBuffer[nCamIdx]->GetFrameHeight();

	memcpy(pBuffer, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx), nWidth * nHeight);

	localLock.Unlock();

	return TRUE;
}

BOOL CTempInspectHikCam::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return FALSE;

	if (pBuffer == NULL)
		return FALSE;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return FALSE;

	int nBeforeFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	DWORD tickCount = GetTickCount();

	BOOL bGrabTimeOver = FALSE;

	while (1)
	{
		if (10000 < GetTickCount() - tickCount)
		{
			bGrabTimeOver = TRUE;
			break;
		}

		if (nBeforeFrameIdx != m_pCameraCurrentFrameIdx[nCamIdx])
			break;

		Sleep(1);
	}

	if (bGrabTimeOver == TRUE)
		return FALSE;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	int nWidth = m_pCameraImageBuffer[nCamIdx]->GetFrameWidth();
	int nHeight = m_pCameraImageBuffer[nCamIdx]->GetFrameHeight();

	memcpy(pBuffer, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx), nWidth * nHeight);

	localLock.Unlock();

	return TRUE;
}

CSharedMemoryBuffer* CTempInspectHikCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

LPBYTE CTempInspectHikCam::GetResultBufferImage(int nPosIdx)
{
	/*if (nPosIdx < 0 || MAX_CAMERA_INSP_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;*/
	return nullptr;
}

int CTempInspectHikCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (nGrabberIndex < 0 || MAX_CAMERA_INSP_COUNT <= nGrabberIndex)
		return -1;

	if (pBuffer == NULL)
		return -1;

	if (m_pCameraImageBuffer[nGrabberIndex] == NULL)
		return -1;

	CSingleLock localLock(&m_csCameraFrameIdx[nGrabberIndex]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nGrabberIndex];

	int nNextFrameIdx = nCurrentFrameIdx + 1;

	nNextFrameIdx = nNextFrameIdx % MAX_FRAME_COUNT;

	m_pCameraImageBuffer[nGrabberIndex]->SetFrameImage(nNextFrameIdx, (LPBYTE)pBuffer);

	if (m_pInterface->GetTempInspectStatus(nGrabberIndex)->GetInspectRunning())
		m_pReceivedImgCallback((LPBYTE)pBuffer, nGrabberIndex, nNextFrameIdx, m_pParam);

	m_pCameraCurrentFrameIdx[nGrabberIndex] = nNextFrameIdx;

	localLock.Unlock();

	return 1;
}

int CTempInspectHikCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}

int CTempInspectHikCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StartGrab();
}

int CTempInspectHikCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StopGrab();
}

int CTempInspectHikCam::SingleGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	int nMode, nSource;
	m_nCamera[nCamIdx]->GetTriggerMode(nMode);
	m_nCamera[nCamIdx]->GetTriggerSource(nSource);
	if (nMode == 0 && nSource == 1)
		return 0;

	return m_nCamera[nCamIdx]->SendTrigger();
}

int CTempInspectHikCam::SetTriggerMode(int nCamIdx, int nMode)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	m_nCamera[nCamIdx]->SetTriggerMode(nMode);
}

int CTempInspectHikCam::SetTriggerSource(int nCamIdx, int nSource)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	m_nCamera[nCamIdx]->SetTriggerSource(nSource);
}

void CTempInspectHikCam::RegisterReceivedImageCallback(ReceivedImageCallback* callback, LPVOID pParam)
{
	m_pParam = pParam;
	m_pReceivedImgCallback = callback;
}
