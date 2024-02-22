#pragma once
#include "FrameGrabber_BaslerCam.h"

#define MAX_CAMERA_COUNT 2

#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024
#define FRAME_COUNT 1

class AFX_EXT_CLASS CInspectionBaslerCam
{
public:
	CInspectionBaslerCam();
	~CInspectionBaslerCam();

public:
	BOOL                            Initialize();
	BOOL                            Destroy();
	LPBYTE                          GetBufferImage(int nCamIdx);
	BOOL                            LiveCamera(int nCamIdx);

private:
	int                             EnumerateDevices();

private:
	CFrameGrabber_BaslerCam* m_pBaslerCam[MAX_CAMERA_COUNT];

	// Enumerate Device list
	Pylon::DeviceInfoList_t m_devices;
	std::vector<const Pylon::CDeviceInfo*> m_vDeviceInfo;
};