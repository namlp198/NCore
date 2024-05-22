#pragma once
#include "FrameGrabber_MVS_GigE.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "SharedMemoryBuffer.h"
#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectSystemSetting.h"
#include <queue>

interface ISealingInspectHikCamToParent
{
	virtual CSealingInspectRecipe* GetRecipe() = 0;
	virtual CSealingInspectSystemSetting* GetSystemSetting() = 0;
};

class AFX_EXT_CLASS CSealingInspectHikCam : public IFrameGrabber2Parent
{
public:
	CSealingInspectHikCam(ISealingInspectHikCamToParent* pInterface);
	~CSealingInspectHikCam();

public:
	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();

	LPBYTE                          GetBufferImage(int nCamIdx);
	BOOL                            GetBufferImage(int nCamIdx, LPBYTE pBuffer);
	BOOL                            GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer);
	CSharedMemoryBuffer*            GetSharedMemoryBuffer(int nCamIdx);
	int							    PopInspectWaitFrame(int nGrabberIdx);

	BOOL                            SetFrameWaitProcess_SideCam(int nCamIdx);
	LPBYTE                          GetFrameWaitProcess_SideCam(int nCamIdx, int nFrame);

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

private:
	ISealingInspectHikCamToParent*      m_pInterface;

	// Area Camera
	BOOL                                m_bCamera_ConnectStatus[MAX_CAMERA_INSPECT_COUNT];
	CFrameGrabber_MVS_GigE*             m_pCamera[MAX_CAMERA_INSPECT_COUNT];

	CCriticalSection                    m_csCameraFrameIdx[MAX_CAMERA_INSPECT_COUNT];

	CSharedMemoryBuffer*                m_pCameraImageBuffer[MAX_CAMERA_INSPECT_COUNT];

	int                                 m_cameraCurrentFrameIdx[MAX_CAMERA_INSPECT_COUNT];
	int                                 m_currentFrameWaitProcessSideCamIdx[MAX_IMAGE_BUFFER_SIDECAM];

	// Inspect Wait Frame List
	CCriticalSection			        m_csInspectWaitList[MAX_CAMERA_INSPECT_COUNT];
	std::queue<int>				        m_queueInspectWaitList[MAX_CAMERA_INSPECT_COUNT];
									    
	CSharedMemoryBuffer*                m_pFrameWaitProcessList[MAX_CAMERA_INSPECT_COUNT];
};