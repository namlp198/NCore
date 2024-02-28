#include "pch.h"
#include "InspectionHikCam.h"

InspectionHikCam::InspectionHikCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_nCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

InspectionHikCam::~InspectionHikCam()
{
	Destroy();
}

BOOL InspectionHikCam::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_COUNT];
	grabberParam[0].SetParam_GrabberPort((CString)_T("02746463021"));
	grabberParam[0].SetParam_FrameWidth(1280);
	grabberParam[0].SetParam_FrameHeight(1024);
	grabberParam[0].SetParam_FrameWidthStep(1280);
	grabberParam[0].SetParam_FrameDepth(8);
	grabberParam[0].SetParam_FrameChannels(1);
	grabberParam[0].SetParam_FrameCount(MAX_BUFFER_FRAME);

	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		// Buffer
		if (m_pCameraImageBuffer[i] != NULL)
		{
			m_pCameraImageBuffer[i]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[i];
			m_pCameraImageBuffer[i] = NULL;
		}

		DWORD dwFrameWidth = (DWORD)1280;
		DWORD dwFrameHeight = (DWORD)1024;
		DWORD dwFrameCount = MAX_BUFFER_FRAME;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight;

		m_pCameraImageBuffer[i] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pCameraImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pCameraImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pCameraImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferOnline", i);
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

BOOL InspectionHikCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
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

BOOL InspectionHikCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE InspectionHikCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx);
}

BOOL InspectionHikCam::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
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

BOOL InspectionHikCam::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
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

CSharedMemoryBuffer* InspectionHikCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

LPBYTE InspectionHikCam::GetResultBufferImage(int nPosIdx)
{
	if (nPosIdx < 0 || MAX_POSITION_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;
}

int InspectionHikCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (nGrabberIndex < 0 || MAX_CAMERA_COUNT <= nGrabberIndex)
		return -1;

	if (pBuffer == NULL)
		return -1;

	if (m_pCameraImageBuffer[nGrabberIndex] == NULL)
		return -1;

	CSingleLock localLock(&m_csCameraFrameIdx[nGrabberIndex]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nGrabberIndex];

	int nNextFrameIdx = nCurrentFrameIdx + 1;

	nNextFrameIdx = nNextFrameIdx % MAX_BUFFER_FRAME;

	m_pCameraImageBuffer[nGrabberIndex]->SetFrameImage(nNextFrameIdx, (LPBYTE)pBuffer);

	m_pCameraCurrentFrameIdx[nGrabberIndex] = nNextFrameIdx;

	localLock.Unlock();

	return 1;
}

int InspectionHikCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}
