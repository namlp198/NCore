#include "pch.h"
#include "InspectionUsbCam.h"

CInspectionUsbCam::CInspectionUsbCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_pCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pCameraImageBuffer[i] = NULL;
	}
}

CInspectionUsbCam::~CInspectionUsbCam()
{
	Destroy();
}

BOOL CInspectionUsbCam::Initialize()
{
	CFrameGrabberUsbParam grabberParam[MAX_CAMERA_COUNT];
	grabberParam[0].SetParam_FrameWidth(1280);
	grabberParam[0].SetParam_FrameHeight(720);
	grabberParam[0].SetParam_FrameWidthStep(1280);
	grabberParam[0].SetParam_FrameDepth(24);
	grabberParam[0].SetParam_FrameChannels(3);
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
		DWORD dwFrameHeight = (DWORD)720;
		DWORD dwFrameCount = MAX_BUFFER_FRAME;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight;
		DWORD dwChannels = 3;

		m_pCameraImageBuffer[i] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pCameraImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pCameraImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pCameraImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize * dwChannels;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "Buffer_Usb_Cam", i);
		m_pCameraImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		// Camera
		m_pCamera[i] = new CFramGrabber_UsbCam(i, this);

		int nRetryCount = 1;
		BOOL bCamConnection = FALSE;

		while (bCamConnection == FALSE)
		{
			if (m_pCamera[i]->Connect(grabberParam[i]) != 1)
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
				strMsg.Format(_T("Usb Cam %d Connection Fail.. Retry %d.."), i, nRetryCount++);
				Sleep(3000);
			}
		}
	}

	return TRUE;
}

BOOL CInspectionUsbCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_pCamera[i] != NULL)
		{
			m_pCamera[i]->StopContinuousGrab();
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

	return TRUE;
}

BOOL CInspectionUsbCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CInspectionUsbCam::GetBufferImage(int nCamIdx)
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

CSharedMemoryBuffer* CInspectionUsbCam::GetSharedMemoryBuffer(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	return m_pCameraImageBuffer[nCamIdx];
}

LPBYTE CInspectionUsbCam::GetResultBufferImage(int nPosIdx)
{
	if (nPosIdx < 0 || MAX_POSITION_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;
}

int CInspectionUsbCam::IFGU2P_FrameGrabbedUsb(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
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

int CInspectionUsbCam::StartContinuousGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

    m_pCamera[nCamIdx]->StartContinuousGrab();

	return 1;
}

int CInspectionUsbCam::StopContinuousGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	m_pCamera[nCamIdx]->StopContinuousGrab();

	return 1;
}

int CInspectionUsbCam::SingleGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	m_pCamera[nCamIdx]->SingleGrab();
	
	return 1;
}
