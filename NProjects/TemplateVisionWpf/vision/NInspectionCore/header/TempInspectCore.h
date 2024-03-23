#pragma once

#include "TempInspectRecipe.h"
#include "TempInspectDefine.h"
#include "TempInspectStatus.h"
#include "TempInspectResult.h"
#include "WorkThreadArray.h"

interface ITempInspectCoreToParent
{
	virtual void							InspectComplete(int nCamIdx) = 0;
	virtual LPBYTE							GetFrameImage(int nCamIdx, UINT nFrameIndex) = 0;
	virtual LPBYTE							GetBufferImage(int nCamIdx, UINT nY) = 0;
	virtual CTempInspectRecipe*             GetRecipe(int nIdx) = 0;
	virtual CTempInspectSystemConfig*       GetSystemConfig() = 0;
	virtual CTempInspectResult*             GetInspectResult(int nIdx) = 0;
	virtual CTempInspectStatus*             GetTempInspectStatus(int nCamIdx) = 0;
	virtual int								PopInspectWaitFrame(int nCamIdx) = 0;
};


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
	CTempInspectCore(ITempInspectCoreToParent* pInterface);
	~CTempInspectCore();

public:
	void CreateInspectThread(int nThreadCount);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	void RunningThread(int nThreadIndex);
	void StopThread();
	void ProcessFrame(CTempInspectRecipe* pRecipe, UINT nThreadIndex, UINT nFrameIndex);

public:
	UINT GetCamIndex() { return m_nCamIndex; }
	void SetCamIndex(UINT nCamIdx) { m_nCamIndex = nCamIdx; }

private:
	UINT								m_nThreadCount;
	UINT                                m_nCamIndex;

	BOOL								m_bRunningThread[MAX_THREAD_COUNT];

	CCriticalSection					m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                   m_pWorkThreadArray[MAX_THREAD_COUNT];

	CCriticalSection					m_csProcessing;
	CCriticalSection					m_csPostProcessing;

	// interface
	ITempInspectCoreToParent*           m_pInterface;

	std::vector<BOOL>					m_vecProcessedFrame;

	CTempInspectRecipe*                 m_pRecipe[MAX_CAMERA_INSP_COUNT];
};