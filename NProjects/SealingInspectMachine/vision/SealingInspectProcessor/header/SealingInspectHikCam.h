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
	virtual CSealingInspectRecipe* GetRecipe(int nIdx) = 0;
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

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int     StartGrab(int nCamIdx);
	int     StopGrab(int nCamIdx);
	int     SingleGrab(int nCamIdx);
	int		SetTriggerMode(int nCamIdx, int nMode);
	int		SetTriggerSource(int nCamIdx, int nSource);

private:
	ISealingInspectHikCamToParent*      m_pInterface;

	// Area Camera
	BOOL                                m_bCamera_ConnectStatus[MAX_NUMBER_OF_CAMERA_INSPECT];
	CFrameGrabber_MVS_GigE*             m_pCamera[MAX_NUMBER_OF_CAMERA_INSPECT];

	CCriticalSection                    m_csCameraFrameIdx[MAX_NUMBER_OF_CAMERA_INSPECT];

	CSharedMemoryBuffer*                m_pCameraImageBuffer[MAX_NUMBER_OF_CAMERA_INSPECT];

	int                                 m_pCameraCurrentFrameIdx[MAX_NUMBER_OF_CAMERA_INSPECT];

	// Inspect Wait Frame List
	CCriticalSection					m_csInspectWaitList[MAX_NUMBER_OF_CAMERA_INSPECT];
	std::queue<int>						m_queueInspectWaitList[MAX_NUMBER_OF_CAMERA_INSPECT];
};