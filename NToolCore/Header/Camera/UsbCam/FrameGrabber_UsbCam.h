#pragma once
#include "FrameGrabberUsb.h"
#include <thread>
#include <chrono>
#include "DeviceEnumerator.h"
#include <opencv2/core.hpp>
#include <opencv2/videoio.hpp>

class CFramGrabber_UsbCam : public CFrameGrabberUsb
{
public:
	CFramGrabber_UsbCam(int nCamId = 0, IFrameGrabberUsb2Parent* pIFGU2P = NULL);
	~CFramGrabber_UsbCam();

public:
	virtual int     Connect(const CFrameGrabberUsbParam& grabberParam);
	virtual int     Disconnect();

	virtual void     StartContinuousGrab();
	virtual void     StopContinuousGrab();
	virtual void     SingleGrab();

	// getter
	virtual int		GetConnected();
	virtual int		GetGrabbing();

public:
	virtual int     EnumerateDevices();

private:
	cv::VideoCapture*        m_pCamera;
	cv::Mat*                 m_pLastFrame;
	std::vector<int>         m_vIdDevice;

	// Protects members.
	mutable CCriticalSection m_MemberLock;

	int m_bStartGrab;

	/*typedef void (*ImageGrabbedEventCallBack)(void*);

	typedef struct {
		void* data;
		ImageGrabbedEventCallBack callback;
	}EventImageCallBack;*/
};