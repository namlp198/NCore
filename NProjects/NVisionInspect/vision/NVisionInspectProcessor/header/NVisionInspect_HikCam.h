#pragma once

#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "FrameGrabber_MVS_GigE.h"
#include "SharedMemoryBuffer.h"
#include <queue>
#include "NVisionInspectRecipe.h"
#include "NVisionInspectSystemSetting.h"
#include "NVisionInspectResult.h"
#include "NVisionInspectDefine.h"
#include "NVisionInspectStatus.h"
#include "NVisionInspectCameraSetting.h"

//typedef void __stdcall ReceivedImageCallback(LPVOID pBuffer, int nGrabberIndex, int nNextFrameIdx, LPVOID pParam);

interface INVisionInspectHikCamToParent
{
	virtual CNVisionInspectRecipe*              GetRecipeControl() = 0;
	virtual CNVisionInspectSystemSetting*       GetSystemSettingControl() = 0;
	virtual CNVisionInspectCameraSetting*       GetCameraSettingControl(int nCamIdx) = 0;
	virtual CNVisionInspectStatus*              GetStatusControl(int nCoreIdx) = 0;
	virtual BOOL                                SetResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual std::vector<int>                    GetVecCameras() = 0;
};

class AFX_EXT_CLASS CNVisionInspect_HikCam : public IFrameGrabber2Parent
{
public:
	CNVisionInspect_HikCam(INVisionInspectHikCamToParent* pInterface);
	~CNVisionInspect_HikCam();

public:
	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();

	LPBYTE                          GetBufferImage(int nCamIdx);
	BOOL                            GetBufferImage(int nCamIdx, LPBYTE pBuffer);
	BOOL                            GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer);
	CSharedMemoryBuffer*            GetSharedMemoryBuffer(int nCamIdx);
	int							    PopInspectWaitFrame(int nGrabberIdx);

	BOOL                            SetFrameWaitProcess(int nCamIdx);
	LPBYTE                          GetFrameWaitProcess(int nCamIdx, int nFrame);

public:
	// getter
	std::queue<int>                 GetQueueInspectWaitList(int nCamIdx) { return m_queueInspectWaitList[nCamIdx]; }

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int     StartGrab(int nCamIdx);
	int     StopGrab(int nCamIdx);
	int     SoftwareTrigger(int nCamIdx);
	int		SetTriggerMode(int nCamIdx, int nMode);
	int		SetTriggerSource(int nCamIdx, int nSource);
	int     SetExposureTime(int nCamIdx, double dExpTime);
	int     SetAnalogGain(int nCamIdx, double dGain);

private:

	INVisionInspectHikCamToParent*             m_pInterface;

	// Area Camera
	BOOL                                       m_bCamera_ConnectStatus[MAX_CAMERA_INSPECT_COUNT];
	CFrameGrabber_MVS_GigE*                    m_pCamera[MAX_CAMERA_INSPECT_COUNT];

	CCriticalSection                           m_csCameraFrameIdx[MAX_CAMERA_INSPECT_COUNT];

	CSharedMemoryBuffer*                       m_pCameraImageBuffer[MAX_CAMERA_INSPECT_COUNT];

	int                                        m_cameraCurrentFrameIdx[MAX_CAMERA_INSPECT_COUNT];
	int                                        m_currentFrameWaitProcessIdx[MAX_IMAGE_BUFFER];

	// Inspect Wait Frame List
	CCriticalSection			               m_csInspectWaitList[MAX_CAMERA_INSPECT_COUNT];
	std::queue<int>				               m_queueInspectWaitList[MAX_CAMERA_INSPECT_COUNT];

	CSharedMemoryBuffer*                       m_pFrameWaitProcessList[MAX_CAMERA_INSPECT_COUNT];
};