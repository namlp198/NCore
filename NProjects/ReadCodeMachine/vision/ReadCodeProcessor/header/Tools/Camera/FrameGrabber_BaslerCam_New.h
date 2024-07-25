#pragma once

#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include <pylon/PylonIncludes.h>
#include <pylon/BaslerUniversalInstantCamera.h>

class CFrameGrabber_BaslerCam_New : public CFrameGrabber
	, public Pylon::CImageEventHandler
	, public Pylon::CConfigurationEventHandler
	, public Pylon::CCameraEventHandler
{
public:
	CFrameGrabber_BaslerCam_New(int nIndex = 0, IFrameGrabber2Parent* pIFG2P = NULL);
	~CFrameGrabber_BaslerCam_New();

public:
	virtual int     Connect(const CFrameGrabberParam& grabberParam);
	virtual int     Disconnect();
				    
	virtual int     StartGrab();
	virtual int     StopGrab();
	virtual int     SingleGrab();
				    
	virtual int     SendTrigger(int nTriggerCount = 1);

	// setter
	virtual	int		SetTriggerMode(int nMode);
	virtual	int		SetTriggerSource(int nSource);
	virtual int		SetExposureTime(double dTime);
	virtual int		SetAnalogGain(double dGain);
	virtual	int		SetFrameRate(double dRate);

	// getter
	virtual int		GetConnected();
	virtual int		GetGrabbing();
	Pylon::CGrabResultPtr GetGrabResult() { return m_ptrGrabResult; }

public:
	Pylon::IIntegerEx& GetExposureTime();
	Pylon::IIntegerEx& GetGain();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::PixelFormatEnums>& GetPixelFormat();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerModeEnums>& GetTriggerMode();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerSourceEnums>& GetTriggerSource();

public:
	// Pylon::CImageEventHandler functions
	virtual void OnImagesSkipped(Pylon::CInstantCamera& camera, size_t countOfSkippedImages);

	// CallBack function
	virtual void OnImageGrabbed(Pylon::CInstantCamera& camera, const Pylon::CGrabResultPtr& grabResult);

private:
	int                             EnumerateDevices();

private:

	// The pylon camera object.
	Pylon::CBaslerUniversalInstantCamera* m_pCamera;
	// The grab result retrieved from the camera.
	Pylon::CGrabResultPtr m_ptrGrabResult;

	// Configurations to apply when starting single or continuous grab.
	Pylon::CAcquireSingleFrameConfiguration m_singleConfiguration;
	Pylon::CAcquireContinuousConfiguration m_continuousConfiguration;

	// Smart pointer to camera features
	Pylon::CIntegerParameter m_exposureTime;
	Pylon::CIntegerParameter m_gain;

	// Enumerate Device list
	Pylon::DeviceInfoList_t m_devices;
	std::vector<const Pylon::CDeviceInfo*> m_vDeviceInfo;

	// Protects members.
	mutable CCriticalSection m_MemberLock;
};