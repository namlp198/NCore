#include "pch.h"
#include "InspectioniRaypleCam.h"

CInspectioniRaypleCam::CInspectioniRaypleCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_nCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

CInspectioniRaypleCam::~CInspectioniRaypleCam()
{
	Destroy();
}

BOOL CInspectioniRaypleCam::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_COUNT];

	grabberParam[0].SetParam_GrabberPort((CString)_T("CG17917AAK00015"));
	grabberParam[0].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[0].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[0].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[0].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[0].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[0].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[1].SetParam_GrabberPort((CString)_T("CG17917AAK00001"));
	grabberParam[1].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[1].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[1].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[1].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[1].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[1].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[2].SetParam_GrabberPort((CString)_T("CG17917AAK00003"));
	grabberParam[2].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[2].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[2].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[2].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[2].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[2].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[3].SetParam_GrabberPort((CString)_T("CG17917AAK00006"));
	grabberParam[3].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[3].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[3].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[3].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[3].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[3].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[4].SetParam_GrabberPort((CString)_T("CG17917AAK00018"));
	grabberParam[4].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[4].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[4].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[4].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[4].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[4].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[5].SetParam_GrabberPort((CString)_T("CG17917AAK00020"));
	grabberParam[5].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[5].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[5].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[5].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[5].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[5].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[6].SetParam_GrabberPort((CString)_T("CG17917AAK00016"));
	grabberParam[6].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[6].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[6].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[6].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[6].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[6].SetParam_FrameCount(MAX_FRAME_COUNT);

	grabberParam[7].SetParam_GrabberPort((CString)_T("CE26550AAK00003"));
	grabberParam[7].SetParam_FrameWidth(FRAME_WIDTH);
	grabberParam[7].SetParam_FrameHeight(FRAME_HEIGHT);
	grabberParam[7].SetParam_FrameWidthStep(FRAME_WIDTH);
	grabberParam[7].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[7].SetParam_FrameChannels(CHANNEL_COUNT);
	grabberParam[7].SetParam_FrameCount(MAX_FRAME_COUNT);

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
		strMemory.Format(_T("%s_%d"), "iRaypleBuffer", i);
		m_pCameraImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		// Camera
		m_nCamera[i] = new CFrameGrabber_iRayple(i, this);

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

BOOL CInspectioniRaypleCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_nCamera[i] != NULL)
		{
			m_nCamera[i]->StopGrab();
			Sleep(500);
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

LPBYTE CInspectioniRaypleCam::GetBufferImage(int nCamIdx)
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

int CInspectioniRaypleCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
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

int CInspectioniRaypleCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return 0;
}

int CInspectioniRaypleCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StartGrab();
}

int CInspectioniRaypleCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_nCamera[nCamIdx] == NULL)
		return 0;

	return m_nCamera[nCamIdx]->StopGrab();
}
