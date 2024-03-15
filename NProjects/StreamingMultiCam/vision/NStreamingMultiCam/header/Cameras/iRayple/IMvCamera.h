#pragma once
#ifndef _MV_CAMERA_H_
#define _MV_CAMERA_H_

#ifndef MV_NULL
#define MV_NULL    0
#endif

#include "IMVApi.h"
#include "IMVDefines.h"

class CIMVCamera
{
public:
	CIMVCamera();
	virtual ~CIMVCamera();

public:
	// en:Enumerate Device
	static int                              EnumDevices(IMV_DeviceList* pstDevList, unsigned int nTLayerType);
	// en:Open Device
	int                                     Open(IMV_ECreateHandleMode mode, IMV_DeviceInfo deviceInfo, unsigned int idx);
	// en:Close Device
	int                                     Close();
	// en:Is The Device Connected
	bool                                    IsDeviceConnected();
	// en:Register Image Data CallBack
	int                                     RegisterImageCallBack(void(*cbOutput)(IMV_Frame* pFrame, void* pUser), void* pUser);
	// en:Start Grabbing
	int                                     StartGrabbing();
	// en:Stop Grabbing
	int                                     StopGrabbing();
	// en:Get device information
	int                                     GetDeviceInfo(IMV_DeviceInfo* pDevInfo);
	// en:Pixel format conversion
	int                                     ConvertPixelType(IMV_PixelConvertParam* pCvtParam);

	int                                     CommandExecute(IN const char* strKey);

	int                                     SetEnumFeature(const char* pFeature, const char* pSymbol);

	IMV_HANDLE                              GetCameraHandle() { return m_hDevHandle; }

private:
	void* m_hDevHandle;
};

#endif // __MVCAMERA_H__