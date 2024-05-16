#pragma once

#ifndef _MV_CAMERA_H_
#define _MV_CAMERA_H_

#include "MvCameraControl.h"
#include <string.h>

#ifndef MV_NULL
#define MV_NULL    0
#endif

class CMvCamera
{
public:
	CMvCamera();
	virtual ~CMvCamera();

	// en:Get SDK Version
	static int GetSDKVersion();

	// en:Enumerate Device
	static int EnumDevices(unsigned int nTLayerType, MV_CC_DEVICE_INFO_LIST* pstDevList);

	// en:Enum Interface Based On GenTL
	static int EnumInterfacsByGenTL(MV_GENTL_IF_INFO_LIST* pstIFList, const char * pGenTLPath);

	// en:Enum Device Based On GenTL
	static int EnumDevicesByGenTL(MV_GENTL_IF_INFO* pstIFInfo, MV_GENTL_DEV_INFO_LIST* pstDevList);

	// en:Is the device accessible
	static bool IsDeviceAccessible(MV_CC_DEVICE_INFO* pstDevInfo, unsigned int nAccessMode);

	// en:Open Device
	int Open(MV_CC_DEVICE_INFO* pstDeviceInfo);
	
	// en:Open Device Based On GenTL
	int OpenDeviceByGenTL(MV_GENTL_DEV_INFO* stDeviceInfo);

	// en:Close Device
	int Close();

	// en:Is The Device Connected
	bool IsDeviceConnected();

	// en:Register Image Data CallBack
	int RegisterImageCallBack(void(__stdcall* cbOutput)(unsigned char * pData, MV_FRAME_OUT_INFO_EX* pFrameInfo, void* pUser), void* pUser);

	// en:Start Grabbing
	int StartGrabbing();

	// en:Stop Grabbing
	int StopGrabbing();

	// en:Get one frame initiatively
	int GetImageBuffer(MV_FRAME_OUT* pFrame, int nMsec);

	// en:Free image buffer
	int FreeImageBuffer(MV_FRAME_OUT* pFrame);

	// en:Display one frame image
	int DisplayOneFrame(MV_DISPLAY_FRAME_INFO* pDisplayInfo);

	// en:Set the number of the internal image cache nodes in SDK
	int SetImageNodeNum(unsigned int nNum);

	// en:Get device information
	int GetDeviceInfo(MV_CC_DEVICE_INFO* pstDevInfo);

	// en:Get detect info of GEV camera
	int GetGevAllMatchInfo(MV_MATCH_INFO_NET_DETECT* pMatchInfoNetDetect);

	// en:Get detect info of U3V camera
	int GetU3VAllMatchInfo(MV_MATCH_INFO_USB_DETECT* pMatchInfoUSBDetect);

	// en:Get Int type parameters, such as Width and Height, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int GetIntValue(IN const char* strKey, OUT MVCC_INTVALUE_EX *pIntValue);
	int SetIntValue(IN const char* strKey, IN int64_t nValue);

	// en:Get Enum type parameters, such as PixelFormat, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int GetEnumValue(IN const char* strKey, OUT MVCC_ENUMVALUE *pEnumValue);
	int SetEnumValue(IN const char* strKey, IN unsigned int nValue);
	int SetEnumValueByString(IN const char* strKey, IN const char* sValue);

	// en:Get Float type parameters, such as ExposureTime and Gain, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int GetFloatValue(IN const char* strKey, OUT MVCC_FLOATVALUE *pFloatValue);
	int SetFloatValue(IN const char* strKey, IN float fValue);

	// en:Get Bool type parameters, such as ReverseX, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int GetBoolValue(IN const char* strKey, OUT bool *pbValue);
	int SetBoolValue(IN const char* strKey, IN bool bValue);

	// en:Get String type parameters, such as DeviceUserID, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int GetStringValue(IN const char* strKey, MVCC_STRINGVALUE *pStringValue);
	int SetStringValue(IN const char* strKey, IN const char * strValue);

	// en:Execute Command once, such as UserSetSave, for details please refer to MvCameraNode.xlsx file under SDK installation directory
	int CommandExecute(IN const char* strKey);

	// en:Detection network optimal package size(It only works for the GigE camera)
	int GetOptimalPacketSize(unsigned int* pOptimalPacketSize);

	// en:Register Message Exception CallBack
	int RegisterExceptionCallBack(void(__stdcall* cbException)(unsigned int nMsgType, void* pUser), void* pUser);

	// en:Register Event CallBack
	int RegisterEventCallBack(const char* pEventName, void(__stdcall* cbEvent)(MV_EVENT_OUT_INFO * pEventInfo, void* pUser), void* pUser);

	// en:Force IP
	int ForceIp(unsigned int nIP, unsigned int nSubNetMask, unsigned int nDefaultGateWay);

	// en:IP configuration method
	int SetIpConfig(unsigned int nType);

	// en:Set Net Transfer Mode
	int SetNetTransMode(unsigned int nType);

	// en:Pixel format conversion
	int ConvertPixelType(MV_CC_PIXEL_CONVERT_PARAM* pstCvtParam);

	// en:save image
	int SaveImage(MV_SAVE_IMAGE_PARAM_EX* pstParam);

	// en:Save the image as a file
	int SaveImageToFile(MV_SAVE_IMG_TO_FILE_PARAM* pstParam);

private:

	void*               m_hDevHandle;

};

#endif//_MV_CAMERA_H_