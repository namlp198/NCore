#include "pch.h"
#include "InspectCore.h"

InspectCore::InspectCore(IInspectCoreToParent* pInterface)
{
	m_pInterface = pInterface;
	m_nThreadCount = 0;
	for (int i = 0; i < MAX_THREAD_COUNT; i++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[i]);
		localLock.Lock();

		m_pWorkThreadArray[i] = NULL;
		localLock.Unlock();
	}
}

InspectCore::~InspectCore(void)
{
	DeleteInspectThread();
}

void InspectCore::CreateInspectThread(int nThreadCount)
{
	DeleteInspectThread();

	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	std::queue<UINT> empty;
	std::swap(m_queueThreadIndex, empty);

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;
		m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new InspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], nThreadIdx));

		localLock.Unlock();
	}
}

void InspectCore::DeleteInspectThread()
{
	for (int nThreadIdx = 0; nThreadIdx < MAX_THREAD_COUNT; nThreadIdx++) {
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

void InspectCore::WorkThreadProcessArray(PVOID pParameter)
{
}

void InspectCore::RunningThread(int nThreadIdx)
{
	while (m_bRunningThread[nThreadIdx] == TRUE)
	{
		// for avoid UI Freezing...
		Sleep(1);


	}
}

void InspectCore::StopThread()
{
}

void InspectCore::FrameProcessing(UINT nThreadIdx, UINT nFrameIdx)
{
}
