#pragma once

enum IFrameGrabber_Connect_Status { FrameGrabber_NotConnect = 0, FrameGrabber_Connected = 1 };

enum GrabberLibraryType		{ GrabberLibrary_Simulation = 0, GrabberLibrary_Sapera, GrabberLibrary_Mil, GrabberLibrary_Coaxlink, GrabberLibrary_Pylon, GrabberLibrary_Vimba, GrabberLibrary_mvIMPACT, GrabberLibrary_Spinnaker, GrabberLibrary_MVS, GrabberLibrary_StApi, GrabberLibrary_VIS, GrabberLibrary_Multicam, GrabberLibrary_Opencv, GrabberLibrary_Count };
enum GrabberInterfaceType	{ GrabberInterface_Simulation = 0, GrabberInterface_Usb, GrabberInterface_GigE, GrabberInterface_1394, GrabberInterface_CameraLink, GrabberInterface_CoaXPress, GrabberInterface_Count };
enum GrabberBayerType		{ GrabberBayer_None = 0, GrabberBayer_Grbg, GrabberBayer_Gbrg, GrabberBayer_Rggb, GrabberBayer_Bggr, GrabberBayer_Count };
enum CameraSensorType		{ CameraSensor_AreaScan = 0, CameraSensor_LineScan, CameraSensor_TdiScan, CameraSensor_Count };
enum TriggerModeType		{ TriggerMode_Internal=0, TriggerMode_External, TriggerMode_Count };
enum BinningModeType		{ BinningMode_X1 = 0, BinningMode_X2, BinningMode_X4, BinningMode_Count };
enum TriggerSourceType		{ TriggerSource_Software=0, TriggerSource_Hardware, TriggerSource_Count };
enum ScanDirectionType		{ ScanDirection_Forward=0, ScanDirection_Reverse, ScanDirection_Count };
enum GrabReturnValue		{ GrabReturn_NotConnect = -1, GrabReturn_NotGrab, GrabReturn_Grabbing = 1 };

interface IFrameGrabber2Parent
{
	virtual void	DisplayMessage(TCHAR* str, ...) = 0;
	virtual void	DisplayMessage(CString strLogMessage) = 0;
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize) = 0;
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize) = 0;
};

class CFrameGrabberParam

{
public:
	CFrameGrabberParam()			{ Reset(); }
	virtual ~CFrameGrabberParam()	{ Reset(); }
	void Reset()
	{
		m_nLibraryType	= GrabberLibrary_Simulation;
		m_nInterfaceType = GrabberInterface_Simulation; 
		m_nSensorType = CameraSensor_AreaScan;
		m_nSoftwareBayer = GrabberBayer_None;
		m_nFrameWidth	= 0;
		m_nFrameHeight = 0;
		m_nFrameWidthStep = 0;
		m_nFrameDepth	= 8;
		m_nFrameChannels = 1;
		m_nFrameCount	= 10;
		m_dGainRed = 1.0f;
		m_dGainGreen = 1.0f;
		m_dGainBlue = 1.0f;
		m_strCamFile = L"";
		m_dExposureTime = 1000.0f;
	}

	// parent getter
	CString GetParam_GrabberPort() const { return m_strConnectAddr; }
	CString GetParam_ChannelPort() const { return m_strConnectPort; }
	// parent setter
	void SetParam_GrabberPort(const CString& strValue) { m_strConnectAddr = strValue; }
	void SetParam_ChannelPort(const CString& strValue) { m_strConnectPort = strValue; }
	
	// getter
	int	GetParam_LibraryType() const {	return m_nLibraryType; };
	int	GetParam_InterfaceType() const { return m_nInterfaceType; }
	int	GetParam_SensortType() const { return m_nSensorType; }
	int GetParam_SoftwareBayer() const { return m_nSoftwareBayer; }
	int	GetParam_FrameWidth() const { return m_nFrameWidth; }
	int	GetParam_FrameHeight() const { return m_nFrameHeight; }
	int	GetParam_FrameWidthStep() const { return m_nFrameWidthStep; }
	int	GetParam_FrameDepth() const { return m_nFrameDepth; }
	int	GetParam_FrameChannels() const { return m_nFrameChannels; }
	int	GetParam_FrameCount() const { return m_nFrameCount; }
	double GetParam_GainRed() const { return m_dGainRed; }
	double GetParam_GainGreen() const { return m_dGainGreen; }
	double GetParam_GainBlue() const { return m_dGainBlue; }
	CString GetParam_CamFile() const { return m_strCamFile; }
	double GetParam_ExposureTime() const { return m_dExposureTime; }

	// setter
	void SetParam_LibraryType(int nValue) { m_nLibraryType = nValue; }
	void SetParam_InterfaceType(int nValue) { m_nInterfaceType = nValue; }
	void SetParam_SensorType(int nValue) { m_nSensorType = nValue; }
	void SetParam_SoftwareBayer(int nValue) { m_nSoftwareBayer = nValue; }
	void SetParam_FrameWidth(int nValue) { m_nFrameWidth = nValue; }
	void SetParam_FrameHeight(int nValue) { m_nFrameHeight = nValue; }
	void SetParam_FrameWidthStep(int nValue) { m_nFrameWidthStep = nValue; }
	void SetParam_FrameDepth(int nValue) { m_nFrameDepth = nValue; }
	void SetParam_FrameChannels(int nValue) { m_nFrameChannels = nValue; }
	void SetParam_FrameCount(int nValue) { m_nFrameCount = nValue; }
	void SetParam_GainRed(double dValue) { m_dGainRed = dValue; }
	void SetParam_GainGreen(double dValue) { m_dGainGreen = dValue; }
	void SetParam_GainBlue(double dValue) { m_dGainBlue = dValue; }
	void SetParam_CamFile(const CString& strValue) { m_strCamFile = strValue; }
	void SetParam_ExposureTime(double dValue) { m_dExposureTime = dValue; }

protected:
	int		m_nLibraryType;		// library type
	int		m_nInterfaceType;	// interface type
	int		m_nSensorType;		// sensor type
	int		m_nSoftwareBayer;	// software bayer

	CString m_strConnectAddr;
	CString m_strConnectPort;
	
	int		m_nFrameWidth;		// frame width
	int		m_nFrameHeight;		// frame height
	int		m_nFrameWidthStep;	// frame width step
	int		m_nFrameDepth;		// frame depth
	int		m_nFrameChannels;	// frame channels
	int		m_nFrameCount;		// frame count
	double	m_dGainRed;			// red gain
	double	m_dGainGreen;		// green gain
	double	m_dGainBlue;		// blue gain

	CString	m_strCamFile;		// camera file
	double	m_dExposureTime;		// ExposureTime
};

class CFrameGrabberStatus : public CFrameGrabberParam
{
public:
	CFrameGrabberStatus() : CFrameGrabberParam()	{ Reset(); }
	virtual ~CFrameGrabberStatus()	{ Reset(); }
	void Reset()
	{
		CFrameGrabberParam::Reset();
		m_nConnected	= 0;
		m_nGrabbing = 0;

		m_nCurFrameIndex = 0;
		m_nCurFrameCount	= 0;
		m_dwCurFramesize	= 0;

		m_nTriggerMode = TriggerMode_Internal;
		m_nBinningMode = BinningMode_X1;
		m_nTriggerSource = 0;
		m_nScanDirection = ScanDirection_Forward;
		m_dExposureTime = 0.0; // usec
		m_dFrameRate	= 0.0; // frame/sec
		m_dAnalogGain = 0.0; // dB

		m_bInspectFrame = FALSE;
	}

	// getter
	int		GetStatus_Connected() const { return m_nConnected; }
	int		GetStatus_Grabbing() const { return m_nGrabbing; }
	int		GetStatus_CurFrameIndex() const { return m_nCurFrameIndex; }
	int		GetStatus_CurFrameCount() const { return m_nCurFrameCount; }
	DWORD64	GetStatus_CurFrameSize() const { return m_dwCurFramesize; }
	int		GetStatus_TriggerMode() const { return m_nTriggerMode; }
	int		GetStatus_BinningMode() const { return m_nBinningMode; }
	int		GetStatus_TriggerSource() const { return m_nTriggerSource; }
	int		GetStatus_ScanDirection() const { return m_nScanDirection; }
	double	GetStatus_ExposureTime() const { return m_dExposureTime; }
	double	GetStatus_FrameRate() const { return m_dFrameRate; }
	double	GetStatus_AnalogGain() const { return m_dAnalogGain; }
	BOOL	GetStatus_InspectFrame() const { return m_bInspectFrame; }

	CString GetParam_ConnectAddr() { return m_strConnectAddr; };
	CString GetParam_ConnectPort() { return m_strConnectPort; };

	// setter
	void	SetStatus_Connected(int nValue) { m_nConnected = nValue; }
	void	SetStatus_Grabbing(int nValue) { m_nGrabbing = nValue; }
	void	SetStatus_CurFrameIndex(int nIndex) 
	{ 
		m_nCurFrameIndex = (INT_MAX == nIndex) ? 0: nIndex; 
	}
	void	SetStatus_CurFrameCount(int nCount)  { m_nCurFrameCount = nCount; }
	void	SetStatus_CurFrameSize(DWORD64 dwSize) { m_dwCurFramesize = dwSize; }
	void	SetStatus_TriggerMode(int nMode) { m_nTriggerMode = nMode; }
	void	SetStatus_BinningMode(int nMode) { m_nBinningMode = nMode; }
	void	SetStatus_TriggerSource(int nSoruce) { m_nTriggerSource = nSoruce; }
	void	SetStatus_ScanDirection(int nDir) { m_nScanDirection = nDir; }
	void	SetStatus_ExposureTime(double dValue) { m_dExposureTime = dValue; }
	void	SetStatus_FrameRate(double dValue) { m_dFrameRate = dValue; }
	void	SetStatus_AnalogGain(double dValue) { m_dAnalogGain = dValue; }
	void	SetStatus_InspectFrame(BOOL bIns) { m_bInspectFrame = bIns; }

protected:
	int		m_nConnected;			// connect status
	int		m_nGrabbing;			// grab status

	int		m_nCurFrameIndex;		// current frame index
	int		m_nCurFrameCount;		// current frame count
	DWORD64	m_dwCurFramesize;		// current frame size

	int		m_nTriggerMode;			// trigger mode
	int		m_nBinningMode;			// binning mode
	int		m_nTriggerSource;		// trigger source
	int		m_nScanDirection;		// scan direction
	double	m_dExposureTime;		// exposure time
	double	m_dFrameRate;			// frame rate
	double	m_dAnalogGain;			// analog gain

	BOOL	m_bInspectFrame;
};

