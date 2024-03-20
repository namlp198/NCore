#pragma once
#include "FrameGrabber_MVS_GigE.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "SharedMemoryBuffer.h"
#include "TempInspectDefine.h"
#include "TempInspectCore.h"
#include "TempInspectRecipe.h"

#include "opencv2/core.hpp"

#define MAX_FRAME_COUNT 15

class AFX_EXT_CLASS CTempInspectHikCam : public IFrameGrabber2Parent
{
public:
	CTempInspectHikCam();
	~CTempInspectHikCam();

	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();
						            
	LPBYTE                          GetBufferImage(int nCamIdx);
	BOOL                            GetBufferImage(int nCamIdx, LPBYTE pBuffer);
	BOOL                            GetGrabBufferImage(int nCamIdx, LPBYTE pBuffer);
	CSharedMemoryBuffer*            GetSharedMemoryBuffer(int nCamIdx);

	LPBYTE                          GetResultBufferImage(int nPosIdx);
	CTempInspectCore*               GetTempInspCore() { return m_pTempInspCore; }

	void                            SetTempInspCore(CTempInspectCore* pInspCore) { m_pTempInspCore = pInspCore; }

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int StartGrab(int nCamIdx);
	int StopGrab(int nCamIdx);

private:
	//CString                         m_strRootPath;

	// Area Camera
	BOOL							m_bCamera_ConnectStatus[MAX_CAMERA_INSP_COUNT];
	CFrameGrabber_MVS_GigE*         m_nCamera[MAX_CAMERA_INSP_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				m_csCameraFrameIdx[MAX_CAMERA_INSP_COUNT];
	int								m_pCameraCurrentFrameIdx[MAX_CAMERA_INSP_COUNT];
	CSharedMemoryBuffer*            m_pCameraImageBuffer[MAX_CAMERA_INSP_COUNT];

	// Result Buffer
	cv::Mat							m_ResultImageBuffer[MAX_CAMERA_INSP_COUNT];

	CTempInspectCore*               m_pTempInspCore;

};