#pragma once
#include "SharedMemoryBuffer.h"
#include "FrameGrabber_UsbCam.h"
#include "JigInspectConfig.h"
#include "JigInspectRecipe.h"
#include "JigInspectResults.h"
#include "JigInspectDefine.h"

interface IJigInspectDinoCamToParent
{
	virtual CJigInspectRecipe*          GetRecipe(int nCamIdx) = 0;
	virtual CJigInspectCameraConfig*    GetCameraConfig(int nCamIdx) = 0;
	virtual CJigInspectSystemConfig*    GetSystemConfig() = 0;
	virtual CJigInspectResults*         GetJigInspectResult(int nCamIdx) = 0;
	virtual void				        InspectComplete() = 0;
};

class AFX_EXT_CLASS CJigInspectDinoCam
{
public:
	CJigInspectDinoCam(IJigInspectDinoCamToParent* pInterface);
	~CJigInspectDinoCam();

	BOOL                            Initialize();
	BOOL                            Destroy();
	int                             EnumerateDevices();

	LPBYTE                          GetBufferImage(int nCamIdx);

	LPBYTE                          GetResultBufferImage(int nCamIdx);

public:
	int                             StartGrab(int nCamIdx);
	int                             StopGrab(int nCamIdx);
	int                             SingleGrab(int nCamIdx);

	int                             Connect(int nCamIdx);
	int                             Disconnect(int nCamIdx);

public:
	BOOL                            CreateResultBuffer(int nCamIdx, CFramGrabber_UsbCam* pUsbCam);
	BOOL                            InspectStart(int nCamIdx);

private:

	IJigInspectDinoCamToParent*              m_pInterface;
	// Usb Camera
	BOOL							         m_bUsbCamera_ConnectStatus[MAX_CAMERA_INSP_COUNT];
	CFramGrabber_UsbCam*                     m_pUsbCamera[MAX_CAMERA_INSP_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				         m_csCameraFrameIdx[MAX_CAMERA_INSP_COUNT];
	int								         m_pCameraCurrentFrameIdx[MAX_CAMERA_INSP_COUNT];
	// Result Buffer
	CSharedMemoryBuffer*                     m_pResultImageBuffer[MAX_CAMERA_INSP_COUNT];
	std::vector<int>                         m_vIdDevices;
	std::map<int, Device>                    m_mapDevices;

};