#include "pch.h"
#include "FrameGrabber_BaslerCam_New.h"

CFrameGrabber_BaslerCam_New::CFrameGrabber_BaslerCam_New(int nIndex /*= 0*/, IFrameGrabber2Parent* pIFG2P /*= NULL*/)
{
	Pylon::PylonInitialize();
	m_pCamera = new Pylon::CBaslerUniversalInstantCamera;

	// Register this object as an image event handler in order to get notified of new images.
   // See Pylon::CImageEventHandler for details.
	m_pCamera->RegisterImageEventHandler(this, Pylon::RegistrationMode_ReplaceAll, Pylon::Cleanup_None);

	// Register this object as a configuration event handler in order to get notified of camera state changes.
	// See Pylon::CConfigurationEventHandler for details.
	m_pCamera->RegisterConfiguration(new Pylon::CAcquireContinuousConfiguration(), Pylon::RegistrationMode_ReplaceAll, Pylon::Cleanup_Delete);
	m_pCamera->RegisterConfiguration(this, Pylon::RegistrationMode_Append, Pylon::Cleanup_None);
}

CFrameGrabber_BaslerCam_New::~CFrameGrabber_BaslerCam_New()
{
	Disconnect();
	if (m_pCamera != NULL)
	{
		delete m_pCamera;
		m_pCamera = NULL;
	}
}

int CFrameGrabber_BaslerCam_New::Connect(const CFrameGrabberParam& grabberParam)
{
	m_GrabberStatus.Reset();
	CFrameGrabberParam* pGrabberParam = dynamic_cast<CFrameGrabberParam*>(&m_GrabberStatus);
	*pGrabberParam = grabberParam;

	USES_CONVERSION;
	char str_serialNum[200];
#ifdef _UNICODE
	sprintf_s(str_serialNum, "%s", W2A(m_GrabberStatus.GetParam_ConnectAddr()));
#else
	sprintf_s(str_serialNum, "%s", strFilename);
#endif

	int deviceCount = EnumerateDevices();
	if (deviceCount == 0)
		return 0;

	for (unsigned int i = 0; i < m_vDeviceInfo.size(); i++)
	{
		if (NULL == m_vDeviceInfo[i])
			break;

		Pylon::String_t TLayer = m_vDeviceInfo[i]->GetTLType();
		Pylon::String_t Seri = m_vDeviceInfo[i]->GetSerialNumber();

		char chTLayer[10];
#ifdef _UNICODE
		sprintf_s(chTLayer, "%s", W2A(CUtf82W(TLayer)));
#else
		sprintf_s(chTLayer, "%s", strFilename);
#endif

		char chSeri[200];
#ifdef _UNICODE
		sprintf_s(chSeri, "%s", W2A(CUtf82W(Seri)));
#else
		sprintf_s(chSeri, "%s", strFilename);
#endif

		//const Pylon::CDeviceInfo* deviceInfo;

		int nValue = strcmp(str_serialNum, chSeri);
		if (0 == nValue)
		{
			CSingleLock lock(&m_MemberLock, TRUE);
			try
			{
				// Create the device and attach it to CInstantCamera.
				// Let CInstantCamera take care of destroying the device.
				Pylon::IPylonDevice* pDevice = Pylon::CTlFactory::GetInstance().CreateDevice(*m_vDeviceInfo[i]);
				m_pCamera->Attach(pDevice, Pylon::Cleanup_Delete);
				m_pCamera->Open();

				if (m_pCamera->IsOpen())
				{
					// Get the ExposureTime feature.
					// On GigE cameras, the feature is called 'ExposureTimeRaw'.
					// On USB cameras, it is called 'ExposureTime'.
					if (m_pCamera->ExposureTime.IsValid())
					{
						// We need the integer representation because the GUI controls can only use integer values.
						// If it doesn't exist, return an empty parameter.
						m_pCamera->ExposureTime.GetAlternativeIntegerRepresentation(m_exposureTime);
					}
					else if (m_pCamera->ExposureTimeRaw.IsValid())
					{
						m_exposureTime.Attach(m_pCamera->ExposureTimeRaw.GetNode());
					}

					int64_t valueExp = m_exposureTime.GetValue();
					m_GrabberStatus.SetStatus_ExposureTime((double)valueExp);

					// Get the Gain feature.
					// On GigE cameras, the feature is called 'GainRaw'.
					// On USB cameras, it is called 'Gain'.
					if (m_pCamera->Gain.IsValid())
					{
						// We need the integer representation for the this sample
						// If it doesn't exist, return an empty parameter.
						m_pCamera->Gain.GetAlternativeIntegerRepresentation(m_gain);
					}
					else if (m_pCamera->GainRaw.IsValid())
					{
						m_gain.Attach(m_pCamera->GainRaw.GetNode());
					}

					int64_t valueGain = m_gain.GetValue();
					m_GrabberStatus.SetStatus_AnalogGain((double)valueGain);

					SetTriggerMode(TriggerMode_Internal);
					SetTriggerSource(TriggerSource_Hardware);
					
					// Add event handlers that will be called when the feature changes.

					m_GrabberStatus.SetStatus_Connected(FrameGrabber_Connected);
					m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);

					if (m_exposureTime.IsValid())
					{
						// If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
						m_pCamera->RegisterCameraEventHandler(this, m_exposureTime.GetNode()->GetName(), 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);
					}

					if (m_gain.IsValid())
					{
						// If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
						m_pCamera->RegisterCameraEventHandler(this, m_gain.GetNode()->GetName(), 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);
					}

					m_pCamera->RegisterCameraEventHandler(this, "PixelFormat", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);

					m_pCamera->RegisterCameraEventHandler(this, "TriggerMode", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);

					m_pCamera->RegisterCameraEventHandler(this, "TriggerSource", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);

					return 1;
				}
			}
			catch (const Pylon::GenericException& e)
			{
				UNUSED(e);
				TRACE(CUtf82W(e.GetDescription()));

				m_GrabberStatus.SetStatus_Connected(FrameGrabber_NotConnect);
				m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
				m_vDeviceInfo.clear();

				// Undo everthing.
				Disconnect();
				return 0;
			}
		}
		return 0;
	}
}

int CFrameGrabber_BaslerCam_New::Disconnect()
{
	CSingleLock lock(&m_MemberLock, TRUE);
	// Stop the grab, so the grab thread will not set new m_ptrGrabResult.
	StopGrab();

	// Free the grab result, if present.
	m_ptrGrabResult.Release();

	// Remove the event handlers that will be called when the feature changes.
	m_pCamera->DeregisterCameraEventHandler(this, "TriggerSource");

	m_pCamera->DeregisterCameraEventHandler(this, "TriggerMode");

	m_pCamera->DeregisterCameraEventHandler(this, "PixelFormat");

	if (m_gain.IsValid())
	{
		// If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
		m_pCamera->DeregisterCameraEventHandler(this, m_gain.GetNode()->GetName());
	}

	if (m_exposureTime.IsValid())
	{
		// If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
		m_pCamera->DeregisterCameraEventHandler(this, m_exposureTime.GetNode()->GetName());
	}

	// Clear the pointers to the features we set manually in Open().
	m_exposureTime.Release();
	m_gain.Release();

	// Close the camera and free all ressources.
	// This will also unregister all 
	m_pCamera->DestroyDevice();

	return 1;
}

int CFrameGrabber_BaslerCam_New::StartGrab()
{
	// Camera may have been disconnected.
	if (!m_pCamera->IsOpen())
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (m_pCamera->IsGrabbing()) // grabbing
	{
		return GrabReturn_Grabbing;
	}

	// Try set continuous frame mode if available
	m_pCamera->AcquisitionMode.TrySetValue(Basler_UniversalCameraParams::AcquisitionMode_Continuous);

	// Start grabbing until StopGrabbing() is called.
	m_pCamera->StartGrabbing(Pylon::GrabStrategy_OneByOne, Pylon::GrabLoop_ProvidedByInstantCamera);

	if (!m_pCamera->IsGrabbing())
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
		return 0;
	}

	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_Grabbing);
	return 1;
}

int CFrameGrabber_BaslerCam_New::StopGrab()
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (GetGrabbing() == GrabReturn_NotGrab) // grabbing?
	{
		return 1;
	}

	m_pCamera->StopGrabbing();

	if (!m_pCamera->IsGrabbing())
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
		return 1;
	}
}

int CFrameGrabber_BaslerCam_New::SingleGrab()
{
	// Camera may have been disconnected.
	if (!m_pCamera->IsOpen() || m_pCamera->IsGrabbing())
	{
		return 0;
	}

	// Try set single frame mode if available
	m_pCamera->AcquisitionMode.TrySetValue(Basler_UniversalCameraParams::AcquisitionMode_SingleFrame);

	// Grab one image.
	// When the image is received, pylon will call the OnImageGrab() handler.
	m_pCamera->StartGrabbing(1, Pylon::GrabStrategy_OneByOne, Pylon::GrabLoop_ProvidedByInstantCamera);

	return 1;
}

int CFrameGrabber_BaslerCam_New::SendTrigger(int nTriggerCount)
{
	if (!m_pCamera->IsGrabbing())
	{
		return 0;
	}

	// Only wait if software trigger is currently turned on.
	if (m_pCamera->TriggerSource.GetValue() == Basler_UniversalCameraParams::TriggerSource_Software
		&& m_pCamera->TriggerMode.GetValue() == Basler_UniversalCameraParams::TriggerMode_On)
	{
		// If the camera is currently processing a previous trigger command,
		// it will silently discard trigger commands.
		// We wait until the camera is ready to process the next trigger.
		m_pCamera->WaitForFrameTriggerReady(3000, Pylon::TimeoutHandling_ThrowException);
	}
	// Send trigger
	m_pCamera->ExecuteSoftwareTrigger();
}

int CFrameGrabber_BaslerCam_New::SetTriggerMode(int nMode)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (GetTriggerMode().IsWritable())
	{
		switch (nMode)
		{
		case TriggerMode_Internal:
			GetTriggerMode().SetIntValue(TriggerMode_Internal);
			break;

		case TriggerMode_External:
			GetTriggerMode().SetIntValue(TriggerMode_External);
			break;

		default:
			return 0;
			break;
		}
	}
	m_GrabberStatus.SetStatus_TriggerMode(nMode);
	return 1;
}

int CFrameGrabber_BaslerCam_New::SetTriggerSource(int nSource)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (GetTriggerSource().IsWritable())
	{
		switch (nSource)
		{
		case TriggerSource_Software:
			GetTriggerSource().SetIntValue(TriggerSource_Software);
			break;

		case TriggerSource_Hardware:
			GetTriggerSource().SetIntValue(TriggerSource_Hardware);
			break;

		default:
			return 0;
			break;
		}
	}
	m_GrabberStatus.SetStatus_TriggerSource(nSource);
	return 1;
}

int CFrameGrabber_BaslerCam_New::SetExposureTime(double dTime)
{
	return 0;
}

int CFrameGrabber_BaslerCam_New::SetAnalogGain(double dGain)
{
	return 0;
}

int CFrameGrabber_BaslerCam_New::SetFrameRate(double dRate)
{
	return 0;
}

int CFrameGrabber_BaslerCam_New::GetConnected()
{
	if (nullptr == m_pCamera)
	{
		m_GrabberStatus.SetStatus_Connected(FrameGrabber_NotConnect);
		return FrameGrabber_NotConnect;
	}

	bool bConnect = m_pCamera->IsOpen();
	m_GrabberStatus.SetStatus_Connected(bConnect);

	return m_GrabberStatus.GetStatus_Connected();
}

int CFrameGrabber_BaslerCam_New::GetGrabbing()
{
	if (GetConnected() != FrameGrabber_Connected) // not connect
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return GrabReturn_NotConnect;
	}
	if (m_pCamera->IsGrabbing())
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_Grabbing);
	else
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);

	return m_GrabberStatus.GetStatus_Grabbing();
}

Pylon::IIntegerEx& CFrameGrabber_BaslerCam_New::GetExposureTime()
{
	return m_exposureTime;
}

Pylon::IIntegerEx& CFrameGrabber_BaslerCam_New::GetGain()
{
	return m_gain;
}

Pylon::IEnumParameterT<Basler_UniversalCameraParams::PixelFormatEnums>& CFrameGrabber_BaslerCam_New::GetPixelFormat()
{
	return m_pCamera->PixelFormat;
}

Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerModeEnums>& CFrameGrabber_BaslerCam_New::GetTriggerMode()
{
	return m_pCamera->TriggerMode;
}

Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerSourceEnums>& CFrameGrabber_BaslerCam_New::GetTriggerSource()
{
	return m_pCamera->TriggerSource;
}

void CFrameGrabber_BaslerCam_New::OnImagesSkipped(Pylon::CInstantCamera& camera, size_t countOfSkippedImages)
{

}

void CFrameGrabber_BaslerCam_New::OnImageGrabbed(Pylon::CInstantCamera& camera, const Pylon::CGrabResultPtr& grabResult)
{
	// When overwriting the current CGrabResultPtr, the previous result will automatically be
	// released and reused by CInstantCamera.
	CSingleLock lockGrabResult(&m_MemberLock, TRUE);
	m_ptrGrabResult = grabResult;
	lockGrabResult.Unlock();

	if (grabResult.IsValid() && grabResult->GrabSucceeded())
	{
		// call interface func
		CSingleLock lockBuffer(&m_MemberLock, TRUE);
		IFG2P_FrameGrabbed(0, reinterpret_cast<uint8_t*>(grabResult->GetBuffer()), grabResult->GetBufferSize());
		lockBuffer.Unlock();
	}
}

int CFrameGrabber_BaslerCam_New::EnumerateDevices()
{
	//Pylon::PylonInitialize();

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
