#pragma once
#include "FrameGrabber.h"
#include "FrameGrabberParam.h"
#include "FrameGrabber_BaslerCam_New.h"
#include "SharedMemoryBuffer.h"
#include "ReadCodeRecipe.h"
#include "ReadCodeSystemSetting.h"
#include "ReadCodeResult.h"
#include "ReadCodeDefine.h"

#include <opencv2/core.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>

#include "ZXingOpenCV.h"

interface IReadCodeBaslerCamToParent
{
	virtual CReadCodeRecipe*         GetRecipeControl() = 0;
	virtual CReadCodeSystemSetting*  GetSystemSettingControl() = 0;
	virtual BOOL                     SetResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual CReadCodeResult*         GetInspectionResultControl(int nCoreIdx) = 0;
	virtual void                     InspectComplete(BOOL bSetting) = 0;
};

class AFX_EXT_CLASS CReadCodeBaslerCam : public IFrameGrabber2Parent
{
public:
	CReadCodeBaslerCam(IReadCodeBaslerCamToParent* pInterface);
	~CReadCodeBaslerCam();

public:
	BOOL                            Initialize();
	BOOL                            Destroy();
	BOOL                            GetCamStatus();

	LPBYTE                          GetBufferImage(int nCamIdx);

public:
	virtual void	DisplayMessage(TCHAR* str, ...) {};
	virtual void	DisplayMessage(CString strLogMessage) {};
	virtual int		IFG2P_FrameGrabbed(int nGrabberIndex, int nFrameIndex, const BYTE* pBuffer, DWORD64 dwBufferSize);
	virtual int		IFG2P_GetFrameBuffer(int nGrabberIndex, int nFrameIndex, BYTE* pBuffer, DWORD64 dwBufferSize);

public:
	int StartGrab(int nCamIdx);
	int SingleGrab(int nCamIdx);
	int StopGrab(int nCamIdx);
	int SetTriggerMode(int nCamIdx, int nMode);
	int SetTriggerSource(int nCamIdx, int nSource);
	int SetExposureTime(int nCamIdx, double dExpTime);
	int SetAnalogGain(int nCamIdx, double dGain);

public:
	// getter
	BOOL GetIsStreaming() { return m_bIsStreaming; }

	// setter
	void SetIsStreaming(BOOL bStatus) { m_bIsStreaming = bStatus; }

private:
	IReadCodeBaslerCamToParent*              m_pInterface;

	// Area Camera
	BOOL							         m_bCamera_ConnectStatus[MAX_CAMERA_INSPECT_COUNT];
	CFrameGrabber_BaslerCam_New*             m_pCamera[MAX_CAMERA_INSPECT_COUNT];

	// Area Cam Live Buffer Control
	CCriticalSection				         m_csCameraFrameIdx[MAX_CAMERA_INSPECT_COUNT];
	int								         m_pCameraCurrentFrameIdx[MAX_CAMERA_INSPECT_COUNT];
	CSharedMemoryBuffer*                     m_pCameraImageBuffer[MAX_CAMERA_INSPECT_COUNT];

	BOOL                                     m_bIsStreaming;
};