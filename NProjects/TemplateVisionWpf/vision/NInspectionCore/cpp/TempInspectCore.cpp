#include "pch.h"
#include "TempInspectCore.h"

CTempInspectCore::CTempInspectCore()
{
}

CTempInspectCore::~CTempInspectCore()
{
}

void CTempInspectCore::CreateInspectThread(int nThreadCount)
{
	DeleteInspectThread();

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;

		m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], emInspectWorkType_Inspect, nThreadIdx));

		localLock.Unlock();
	}
}

void CTempInspectCore::DeleteInspectThread()
{
	for (int nThreadIdx = 0; nThreadIdx < MAX_THREAD_COUNT; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] != NULL)
		{
			delete m_pWorkThreadArray[nThreadIdx];
			m_pWorkThreadArray[nThreadIdx] = NULL;
		}

		localLock.Unlock();
	}
}

void CTempInspectCore::WorkThreadProcessArray(PVOID pParameter)
{
	CTempInspectCoreThreadData* pThreadData = static_cast<CTempInspectCoreThreadData*>(pParameter);

	if (pThreadData == NULL) return;

	UINT nThreadOrder = pThreadData->m_nThreadIdx;

	switch (pThreadData->m_nProcessType)
	{
	case emInspectWorkType_Inspect:
		RunningThread((int)nThreadOrder);
		break;
	}
}

void CTempInspectCore::RunningThread(int nThreadIndex)
{
	
}

void CTempInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++)
		m_bRunningThread[i] = FALSE;
}
