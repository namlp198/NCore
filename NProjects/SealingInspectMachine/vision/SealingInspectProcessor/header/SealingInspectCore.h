#pragma once
#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectSystemSetting.h"
#include "SealingInspectResult.h"
#include "WorkThreadArray.h"

interface ISealingInspectCoreToParent
{
	virtual void							InspectComplete(emInspectCavity nSetInsp) = 0;
	virtual CSealingInspectRecipe*          GetRecipe(int nIdx) = 0;
	virtual CSealingInspectSystemSetting*   GetSystemSetting() = 0;
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
	void CreateInspectThread(int nThreadCount, emInspectCavity nSetCam);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread_SETINSPECT1(int nThreadIndex);
	void RunningThread_SETINSPECT2(int nThreadIndex);
	void StopThread();
	void ProcessFrame(CSealingInspectRecipe* pRecipe, UINT nThreadIndex, UINT nFrameIndex);

private:

	// interface
	ISealingInspectCoreToParent*        m_pInterface;

	UINT								m_nThreadCount;

	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];
};