#include "pch.h"
#include "TimerThreadPool.h"

CTimerThreadPool::CTimerThreadPool(DWORD dwPeriod, int nThreadCount) : m_dwPeriod(dwPeriod), CThreadPool(nThreadCount)
{
	m_pTimer			= NULL;
	m_pTimerCallback	= TimerCallback;
}

CTimerThreadPool::~CTimerThreadPool(void)
{
	StopThread();
}

DWORD CTimerThreadPool::GetPeriod() const
{
	return m_dwPeriod;
}

VOID CALLBACK CTimerThreadPool::TimerCallback(PTP_CALLBACK_INSTANCE pInstance, PVOID pParameter, PTP_TIMER pTimer)
{
	// Instance, Parameter, and Work not used in this example.
	UNREFERENCED_PARAMETER(pInstance);
	UNREFERENCED_PARAMETER(pParameter);
	UNREFERENCED_PARAMETER(pTimer);

	// Do something when the work callback is invoked.
	CTimerThreadPool *pThreadPtr = static_cast<CTimerThreadPool*>(pParameter);
	if (pThreadPtr)
	{
		pThreadPtr->TimerThreadProcess(pParameter);
	}

	return;
}

BOOL CTimerThreadPool::CreateTimerThread(PVOID pParameter)
{
	if (m_pTimer) return FALSE;

	if (NULL==m_pPool || NULL==m_pCleanupGroup) return FALSE;
	
	TP_CALLBACK_ENVIRON* pCallBackEnviron = (TP_CALLBACK_ENVIRON*)&m_CallBackEnviron;

	m_pTimer = CreateThreadpoolTimer((PTP_TIMER_CALLBACK)TimerCallback, pParameter, pCallBackEnviron);
	if (NULL==m_pTimer) return FALSE;

	ULARGE_INTEGER ulDueTime;
	FILETIME FileDueTime;

	ulDueTime.QuadPart = (ULONGLONG)-(1 * 10 * 1000 * 1000);
	FileDueTime.dwHighDateTime = ulDueTime.HighPart;
	FileDueTime.dwLowDateTime = ulDueTime.LowPart;

	SetThreadpoolTimer(m_pTimer, &FileDueTime, m_dwPeriod, 0);

	return TRUE;
}

void CTimerThreadPool::CloseTimerThread()
{
	if (NULL==m_pTimer) return;

	WaitForThreadpoolTimerCallbacks(m_pTimer, TRUE);
	
	CloseThreadpoolTimer(m_pTimer);
	
	m_pTimer = NULL;
}

BOOL CTimerThreadPool::StartThread()
{
	return CreateTimerThread(this);
}

void CTimerThreadPool::StopThread()
{
	CloseTimerThread();
}