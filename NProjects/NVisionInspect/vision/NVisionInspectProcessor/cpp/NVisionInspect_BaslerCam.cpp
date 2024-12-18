#include "pch.h"
#include "NVisionInspect_BaslerCam.h"

CNVisionInspect_BaslerCam::CNVisionInspect_BaslerCam(INVisionInspectBaslerCamToParent* pInterface)
{
	m_pInterface = pInterface;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		m_pCamera[i] = NULL;
		m_pCameraImageBuffer[i] = NULL;
		m_cameraCurrentFrameIdx[i] = 0;
	}

	for (int i = 0; i < MAX_IMAGE_BUFFER; i++) {
		m_currentFrameWaitProcessIdx[i] = 0;
	}

	std::queue<int> emptyList;
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		m_pCameraImageBuffer[i] = NULL;
		m_pCamera[i] = NULL;
		std::swap(m_queueInspectWaitList[i], emptyList);
	}
}

CNVisionInspect_BaslerCam::~CNVisionInspect_BaslerCam()
{
	Destroy();
}

BOOL CNVisionInspect_BaslerCam::Initialize()
{
	int nHikCamCount = m_pInterface->GetVecCameras().at(0);
	int nbaslerCamCount = m_pInterface->GetVecCameras().at(1); /*pos 1: number of Basler Cam*/

	std::vector<CFrameGrabberParam> grabberParam;
	grabberParam.resize(nbaslerCamCount);

	//CFrameGrabberParam grabberParam[nCamCount];

	for (int i = 0; i < nbaslerCamCount; i++)
	{
		int nBaslerCamSettingIdx = nHikCamCount + i;

		grabberParam[i].SetParam_GrabberPort((CString)m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_sSerialNumber);
		grabberParam[i].SetParam_FrameWidth(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameWidth);
		grabberParam[i].SetParam_FrameHeight(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameHeight);
		grabberParam[i].SetParam_FrameWidthStep(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameWidth);
		grabberParam[i].SetParam_FrameDepth(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameDepth);
		grabberParam[i].SetParam_FrameChannels(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nChannels);
		grabberParam[i].SetParam_FrameCount(m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nMaxFrameCount);
	}

	for (int nCamIdx = 0; nCamIdx < nbaslerCamCount; nCamIdx++)
	{
		int nBaslerCamSettingIdx = nHikCamCount + nCamIdx;

		// Image Buffer
		if (m_pCameraImageBuffer[nCamIdx] != NULL)
		{
			m_pCameraImageBuffer[nCamIdx]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[nCamIdx];
			m_pCameraImageBuffer[nCamIdx] = NULL;
		}

		DWORD dwFrameWidth = (DWORD)m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nFrameHeight;
		DWORD dwFrameCount = (DWORD)m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nMaxFrameCount;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * m_pInterface->GetCameraSettingControl(nBaslerCamSettingIdx)->m_nChannels;

		m_pCameraImageBuffer[nCamIdx] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[nCamIdx]->SetFrameWidth(dwFrameWidth);
		m_pCameraImageBuffer[nCamIdx]->SetFrameHeight(dwFrameHeight);
		m_pCameraImageBuffer[nCamIdx]->SetFrameCount(dwFrameCount);
		m_pCameraImageBuffer[nCamIdx]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size_TopCam = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferBaslerCam", nCamIdx);
		m_pCameraImageBuffer[nCamIdx]->CreateSharedMemory(strMemory, dw64Size_TopCam);

		// Camera
		m_pCamera[nCamIdx] = new CFrameGrabber_BaslerCam_New(nCamIdx, this);

		int nRetryCount = 1;
		BOOL bCamConnection = FALSE;

		while (bCamConnection == FALSE)
		{
			if (m_pCamera[nCamIdx]->Connect(grabberParam[nCamIdx]) != 1)
				m_bCamera_ConnectStatus[nCamIdx] = FALSE;

			/*if (m_nCamera[i]->StartGrab() != 1)
				m_bCamera_ConnectStatus[i] = FALSE;*/
			else
				m_bCamera_ConnectStatus[nCamIdx] = TRUE;

			if (m_bCamera_ConnectStatus[nCamIdx] == TRUE)
				bCamConnection = TRUE;
			else if (10 < nRetryCount)
				bCamConnection = TRUE;
			else
			{
				CString strMsg;
				strMsg.Format(_T("Align Cam %d Connection Fail.. Retry %d.."), nCamIdx, nRetryCount++);
				Sleep(3000);
			}
		}
	}
}

BOOL CNVisionInspect_BaslerCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_pCamera[i] != NULL)
		{
			m_pCamera[i]->StopGrab();
			Sleep(1000);
			m_pCamera[i]->Disconnect();
			delete m_pCamera[i], m_pCamera[i] = NULL;
		}

		if (m_pCameraImageBuffer[i] != NULL)
		{
			m_pCameraImageBuffer[i]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[i], m_pCameraImageBuffer[i] = NULL;
		}
	}

	std::queue<int> emptyList;
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		std::swap(m_queueInspectWaitList[i], emptyList);
	}

	return TRUE;
}

BOOL CNVisionInspect_BaslerCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CNVisionInspect_BaslerCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx);
}
BOOL CNVisionInspect_BaslerCam::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return FALSE;

	if (pBuffer == NULL)
		return FALSE;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return FALSE;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];

	int nWidth = m_pCameraImageBuffer[nCamIdx]->GetFrameWidth();
	int nHeight = m_pCameraImageBuffer[nCamIdx]->GetFrameHeight();

	memcpy(pBuffer, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx), nWidth * nHeight);

	localLock.Unlock();

	return TRUE;
}

BOOL CNVisionInspect_BaslerCam::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return FALSE;

	if (pBuffer == NULL)
		return FALSE;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return FALSE;

	int nBeforeFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];

	DWORD tickCount = GetTickCount64();

	BOOL bGrabTimeOver = FALSE;

	while (1)
	{
		if (10000 < GetTickCount64() - tickCount)
		{
			bGrabTimeOver = TRUE;
			break;
		}

		if (nBeforeFrameIdx != m_cameraCurrentFrameIdx[nCamIdx])
			break;

		Sleep(1);
	}

	if (bGrabTimeOver == TRUE)
		return FALSE;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];

	int nWidth = m_pCameraImageBuffer[nCamIdx]->GetFrameWidth();
	int nHeight = m_pCameraImageBuffer[nCamIdx]->GetFrameHeight();
	int nFrameSize = m_pCameraImageBuffer[nCamIdx]->GetFrameSize();

	memcpy(pBuffer, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx), nFrameSize);

	localLock.Unlock();

	return TRUE;
}

CSharedMemoryBuffer* CNVisionInspect_BaslerCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

int CNVisionInspect_BaslerCam::PopInspectWaitFrame(int nGrabberIdx)
{
	int nProcessFrame = 0;

	CSingleLock localLock(&m_csInspectWaitList[nGrabberIdx]);
	localLock.Lock();

	if (m_queueInspectWaitList[nGrabberIdx].empty())
		return -1;

	nProcessFrame = m_queueInspectWaitList[nGrabberIdx].front();

	m_queueInspectWaitList[nGrabberIdx].pop();

	return nProcessFrame;
}

BOOL CNVisionInspect_BaslerCam::SetFrameWaitProcess(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csInspectWaitList[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameWaitProcessIdx = m_currentFrameWaitProcessIdx[nCamIdx];
	int nNextFrameWaitProcessIdx = nCurrentFrameWaitProcessIdx + 1;

	CSingleLock lockCamCurrentFrame(&m_csCameraFrameIdx[nCamIdx]);
	lockCamCurrentFrame.Lock();
	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];
	lockCamCurrentFrame.Unlock();

	m_pFrameWaitProcessList[nCamIdx]->SetFrameImage(nCurrentFrameWaitProcessIdx, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx));

	m_queueInspectWaitList[nCamIdx].push(nCurrentFrameWaitProcessIdx);

	nNextFrameWaitProcessIdx = nNextFrameWaitProcessIdx % MAX_IMAGE_BUFFER;
	m_currentFrameWaitProcessIdx[nCamIdx] = nNextFrameWaitProcessIdx;

	localLock.Unlock();

	return TRUE;
}

LPBYTE CNVisionInspect_BaslerCam::GetFrameWaitProcess(int nCamIdx, int nFrame)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	if (m_pFrameWaitProcessList[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csInspectWaitList[nCamIdx]);
	localLock.Lock();

	return m_pFrameWaitProcessList[nCamIdx]->GetFrameImage(nFrame);

	localLock.Unlock();
}

void CNVisionInspect_BaslerCam::RegisterReceivedImageCallback(ReceivedImageCallback* callback, LPVOID pParam)
{
	m_pParam = pParam;
	m_pReceivedImgCallback = callback;
}

int CNVisionInspect_BaslerCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (nGrabberIndex < 0 || MAX_CAMERA_INSPECT_COUNT <= nGrabberIndex)
		return -1;

	if (pBuffer == NULL)
		return -1;

	if (m_pCameraImageBuffer[nGrabberIndex] == NULL)
		return -1;

	CSingleLock localLock(&m_csCameraFrameIdx[nGrabberIndex]);
	localLock.Lock();

	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nGrabberIndex];
	int nNextFrameIdx = nCurrentFrameIdx + 1;

	nNextFrameIdx = nNextFrameIdx % MAX_FRAME_COUNT;

	localLock.Unlock();

	m_pCameraImageBuffer[nGrabberIndex]->SetFrameImage(nNextFrameIdx, (LPBYTE)pBuffer);

	m_cameraCurrentFrameIdx[nGrabberIndex] = nNextFrameIdx;

	int nCoreIdx = nGrabberIndex;
	if (m_pInterface->GetStatusControl(nCoreIdx)->GetStreaming() == FALSE
		&& m_pInterface->GetStatusControl(nCoreIdx)->GetInspectRunning() == TRUE)
	{
		m_pReceivedImgCallback((LPBYTE)pBuffer, nGrabberIndex, nNextFrameIdx, CameraBrand_Basler, m_pParam);

		return TRUE;
	}

	/*if (nGrabberIndex == 2 || nGrabberIndex == 3)
	{
		CSingleLock lockCamCurrentFrame(&m_csCameraFrameIdx[nGrabberIndex]);
		lockCamCurrentFrame.Lock();
		int nCurrentFrameWaitProcessIdx = m_currentFrameWaitProcessIdx[nGrabberIndex];
		int nNextFrameWaitProcessIdx = nCurrentFrameWaitProcessIdx + 1;

		m_pFrameWaitProcessList[nGrabberIndex]->SetFrameImage(nCurrentFrameWaitProcessIdx, (LPBYTE)pBuffer);

		m_queueInspectWaitList[nGrabberIndex].push(nCurrentFrameWaitProcessIdx);

		nNextFrameWaitProcessIdx = nNextFrameWaitProcessIdx % MAX_IMAGE_BUFFER;
		m_currentFrameWaitProcessIdx[nGrabberIndex] = nNextFrameWaitProcessIdx;

		lockCamCurrentFrame.Unlock();
	}*/

	return TRUE;
}

int CNVisionInspect_BaslerCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}

int CNVisionInspect_BaslerCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StartGrab();
}

int CNVisionInspect_BaslerCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StopGrab();
}

int CNVisionInspect_BaslerCam::SoftwareTrigger(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	int nMode, nSource;
	nMode = m_pCamera[nCamIdx]->GetTriggerMode().GetIntValue();
	nSource = m_pCamera[nCamIdx]->GetTriggerSource().GetIntValue();
	if (nMode == 0 && nSource == 1)
		return 0;

	return m_pCamera[nCamIdx]->SendTrigger();
}

int CNVisionInspect_BaslerCam::SetTriggerMode(int nCamIdx, int nMode)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetTriggerMode(nMode);
}

int CNVisionInspect_BaslerCam::SetTriggerSource(int nCamIdx, int nSource)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetTriggerSource(nSource);
}

int CNVisionInspect_BaslerCam::SetExposureTime(int nCamIdx, double dExpTime)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetExposureTime(dExpTime);
}

int CNVisionInspect_BaslerCam::SetAnalogGain(int nCamIdx, double dGain)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetAnalogGain(dGain);
}
