#pragma once
#include "SharedMemoryBuffer.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "FrameGrabber_BaslerCam_New.h"
#include <opencv2/core.hpp>

#define MAX_CAMERA_COUNT 1
#define MAX_BUFFER_FRAME 15
#define MAX_POSITION_COUNT 1

class AFX_EXT_CLASS CInspectionBaslerCam_New : public IFrameGrabber2Parent
{
public:
	CInspectionBaslerCam_New();
	~CInspectionBaslerCam_New();

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
	int SingleGrab(int nCamIdx);
	int StopGrab(int nCamIdx);

private:
	// Area Camera
	BOOL							         m_bCamera_ConnectStatus[MAX_CAMERA_COUNT];
	CFrameGrabber_BaslerCam_New*             m_nCamera[MAX_CAMERA_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				         m_csCameraFrameIdx[MAX_CAMERA_COUNT];
	int								         m_pCameraCurrentFrameIdx[MAX_CAMERA_COUNT];
	CSharedMemoryBuffer*                     m_pCameraImageBuffer[MAX_CAMERA_COUNT];

	// Result Buffer
	cv::Mat							         m_ResultImageBuffer[MAX_POSITION_COUNT];
};