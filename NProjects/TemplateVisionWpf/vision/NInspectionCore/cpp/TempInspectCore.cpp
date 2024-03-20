#include "pch.h"
#include "TempInspectCore.h"

CTempInspectCore::CTempInspectCore()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pTempInspRecipe[i] = NULL;
	}
}

CTempInspectCore::~CTempInspectCore()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pTempInspRecipe[i] != NULL)
			delete m_pTempInspRecipe[i], m_pTempInspRecipe[i] = NULL;
	}
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

void CTempInspectCore::LoadRecipe()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pTempInspRecipe[i] = new CTempInspectRecipe;
		m_pTempInspRecipe[i]->LoadRecipe(i);
	}
}

void CTempInspectCore::RunningThread(int nThreadIndex)
{
	if (m_pTempInspRecipe[nThreadIndex] == NULL)
		return;

	QueueLocTools* queueLocTools = m_pTempInspRecipe[nThreadIndex]->GetQueueLocTools();
	QueueSelROITools* queueSelROITools = m_pTempInspRecipe[nThreadIndex]->GetQueueSelROITools();

	while (!queueLocTools->empty())
	{
		CLocatorTool locTool = queueLocTools->front();
		locTool.Run();
		queueLocTools->pop();
	}

	while (!queueSelROITools->empty())
	{
		CSelectROITool selTool = queueSelROITools->front();
		selTool.Run();
		queueSelROITools->pop();
	}
}

void CTempInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++)
		m_bRunningThread[i] = FALSE;
}
