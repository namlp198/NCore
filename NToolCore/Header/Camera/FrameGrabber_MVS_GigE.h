#pragma once

#include "FrameGrabber.h"

#ifdef _DEBUG
#pragma comment(lib, "MvCameraControl.lib")
#else
#pragma comment(lib, "MvCameraControl.lib")
#endif

class CMvCamera;

class AFX_EXT_CLASS CFrameGrabber_MVS_GigE : public CFrameGrabber
{
public:
	CFrameGrabber_MVS_GigE(int nIndex = 0, IFrameGrabber2Parent* pIFG2P = NULL);
	virtual ~CFrameGrabber_MVS_GigE();

public: // pure virtual functions
// connect disconnect
	virtual int		Connect(const CFrameGrabberParam& grabberParam);
	virtual int		Disconnect();

	// grab command
	virtual int		StartGrab();
	virtual int		StopGrab();

	// trigger command
	virtual int		SendTrigger(int nTriggerCount = 1);

	// setter
	virtual	int		SetTriggerMode(int nMode);
	virtual	int		SetTriggerSource(int nSource);
	virtual int		SetExposureTime(double dTime);
	virtual int		SetAnalogGain(double dGain);
	virtual	int		SetFrameRate(double dRate);

	// getter
	virtual int		GetConnected();
	virtual int		GetGrabbing();

public:
	void ImageCallBack(unsigned char * pData, void* pFrameInfo);

protected:
	CMvCamera *m_pCamera;

};

