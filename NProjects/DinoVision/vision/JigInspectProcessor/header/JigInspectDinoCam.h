#pragma once
#include "SharedMemoryBuffer.h"
#include "FrameGrabber_UsbCam.h"

#define MAX_CAMERA_COUNT 1
#define MAX_BUFFER_FRAME 15
#define MAX_POSITION_COUNT 1

class AFX_EXT_CLASS CJigInspectDinoCam
{
public:
	CJigInspectDinoCam();
	~CJigInspectDinoCam();

	BOOL                            Initialize();
	BOOL                            Destroy();
	int                             EnumerateDevices();

	LPBYTE                          GetBufferImage(int nCamIdx);

	LPBYTE                          GetResultBufferImage(int nPosIdx);

public:
	//int                             StartGrab(int nCamIdx);
	int                             StopGrab(int nCamIdx);
	int                             SingleGrab(int nCamIdx);

private:
	// Usb Camera
	BOOL							         m_bUsbCamera_ConnectStatus[MAX_CAMERA_COUNT];
	CFramGrabber_UsbCam*                     m_pUsbCamera[MAX_CAMERA_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				         m_csCameraFrameIdx[MAX_CAMERA_COUNT];
	int								         m_pCameraCurrentFrameIdx[MAX_CAMERA_COUNT];
	// Result Buffer
	cv::Mat							         m_ResultImageBuffer[MAX_POSITION_COUNT];
	std::vector<int>                         m_vIdDevices;
	std::map<int, Device>                    m_mapDevices;

};