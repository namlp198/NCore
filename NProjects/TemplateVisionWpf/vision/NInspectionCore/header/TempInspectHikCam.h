#pragma once
#include "FrameGrabber_MVS_GigE.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "SharedMemoryBuffer.h"
#include "TempInspectDefine.h"
#include "opencv2/core.hpp"

#define MAX_FRAME_COUNT 15
#define MAX_CAMERA_COUNT 2
#define MAX_POSITION_COUNT 1
#define FRAME_WIDTH 2448
#define FRAME_HEIGHT 2048
#define CHANNEL_COUNT 3
#define FRAME_DEPTH 24

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
	cv::Mat							m_ResultImageBuffer[MAX_POSITION_COUNT];


};