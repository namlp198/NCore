#include "pch.h"
#include "InspectionBaslerCam_New.h"

CInspectionBaslerCam_New::CInspectionBaslerCam_New()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_nCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

CInspectionBaslerCam_New::~CInspectionBaslerCam_New()
{
	Destroy();
}

BOOL CInspectionBaslerCam_New::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_COUNT];
	grabberParam[0].SetParam_GrabberPort((CString)_T("22125820"));
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
		strMemory.Format(_T("%s_%d"), "Buffer_Basler_New", i);
		m_pCameraImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		// Camera
		m_nCamera[i] = new CFrameGrabber_BaslerCam_New(i, this);

		int nRetryCount = 1;
		BOOL bCamConnection = FALSE;

		while (bCamConnection == FALSE)
		{
			if (m_nCamera[i]->Connect(grabberParam[i]) != 1)
				m_bCamera_ConnectStatus[i] = FALSE;

			if (m_nCamera[i]->StartGrab() != 1)
				m_bCamera_ConnectStatus[i] = FALSE;
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

BOOL CInspectionBaslerCam_New::Destroy()
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

BOOL CInspectionBaslerCam_New::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CInspectionBaslerCam_New::GetBufferImage(int nCamIdx)
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

BOOL CInspectionBaslerCam_New::GetBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	return 0;
}

BOOL CInspectionBaslerCam_New::GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer)
{
	return 0;
}

CSharedMemoryBuffer* CInspectionBaslerCam_New::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

LPBYTE CInspectionBaslerCam_New::GetResultBufferImage(int nPosIdx)
{
	if (nPosIdx < 0 || MAX_POSITION_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;
}

int CInspectionBaslerCam_New::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
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

int CInspectionBaslerCam_New::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}
