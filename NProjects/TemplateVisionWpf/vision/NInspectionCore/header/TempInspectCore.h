#pragma once

#include "TempInspectRecipe.h"
#include "TempInspectDefine.h"
#include "WorkThreadArray.h"

class AFX_EXT_CLASS CTempInspectCoreThreadData : public CWorkThreadData
{
public:
	CTempInspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	CTempInspectCoreThreadData(PVOID pPtr, emTempInspectWorkType emProcessType, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nProcessType = emProcessType;
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~CTempInspectCoreThreadData() { Reset(); }
	void Reset()
	{
		m_nProcessType = emInspectWorkType_Count;
		m_nThreadIdx = -1;
	}

public:
	emTempInspectWorkType			m_nProcessType;		// process type
	UINT							m_nThreadIdx;
};

class AFX_EXT_CLASS CTempInspectCore : public IWorkThreadArray2Parent
{
public:
	CTempInspectCore();
	~CTempInspectCore();

public:
	void CreateInspectThread(int nThreadCount);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread(int nThreadIndex);
	void StopThread();

private:
	UINT								m_nThreadCount;

	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	CCriticalSection					m_csPostProcessing;
};