#include "pch.h"
#include "InspectionBaslerCam.h"

CInspectionBaslerCam::CInspectionBaslerCam()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_pBaslerCam[i] = NULL;
	}
}

CInspectionBaslerCam::~CInspectionBaslerCam()
{
	Destroy();
}

BOOL CInspectionBaslerCam::Initialize()
{
    Pylon::PylonInitialize();

    // Enumerate device list found.
    int deviceCount = EnumerateDevices();
    if (deviceCount == 0)
        return FALSE;

    DWORD dwFrameWidth = (DWORD)FRAME_WIDTH;
    DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT;
    DWORD dwFrameSize = dwFrameWidth * dwFrameHeight;
    DWORD dwFrameCount = (DWORD)FRAME_COUNT;

    int devicesCur = (deviceCount < MAX_CAMERA_COUNT) ? deviceCount : MAX_CAMERA_COUNT;

	for (int i = 0; i < devicesCur; i++)
	{
		m_pBaslerCam[i] = new CFrameGrabber_BaslerCam;
        m_pBaslerCam[i]->SetFrameWidth(dwFrameWidth);
        m_pBaslerCam[i]->SetFrameHeight(dwFrameHeight);
        m_pBaslerCam[i]->SetFrameSize(dwFrameSize);
        m_pBaslerCam[i]->SetFrameCount(dwFrameCount);

        CString strMemory;
        strMemory.Format(_T("%s_%d"), "BufferBasler", i);

        m_pBaslerCam[i]->SetMapFileName(strMemory);

        m_pBaslerCam[i]->Initialize();
        m_pBaslerCam[i]->Open(*m_vDeviceInfo[i]);
	}

    return TRUE;
}

BOOL CInspectionBaslerCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_COUNT; i++) 
	{
		if (m_pBaslerCam[i] != NULL)
		{
			m_pBaslerCam[i]->Destroy();
			delete m_pBaslerCam, m_pBaslerCam[i] = NULL;
		}
	}
	return TRUE;
}

LPBYTE CInspectionBaslerCam::GetBufferImage(int nCamIdx)
{
    if (m_pBaslerCam[nCamIdx] == NULL)
        return NULL;

    return m_pBaslerCam[nCamIdx]->GetBufferImage();
}

BOOL CInspectionBaslerCam::LiveCamera(int nCamIdx)
{
    if (m_pBaslerCam[nCamIdx] == NULL)
        return FALSE;

    m_pBaslerCam[nCamIdx]->ContinuousGrab();
    return TRUE;
}

int CInspectionBaslerCam::EnumerateDevices()
{
    Pylon::DeviceInfoList_t devices;
    try
    {
        // Get the transport layer factory.
        Pylon::CTlFactory& TlFactory = Pylon::CTlFactory::GetInstance();

        // Get all attached cameras.
        TlFactory.EnumerateDevices(devices);
    }
    catch (const Pylon::GenericException& e)
    {
        UNUSED(e);
        devices.clear();
    }

    m_devices = devices;

    // Fill the combo box.
    for (Pylon::DeviceInfoList_t::const_iterator it = m_devices.begin(); it != m_devices.end(); ++it)
    {
        // Get the pointer to the current device info.
        const Pylon::CDeviceInfo* const pDeviceInfo = &(*it);
        m_vDeviceInfo.push_back(pDeviceInfo);
    }
    // When calling this function, make sure to update the device list control,
    // because its items store pointers to elements in the m_devices list.
    return (int)m_devices.size();
}
