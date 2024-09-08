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
	virtual void							SystemMessage(const TCHAR* lpstrFormat, ...) = 0;
	virtual void							AlarmMessage(CString strAlarmMessage) = 0;
	virtual CNVisionInspectRecipe*          GetRecipeControl() = 0;
	virtual CNVisionInspectSystemSetting*   GetSystemSettingControl() = 0;
	virtual CNVisionInspectCameraSetting*   GetCameraSettingControl(int nCamIdx) = 0;
	virtual CNVisionInspectResult*          GetResultControl(int nCoreIdx) = 0;
	virtual CNVisionInspectStatus*          GetStatusControl(int nCoreIdx) = 0;
	virtual BOOL                            SetResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual LPBYTE                          GetSimulatorBuffer(int nBuff, int nFrame) = 0;
	virtual void                            InspectComplete(BOOL bSetting) = 0;
	virtual void                            LocatorTrainComplete(int CamIdx) = 0;
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
	void                       Inspect_Simulation(int nBuff, int nFrame);
	void                       Inspect_Reality(int nCamIdx, LPBYTE pBuffer);
	void                       LocatorTool_Train(LPBYTE pBuffer);

private:
	void                       ProcessFrame(int nCamIdx, LPBYTE pBuffer);
	void                       SaveTemplateImage(cv::Mat& matTemplate, int nCamIdx);

private:

	// interface
	INVisionInspectCoreToParent*        m_pInterface;

	UINT								m_nThreadCount;
	BOOL                                m_bSimulator;
	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	std::vector<BOOL>					m_vecProcessedFrame;

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	CCriticalSection					m_csPostProcessing;
};