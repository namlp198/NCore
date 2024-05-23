#pragma once

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectSystemSetting.h"
#include "SealingInspectResult.h"
#include "SealingInspectHikCam.h"
#include "SealingInspect_Simulation_IO.h"
#include "WorkThreadArray.h"

#define TEST_INSPECT_CAVITY_1
//#undef TEST_INSPECT_CAVITY_1
#define TEST_INSPECT_CAVITY_2
//#undef TEST_INSPECT_CAVITY_2

interface ISealingInspectCoreToParent
{
	virtual void							InspectComplete(emInspectCavity nSetInsp) = 0;
	virtual CSealingInspectRecipe*          GetRecipe() = 0;
	virtual CSealingInspectSystemSetting*   GetSystemSetting() = 0;
	virtual CSealingInspectHikCam*          GetHikCamControl() = 0;
	virtual BOOL                            SetTopCamResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual BOOL                            SetSideCamResultBuffer(int nBuff, int nFrame, BYTE* buff) = 0;
	virtual CSealingInspectResult*          GetSealingInspectResultControl(int nResIdx) = 0;
	virtual CSealingInspect_Simulation_IO*  GetSealingInspectSimulationIO(int nCoreIdx) = 0;
};

class AFX_EXT_CLASS CTempInspectCoreThreadData : public CWorkThreadData
{
public:
	CTempInspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	CTempInspectCoreThreadData(PVOID pPtr, emInspectCavity nSetInsp, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nSetInspect = nSetInsp;
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~CTempInspectCoreThreadData() { Reset(); }
	void Reset()
	{
		m_nSetInspect = emUNKNOWN;
		m_nThreadIdx = -1;
	}

public:
	emInspectCavity			        m_nSetInspect;		// set Cam
	UINT							m_nThreadIdx;
};

class AFX_EXT_CLASS CSealingInspectCore : public IWorkThreadArray2Parent
{
public:
	CSealingInspectCore(ISealingInspectCoreToParent* pInterface);
	~CSealingInspectCore();

public:
	void CreateInspectThread(int nThreadCount, emInspectCavity nInspCavity);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread_INSPECT_CAVITY1(int nThreadIndex);
	void RunningThread_INSPECT_CAVITY2(int nThreadIndex);
	void StopThread();

	void ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat);
	void ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat);
	void ProcessFrame_SideCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, int nFrameIdx);

	void TestInspectCavity1(int nCoreIdx);
	void TestInspectCavity2(int nCoreIdx);

public:

	// setter
	void        SetSimulatorMode(BOOL bSimu) { m_bSimulator = bSimu; }
	void        SetCoreIndex(UINT nCoreIdx) { m_nCoreIdx = nCoreIdx; }
	// getter
	BOOL        GetSimulatorMode() { return m_bSimulator; }
	UINT        GetCoreIndex() { return m_nCoreIdx; }

private:

	// interface
	ISealingInspectCoreToParent*        m_pInterface;

	UINT								m_nThreadCount;

	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	BOOL                                m_bSimulator;

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	std::vector<BOOL>					m_vecProcessedFrame;

	CCriticalSection					m_csPostProcessing;

	UINT                                m_nCoreIdx;
};