#pragma once

#include "FrameGrabber.h"
#include "IMvCamera.h"

//#ifdef _DEBUG
//#pragma comment(lib, "MVSDKmd.lib")
//#else
//#pragma comment(lib, "MVSDKmd.lib")
//#endif
class CIMVCamera;

class AFX_EXT_CLASS CFrameGrabber_iRayple : public CFrameGrabber
{
public:
	CFrameGrabber_iRayple(int nIndex = 0, IFrameGrabber2Parent* pIFG2P = NULL);
	virtual ~CFrameGrabber_iRayple();

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

	virtual void               SetPixelFormat(IMV_EPixelType format) { m_ePixelType = format; }
	virtual IMV_EPixelType     GetPixelFormat() { return m_ePixelType; }

public:
	void ImageCallback(IMV_Frame* pFrame, void* pUser);

protected:
	CIMVCamera* cImvCamera;
private:
	void ImageConvert(IMV_Frame frame, IMV_EPixelType convertFormat);

private:
	IMV_PixelConvertParam               m_stPixelConvertParam;
	BYTE*                               m_pDstBuf = NULL;
	unsigned int			            m_nDstBufSize = 0;
	IMV_EPixelType                      m_ePixelType = gvspPixelBGR8;
};