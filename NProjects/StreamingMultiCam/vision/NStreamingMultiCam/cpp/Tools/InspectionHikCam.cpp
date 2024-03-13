#include "pch.h"
#include "InspectionHikCam.h"

CInspectionHikCam::CInspectionHikCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_nCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

CInspectionHikCam::~CInspectionHikCam()
{
	Destroy();
}

BOOL CInspectionHikCam::Initialize()
{
	// Cam 1
	CFrameGrabberParam grabberParam[MAX_CAMERA_COUNT];

	grabberParam[0].SetParam_GrabberPort((CString)_T("DA0069518"));
	grabberParam[0].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[0].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[0].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[0].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[0].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[0].SetParam_FrameCount(MAX_FRAME_COUNT);

	// Cam 2
	grabberParam[1].SetParam_GrabberPort((CString)_T("DA0069525"));
	grabberParam[1].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[1].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[1].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[1].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[1].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[1].SetParam_FrameCount(MAX_FRAME_COUNT);

	// Cam 3
	grabberParam[2].SetParam_GrabberPort((CString)_T("DA0069522"));
	grabberParam[2].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[2].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[2].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[2].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[2].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[2].SetParam_FrameCount(MAX_FRAME_COUNT);

	// Cam 4
	grabberParam[3].SetParam_GrabberPort((CString)_T("DA0069524"));
	grabberParam[3].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[3].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[3].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[3].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[3].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[3].SetParam_FrameCount(MAX_FRAME_COUNT);

	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		// Buffer
		if (m_pCameraImageBuffer[i] != NULL)
		{
			m_pCameraImageBuffer[i]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[i];
			m_pCameraImageBuffer[i] = NULL;
		}

		DWORD dwFrameWidth = (DWORD)FRAME_WIDTH;
		DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT;
		DWORD dwFrameCount = MAX_FRAME_COUNT;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * CHANNEL_COUNT;

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

BOOL CInspectionHikCam::Destroy()
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

BOOL CInspectionHikCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CInspectionHikCam::GetBufferImage(int nCamIdx)
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

BOOL CInspectionHikCam::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
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

BOOL CInspectionHikCam::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
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

CSharedMemoryBuffer* CInspectionHikCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

LPBYTE CInspectionHikCam::GetResultBufferImage(int nPosIdx)
{
	if (nPosIdx < 0 || MAX_POSITION_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;
}

int CInspectionHikCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
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

	nNextFrameIdx = nNextFrameIdx % MAX_FRAME_COUNT;

	m_pCameraImageBuffer[nGrabberIndex]->SetFrameImage(nNextFrameIdx, (LPBYTE)pBuffer);

	m_pCameraCurrentFrameIdx[nGrabberIndex] = nNextFrameIdx;

	localLock.Unlock();

	return 1;
}

int CInspectionHikCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}

int CInspectionHikCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StartGrab();
}

int CInspectionHikCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StopGrab();
}
