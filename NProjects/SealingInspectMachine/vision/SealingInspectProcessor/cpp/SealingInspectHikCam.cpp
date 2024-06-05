#include "pch.h"
#include "SealingInspectHikCam.h"

CSealingInspectHikCam::CSealingInspectHikCam(ISealingInspectHikCamToParent* pInterface)
{
	m_pInterface = pInterface;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++) {
		m_pCamera[i] = NULL;
		m_pCameraImageBuffer[i] = NULL;
		m_cameraCurrentFrameIdx[i] = 0;
	}

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++) {
		m_currentFrameWaitProcessSideCamIdx[i] = 0;
	}

	std::queue<int> emptyList;
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		m_pCameraImageBuffer[i] = NULL;
		m_pCamera[i] = NULL;
		std::swap(m_queueInspectWaitList[i], emptyList);
	}
}

CSealingInspectHikCam::~CSealingInspectHikCam()
{
	Destroy();
}

BOOL CSealingInspectHikCam::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_INSPECT_COUNT];

	// TOP Cam 1
	grabberParam[0].SetParam_GrabberPort((CString)_T("DA3494062"));
	grabberParam[0].SetParam_FrameWidth(FRAME_WIDTH_TOPCAM);
	grabberParam[0].SetParam_FrameHeight(FRAME_HEIGHT_TOPCAM);
	grabberParam[0].SetParam_FrameWidthStep(FRAME_WIDTH_TOPCAM);
	grabberParam[0].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[0].SetParam_FrameChannels(NUMBER_OF_CHANNEL);
	grabberParam[0].SetParam_FrameCount(MAX_FRAME_COUNT);

	// TOP Cam 2
	grabberParam[1].SetParam_GrabberPort((CString)_T("DA3494064"));
	grabberParam[1].SetParam_FrameWidth(FRAME_WIDTH_TOPCAM);
	grabberParam[1].SetParam_FrameHeight(FRAME_HEIGHT_TOPCAM);
	grabberParam[1].SetParam_FrameWidthStep(FRAME_WIDTH_TOPCAM);
	grabberParam[1].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[1].SetParam_FrameChannels(NUMBER_OF_CHANNEL);
	grabberParam[1].SetParam_FrameCount(MAX_FRAME_COUNT);

	// SIDE Cam 1
	grabberParam[2].SetParam_GrabberPort((CString)_T("DA3491412"));
	grabberParam[2].SetParam_FrameWidth(FRAME_WIDTH_SIDECAM);
	grabberParam[2].SetParam_FrameHeight(FRAME_HEIGHT_SIDECAM);
	grabberParam[2].SetParam_FrameWidthStep(FRAME_WIDTH_SIDECAM);
	grabberParam[2].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[2].SetParam_FrameChannels(NUMBER_OF_CHANNEL);
	grabberParam[2].SetParam_FrameCount(MAX_FRAME_COUNT);

	// SIDE Cam 2
	grabberParam[3].SetParam_GrabberPort((CString)_T("DA3491407"));
	grabberParam[3].SetParam_FrameWidth(FRAME_WIDTH_SIDECAM);
	grabberParam[3].SetParam_FrameHeight(FRAME_HEIGHT_SIDECAM);
	grabberParam[3].SetParam_FrameWidthStep(FRAME_WIDTH_SIDECAM);
	grabberParam[3].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[3].SetParam_FrameChannels(NUMBER_OF_CHANNEL);
	grabberParam[3].SetParam_FrameCount(MAX_FRAME_COUNT);

	int nCamIdx = 0;
	for (nCamIdx = 0; nCamIdx < MAX_CAMERA_INSPECT_COUNT / 2; nCamIdx++)
	{
		// Buffer
		if (m_pCameraImageBuffer[nCamIdx] != NULL)
		{
			m_pCameraImageBuffer[nCamIdx]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[nCamIdx];
			m_pCameraImageBuffer[nCamIdx] = NULL;
		}

		// frame wait process list buffer
		if (m_pFrameWaitProcessList[nCamIdx] != NULL)
		{
			m_pFrameWaitProcessList[nCamIdx]->DeleteSharedMemory();
			delete m_pFrameWaitProcessList[nCamIdx];
			m_pFrameWaitProcessList[nCamIdx] = NULL;
		}

		DWORD dwFrameWidth_TopCam = (DWORD)FRAME_WIDTH_TOPCAM;
		DWORD dwFrameHeight_TopCam = (DWORD)FRAME_HEIGHT_TOPCAM;
		DWORD dwFrameCount_TopCam = MAX_FRAME_COUNT;
		DWORD dwFrameWaitProcessCount_TopCam = MAX_FRAME_WAIT_TOPCAM;
		DWORD dwFrameSize_TopCam = dwFrameWidth_TopCam * dwFrameHeight_TopCam * NUMBER_OF_CHANNEL_TOPCAM;

		m_pCameraImageBuffer[nCamIdx] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[nCamIdx]->SetFrameWidth(dwFrameWidth_TopCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameHeight(dwFrameHeight_TopCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameCount(dwFrameCount_TopCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameSize(dwFrameSize_TopCam);

		m_pFrameWaitProcessList[nCamIdx] = new CSharedMemoryBuffer;
		m_pFrameWaitProcessList[nCamIdx]->SetFrameWidth(dwFrameWidth_TopCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameHeight(dwFrameHeight_TopCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameCount(dwFrameWaitProcessCount_TopCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameSize(dwFrameSize_TopCam);

		DWORD64 dw64Size_TopCam = (DWORD64)dwFrameCount_TopCam * dwFrameSize_TopCam;
		DWORD64 dw64Size_FrameWaitProcess_TopCam = (DWORD64)dwFrameWaitProcessCount_TopCam * dwFrameSize_TopCam;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferTOPCam", nCamIdx);
		m_pCameraImageBuffer[nCamIdx]->CreateSharedMemory(strMemory, dw64Size_TopCam);

		CString strMemory_FrameWaitProcess_TopCam;
		strMemory.Format(_T("%s_%d"), "Buffer_FrameWaitProcessTOPCam", nCamIdx);
		m_pFrameWaitProcessList[nCamIdx]->CreateSharedMemory(strMemory_FrameWaitProcess_TopCam, dw64Size_FrameWaitProcess_TopCam);

		// Camera
		m_pCamera[nCamIdx] = new CFrameGrabber_MVS_GigE(nCamIdx, this);

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

	for (nCamIdx = 2; nCamIdx < MAX_CAMERA_INSPECT_COUNT; nCamIdx++)
	{
		// Buffer
		if (m_pCameraImageBuffer[nCamIdx] != NULL)
		{
			m_pCameraImageBuffer[nCamIdx]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[nCamIdx];
			m_pCameraImageBuffer[nCamIdx] = NULL;
		}

		// frame wait process list buffer
		if (m_pFrameWaitProcessList[nCamIdx] != NULL)
		{
			m_pFrameWaitProcessList[nCamIdx]->DeleteSharedMemory();
			delete m_pFrameWaitProcessList[nCamIdx];
			m_pFrameWaitProcessList[nCamIdx] = NULL;
		}

		DWORD dwFrameWidth_SideCam = (DWORD)FRAME_WIDTH_SIDECAM;
		DWORD dwFrameHeight_SideCam = (DWORD)FRAME_HEIGHT_SIDECAM;
		DWORD dwFrameCount_SideCam = MAX_FRAME_COUNT;
		DWORD dwFrameWaitProcessCount_SideCam = MAX_FRAME_WAIT_SIDECAM;
		DWORD dwFrameSize_SideCam = dwFrameWidth_SideCam * dwFrameHeight_SideCam * NUMBER_OF_CHANNEL;

		m_pCameraImageBuffer[nCamIdx] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[nCamIdx]->SetFrameWidth(dwFrameWidth_SideCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameHeight(dwFrameHeight_SideCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameCount(dwFrameCount_SideCam);
		m_pCameraImageBuffer[nCamIdx]->SetFrameSize(dwFrameSize_SideCam);

		m_pFrameWaitProcessList[nCamIdx] = new CSharedMemoryBuffer;
		m_pFrameWaitProcessList[nCamIdx]->SetFrameWidth(dwFrameWidth_SideCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameHeight(dwFrameHeight_SideCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameCount(dwFrameWaitProcessCount_SideCam);
		m_pFrameWaitProcessList[nCamIdx]->SetFrameSize(dwFrameSize_SideCam);

		DWORD64 dw64Size_SideCam = (DWORD64)dwFrameCount_SideCam * dwFrameSize_SideCam;
		DWORD64 dw64Size_FrameWaitProcess_SideCam = (DWORD64)dwFrameWaitProcessCount_SideCam * dwFrameSize_SideCam;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferSIDECam", nCamIdx);
		m_pCameraImageBuffer[nCamIdx]->CreateSharedMemory(strMemory, dw64Size_SideCam);

		CString strMemory_FrameWaitProcess_SideCam;
		strMemory.Format(_T("%s_%d"), "Buffer_FrameWaitProcessSIDECam", nCamIdx);
		m_pFrameWaitProcessList[nCamIdx]->CreateSharedMemory(strMemory_FrameWaitProcess_SideCam, dw64Size_FrameWaitProcess_SideCam);

		// Camera
		m_pCamera[nCamIdx] = new CFrameGrabber_MVS_GigE(nCamIdx, this);

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

BOOL CSealingInspectHikCam::Destroy()
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

BOOL CSealingInspectHikCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CSealingInspectHikCam::GetBufferImage(int nCamIdx)
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

BOOL CSealingInspectHikCam::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
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

BOOL CSealingInspectHikCam::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
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

CSharedMemoryBuffer* CSealingInspectHikCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

int CSealingInspectHikCam::PopInspectWaitFrame(int nGrabberIdx)
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

BOOL CSealingInspectHikCam::SetFrameWaitProcess_SideCam(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csInspectWaitList[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameWaitProcessIdx = m_currentFrameWaitProcessSideCamIdx[nCamIdx];
	int nNextFrameWaitProcessIdx = nCurrentFrameWaitProcessIdx + 1;

	CSingleLock lockCamCurrentFrame(&m_csCameraFrameIdx[nCamIdx]);
	lockCamCurrentFrame.Lock();
	int nCurrentFrameIdx = m_cameraCurrentFrameIdx[nCamIdx];
	lockCamCurrentFrame.Unlock();
	
	m_pFrameWaitProcessList[nCamIdx]->SetFrameImage(nCurrentFrameWaitProcessIdx, m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx));

	m_queueInspectWaitList[nCamIdx].push(nCurrentFrameWaitProcessIdx);

	nNextFrameWaitProcessIdx = nNextFrameWaitProcessIdx % MAX_IMAGE_BUFFER_SIDECAM;
	m_currentFrameWaitProcessSideCamIdx[nCamIdx] = nNextFrameWaitProcessIdx;

	localLock.Unlock();

	return TRUE;

}

LPBYTE CSealingInspectHikCam::GetFrameWaitProcess_SideCam(int nCamIdx, int nFrame)
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

int CSealingInspectHikCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
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

	m_pCameraImageBuffer[nGrabberIndex]->SetFrameImage(nNextFrameIdx, (LPBYTE)pBuffer);

	m_cameraCurrentFrameIdx[nGrabberIndex] = nNextFrameIdx;

	localLock.Unlock();

	return TRUE;
}

int CSealingInspectHikCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}

int CSealingInspectHikCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StartGrab();
}

int CSealingInspectHikCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StopGrab();
}

int CSealingInspectHikCam::SoftwareTrigger(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	int nMode, nSource;
	m_pCamera[nCamIdx]->GetTriggerMode(nMode);
	m_pCamera[nCamIdx]->GetTriggerSource(nSource);
	if (nMode == 0 && nSource == 1)
		return 0;

	return m_pCamera[nCamIdx]->SendTrigger();
}

int CSealingInspectHikCam::SetTriggerMode(int nCamIdx, int nMode)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	m_pCamera[nCamIdx]->SetTriggerMode(nMode);
}

int CSealingInspectHikCam::SetTriggerSource(int nCamIdx, int nSource)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	m_pCamera[nCamIdx]->SetTriggerSource(nSource);
}

