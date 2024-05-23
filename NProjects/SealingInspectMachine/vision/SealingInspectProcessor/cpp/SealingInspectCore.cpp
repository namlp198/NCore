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

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam1_BufferHikCamIdx = 0;
	int nSideCam1_BufferHikCamIdx = 2;

	int nTopCam1_BufferProcessor = 0;
	int nSideCam1_BufferProcessor = 0;

	while (m_bRunningThread[nThreadIndex] == TRUE)
	{
		// for avoid UI Freezing
		Sleep(1);

		if (m_bSimulator == TRUE) {
			// LOCK PROCESS
			while (m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS != TRUE)
			{
				Sleep(100);
			}
			m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS = FALSE;
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matTopResult.data) == FALSE)
			return;

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(500);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matTopResult.data) == FALSE)
			return;

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			hikCamControl->SetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx);
			Sleep(100);
		}

#endif // TEST_INSPECT_CAVITY_1

		while (!hikCamControl->GetQueueInspectWaitList(nSideCam1_BufferHikCamIdx).empty())
		{
			int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam1_BufferHikCamIdx);

			// Not Grab Image..
			if (nFrameIdx == -1)
				continue;

			ProcessFrame_SideCam(recipe, nSideCam1_BufferHikCamIdx, nSideCam1_BufferProcessor, nFrameIdx);
		}

		m_pInterface->InspectComplete(emInspectCavity_Cavity1);
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
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam2_BufferHikCamIdx = 1;
	int nSideCam2_BufferHikCamIdx = 3;

	int nTopCam2_BufferProcessor = 1;
	int nSideCam2_BufferProcessor = 1;

	while (m_bRunningThread[nThreadIndex == TRUE]) 
	{
		// for avoid UI Freezing
		Sleep(1);

		if (m_bSimulator == TRUE) {
			// LOCK PROCESS
			while (m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS != TRUE)
			{
				Sleep(100);
			}
			m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS = FALSE;
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);
		cv::Mat matSideResult = cv::Mat::zeros(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matTopResult.data) == FALSE)
			return;

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(500);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matTopResult.data) == FALSE)
			return;

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			hikCamControl->SetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx);
			Sleep(100);
		}

#endif // TEST_INSPECT_CAVITY_1

		while (!hikCamControl->GetQueueInspectWaitList(nSideCam2_BufferHikCamIdx).empty())
		{
			int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam2_BufferHikCamIdx);

			// Not Grab Image..
			if (nFrameIdx == -1)
				continue;

			ProcessFrame_SideCam(recipe, nSideCam2_BufferHikCamIdx, nSideCam2_BufferProcessor, nFrameIdx);
		}

		m_pInterface->InspectComplete(emInspectCavity_Cavity2);
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

	m_pInterface->InspectComplete(emInspectCavity_Cavity2);
}

void CSealingInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++) {
		CSingleLock lockLocal(&m_csWorkThreadArray[i]);
		lockLocal.Lock();
		m_bRunningThread[i] = FALSE;
		lockLocal.Unlock();
	}
}

void CSealingInspectCore::TestInspectCavity1(int nCoreIdx)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam1_BufferHikCamIdx = 0;
	int nSideCam1_BufferHikCamIdx = 2;

	int nTopCam1_BufferProcessor = 0;
	int nSideCam1_BufferProcessor = 0;


	// for avoid UI Freezing
	Sleep(1);

	// 2. turn on ring light

	// 3. wait for the signal lighting cluster go to the position capture.

	// 4. grab frame
	CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

	cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);
	cv::Mat matSideResult = cv::Mat::zeros(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3);

	// 5. grab frame 1
	if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matTopResult.data) == FALSE)
		return;

	// 6. process frame 1 (top cam 1)
	ProcessFrame1_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

	// 7. turn on 4 bar light

	Sleep(500);

	// 8. grab frame
	if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matTopResult.data) == FALSE)
		return;

	// 9. process frame 2 (top cam 1)
	ProcessFrame2_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

	// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
	{
		hikCamControl->SetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx);
		Sleep(100);
	}

#endif // TEST_INSPECT_CAVITY_1

	while (!hikCamControl->GetQueueInspectWaitList(nSideCam1_BufferHikCamIdx).empty())
	{
		int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam1_BufferHikCamIdx);

		// Not Grab Image..
		if (nFrameIdx == -1)
			continue;

		ProcessFrame_SideCam(recipe, nSideCam1_BufferHikCamIdx, nSideCam1_BufferProcessor, nFrameIdx);
	}

	m_pInterface->InspectComplete(emInspectCavity_Cavity1);
}

void CSealingInspectCore::TestInspectCavity2(int nCoreIdx)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam2_BufferHikCamIdx = 1;
	int nSideCam2_BufferHikCamIdx = 3;

	int nTopCam2_BufferProcessor = 1;
	int nSideCam2_BufferProcessor = 1;


	// for avoid UI Freezing
	Sleep(1);

	// 2. turn on ring light

	// 3. wait for the signal lighting cluster go to the position capture.

	// 4. grab frame
	CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

	cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);
	cv::Mat matSideResult = cv::Mat::zeros(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3);

	// 5. grab frame 1
	if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matTopResult.data) == FALSE)
		return;

	// 6. process frame 1 (top cam 1)
	ProcessFrame1_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

	// 7. turn on 4 bar light

	Sleep(500);

	// 8. grab frame
	if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matTopResult.data) == FALSE)
		return;

	// 9. process frame 2 (top cam 1)
	ProcessFrame2_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

	// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
	{
		hikCamControl->SetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx);
		Sleep(100);
	}

#endif // TEST_INSPECT_CAVITY_1

	while (!hikCamControl->GetQueueInspectWaitList(nSideCam2_BufferHikCamIdx).empty())
	{
		int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam2_BufferHikCamIdx);

		// Not Grab Image..
		if (nFrameIdx == -1)
			continue;

		ProcessFrame_SideCam(recipe, nSideCam2_BufferHikCamIdx, nSideCam2_BufferProcessor, nFrameIdx);
	}

	m_pInterface->InspectComplete(emInspectCavity_Cavity2);
}

void CSealingInspectCore::ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	// set buffer
	m_pInterface->SetTopCamResultBuffer(nBufferProcessorIdx, 0, mat.data);

	char ch[255] = {};
	sprintf_s(ch, "%s%s%d%s", "E:\\SealingImage\\FullImage\\20240522\\SealingAllInspect_12345\\", "TopCam", (nBufferProcessorIdx + 1), "_Frame1.bmp");
	cv::imwrite(ch, mat);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = TRUE;
}

void CSealingInspectCore::ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	// set buffer
	m_pInterface->SetTopCamResultBuffer(nBufferProcessorIdx, 1, mat.data);

	char ch[255] = {};
	sprintf_s(ch, "%s%s%d%s", "E:\\SealingImage\\FullImage\\20240522\\SealingAllInspect_12345\\", "TopCam", (nBufferProcessorIdx + 1), "_Frame2.bmp");
	cv::imwrite(ch, mat);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame2 = TRUE;
	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bInspectComplete = TRUE;
}

void CSealingInspectCore::ProcessFrame_SideCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, int nFrameIdx)
{
	// 1. Get Buffer..
	LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nCamIdx, nFrameIdx);

	if (pImageBuffer == NULL)
		return;

	// 2. Process

	// 3. Set buff

	m_pInterface->SetSideCamResultBuffer(nBufferProcessorIdx, nFrameIdx, pImageBuffer);

	cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

	char ch[100] = {};
	sprintf_s(ch, "%s%s%d%s%d%s", "E:\\SealingImage\\FullImage\\20240522\\SealingAllInspect_12345\\", "SideCam", (nBufferProcessorIdx + 1), "_Frame", nFrameIdx + 1, ".bmp");
	cv::imwrite(ch, mat);

	switch (nFrameIdx)
	{
	case 0:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = TRUE;
		break;
	case 1:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = TRUE;
		break;
	case 2:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = TRUE;
		break;
	case 3:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = TRUE;
		break;
	}
}

