#include "pch.h"
#include "FrameGrabber_UsbCam.h"

CFramGrabber_UsbCam::CFramGrabber_UsbCam(int nCamId, IFrameGrabberUsb2Parent* pIFGU2P) : CFrameGrabberUsb(nCamId, pIFGU2P)
{
	m_pCamera = new cv::VideoCapture;
}

CFramGrabber_UsbCam::~CFramGrabber_UsbCam()
{
	Disconnect();
}

int CFramGrabber_UsbCam::Connect(const CFrameGrabberUsbParam& grabberParam)
{
	m_GrabberUsbStatus.Reset();
	CFrameGrabberUsbParam* pGrabberParamUsb = dynamic_cast<CFrameGrabberUsbParam*>(&m_GrabberUsbStatus);
	*pGrabberParamUsb = grabberParam;

	int deviceCount = EnumerateDevices();
	if (deviceCount == 0)
		return 0;

	for (int i = 0; i < deviceCount; i++)
	{
		if (pGrabberParamUsb->GetParam_DeviceId() == m_vIdDevice[i])
		{
			if (m_pCamera != NULL)
				m_pCamera->open(m_vIdDevice[i]);

			if (m_pCamera->isOpened())
				m_GrabberUsbStatus.SetStatus_Connected(1);
		}
	}
}

int CFramGrabber_UsbCam::Disconnect()
{
	if (m_pCamera != NULL)
		m_pCamera->release();

	delete m_pCamera;
	m_pCamera = NULL;
	return 1;
}

void CFramGrabber_UsbCam::StartContinuousGrab()
{
	if (m_GrabberUsbStatus.GetStatus_Connected() == 0)
	{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return;
	}

	CSingleLock lockLocal(&m_MemberLock, TRUE);
	m_bStartGrab = 1;
	lockLocal.Unlock();

	m_GrabberUsbStatus.SetStatus_Grabbing(1);

	while (m_bStartGrab)
	{
		// Read next frame and save into m_pLastFrame
		m_pCamera->read(*m_pLastFrame);

		// check m_pLastFram is null
		if (!m_pLastFrame->empty())
		{
			// call interface func
			CSingleLock lockBuffer(&m_MemberLock, TRUE);
			int size = m_GrabberUsbStatus.GetParam_FrameWidth() * m_GrabberUsbStatus.GetParam_FrameHeight() * m_GrabberUsbStatus.GetParam_FrameChannels() * m_GrabberUsbStatus.GetParam_FrameCount();;
			BYTE* pData = new BYTE[size];
			memcpy(pData, m_pLastFrame->data, size);
			IFGU2P_FrameGrabbed(0, pData, size);
			lockBuffer.Unlock();

			delete[] pData;
			pData = NULL;
		}

		std::this_thread::sleep_for(std::chrono::milliseconds(33));
	}

}

void CFramGrabber_UsbCam::StopContinuousGrab()
{
	if (m_GrabberUsbStatus.GetStatus_Connected() == 0)
	{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return;
	}
	CSingleLock lockLocal(&m_MemberLock, TRUE);
	m_bStartGrab = 0;
	m_GrabberUsbStatus.SetStatus_Grabbing(0);
	lockLocal.Unlock();
}

void CFramGrabber_UsbCam::SingleGrab()
{
	if (m_GrabberUsbStatus.GetStatus_Connected() == 0)
	{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return;
	}
	m_pCamera->read(*m_pLastFrame);

	// call interface func
	CSingleLock lockBuffer(&m_MemberLock, TRUE);
	int size = m_GrabberUsbStatus.GetParam_FrameWidth() * m_GrabberUsbStatus.GetParam_FrameHeight() * m_GrabberUsbStatus.GetParam_FrameChannels() * m_GrabberUsbStatus.GetParam_FrameCount();;
	BYTE* pData = new BYTE[size];
	memcpy(pData, m_pLastFrame->data, size);
	IFGU2P_FrameGrabbed(0, pData, size);
	lockBuffer.Unlock();

	delete[] pData;
	pData = NULL;
}

int CFramGrabber_UsbCam::GetConnected()
{
	if (m_pCamera == nullptr)
	{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return 0;
	}

	bool bValue = m_pCamera->isOpened();
	if (bValue)
	{
		m_GrabberUsbStatus.SetStatus_Connected(1);
		return 1;
	}
	else{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return 0;
	}
}

int CFramGrabber_UsbCam::GetGrabbing()
{
	if (m_pCamera == nullptr)
	{
		m_GrabberUsbStatus.SetStatus_Connected(0);
		return 0;
	}
	return m_GrabberUsbStatus.GetStatus_Grabbing();
}

int CFramGrabber_UsbCam::EnumerateDevices()
{
	DeviceEnumerator de;
	std::map<int, Device> devices;

	devices = de.getVideoDevicesMap();
	for (auto const& device : devices)
	{
		m_GrabberUsbStatus.SetParam_DeviceId(device.first);
		m_GrabberUsbStatus.SetParam_DeviceName(device.second.deviceName);
		m_GrabberUsbStatus.SetParam_DevicePath(device.second.devicePath);

		m_vIdDevice.push_back(device.first);
	}
	return m_vIdDevice.size();
}

