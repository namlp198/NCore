#pragma once

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

#include "SharedMemoryBuffer.h"
#include "NVisionInspectRecipe.h"
#include "NVisionInspectSystemSetting.h"
#include "NVisionInspectCameraSetting.h"
#include "NVisionInspectResult.h"
#include "NVisionInspectDefine.h"
#include "NVisionInspectStatus.h"

#include "ZXingOpenCV.h"
#include "WorkThreadArray.h"

interface INVisionInspectCoreToParent
{
	virtual void							       SystemMessage(const TCHAR* lpstrFormat, ...) = 0;
	virtual void							       AlarmMessage(CString strAlarmMessage) = 0;
	virtual std::vector<int>                       GetVecCameras() = 0;

	virtual CNVisionInspectRecipe*                 GetRecipeControl() = 0;
	virtual CNVisionInspectRecipe_FakeCam*         GetRecipe_FakeCamControl() = 0;
	virtual CNVisionInspectSystemSetting*          GetSystemSettingControl() = 0;
	virtual CNVisionInspectCameraSetting*          GetCameraSettingControl(int nCamIdx) = 0;
	virtual CNVisionInspect_FakeCameraSetting*     GetFakeCameraSettingControl() = 0;
	virtual CNVisionInspectResult*                 GetResultControl() = 0;
	virtual CNVisionInspectResult_FakeCam*         GetResult_FakeCamControl() = 0;
	virtual CNVisionInspectStatus*                 GetStatusControl(int nCamIdx) = 0;
	virtual LPBYTE                                 GetSimulatorBuffer(int nBuff, int nFrame) = 0;
	virtual LPBYTE                                 GetSimulatorBuffer_FakeCam(int nFrame) = 0;
	virtual BOOL                                   SetResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual BOOL                                   SetResultBuffer_FakeCam(int nFrame, BYTE* buff) = 0;
	virtual void                                   InspectComplete(int nCamIdx, BOOL bSetting) = 0;
	virtual void                                   InspectComplete_FakeCam(emInspectTool eInspTool) = 0;
	virtual void                                   LocatorTrainComplete(int CamIdx) = 0;
	virtual void                                   HSVTrainComplete(int nCamIdx) = 0;
};

class AFX_EXT_CLASS CNVisionInspectCoreThreadData : public CWorkThreadData
{
public:
	CNVisionInspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	CNVisionInspectCoreThreadData(PVOID pPtr, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~CNVisionInspectCoreThreadData() { Reset(); }
	void Reset()
	{
		m_nThreadIdx = -1;
	}

public:

	UINT							m_nThreadIdx;
};

class AFX_EXT_CLASS CNVisionInspectCore : public IWorkThreadArray2Parent
{
public:
	CNVisionInspectCore(INVisionInspectCoreToParent* pInterface);
	~CNVisionInspectCore();

public:
	void CreateInspectThread(int nThreadCount);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread_INSPECT(int nThreadIndex);
	void StopThread();
	void StartThread(int nThreadCount);

public:
	void                       Inspect_Simulation(emCameraBrand camBrand, int nCamIdx, int nBuff, int nFrame);
	void                       Inspect_Reality(emCameraBrand camBrand, int nCamIdx, LPBYTE pBuffer);
	void                       LocatorTool_Train(int nCamIdx, LPBYTE pBuffer);
	void                       LocatorTool_FakeCam_Train(LPBYTE pBuffer);
	void                       HSVTrain(int nCamIdx, int nFrame, CNVisionInspectRecipe_HSV* pRecipeHSV);

public:
	void                       MakeROI(int nCamIdx, int nROIIdx, cv::Mat mat, int nROIX, int nROIY, int nROIWidth, int nROIHeight);
	void                       MakeROI_FakeCam(LPBYTE pBuffer, int nROIX, int nROIY, int nROIWidth, int nROIHeight);

public:
	void                       Algorithm_CountPixel(int nCamIdx, int nROIIdx, LPBYTE pBuffer);
	void                       Algorithm_Decode(int nCamIdx, int nROIIdx, LPBYTE pBuffer);
	void                       Algorithm_Locator(int nCamIdx, LPBYTE pBuffer);

private:
	// INSPECT TOOL
	BOOL                       Algorithm_CountPixel(int nCamIdx, int nROIIdx, LPBYTE pBuffer, cv::Mat& matRes);
	BOOL                       Algorithm_Decode(int nCamIdx, int nROIIdx, LPBYTE pBuffer, cv::Mat& matRes);
	BOOL                       Algorithm_Locator(int nCamIdx, LPBYTE pBuffer, cv::Mat& matRes);

	// Process Hik Cam
private:
	void                       ProcessFrame(int nCamIdx, LPBYTE pBuffer);
	void                       ProcessFrame_Cam1(LPBYTE pBuffer);
	void                       ProcessFrame_Cam2(LPBYTE pBuffer);
	void                       ProcessFrame_Cam3(LPBYTE pBuffer);
	void                       ProcessFrame_Cam4(LPBYTE pBuffer);
	void                       ProcessFrame_Cam5(LPBYTE pBuffer);
	void                       ProcessFrame_Cam6(LPBYTE pBuffer);
	void                       ProcessFrame_Cam7(LPBYTE pBuffer);
	void                       ProcessFrame_Cam8(LPBYTE pBuffer);

private:
	void                       SaveTemplateImage(cv::Mat& matTemplate, int nCamIdx);
	void                       SaveTemplateImage_FakeCam(cv::Mat& matTemplate);
	void                       SaveROIImage(cv::Mat& matROI, int nCamIdx, int nROIIdx);
	void                       SaveROIImage(cv::Mat& matROI, CString strFilePath);

private:

	// interface
	INVisionInspectCoreToParent*        m_pInterface;

	UINT								m_nThreadCount;
	BOOL                                m_bSimulator;
	BOOL								m_bRunningThread[MAX_THREAD_COUNT];
	BOOL                                m_bIsFakeCam;

	std::vector<BOOL>					m_vecProcessedFrame;

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	CCriticalSection					m_csPostProcessing;

	cv::Mat                             m_matROI;
	cv::Mat                             m_matResult;
	cv::Rect                            m_rectROI;
};
