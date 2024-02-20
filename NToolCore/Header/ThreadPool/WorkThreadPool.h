#pragma once
#include <afxmt.h>

#include "ThreadData.h"
#include "ThreadPool.h"

#include <deque>
typedef std::deque<CWorkThreadData*>				DequeWorkThreadData;
typedef std::deque<CWorkThreadData*>::iterator		DequeWorkThreadDataIt;

class CWorkThreadPool : public CThreadPool
{
public:
	CWorkThreadPool(int nThreadCount=1);
	virtual ~CWorkThreadPool(void);
	size_t GetThreadDataCount();

protected:
	virtual void WorkThreadProcess(PVOID pParameter) = 0;

protected:
	BOOL CreateWorkThread(PVOID pParameter);
	void CloseWorkThread();
	void ClearThreadData();
	static VOID CALLBACK WorkCallback(PTP_CALLBACK_INSTANCE pInstance, PVOID pParameter, PTP_WORK pWork);

private:
	CWorkThreadData* AddThreadData(CWorkThreadData* pThreadData);

protected:
	PTP_WORK				m_pWork;
	PTP_WORK_CALLBACK		m_pWorkCallback;

private:
	DequeWorkThreadData		m_deqThreadData;
	CCriticalSection		m_csThreadData;
};

