#include "SharedMemoryBuffer.h"
#include "FrameGrabberUsb.h"
#include "FrameGrabber_UsbCam.h"

#define MAX_CAMERA_COUNT 1
#define MAX_BUFFER_FRAME 15
#define MAX_POSITION_COUNT 1

class AFX_EXT_CLASS CInspectionUsbCam : public IFrameGrabberUsb2Parent
{
public:
	CInspectionUsbCam();
	~CInspectionUsbCam();


	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();

	LPBYTE                          GetBufferImage(int nCamIdx);
	CSharedMemoryBuffer*            GetSharedMemoryBuffer(int nCamIdx);

	LPBYTE                          GetResultBufferImage(int nPosIdx);

public:
	virtual int                     IFGU2P_FrameGrabbedUsb(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int                             StartContinuousGrab(int nCamIdx);
	int                             StopContinuousGrab(int nCamIdx);
	int                             SingleGrab(int nCamIdx);

private:
	// Usb Camera
	BOOL							         m_bCamera_ConnectStatus[MAX_CAMERA_COUNT];
	CFramGrabber_UsbCam*                     m_pCamera[MAX_CAMERA_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				         m_csCameraFrameIdx[MAX_CAMERA_COUNT];
	int								         m_pCameraCurrentFrameIdx[MAX_CAMERA_COUNT];
	CSharedMemoryBuffer*                     m_pCameraImageBuffer[MAX_CAMERA_COUNT];

	// Result Buffer
	cv::Mat							         m_ResultImageBuffer[MAX_POSITION_COUNT];
};