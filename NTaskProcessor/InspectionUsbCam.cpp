#include "pch.h"
#include "InspectionUsbCam.h"

CInspectionUsbCam::CInspectionUsbCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_pUsbCamera[i] = NULL;

		m_pCameraCurrentFrameIdx[i] = 0;
		m_pUsbCamera[i] = NULL;
	}
}

CInspectionUsbCam::~CInspectionUsbCam()
{
	Destroy();
}

BOOL CInspectionUsbCam::Initialize()
{
	int deviceCount = EnumerateDevices();
	if (deviceCount == 0)
		return FALSE;
	int devices = deviceCount < MAX_CAMERA_COUNT ? deviceCount : MAX_CAMERA_COUNT;

	for (int i = 0; i < deviceCount; i++)
	{
		int nChannels = 3;
		DWORD dwFrameWidth = (DWORD)640;
		DWORD dwFrameHeight = (DWORD)480;
		DWORD dwFrameCount = MAX_BUFFER_FRAME;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nChannels;

		// Camera
		m_pUsbCamera[i] = new CFramGrabber_UsbCam(i);
		m_pUsbCamera[i]->SetFrameWidth(dwFrameWidth);
		m_pUsbCamera[i]->SetFrameHeight(dwFrameHeight);
		m_pUsbCamera[i]->SetFrameSize(dwFrameSize);
		m_pUsbCamera[i]->SetFrameCount(dwFrameCount);
		m_pUsbCamera[i]->SetChannels(nChannels);

		m_pUsbCamera[i]->Initialize();

		if (m_pUsbCamera[i]->Connect(m_mapDevices[i].id))
		{
			m_pUsbCamera[i]->SetId(m_mapDevices[i].id);
			m_pUsbCamera[i]->SetDeviceName(m_mapDevices[i].deviceName);
			m_pUsbCamera[i]->SetDevicePath(m_mapDevices[i].devicePath);
		}
	}

	return TRUE;
}

BOOL CInspectionUsbCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		if (m_pUsbCamera[i] != NULL)
		{
			m_pUsbCamera[i]->StopGrab();
			//Sleep(500);
			m_pUsbCamera[i]->Disconnect();
			delete m_pUsbCamera[i], m_pUsbCamera[i] = NULL;
		}
	}

	return TRUE;
}

int CInspectionUsbCam::EnumerateDevices()
{
	DeviceEnumerator de;

	m_mapDevices = de.getVideoDevicesMap();
	for (auto const& device : m_mapDevices)
	{
		m_vIdDevices.push_back(device.first);
	}
	return m_vIdDevices.size();
}


LPBYTE CInspectionUsbCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pUsbCamera[nCamIdx]->GetBufferImage();
}

LPBYTE CInspectionUsbCam::GetResultBufferImage(int nPosIdx)
{
	if (nPosIdx < 0 || MAX_POSITION_COUNT <= nPosIdx)
		return NULL;

	if (m_ResultImageBuffer[nPosIdx].empty())
		return NULL;

	return (LPBYTE)m_ResultImageBuffer[nPosIdx].data;
}



int CInspectionUsbCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->StartGrab();

	return 1;
}

int CInspectionUsbCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->StopGrab();

	return 1;
}

int CInspectionUsbCam::SingleGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->SingleGrab();

	return 1;
}
