#pragma once

#include "ThreadPool.h"

class AFX_EXT_CLASS CWaitThreadPool : public CThreadPool
{
public:
	CWaitThreadPool(int nThreadCount=1);
	virtual ~CWaitThreadPool(void);
	virtual BOOL SetEvent1();
	virtual BOOL ResetEvent1();
		
protected:
	virtual void WaitThreadProcess(PVOID pParameter) = 0;

protected:
	BOOL CreateWaitThread(PVOID pParameter);
	void CloseWaitThread();
	static VOID CALLBACK WaitCallback(PTP_CALLBACK_INSTANCE pInstance, PVOID pParameter, PTP_WAIT pWait, TP_WAIT_RESULT WaitResult);

protected:
	HANDLE					m_hEvent;
	PTP_WAIT				m_pWait;
	PTP_WAIT_CALLBACK		m_pWaitCallback;
};