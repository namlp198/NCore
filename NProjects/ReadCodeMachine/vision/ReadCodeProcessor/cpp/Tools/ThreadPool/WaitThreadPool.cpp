#include "pch.h"
#include "WaitThreadPool.h"

CWaitThreadPool::CWaitThreadPool(int nThreadCount) : CThreadPool(nThreadCount)
{
	m_pWait				= NULL;
	m_pWaitCallback		= WaitCallback;

	m_hEvent			= CreateEvent(NULL, FALSE, FALSE, NULL);
}

CWaitThreadPool::~CWaitThreadPool(void)
{
	CloseWaitThread();

	CloseHandle(m_hEvent);
}

VOID CALLBACK CWaitThreadPool::WaitCallback(PTP_CALLBACK_INSTANCE pInstance, PVOID pParameter, PTP_WAIT pWait, TP_WAIT_RESULT WaitResult)
{
	// Instance, Parameter, and Work not used in this example.
	UNREFERENCED_PARAMETER(pInstance);
	UNREFERENCED_PARAMETER(pParameter);
	UNREFERENCED_PARAMETER(pWait);
	UNREFERENCED_PARAMETER(WaitResult);

	return;
}

BOOL CWaitThreadPool::CreateWaitThread(PVOID pParameter)
{
	if (NULL==m_pPool || NULL==m_pCleanupGroup) return FALSE;

	TP_CALLBACK_ENVIRON* pCallBackEnviron = (TP_CALLBACK_ENVIRON*)&m_CallBackEnviron;

	PTP_WAIT pWait = CreateThreadpoolWait((PTP_WAIT_CALLBACK)WaitCallback, pParameter, pCallBackEnviron);
	if (NULL==pWait) return FALSE;

	SetThreadpoolWait(pWait, m_hEvent, NULL);

	return TRUE;
}

void CWaitThreadPool::CloseWaitThread()
{
	if (NULL==m_pWait) return;

	WaitForThreadpoolWaitCallbacks(m_pWait, TRUE);

	CloseThreadpoolWait(m_pWait);

	m_pWait = NULL;
}

BOOL CWaitThreadPool::SetEvent1()
{
	if (m_hEvent==NULL) return FALSE;

	return SetEvent(m_hEvent);
}

BOOL CWaitThreadPool::ResetEvent1()
{
	if (m_hEvent==NULL) return FALSE;

	return ResetEvent(m_hEvent);
}