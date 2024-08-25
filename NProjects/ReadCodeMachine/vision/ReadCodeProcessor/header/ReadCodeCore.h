#pragma once
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

#include "SharedMemoryBuffer.h"
#include "ReadCodeRecipe.h"
#include "ReadCodeSystemSetting.h"
#include "ReadCodeCameraSetting.h"
#include "ReadCodeResult.h"
#include "ReadCodeDefine.h"
#include "ReadCodeStatus.h"

#include "ZXingOpenCV.h"
#include "WorkThreadArray.h"

interface IReadCodeCoreToParent
{
	virtual CReadCodeRecipe*          GetRecipeControl() = 0;
	virtual CReadCodeSystemSetting*   GetSystemSettingControl() = 0;
	virtual CReadCodeCameraSetting*   GetCameraSettingControl(int nCamIdx) = 0;
	virtual BOOL                      SetResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual LPBYTE                    GetSimulatorBuffer(int nBuff, int nFrame) = 0;
	virtual CReadCodeResult*          GetReadCodeResultControl(int nCoreIdx) = 0;
	virtual CReadCodeStatus*          GetReadCodeStatusControl(int nCoreIdx) = 0;
	virtual void                      InspectComplete(BOOL bSetting) = 0;
	virtual void                      LocatorTrained(BOOL bSetting) = 0;
};

class AFX_EXT_CLASS CTempInspectCoreThreadData : public CWorkThreadData
{
public:
	CTempInspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	CTempInspectCoreThreadData(PVOID pPtr, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~CTempInspectCoreThreadData() { Reset(); }
	void Reset()
	{	
		m_nThreadIdx = -1;
	}

public:
	
	UINT							m_nThreadIdx;
};

class AFX_EXT_CLASS CReadCodeCore : public IWorkThreadArray2Parent
{
public:
	CReadCodeCore(IReadCodeCoreToParent* pInterface);
	~CReadCodeCore();

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
	void                       Inspect_Real(int nCamIdx, LPBYTE pBuffer);
	BOOL                       LocatorTool_Train(LPBYTE pBuffer);

private:
	void                       ProcessFrame(int nCamIdx, LPBYTE pBuffer);
	BOOL                       SaveTemplateImage(cv::Mat& matTemplate);

private:
	// interface
	IReadCodeCoreToParent*              m_pInterface;

	UINT								m_nThreadCount;
	BOOL                                m_bSimulator;
	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	std::vector<BOOL>					m_vecProcessedFrame;

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	CCriticalSection					m_csPostProcessing;
};