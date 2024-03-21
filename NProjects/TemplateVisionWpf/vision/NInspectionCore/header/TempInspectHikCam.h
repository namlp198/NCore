#pragma once
#include "FrameGrabber_MVS_GigE.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "SharedMemoryBuffer.h"
#include "TempInspectDefine.h"
#include "TempInspectCore.h"
#include "TempInspectRecipe.h"

typedef void __stdcall ReceivedImageCallback(LPVOID pBuffer, int nGrabberIndex, int nNextFrameIdx, LPVOID pParam);

interface ITempInspectHikCamToParent
{
	virtual CTempInspectRecipe* GetRecipe(int nIdx) = 0;
	virtual CTempInspectSystemConfig* GetSystemConfig() = 0;
};

class AFX_EXT_CLASS CTempInspectHikCam : public IFrameGrabber2Parent
{
public:
	CTempInspectHikCam(ITempInspectHikCamToParent* pInterface);
	~CTempInspectHikCam();

	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();
						            
	LPBYTE                          GetBufferImage(int nCamIdx);
	BOOL                            GetBufferImage(int nCamIdx, LPBYTE pBuffer);
	BOOL                            GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer);
	CSharedMemoryBuffer*            GetSharedMemoryBuffer(int nCamIdx);

	LPBYTE                          GetResultBufferImage(int nPosIdx);

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int StartGrab(int nCamIdx);
	int StopGrab(int nCamIdx);

	// Register image callback
	void RegisterReceivedImageCallback(ReceivedImageCallback* callback, LPVOID param);

private:
	ITempInspectHikCamToParent*     m_pInterface;
	// callback
	ReceivedImageCallback*          m_pReceivedImgCallback;
	LPVOID                          m_pParam;

	// Area Camera
	BOOL							m_bCamera_ConnectStatus[MAX_CAMERA_INSP_COUNT];
	CFrameGrabber_MVS_GigE*         m_nCamera[MAX_CAMERA_INSP_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				m_csCameraFrameIdx[MAX_CAMERA_INSP_COUNT];
	int								m_pCameraCurrentFrameIdx[MAX_CAMERA_INSP_COUNT];
	CSharedMemoryBuffer*            m_pCameraImageBuffer[MAX_CAMERA_INSP_COUNT];
};