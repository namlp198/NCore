#include "pch.h"
#include "WorkThreadPool.h"

#include "ThreadData.h"

CWorkThreadPool::CWorkThreadPool(int nThreadCount) : CThreadPool(nThreadCount)
{
	m_pWork				= NULL;
	m_pWorkCallback		= WorkCallback;
}

CWorkThreadPool::~CWorkThreadPool(void)
{
	CloseWorkThread();

	ClearThreadData();
}

VOID CALLBACK CWorkThreadPool::WorkCallback(PTP_CALLBACK_INSTANCE pInstance, PVOID pParameter, PTP_WORK pWork)
{
	// Instance, Parameter, and Work not used in this example.
	UNREFERENCED_PARAMETER(pInstance);
	UNREFERENCED_PARAMETER(pParameter);
	UNREFERENCED_PARAMETER(pWork);

	// Do something when the work callback is invoked.
	CWorkThreadData *pDataPtr = static_cast<CWorkThreadData*>(pParameter);
	if (pDataPtr)
	{
		CWorkThreadPool *pThreadPtr = static_cast<CWorkThreadPool*>(pDataPtr->pCallerPtr);
		pThreadPtr->WorkThreadProcess(pDataPtr);
		delete pDataPtr;
	}

	return;
}

BOOL CWorkThreadPool::CreateWorkThread(PVOID pParameter)
{
	if (NULL==pParameter || NULL==m_pPool || NULL==m_pCleanupGroup) return FALSE;

	if(GetThreadDataCount()==0 && m_pWork)
	{
		CloseThreadpoolCleanupGroupMembers(m_pCleanupGroup, FALSE, NULL);
	}	

	TP_CALLBACK_ENVIRON* pCallBackEnviron = (TP_CALLBACK_ENVIRON*)&m_CallBackEnviron;

	// add data
	CWorkThreadData* pThreadData = AddThreadData(static_cast<CWorkThreadData*>(pParameter));

	// create work
	m_pWork = CreateThreadpoolWork((PTP_WORK_CALLBACK)WorkCallback, (PVOID)pThreadData, pCallBackEnviron);

	// create work fail
	if (NULL==m_pWork) 
	{
		delete pThreadData;
		return FALSE;
	}

	// process work
	SubmitThreadpoolWork(m_pWork);

	return TRUE;
}

void CWorkThreadPool::CloseWorkThread()
{
	if (NULL==m_pWork) return;

	WaitForThreadpoolWorkCallbacks(m_pWork, TRUE);

	CloseThreadpoolWork(m_pWork);

	m_pWork = NULL;
}

size_t CWorkThreadPool::GetThreadDataCount()
{
	CSingleLock MyLock(&m_csThreadData);
	MyLock.Lock();

	return m_deqThreadData.size();
}

CWorkThreadData* CWorkThreadPool::AddThreadData( CWorkThreadData* pThreadData )
{
	CSingleLock MyLock(&m_csThreadData);
	MyLock.Lock();

	// push
	m_deqThreadData.push_back(pThreadData);

	// pop
	CWorkThreadData* pReturnData = static_cast<CWorkThreadData*>(m_deqThreadData.front());
	m_deqThreadData.pop_front();
	return pReturnData;
}

void CWorkThreadPool::ClearThreadData()
{
	CSingleLock MyLock(&m_csThreadData);
	MyLock.Lock();

	for (DequeWorkThreadDataIt it=m_deqThreadData.begin(); it!=m_deqThreadData.end(); it++)
	{
		CWorkThreadData *pNode = static_cast<CWorkThreadData*>(*it);
		if (pNode) delete pNode;
	}
	m_deqThreadData.clear();
}
