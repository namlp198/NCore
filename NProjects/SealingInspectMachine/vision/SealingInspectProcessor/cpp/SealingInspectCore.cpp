#include "pch.h"
#include "SealingInspectCore.h"

CSealingInspectCore::CSealingInspectCore(ISealingInspectCoreToParent* pInterface)
{
	m_pInterface = pInterface;
}

CSealingInspectCore::~CSealingInspectCore()
{
}

void CSealingInspectCore::CreateInspectThread(int nThreadCount, emInspectCavity nInspCavity)
{
	DeleteInspectThread();

	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	m_vecProcessedFrame.clear();
	m_vecProcessedFrame.resize(MAX_FRAME_WAIT_SIDECAM, FALSE);

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;

		switch (nInspCavity)
		{
		case emInspectCavity_Cavity1:
			m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], emInspectCavity_Cavity1, nThreadIdx));
			break;
		case emInspectCavity_Cavity2:
			m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], emInspectCavity_Cavity2, nThreadIdx));
			break;
		}

		localLock.Unlock();
	}
}

void CSealingInspectCore::DeleteInspectThread()
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

void CSealingInspectCore::WorkThreadProcessArray(PVOID pParameter)
{
	CTempInspectCoreThreadData* pThreadData = static_cast<CTempInspectCoreThreadData*>(pParameter);

	if (pThreadData == NULL) return;

	UINT nThreadOrder = pThreadData->m_nThreadIdx;

	switch (pThreadData->m_nSetInspect)
	{
	case emInspectCavity_Cavity1:
		RunningThread_INSPECT_CAVITY1((int)nThreadOrder);
	case emInspectCavity_Cavity2:
		RunningThread_INSPECT_CAVITY2((int)nThreadOrder);
		break;
	}
}

void CSealingInspectCore::RunningThread_INSPECT_CAVITY1(int nThreadIndex)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();

	int nTopCamIdx = 0;
	int nSideCamIdx = 2;

	while (m_bRunningThread[nThreadIndex] == TRUE) 
	{
		// for avoid UI Freezing
		Sleep(1);

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matTopResult = cv::Mat::zeros(FRAME_WIDTH_TOPCAM, FRAME_HEIGHT_TOPCAM, CV_8UC3);
		cv::Mat matSideResult = cv::Mat::zeros(FRAME_WIDTH_SIDECAM, FRAME_HEIGHT_SIDECAM, CV_8UC3);

		if (hikCamControl->GetGrabBufferImage(nTopCamIdx, matTopResult.data) == FALSE)
			return;

		//***note: need some delays, to avoid the blurring effect
		Sleep(200);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCamIdx, matTopResult.data) == FALSE)
			return;

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCamIdx, matTopResult);

		// 7. turn on 4 bar light

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCamIdx, matTopResult.data) == FALSE)
			return;

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCamIdx, matTopResult);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		for (int i = 0; i < MAX_FRAME_WAIT_SIDECAM; i++) {
			hikCamControl->SetFrameWaitProcess_SideCam(nSideCamIdx, i + 1);
		}

#endif // TEST_INSPECT_CAVITY_1

		while (!hikCamControl->GetQueueInspectWaitList(nSideCamIdx).empty())
		{
			int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCamIdx);

			// Not Grab Image..
			if (nFrameIdx == -1)
				continue;

			// Process Started..
			m_vecProcessedFrame[nFrameIdx] = TRUE;

			ProcessFrame_SideCam(recipe, nSideCamIdx, nFrameIdx);
		}

		if (m_bSimulator)
			m_bRunningThread[nThreadIndex] == FALSE;
	}

	CSingleLock localLock(&m_csPostProcessing);
	m_csPostProcessing.Lock();

	for (int i = 0; i < MAX_THREAD_COUNT; i++)
	{
		if (nThreadIndex != i && m_bRunningThread[i] == TRUE)
		{
			m_bRunningThread[nThreadIndex] = FALSE;
			m_csPostProcessing.Unlock();
			return;
		}
	}

	m_csPostProcessing.Unlock();

	m_bRunningThread[nThreadIndex] = FALSE;

	m_pInterface->InspectComplete(emInspectCavity_Cavity1);
}

void CSealingInspectCore::RunningThread_INSPECT_CAVITY2(int nThreadIndex)
{
}

void CSealingInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++)
		m_bRunningThread[i] = FALSE;
}

void CSealingInspectCore::ProcessFrame(CSealingInspectRecipe* pRecipe, UINT nThreadIndex, UINT nFrameIndex)
{
}

void CSealingInspectCore::ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, cv::Mat& mat)
{
	// set buffer
	m_pInterface->SetTopCamResultBuffer(nCamIdx, 0, mat.data);
}

void CSealingInspectCore::ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, cv::Mat& mat)
{
	// set buffer
	m_pInterface->SetTopCamResultBuffer(nCamIdx, 1, mat.data);
}

void CSealingInspectCore::ProcessFrame_SideCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nFrameIdx)
{
	// 1. Get Buffer..
	LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nCamIdx, nFrameIdx);

	if (pImageBuffer == NULL)
		return;

	// 2. Process

	// 3. Set buff

	m_pInterface->SetSideCamResultBuffer(nCamIdx, nFrameIdx, pImageBuffer);
}

