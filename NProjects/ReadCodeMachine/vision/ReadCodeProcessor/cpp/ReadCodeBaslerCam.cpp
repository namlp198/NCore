#include "pch.h"
#include "ReadCodeBaslerCam.h"

CReadCodeBaslerCam::CReadCodeBaslerCam(IReadCodeBaslerCamToParent* pInterface)
{
	m_pInterface = pInterface;

	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_pCamera[i] != NULL)
			delete m_pCamera[i], m_pCamera[i] = NULL;
	}
}

CReadCodeBaslerCam::~CReadCodeBaslerCam()
{
	Destroy();
}

BOOL CReadCodeBaslerCam::Initialize()
{
	CFrameGrabberParam grabberParam[MAX_CAMERA_INSPECT_COUNT];

	//grabberParam[0].SetParam_GrabberPort((CString)_T("22125820"));
	grabberParam[0].SetParam_GrabberPort((CString)m_pInterface->GetCameraSettingControl(0)->m_sSerialNumber);
	grabberParam[0].SetParam_FrameWidth(m_pInterface->GetCameraSettingControl(0)->m_nFrameWidth);
	grabberParam[0].SetParam_FrameHeight(m_pInterface->GetCameraSettingControl(0)->m_nFrameHeight);
	grabberParam[0].SetParam_FrameWidthStep(m_pInterface->GetCameraSettingControl(0)->m_nFrameWidth);
	grabberParam[0].SetParam_FrameDepth(FRAME_DEPTH);
	grabberParam[0].SetParam_FrameChannels(m_pInterface->GetCameraSettingControl(0)->m_nChannel);
	grabberParam[0].SetParam_FrameCount(MAX_FRAME_COUNT);

	int nCamIdx = 0;
	for (nCamIdx = 0; nCamIdx < MAX_CAMERA_INSPECT_COUNT; nCamIdx++)
	{
		// Image Buffer
		if (m_pCameraImageBuffer[nCamIdx] != NULL)
		{
			m_pCameraImageBuffer[nCamIdx]->DeleteSharedMemory();
			delete m_pCameraImageBuffer[nCamIdx];
			m_pCameraImageBuffer[nCamIdx] = NULL;
		}

		DWORD dwFrameWidth = (DWORD)m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameHeight;
		DWORD dwFrameCount = MAX_FRAME_COUNT;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * m_pInterface->GetCameraSettingControl(nCamIdx)->m_nChannel;

		m_pCameraImageBuffer[nCamIdx] = new CSharedMemoryBuffer;
		m_pCameraImageBuffer[nCamIdx]->SetFrameWidth(dwFrameWidth);
		m_pCameraImageBuffer[nCamIdx]->SetFrameHeight(dwFrameHeight);
		m_pCameraImageBuffer[nCamIdx]->SetFrameCount(dwFrameCount);
		m_pCameraImageBuffer[nCamIdx]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size_TopCam = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferBasler", nCamIdx);
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

BOOL CReadCodeBaslerCam::Destroy()
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

	return TRUE;
}

BOOL CReadCodeBaslerCam::GetCamStatus()
{
	for (int i = 0; i < MAX_CAMERA_INSPECT_COUNT; i++)
	{
		if (m_bCamera_ConnectStatus[i] == FALSE)
			return FALSE;
	}

	return TRUE;
}

LPBYTE CReadCodeBaslerCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return NULL;

	if (m_pCameraImageBuffer[nCamIdx] == NULL)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pCameraImageBuffer[nCamIdx]->GetFrameImage(nCurrentFrameIdx);
}

//void CReadCodeBaslerCam::RegisterReceivedImageCallback(ReceivedImageCallback* callback, LPVOID pParam)
//{
//	m_pParam = pParam;
//	m_pReceivedImgCallback = callback;
//}

int CReadCodeBaslerCam::IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize)
{
	if (nGrabberIndex < 0 || MAX_CAMERA_INSPECT_COUNT <= nGrabberIndex)
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

	if (m_pInterface->GetReadCodeStatusControl(nGrabberIndex)->GetStreaming() == TRUE)
	{
		localLock.Unlock();
		return TRUE;
	}

	//m_pReceivedImgCallback((LPBYTE)pBuffer, nGrabberIndex, nCurrentFrameIdx, m_pParam);
	m_pInterface->ProcessFrame(nGrabberIndex, (LPBYTE)pBuffer);
}

int CReadCodeBaslerCam::IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize)
{
	return -1;
}

int CReadCodeBaslerCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StartGrab();
}

int CReadCodeBaslerCam::SingleGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SingleGrab();
}

int CReadCodeBaslerCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->StopGrab();
}

int CReadCodeBaslerCam::SetTriggerMode(int nCamIdx, int nMode)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetTriggerMode(nMode);
}

int CReadCodeBaslerCam::SetTriggerSource(int nCamIdx, int nSource)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetTriggerSource(nSource);
}

int CReadCodeBaslerCam::SetExposureTime(int nCamIdx, double dExpTime)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetExposureTime(dExpTime);
}

int CReadCodeBaslerCam::SetAnalogGain(int nCamIdx, double dGain)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSPECT_COUNT <= nCamIdx)
		return 0;

	if (m_pCamera[nCamIdx] == NULL)
		return 0;

	return m_pCamera[nCamIdx]->SetAnalogGain(dGain);
}
