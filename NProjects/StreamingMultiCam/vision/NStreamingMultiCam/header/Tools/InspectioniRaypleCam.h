#pragma once
#include "FrameGrabber_iRayple.h"
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "SharedMemoryBuffer.h"

#define MAX_FRAME_COUNT 15
#define MAX_CAMERA_COUNT 8
#define MAX_POSITION_COUNT 1
#define FRAME_WIDTH 2448
#define FRAME_HEIGHT 2048
#define CHANNEL_COUNT 3
#define FRAME_DEPTH 24

class AFX_EXT_CLASS CInspectioniRaypleCam : public IFrameGrabber2Parent
{
public:
	CInspectioniRaypleCam();
	~CInspectioniRaypleCam();

public:
	BOOL                         Initialize();
	BOOL                         Destroy();

	LPBYTE                       GetBufferImage(int nCamIdx);

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int StartGrab(int nCamIdx);
	int StopGrab(int nCamIdx);

private:

	// Area Camera
	BOOL							m_bCamera_ConnectStatus[MAX_CAMERA_COUNT];
	CFrameGrabber_iRayple*          m_nCamera[MAX_CAMERA_COUNT];
	 
	// Area Cam Live Buffer Control
	CCriticalSection				m_csCameraFrameIdx[MAX_CAMERA_COUNT];
	int								m_pCameraCurrentFrameIdx[MAX_CAMERA_COUNT];
	CSharedMemoryBuffer*            m_pCameraImageBuffer[MAX_CAMERA_COUNT];
};