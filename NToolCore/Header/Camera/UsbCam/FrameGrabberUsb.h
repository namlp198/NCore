#pragma once

#include <string>

interface IFrameGrabberUsb2Parent
{
	virtual int IFGU2P_FrameGrabbedUsb(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize) = 0;
};

class CFrameGrabberUsbParam
{
public:
	CFrameGrabberUsbParam() { Reset(); }
	virtual ~CFrameGrabberUsbParam(){}
	void Reset()
	{
		m_nFrameWidth = 0;
		m_nFrameHeight = 0;
		m_nFrameChannels = 1;
		m_nFrameDepth = 8;
		m_nFrameWidthStep = 0;
		m_nFrameCount = 10;

		m_nId = -1;
		m_sDeviceName = "";
		m_sDevicePath = "";
	}
public:
	// getter
	int	GetParam_FrameWidth() const { return m_nFrameWidth; }
	int	GetParam_FrameHeight() const { return m_nFrameHeight; }
	int	GetParam_FrameWidthStep() const { return m_nFrameWidthStep; }
	int	GetParam_FrameDepth() const { return m_nFrameDepth; }
	int	GetParam_FrameChannels() const { return m_nFrameChannels; }
	int	GetParam_FrameCount() const { return m_nFrameCount; }

	int GetParam_DeviceId() const { return m_nId; }
	std::string GetParam_DeviceName() const { return m_sDeviceName; }
	std::string GetParam_DevicePath() const { return m_sDevicePath; }

	// setter
	void SetParam_FrameWidth(int nValue) { m_nFrameWidth = nValue; }
	void SetParam_FrameHeight(int nValue) { m_nFrameHeight = nValue; }
	void SetParam_FrameWidthStep(int nValue) { m_nFrameWidthStep = nValue; }
	void SetParam_FrameDepth(int nValue) { m_nFrameDepth = nValue; }
	void SetParam_FrameChannels(int nValue) { m_nFrameChannels = nValue; }
	void SetParam_FrameCount(int nValue) { m_nFrameCount = nValue; }

	void SetParam_DeviceId(int nValue) { m_nId = nValue; }
	void SetParam_DeviceName(std::string sValue) { m_sDeviceName = sValue; }
	void SetParam_DevicePath(std::string sValue) { m_sDevicePath = sValue; }

protected:
	int             m_nFrameHeight;
	int             m_nFrameWidth;
	int		        m_nFrameWidthStep;	// frame width step
	int		        m_nFrameDepth;		// frame depth
	int		        m_nFrameChannels;	// frame channels
	int		        m_nFrameCount;		// frame count
	int             m_nId;
	std::string     m_sDeviceName;
	std::string     m_sDevicePath;
};

class CFrameGrabberUsbStatus : public CFrameGrabberUsbParam
{
public:
	CFrameGrabberUsbStatus() : CFrameGrabberUsbParam() { Reset(); }
	virtual ~CFrameGrabberUsbStatus() { Reset(); };
	void Reset()
	{
		m_nConnected = 0;
		m_nGrabbing = 0;
		m_nCurFrameCount = 0;
		m_nCurFrameIndex = 0;
		m_dwCurFramesize = 0;
	}

public:

	// getter
	int		GetStatus_Connected() const { return m_nConnected; }
	int		GetStatus_Grabbing() const { return m_nGrabbing; }
	int		GetStatus_CurFrameIndex() const { return m_nCurFrameIndex; }
	int		GetStatus_CurFrameCount() const { return m_nCurFrameCount; }
	DWORD64	GetStatus_CurFrameSize() const { return m_dwCurFramesize; }

	// setter
	void	SetStatus_Connected(int nValue) { m_nConnected = nValue; }
	void	SetStatus_Grabbing(int nValue) { m_nGrabbing = nValue; }
	void	SetStatus_CurFrameIndex(int nIndex)
	{
		m_nCurFrameIndex = (INT_MAX == nIndex) ? 0 : nIndex;
	}
	void	SetStatus_CurFrameCount(int nCount) { m_nCurFrameCount = nCount; }
	void	SetStatus_CurFrameSize(DWORD64 dwSize) { m_dwCurFramesize = dwSize; }

protected:
	int		m_nConnected;			// connect status
	int		m_nGrabbing;			// grab status

	int		m_nCurFrameIndex;		// current frame index
	int		m_nCurFrameCount;		// current frame count
	DWORD64	m_dwCurFramesize;		// current frame size
};



class CFrameGrabberUsb
{
public:
	CFrameGrabberUsb(int nIdCam, IFrameGrabberUsb2Parent* pIFGU2P=NULL);
	virtual ~CFrameGrabberUsb(void);

public: // pure virtual functions
	// connect disconnect
	virtual int		Connect(const CFrameGrabberUsbParam& grabberParam) = 0;
	virtual int		Disconnect() = 0;

	// grab command
	virtual void		StartContinuousGrab() = 0;
	virtual void		StopContinuousGrab() = 0;
	virtual void        SingleGrab() = 0;
	virtual int         EnumerateDevices() = 0;

public: // virtual functions

	// status getter
	virtual int		GetConnected()      { return m_GrabberUsbStatus.GetStatus_Connected(); }
	virtual int		GetGrabbing()       { if (!GetConnected()) return 0; return m_GrabberUsbStatus.GetStatus_Grabbing(); }
	virtual int		GetCurFrameIndex()  { if (!GetConnected()) return -1; return m_GrabberUsbStatus.GetStatus_CurFrameIndex(); }
	virtual int		GetCurFrameCount()  { if (!GetConnected()) return -1; return m_GrabberUsbStatus.GetStatus_CurFrameCount(); }

public: // common functions
	const CFrameGrabberUsbStatus* GetStatus()	const { return &m_GrabberUsbStatus; }

protected: // virtual functions
// status setter
	virtual void	SetConnected(int nConnected) { m_GrabberUsbStatus.SetStatus_Connected(nConnected); }
	virtual void	SetGrabbing(int nGrabbing) { m_GrabberUsbStatus.SetStatus_Grabbing(nGrabbing); }
	virtual void	SetCurFrameIndex(int nFrameIndex) { m_GrabberUsbStatus.SetStatus_CurFrameIndex(nFrameIndex); }
	virtual void	SetCurFrameCount(int nFrameCount) { m_GrabberUsbStatus.SetStatus_CurFrameCount(nFrameCount); }

protected:

	int IFGU2P_FrameGrabbed(int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);

protected:
	int						    m_nIdCam;

	CFrameGrabberUsbStatus 		m_GrabberUsbStatus;
	CCriticalSection*           m_pFrameCriticalSection;

protected:
	IFrameGrabberUsb2Parent*    m_pIFGU2P;

};