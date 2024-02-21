#pragma once
#include "ThreadData.h"
#include "WorkThreadArray.h"
#include <queue>

#define MAX_THREAD_COUNT 1

// Callback
interface IInspectCoreToParent
{
	/*virtual void             InspectComplete() = 0;
	virtual LPBYTE           GetFramImage(UINT nFrameIdx) = 0;
	virtual LPBYTE           GetBufferImage(UINT nY) = 0;
	virtual int              PopInspectWaitFrame() = 0;*/
};

class AFX_EXT_CLASS InspectCoreThreadData : public CWorkThreadData
{
public:
	InspectCoreThreadData(PVOID pPtr) : CWorkThreadData(pPtr) { Reset(); }
	InspectCoreThreadData(PVOID pPtr, UINT nThreadIdx) : CWorkThreadData(pPtr)
	{
		m_nThreadIdx = nThreadIdx;
	}
	virtual ~InspectCoreThreadData() {
		Reset();
	}

	void Reset() {
		m_nThreadIdx = -1;
	}
private:
	UINT                   m_nThreadIdx;
};

class AFX_EXT_CLASS InspectCore : public IWorkThreadArray2Parent
{
public:
	InspectCore(IInspectCoreToParent* pInterface);
	~InspectCore(void);

public:
	void CreateInspectThread(int nThreadCount);
	void DeleteInspectThread();
	virtual void WorkThreadProcessArray(PVOID pParameter);

public:
	std::queue<UINT>* GetThreadIndexQueue() { return &m_queueThreadIndex; }
	void RunningThread(int nThreadIdx);

private:
	void StopThread();
	void FrameProcessing(UINT nThreadIdx, UINT nFrameIdx);

private:
	IInspectCoreToParent*             m_pInterface;

private:
	UINT                              m_nThreadCount;
	std::queue<UINT>                  m_queueThreadIndex;
	BOOL                              m_bRunningThread[MAX_THREAD_COUNT];
	CCriticalSection                  m_csWorkThreadArray[MAX_THREAD_COUNT];
	CWorkThreadArray*                 m_pWorkThreadArray[MAX_THREAD_COUNT];
};