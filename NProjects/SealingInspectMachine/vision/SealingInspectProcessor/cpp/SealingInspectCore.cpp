#include "pch.h"
#include "SealingInspectCore.h"

template<typename Iterator>
std::vector<size_t> n_largest_indices(Iterator it, Iterator end, size_t n) {
	struct Element {
		Iterator it;
		size_t index;
	};

	std::vector<Element> top_elements;
	top_elements.reserve(n + 1);

	for (size_t index = 0; it != end; ++index, ++it) {
		top_elements.insert(std::upper_bound(top_elements.begin(), top_elements.end(), *it, [](auto value, auto element) {return value > *element.it; }), { it, index });
		if (index >= n)
			top_elements.pop_back();
	}

	std::vector<size_t> result;
	result.reserve(top_elements.size());

	for (auto& element : top_elements)
		result.push_back(element.index);

	return result;
}

CSealingInspectCore::CSealingInspectCore(ISealingInspectCoreToParent* pInterface)
{
	m_pInterface = pInterface;
	m_bSimulator = FALSE;
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

	int nDelayTimeGrabImage_TopCam_Frame1 = recipe->m_sealingInspRecipe_TopCam[0].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_TopCam_Frame2 = recipe->m_sealingInspRecipe_TopCam[0].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame1 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame2 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame3 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame3.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame4 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame4.m_nDelayTimeGrab;

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
		else if (m_bSimulator == FALSE)
		{
			while (m_pInterface->GetProcessStatus(m_nCoreIdx) != TRUE)
			{
				Sleep(100);
			}
			m_pInterface->SetProcessStatus(m_nCoreIdx, FALSE);
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

		Sleep(nDelayTimeGrabImage_TopCam_Frame1);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(nDelayTimeGrabImage_TopCam_Frame2);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		m_pInterface->InspectTopCamComplete(emInspectCavity_Cavity1);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			switch (i)
			{
			case 0:
				Sleep(nDelayTimeGrabImage_SideCam_Frame1);
				break;
			case 1:
				Sleep(nDelayTimeGrabImage_SideCam_Frame2);
				break;
			case 2:
				Sleep(nDelayTimeGrabImage_SideCam_Frame3);
				break;
			case 3:
				Sleep(nDelayTimeGrabImage_SideCam_Frame4);
				break;
			}
			hikCamControl->SetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx);
		}

#endif // TEST_INSPECT_CAVITY_1

		while (!hikCamControl->GetQueueInspectWaitList(nSideCam1_BufferHikCamIdx).empty())
		{
			int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam1_BufferHikCamIdx);

			// Not Grab Image..
			if (nFrameIdx == -1)
				continue;

			// 1. Get Buffer..
			LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx, nFrameIdx);

			if (pImageBuffer == NULL)
				return;

			cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

			ProcessFrame_SideCam(recipe, nSideCam1_BufferHikCamIdx, nSideCam1_BufferProcessor, nFrameIdx, mat);
		}

		m_pInterface->InspectComplete(emInspectCavity_Cavity1, FALSE);
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

	m_pInterface->InspectComplete(emInspectCavity_Cavity1, FALSE);
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

	int nDelayTimeGrabImage_TopCam_Frame1 = recipe->m_sealingInspRecipe_TopCam[1].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_TopCam_Frame2 = recipe->m_sealingInspRecipe_TopCam[1].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame1 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame2 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame3 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame3.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame4 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame4.m_nDelayTimeGrab;

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
		else if (m_bSimulator == FALSE)
		{
			while (m_pInterface->GetProcessStatus(m_nCoreIdx) != TRUE)
			{
				Sleep(100);
			}
			m_pInterface->SetProcessStatus(m_nCoreIdx, FALSE);
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

		Sleep(nDelayTimeGrabImage_TopCam_Frame1);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(nDelayTimeGrabImage_TopCam_Frame2);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		m_pInterface->InspectTopCamComplete(emInspectCavity_Cavity2);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			switch (i)
			{
			case 0:
				Sleep(nDelayTimeGrabImage_SideCam_Frame1);
				break;
			case 1:
				Sleep(nDelayTimeGrabImage_SideCam_Frame2);
				break;
			case 2:
				Sleep(nDelayTimeGrabImage_SideCam_Frame3);
				break;
			case 3:
				Sleep(nDelayTimeGrabImage_SideCam_Frame4);
				break;
			}
			hikCamControl->SetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx);
		}

#endif // TEST_INSPECT_CAVITY_1

		while (!hikCamControl->GetQueueInspectWaitList(nSideCam2_BufferHikCamIdx).empty())
		{
			int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam2_BufferHikCamIdx);

			// Not Grab Image..
			if (nFrameIdx == -1)
				continue;

			// 1. Get Buffer..
			LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx, nFrameIdx);

			if (pImageBuffer == NULL)
				return;

			cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

			ProcessFrame_SideCam(recipe, nSideCam2_BufferHikCamIdx, nSideCam2_BufferProcessor, nFrameIdx, mat);
		}

		m_pInterface->InspectComplete(emInspectCavity_Cavity2, FALSE);
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

	m_pInterface->InspectComplete(emInspectCavity_Cavity2, FALSE);
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

	int nDelayTimeGrabImage_TopCam_Frame1 = recipe->m_sealingInspRecipe_TopCam[0].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_TopCam_Frame2 = recipe->m_sealingInspRecipe_TopCam[0].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame1 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame2 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame3 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame3.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame4 = recipe->m_sealingInspRecipe_SideCam[0].m_recipeFrame4.m_nDelayTimeGrab;

	// for avoid UI Freezing
	Sleep(1);

	// 2. turn on ring light

	// 3. wait for the signal lighting cluster go to the position capture.

	// 4. grab frame
	CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

	cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
	cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

	Sleep(nDelayTimeGrabImage_TopCam_Frame1);

	// 5. grab frame 1
	if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
		return;

	cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerRG2BGR);

	// 6. process frame 1 (top cam 1)
	ProcessFrame1_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

	// 7. turn on 4 bar light

	Sleep(nDelayTimeGrabImage_TopCam_Frame2);

	// 8. grab frame
	if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
		return;

	cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerRG2BGR);

	// 9. process frame 2 (top cam 1)
	ProcessFrame2_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

	m_pInterface->InspectTopCamComplete(emInspectCavity_Cavity1);

	// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
	{
		switch (i)
		{
		case 0:
			Sleep(nDelayTimeGrabImage_SideCam_Frame1);
			break;
		case 1:
			Sleep(nDelayTimeGrabImage_SideCam_Frame2);
			break;
		case 2:
			Sleep(nDelayTimeGrabImage_SideCam_Frame3);
			break;
		case 3:
			Sleep(nDelayTimeGrabImage_SideCam_Frame4);
			break;
		}

		hikCamControl->SetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx);
	}

#endif // TEST_INSPECT_CAVITY_1

	while (!hikCamControl->GetQueueInspectWaitList(nSideCam1_BufferHikCamIdx).empty())
	{
		int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam1_BufferHikCamIdx);

		// Not Grab Image..
		if (nFrameIdx == -1)
			continue;

		// 1. Get Buffer..
		LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx, nFrameIdx);

		if (pImageBuffer == NULL)
			return;

		cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

		ProcessFrame_SideCam(recipe, nSideCam1_BufferHikCamIdx, nSideCam1_BufferProcessor, nFrameIdx, mat);
	}

	m_pInterface->InspectComplete(emInspectCavity_Cavity1, FALSE);
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

	int nDelayTimeGrabImage_TopCam_Frame1 = recipe->m_sealingInspRecipe_TopCam[1].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_TopCam_Frame2 = recipe->m_sealingInspRecipe_TopCam[1].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame1 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame1.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame2 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame2.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame3 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame3.m_nDelayTimeGrab;
	int nDelayTimeGrabImage_SideCam_Frame4 = recipe->m_sealingInspRecipe_SideCam[1].m_recipeFrame4.m_nDelayTimeGrab;

	// for avoid UI Freezing
	Sleep(1);

	// 2. turn on ring light

	// 3. wait for the signal lighting cluster go to the position capture.

	// 4. grab frame
	CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

	cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
	cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

	Sleep(nDelayTimeGrabImage_TopCam_Frame1);

	// 5. grab frame 1
	if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
		return;

	cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerRG2BGR);

	// 6. process frame 1 (top cam 1)
	ProcessFrame1_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

	// 7. turn on 4 bar light

	Sleep(nDelayTimeGrabImage_TopCam_Frame2);

	// 8. grab frame
	if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
		return;

	cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerRG2BGR);

	// 9. process frame 2 (top cam 1)
	ProcessFrame2_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

	m_pInterface->InspectTopCamComplete(emInspectCavity_Cavity2);

	// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
	{
		switch (i)
		{
		case 0:
			Sleep(nDelayTimeGrabImage_SideCam_Frame1);
			break;
		case 1:
			Sleep(nDelayTimeGrabImage_SideCam_Frame2);
			break;
		case 2:
			Sleep(nDelayTimeGrabImage_SideCam_Frame3);
			break;
		case 3:
			Sleep(nDelayTimeGrabImage_SideCam_Frame4);
			break;
		}
		hikCamControl->SetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx);
	}

#endif // TEST_INSPECT_CAVITY_1

	while (!hikCamControl->GetQueueInspectWaitList(nSideCam2_BufferHikCamIdx).empty())
	{
		int nFrameIdx = hikCamControl->PopInspectWaitFrame(nSideCam2_BufferHikCamIdx);

		// Not Grab Image..
		if (nFrameIdx == -1)
			continue;

		// 1. Get Buffer..
		LPBYTE pImageBuffer = m_pInterface->GetHikCamControl()->GetFrameWaitProcess_SideCam(nSideCam2_BufferHikCamIdx, nFrameIdx);

		if (pImageBuffer == NULL)
			return;

		cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

		ProcessFrame_SideCam(recipe, nSideCam2_BufferHikCamIdx, nSideCam2_BufferProcessor, nFrameIdx, mat);
	}

	m_pInterface->InspectComplete(emInspectCavity_Cavity2, FALSE);
}

void CSealingInspectCore::Inspect_TopCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();

	int nTopCam1_ResultBuffer = 0;
	int nTopCam2_ResultBuffer = 1;

	// 1. Get Buffer..
	LPBYTE pImageBuffer = m_pInterface->GetBufferImage_TOP(nCamIdx, nFrame - 1);

	if (pImageBuffer == NULL)
		return;

	cv::Mat mat(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3, pImageBuffer);

	if (nFrame == 1) {
		ProcessFrame1_TopCam(recipe, nCamIdx, nTopCam1_ResultBuffer, mat);
	}
	else if (nFrame == 2) {
		ProcessFrame2_TopCam(recipe, nCamIdx, nTopCam2_ResultBuffer, mat);
	}

	if (nCoreIdx == 0)
		m_pInterface->InspectComplete(emInspectCavity_Cavity1, TRUE);
	else if (nCoreIdx == 1)
		m_pInterface->InspectComplete(emInspectCavity_Cavity2, TRUE);
}

void CSealingInspectCore::Inspect_SideCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* pRecipe = m_pInterface->GetRecipe();

	int nSideCam1_ResultBuffer = 0;
	int nSideCam2_ResultBuffer = 1;

	// 1. Get Buffer..
	LPBYTE pImageBuffer = m_pInterface->GetBufferImage_SIDE(nCamIdx, nFrame - 1);

	if (pImageBuffer == NULL)
		return;

	cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

	if (nCamIdx == 0)
		ProcessFrame_SideCam(pRecipe, nCamIdx, nSideCam1_ResultBuffer, nFrame, mat);
	else if (nCamIdx == 1)
		ProcessFrame_SideCam(pRecipe, nCamIdx, nSideCam2_ResultBuffer, nFrame, mat);

	if (nCoreIdx == 0)
		m_pInterface->InspectComplete(emInspectCavity_Cavity1, TRUE);
	else if (nCoreIdx == 1)
		m_pInterface->InspectComplete(emInspectCavity_Cavity2, TRUE);
}

BOOL CSealingInspectCore::FindCircle_MinEnclosing(cv::Mat* matProcess, int nThresholdBinary, int nContourSizeMin, int nContourSizeMax, int nRadiusInnerMin, int nRadiusInnerMax, std::vector<std::vector<cv::Point>>& vecContours, std::vector<cv::Vec4i>& vecHierarchy, std::vector<cv::Point2f>& vecCenters, std::vector<float>& vecRadius, cv::Point2f& center, double& dRadius)
{
	cv::Mat matGray, matBinary;
	cv::cvtColor(*matProcess, matGray, cv::COLOR_BGR2GRAY);
	cv::threshold(matGray, matBinary, nThresholdBinary, 255, cv::THRESH_BINARY_INV);
	//cv::imshow("mat binary", matBinary);

	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;
	findContours(matBinary, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

	// list all polygons found.
	std::vector<std::vector<cv::Point> > contours_poly(contours.size());
	// list all center point of polygons found.
	std::vector<cv::Point2f> centers(contours.size());
	// list all radius of them.
	std::vector<float> radius(contours.size());

	for (size_t i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() < nContourSizeMin || contours[i].size() > nContourSizeMax)
			continue;

		// approximate contour become the polygon
		approxPolyDP(contours[i], contours_poly[i], 3, true);
		// find minimal enclosing circle of contours
		minEnclosingCircle(contours_poly[i], centers[i], radius[i]);
		if ((int)radius[i] > nRadiusInnerMin && (int)radius[i] < nRadiusInnerMax)
		{
			center = centers[i];
			dRadius = radius[i];

			if (center.x == 0 && center.y == 0 && dRadius == 0)
				return FALSE;
		}
	}

	vecContours = contours;
	vecHierarchy = hierarchy;
	vecCenters = centers;
	vecRadius = radius;

	return TRUE;
}

BOOL CSealingInspectCore::FindCircle_HoughCircle(cv::Mat* matProcess, std::vector<cv::Vec3f>& vecCircles, std::vector<cv::Point2i>& vecPts, int nThresholdCanny, int minDist, int nRadiusOuterMin, int nRadiusOuterMax, double dIncreAngle)
{
	cv::Mat gray, blur, canny;

	cv::cvtColor(*matProcess, gray, cv::COLOR_BGR2GRAY);

	cv::GaussianBlur(gray, blur, cv::Size(3, 3), 0.7, 0.7);
	cv::Canny(blur, canny, 255, nThresholdCanny);

	// Apply the Hough Transform to find the circles
	cv::HoughCircles(gray, vecCircles, cv::HOUGH_GRADIENT, 1, minDist, 200, 80, nRadiusOuterMin, nRadiusOuterMax);

	if (vecCircles.size() == 0)
		return FALSE;

	//compute distance transform:
	cv::Mat dt;
	cv::distanceTransform(255 - (canny > 0), dt, cv::DIST_L2, 3);

	// test for semi-circles:
	float minInlierDist = 2.0f;
	for (size_t i = 0; i < vecCircles.size(); i++)
	{
		// test inlier percentage:
		// sample the circle and check for distance to the next edge
		unsigned int counter = 0;
		unsigned int inlier = 0;

		cv::Point2f center((vecCircles[i][0]), (vecCircles[i][1]));
		float radius = (vecCircles[i][2]);

		// maximal distance of inlier might depend on the size of the circle
		float maxInlierDist = radius / 25.0f;
		if (maxInlierDist < minInlierDist) maxInlierDist = minInlierDist;

		//TODO: maybe paramter incrementation might depend on circle size!
		for (float t = 0; t < 2 * PI; t += dIncreAngle)
		{
			counter++;
			float cX = radius * cos(t) + vecCircles[i][0];
			float cY = radius * sin(t) + vecCircles[i][1];

			if (dt.at<float>(cY, cX) < maxInlierDist)
			{
				inlier++;
				vecPts.push_back(cv::Point2i(cX, cY));
			}
		}
	}

	return TRUE;
}

BOOL CSealingInspectCore::FindDistanceAll_OuterToInner(std::vector<cv::Point2i>& vecPts, std::vector<cv::Point2i>& vecPtsIntersection, std::vector<cv::Point2f>& vecIntsecPtsFound, cv::Point2f center, double radius, std::vector<double>& vecDistance)
{
	for (int i = 0; i < vecPts.size(); i++) {
		double distance = CalculateDistancePointToCircle(vecPts[i], center, radius);
		vecDistance.push_back(distance);
		vecPtsIntersection.push_back(CalculateIntersectionPointCoordinate(vecPts[i], center, radius, distance));

		//vecIntsecPtsFound.push_back(FindIntersectionPoint_LineCircle(vecPts[i], center, radius));
	}

	return TRUE;
}

BOOL CSealingInspectCore::JudgementInspectDistanceMeasurement(std::vector<double>& vecDistance, std::vector<int>& vecPosNG, double nDistanceMin, double nDistanceMax)
{
	BOOL nRet = TRUE;

	for (int i = 0; i < vecDistance.size(); i++) {
		if (vecDistance[i] > nDistanceMax || vecDistance[i] < nDistanceMin) {
			nRet &= FALSE;
			vecPosNG.push_back(i);
		}
	}

	return nRet;
}

BOOL CSealingInspectCore::JudgementInspectDistanceMeasurement_AdvancedAlgorithms(std::vector<double>& vecDistance, std::vector<int>& vecPosNG, double nDistanceMin, double nDistanceMax, int nNumberOfDistNGMax)
{
	BOOL nRet = TRUE;
	int nCounter = 0;

	for (int i = 0; i < vecDistance.size(); i++) {
		if (vecDistance[i] > nDistanceMax || vecDistance[i] < nDistanceMin) {
			//nRet &= FALSE;
			nCounter++;
			vecPosNG.push_back(i);
		}
	}
	if (nCounter > nNumberOfDistNGMax)
		nRet &= FALSE;

	return nRet;
}

BOOL CSealingInspectCore::FindMeasurePointsAtPosMinMax(CRecipe_TopCam_Frame1* pRecipeTopCamFrame1, cv::Mat* pMatProcess, cv::Rect& rectROI, std::vector<cv::Point>& vecMeaPts, int nROIIdx)
{
	if (pMatProcess == NULL)
		return FALSE;

	cv::Mat matGray;
	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	cv::Mat pImageDataROI(rectROI.height, rectROI.width, CV_8UC1);
	for (int i = 0; i < pImageDataROI.rows; i++)
		memcpy(&pImageDataROI.data[i * pImageDataROI.step1()], &pMatProcess->data[(i + rectROI.y) * pMatProcess->step1() + rectROI.x], pImageDataROI.cols);

	int nWidthROI = 0;
	int nHeightROI = 0;

	switch (nROIIdx)
	{
	case 0:
	case 2:
		nWidthROI = pRecipeTopCamFrame1->m_nROIWidth_Hor;
		nHeightROI = pRecipeTopCamFrame1->m_nROIHeight_Hor;
		break;
	case 1:
	case 3:
		nWidthROI = pRecipeTopCamFrame1->m_nROIWidth_Ver;
		nHeightROI = pRecipeTopCamFrame1->m_nROIHeight_Ver;
		break;
	}

	double dTheshold1 = pRecipeTopCamFrame1->m_dThresholdCanny1_MakeROI;
	double dTheshold2 = pRecipeTopCamFrame1->m_dThresholdCanny2_MakeROI;

	vecMeaPts.clear();

	cv::Mat matCanny;
	MakeCannyEdgeImage(pMatProcess, matCanny, dTheshold1, dTheshold2);

	/*char chCanny[10] = {};
	sprintf_s(chCanny, "%s_%d", "Canny", nIdx);
	cv::imshow(chCanny, matCanny);*/

	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;

	findContours(matCanny, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

	for (int i = 0; i < contours.size(); i++) {
		if (contours[i].size() < 20)
			continue;

		int distMaxPts = MEASUREMENT_POINTS_DIST_MAX_ADVANCED_ALGORITHMS_COUNT;
		int distMinPts = MEASUREMENT_POINTS_DIST_MIN_ADVANCED_ALGORITHMS_COUNT;

		if (nWidthROI > nHeightROI)
		{
			//int maxPts = nWidthROI / 10;

			std::vector<int> vecY;

			for (int j = 0; j < contours[i].size(); j++) {
				vecY.push_back(contours[i][j].y);
			}

			std::vector<int> vecdistMinIndices;
			std::vector<int> vecdistMaxIndices;

			FindSmallestElementsInVector(vecY, distMinPts, vecdistMinIndices);
			FindLagestElementsInVector(vecY, distMaxPts, vecdistMaxIndices);

			//auto indices = n_largest_indices(vecY.begin(), vecY.end(), maxPts);
			for (auto k : vecdistMinIndices) {
				cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
			for (auto k : vecdistMaxIndices) {
				cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
		}
		else
		{
			std::vector<int> vecX;

			for (int j = 0; j < contours[i].size(); j++) {
				vecX.push_back(contours[i][j].x);
			}

			std::vector<int> vecdistMinIndices;
			std::vector<int> vecdistMaxIndices;

			FindSmallestElementsInVector(vecX, distMinPts, vecdistMinIndices);
			FindLagestElementsInVector(vecX, distMaxPts, vecdistMaxIndices);

			//auto indices = n_largest_indices(vecY.begin(), vecY.end(), maxPts);
			for (auto k : vecdistMinIndices) {
				cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
			for (auto k : vecdistMaxIndices) {
				cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
		}

		/*if (nWidthROI > nHeightROI) {
			int nSize = contours[i].size();
			int nY_Average = 0;

			for (int j = 0; j < nSize; j++) {
				nY_Average += contours[i][j].y;
			}
			nY_Average = nY_Average / nSize;

			int increX = 0;
			for (int k = 0; k < nWidthROI / 20; k++) {
				cv::Point pt(increX + rectROI.x, nY_Average + rectROI.y);
				vecPtsOnContour.push_back(pt);
				increX += nWidthROI / 5;
			}
		}
		else {
			int nX_Average = 0;
			int nSize = contours.size();

			for (int j = 0; j < nSize; j++) {
				nX_Average += contours[i][j].x;
			}
			nX_Average = nX_Average / nSize;

			int increY = 0;
			for (int k = 0; k < nHeightROI / 20; k++) {
				cv::Point pt(nX_Average + rectROI.x, increY + rectROI.y);
				vecPtsOnContour.push_back(pt);
				increY += nHeightROI / 5;
			}
		}*/
	}

	return TRUE;
}

BOOL CSealingInspectCore::FindMeasurePointsAtPosDistMinMax_SideCam(CSealingInspectRecipe_SideCam* pRecipeSideCam, cv::Mat* pImageData, int nFrame, cv::Rect rectROI, std::vector<cv::Point>& vecMeaPts)
{
	if (pImageData == NULL)
		return FALSE;

	double dTheshold1 = 0.0;
	double dTheshold2 = 0.0;
	BOOL useAdvancedAlgorithms = FALSE;

	switch (nFrame)
	{
	case 1:
		dTheshold1 = pRecipeSideCam->m_recipeFrame1.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam->m_recipeFrame1.m_dThresholdCanny2_MakeROI;
		useAdvancedAlgorithms = pRecipeSideCam->m_recipeFrame1.m_bUseAdvancedAlgorithms;
		break;
	case 2:
		dTheshold1 = pRecipeSideCam->m_recipeFrame2.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam->m_recipeFrame2.m_dThresholdCanny2_MakeROI;
		useAdvancedAlgorithms = pRecipeSideCam->m_recipeFrame2.m_bUseAdvancedAlgorithms;
		break;
	case 3:
		dTheshold1 = pRecipeSideCam->m_recipeFrame3.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam->m_recipeFrame3.m_dThresholdCanny2_MakeROI;
		useAdvancedAlgorithms = pRecipeSideCam->m_recipeFrame3.m_bUseAdvancedAlgorithms;
		break;
	case 4:
		dTheshold1 = pRecipeSideCam->m_recipeFrame4.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam->m_recipeFrame4.m_dThresholdCanny2_MakeROI;
		useAdvancedAlgorithms = pRecipeSideCam->m_recipeFrame4.m_bUseAdvancedAlgorithms;
		break;
	}

	if (useAdvancedAlgorithms == FALSE)
		return FALSE;

	vecMeaPts.clear();

	cv::Mat matCanny;
	MakeCannyEdgeImage(pImageData, matCanny, dTheshold1, dTheshold2);

	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;

	findContours(matCanny, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

	for (int i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() < 100)
			continue;

		int distMaxPts = MAX_MEASUREMENT_POINTS_DIST_MAX_SIDECAM_COUNT;
		int distMinPts = MAX_MEASUREMENT_POINTS_DIST_MIN_SIDECAM_COUNT;

		std::vector<int> vecY;

		for (int j = 0; j < contours[i].size(); j++) {
			vecY.push_back(contours[i][j].y);
		}

		std::vector<int> vecdistMinIndices;
		std::vector<int> vecdistMaxIndices;

		FindSmallestElementsInVector(vecY, distMinPts, vecdistMinIndices);
		FindLagestElementsInVector(vecY, distMaxPts, vecdistMaxIndices);

		//auto indices = n_largest_indices(vecY.begin(), vecY.end(), maxPts);
		for (auto k : vecdistMinIndices) {
			cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
			vecMeaPts.push_back(pt);
		}
		for (auto k : vecdistMaxIndices) {
			cv::Point pt(contours[i].at(k).x + rectROI.x, contours[i].at(k).y + rectROI.y);
			vecMeaPts.push_back(pt);
		}
	}
}

BOOL CSealingInspectCore::FindMeasurePoints_SideCam(const CSealingInspectRecipe_SideCam pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect rectROI, std::vector<cv::Point>& vecMeaPts)
{
	if (pMatProcess == NULL)
		return FALSE;

	cv::Mat matGray;
	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	cv::Mat matROIFindMearuePt(rectROI.height, rectROI.width, CV_8UC1);
	for (int i = 0; i < matROIFindMearuePt.rows; i++)
		memcpy(&matROIFindMearuePt.data[i * matROIFindMearuePt.step1()], &matGray.data[(i + rectROI.y) * matGray.step1() + rectROI.x], matROIFindMearuePt.cols);


	double dTheshold1 = 0.0;
	double dTheshold2 = 0.0;
	int nWidthROI = rectROI.width;
	int nHeightROI = rectROI.height;

	switch (nFrame)
	{
	case 1:
		dTheshold1 = pRecipeSideCam.m_recipeFrame1.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame1.m_dThresholdCanny2_MakeROI;
		break;
	case 2:
		dTheshold1 = pRecipeSideCam.m_recipeFrame2.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame2.m_dThresholdCanny2_MakeROI;
		break;
	case 3:
		dTheshold1 = pRecipeSideCam.m_recipeFrame3.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame3.m_dThresholdCanny2_MakeROI;
		break;
	case 4:
		dTheshold1 = pRecipeSideCam.m_recipeFrame4.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame4.m_dThresholdCanny2_MakeROI;
		break;
	}

	vecMeaPts.clear();

	cv::Mat matCanny;
	MakeCannyEdgeImage(&matROIFindMearuePt, matCanny, dTheshold1, dTheshold2);

	int nMeasurePointCount = MAX_DIMENSION_MEASURE_POINT_SIDECAM;
	int nGapWidth = nWidthROI / nMeasurePointCount;
	int nGapHeight = nHeightROI;

	int nROI_X = 0;
	int nROI_Y = 0;

#ifdef _USE_TBB
	tbb::parallel_for(0, nMeasurePointCount, [&](int nMeasureIdx)
#else
	for (int nMeasureIdx = 0; nMeasureIdx < nMeasurePointCount; nMeasureIdx++)
#endif
	{

		cv::Rect rect(nROI_X, nROI_Y, nGapWidth, nGapHeight);
		cv::Mat roiCanny(rect.height, rect.width, CV_8UC1);
		for (int i = 0; i < roiCanny.rows; i++) {
			memcpy(&roiCanny.data[i * roiCanny.step1()], &matCanny.data[(i + nROI_Y) * matCanny.step1() + nROI_X], roiCanny.cols);
		}

		std::vector<std::vector<cv::Point> > contours;
		std::vector<cv::Vec4i> hierarchy;

		findContours(roiCanny, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

		for (int i = 0; i < contours.size(); i++)
		{
			if (contours[i].size() < 30)
				continue;

			int distMaxPts = MEASUREMENT_POINTS_DIST_MAX_SIDECAM_COUNT;
			int distMinPts = MEASUREMENT_POINTS_DIST_MIN_SIDECAM_COUNT;

			std::vector<int> vecY;

			for (int j = 0; j < contours[i].size(); j++) {
				vecY.push_back(contours[i][j].y);
			}

			std::vector<int> vecdistMinIndices;
			std::vector<int> vecdistMaxIndices;

			FindSmallestElementsInVector(vecY, distMinPts, vecdistMinIndices);
			FindLagestElementsInVector(vecY, distMaxPts, vecdistMaxIndices);

			//auto indices = n_largest_indices(vecY.begin(), vecY.end(), maxPts);
			for (auto k : vecdistMinIndices) {
				cv::Point pt(contours[i].at(k).x + nROI_X, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
			for (auto k : vecdistMaxIndices) {
				cv::Point pt(contours[i].at(k).x + nROI_X, contours[i].at(k).y + rectROI.y);
				vecMeaPts.push_back(pt);
			}
		}

		nROI_X += nGapWidth;

#ifdef _USE_TBB
	});
#else
	}

#endif

	if (vecMeaPts.size() == 0)
		return FALSE;

	return TRUE;
}

BOOL CSealingInspectCore::CalculateDistancePointToLine(cv::Point measurePt, cv::Point2f p1, cv::Point2f p2, cv::Point2f& closesPt, float& fDistance)
{
	float dx = p2.x - p1.x;
	float dy = p2.y - p1.y;
	float fDist = 0.0;

	if (dx == 0 && dy == 0)
	{
		// It's a point not a line segment.
		closesPt = p1;
		dx = measurePt.x - p1.x;
		dy = measurePt.y - p1.y;
		fDist = std::sqrt(dx * dx + dy * dy);
		return FALSE;
	}

	// Calculate the t that minimizes the distance.
	float t = ((measurePt.x - p1.x) * dx + (measurePt.y - p1.y) * dy) / (dx * dx + dy * dy);

	// See if this represents one of the segment's
	// end points or a point in the middle.
	if (t < 0)
	{
		closesPt = cv::Point2f(p1.x, p1.y);
		dx = measurePt.x - p1.y;
		dy = measurePt.y - p1.y;
	}
	else if (t > 1)
	{
		closesPt = cv::Point2f(p2.x, p2.y);
		dx = measurePt.x - p2.x;
		dy = measurePt.y - p2.y;
	}
	else
	{
		closesPt = cv::Point2f(p1.x + t * dx, p1.y + t * dy);
		dx = measurePt.x - closesPt.x;
		dy = measurePt.y - closesPt.y;
	}

	fDist = std::sqrt(dx * dx + dy * dy);
	fDistance = fDist;

	return TRUE;
}

BOOL CSealingInspectCore::FindClosesPointAndDistancePointToLine(const std::vector<cv::Point>& vecMeasurePt, const std::vector<cv::Point2f>& vecPtLineTop, std::vector<cv::Point2f>& vecClosesPt, std::vector<double>& vecDist)
{
	if (vecMeasurePt.empty())
		return FALSE;

	if (vecPtLineTop.empty())
		return FALSE;

	cv::Point2f closesPt;
	float fDist = 0.0;
	for (auto it : vecMeasurePt)
	{
		CalculateDistancePointToLine(it, vecPtLineTop[0], vecPtLineTop[1], closesPt, fDist);
		vecClosesPt.push_back(closesPt);
		vecDist.push_back(fDist);
	}

	return TRUE;
}

void CSealingInspectCore::MakeROIAdvancedAlgorithms(CRecipe_TopCam_Frame1 recipeTopCamFrame1, std::vector<cv::Rect>& vecRectROI, cv::Point centerPt, double dRadius)
{

	// Width , Height
	int nROIWidth_Hor = recipeTopCamFrame1.m_nROIWidth_Hor;
	int nROIHeight_Hor = recipeTopCamFrame1.m_nROIHeight_Hor;
	int nROIWidth_Ver = recipeTopCamFrame1.m_nROIWidth_Ver;
	int nROIHeight_Ver = recipeTopCamFrame1.m_nROIHeight_Ver;

	// Offset
	int nROI12H_Offset_X = recipeTopCamFrame1.m_nROI12H_OffsetX;
	int nROI12H_Offset_Y = recipeTopCamFrame1.m_nROI12H_OffsetY;
	int nROI3H_Offset_X = recipeTopCamFrame1.m_nROI3H_OffsetX;
	int nROI3H_Offset_Y = recipeTopCamFrame1.m_nROI3H_OffsetY;
	int nROI6H_Offset_X = recipeTopCamFrame1.m_nROI6H_OffsetX;
	int nROI6H_Offset_Y = recipeTopCamFrame1.m_nROI6H_OffsetY;
	int nROI9H_Offset_X = recipeTopCamFrame1.m_nROI9H_OffsetX;
	int nROI9H_Offset_Y = recipeTopCamFrame1.m_nROI9H_OffsetY;

	// Hor
	int nX_12H = centerPt.x - nROI12H_Offset_X;
	int nY_12H = centerPt.y - ((int)dRadius + nROI12H_Offset_Y);
	int nX_6H = centerPt.x - nROI6H_Offset_X;
	int nY_6H = centerPt.y + ((int)dRadius - nROI6H_Offset_Y);

	// Ver
	int nX_3H = centerPt.x + ((int)dRadius - nROI3H_Offset_X);
	int nY_3H = centerPt.y - nROI3H_Offset_Y;
	int nX_9H = centerPt.x - ((int)dRadius + nROI9H_Offset_X);
	int nY_9H = centerPt.y - nROI9H_Offset_Y;

	// Rect ROI
	cv::Rect rectROI12H(nX_12H, nY_12H, nROIWidth_Hor, nROIHeight_Hor);
	cv::Rect rectROI3H(nX_3H, nY_3H, nROIWidth_Ver, nROIHeight_Ver);
	cv::Rect rectROI6H(nX_6H, nY_6H, nROIWidth_Hor, nROIHeight_Hor);
	cv::Rect rectROI9H(nX_9H, nY_9H, nROIWidth_Ver, nROIHeight_Ver);

	vecRectROI.push_back(rectROI12H);
	vecRectROI.push_back(rectROI3H);
	vecRectROI.push_back(rectROI6H);
	vecRectROI.push_back(rectROI9H);
}

void CSealingInspectCore::MakeROITopCamFrame2(const CRecipe_TopCam_Frame2* pRecipeTopCamFrame2, std::vector<cv::Rect>& vecRectROI, cv::Mat* pMatProcess, cv::Point centerPt, double dRadius)
{
	if (pMatProcess->empty())
		return;

	cv::Mat matGray;
	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	// Width , Height
	int nROIWidth = pRecipeTopCamFrame2->m_nROIWidth;
	int nROIHeight = pRecipeTopCamFrame2->m_nROIHeight;

	// Offset
	int nROI1H_Offset_X = pRecipeTopCamFrame2->m_nROI1H_OffsetX;
	int nROI1H_Offset_Y = pRecipeTopCamFrame2->m_nROI1H_OffsetY;
	int nROI5H_Offset_X = pRecipeTopCamFrame2->m_nROI5H_OffsetX;
	int nROI5H_Offset_Y = pRecipeTopCamFrame2->m_nROI5H_OffsetY;
	int nROI7H_Offset_X = pRecipeTopCamFrame2->m_nROI7H_OffsetX;
	int nROI7H_Offset_Y = pRecipeTopCamFrame2->m_nROI7H_OffsetY;
	int nROI11H_Offset_X = pRecipeTopCamFrame2->m_nROI11H_OffsetX;
	int nROI11H_Offset_Y = pRecipeTopCamFrame2->m_nROI11H_OffsetY;

	int nX_1H = centerPt.x + nROI1H_Offset_X;
	int nY_1H = centerPt.y - nROI1H_Offset_Y;

	int nX_5H = centerPt.x + nROI5H_Offset_X;
	int nY_5H = centerPt.y + nROI5H_Offset_Y;

	int nX_7H = centerPt.x - nROI7H_Offset_X;
	int nY_7H = centerPt.y + nROI7H_Offset_Y;

	int nX_11H = centerPt.x - nROI11H_Offset_X;
	int nY_11H = centerPt.y - nROI11H_Offset_Y;

	// Rect ROI
	cv::Rect rectROI1H(nX_1H, nY_1H, nROIWidth, nROIHeight);
	cv::Rect rectROI5H(nX_5H, nY_5H, nROIWidth, nROIHeight);
	cv::Rect rectROI7H(nX_7H, nY_7H, nROIWidth, nROIHeight);
	cv::Rect rectROI11H(nX_11H, nY_11H, nROIWidth, nROIHeight);

	vecRectROI.push_back(rectROI1H);
	vecRectROI.push_back(rectROI5H);
	vecRectROI.push_back(rectROI7H);
	vecRectROI.push_back(rectROI11H);
}

void CSealingInspectCore::MakeROIFindLine(CSealingInspectRecipe_SideCam& pRecipeSideCam, int nFrame, cv::Rect& rectROIFindLIne)
{

	int nROI_X = 0;
	int nROI_Y = 0;
	int nROIWidth = 0;
	int nROIHeight = 0;

	switch (nFrame)
	{
	case 1:
		nROI_X = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[3];
		break;
	case 2:
		nROI_X = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[3];
		break;
	case 3:
		nROI_X = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[3];
		break;
	case 4:
		nROI_X = pRecipeSideCam.m_recipeFrame4.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame4.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame4.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame4.m_nROI_Top[3];
		break;
	}

	if (nROIWidth == 0 && nROIHeight == 0)
		return;

	cv::Rect rectROI(nROI_X, nROI_Y, nROIWidth, nROIHeight);

	rectROIFindLIne = rectROI;
}

void CSealingInspectCore::MakeROIFindPoints(CSealingInspectRecipe_SideCam& pRecipeSideCam, int nFrame, cv::Rect& rectROIFindPts)
{

	int nROI_X = 0;
	int nROI_Y = 0;
	int nROIWidth = 0;
	int nROIHeight = 0;

	switch (nFrame)
	{
	case 1:
		nROI_X = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[3];
		break;
	case 2:
		nROI_X = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[3];
		break;
	case 3:
		nROI_X = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[3];
		break;
	case 4:
		nROI_X = pRecipeSideCam.m_recipeFrame4.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame4.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame4.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame4.m_nROI_Bottom[3];
		break;
	}

	if (nROIWidth == 0 && nROIHeight == 0)
		return;

	cv::Rect rectROI(nROI_X, nROI_Y, nROIWidth, nROIHeight);

	rectROIFindPts = rectROI;
}

BOOL CSealingInspectCore::MakeCannyEdgeImage(cv::Mat* pImageData, cv::Mat& pEdgeImageData, double dThreshold1, double dThreshold2, int nGaussianMask /* = 3 */)
{
	if (pImageData->empty())
		return FALSE;

	cv::Mat matBlur;

	if (nGaussianMask < 3)
		matBlur = *(pImageData);
	else
		cv::GaussianBlur(*(pImageData), matBlur, cv::Size(nGaussianMask, nGaussianMask), 3.0);

	cv::Canny(matBlur, pEdgeImageData, dThreshold1, dThreshold2);
	//cv::imshow("Canny Side Cam", pEdgeImageData);

	return TRUE;
}

BOOL CSealingInspectCore::FindLagestElementsInVector(std::vector<int>& vecNum, int k, std::vector<int>& vecElementIndex)
{
	std::priority_queue< std::pair<int, int>, std::vector< std::pair<int, int> >, std::greater <std::pair<int, int> > > q;

	for (int i = 0; i < vecNum.size(); ++i) {
		if (q.size() < k)
			q.push(std::pair<int, int>(vecNum[i], i));
		else if (q.top().first < vecNum[i]) {
			q.pop();
			q.push(std::pair<int, int>(vecNum[i], i));
		}
	}
	std::vector<int> res(k);
	for (int j = 0; j < k; ++j) {
		res[k - j - 1] = q.top().second;
		q.pop();
	}

	vecElementIndex = res;

	return TRUE;
}

BOOL CSealingInspectCore::FindSmallestElementsInVector(std::vector<int>& vecNum, int k, std::vector<int>& vecElementIndex)
{
	std::priority_queue< std::pair<int, int>, std::vector< std::pair<int, int> >, std::less <std::pair<int, int> > > q;

	for (int i = 0; i < vecNum.size(); ++i) {
		if (q.size() < k)
			q.push(std::pair<int, int>(vecNum[i], i));
		else if (q.top().first > vecNum[i]) {
			q.pop();
			q.push(std::pair<int, int>(vecNum[i], i));
		}
	}
	std::vector<int> res(k);
	for (int j = 0; j < k; ++j) {
		res[k - j - 1] = q.top().second;
		q.pop();
	}

	vecElementIndex = res;

	return TRUE;
}

BOOL CSealingInspectCore::FindLine_Top_Bottom_Average(CSealingInspectRecipe_SideCam& pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROI, std::vector<cv::Point2f>& vecPtsLine)
{
	if (pMatProcess->empty())
		return FALSE;

	cv::Mat matGray;
	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	cv::Mat matROIFindLine(rectROI.height, rectROI.width, CV_8UC1);
	for (int i = 0; i < matROIFindLine.rows; i++)
		memcpy(&matROIFindLine.data[i * matROIFindLine.step1()], &matGray.data[(i + rectROI.y) * matGray.step1() + rectROI.x], matROIFindLine.cols);

	int nFindStartEndLineX = 0;
	int nFindStartEndLineSearchRangeX = 0;
	int nThreshold = 0;

	switch (nFrame)
	{
	case 1:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndXThresholdGray;
		break;
	case 2:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndXThresholdGray;
		break;
	case 3:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndXThresholdGray;
		break;
	case 4:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame4.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame4.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame4.m_nFindStartEndXThresholdGray;
		break;
	}

	int nSize = matROIFindLine.total() * matROIFindLine.elemSize();

	LPBYTE pBuffer = new BYTE[nSize];
	std::memcpy(pBuffer, matROIFindLine.data, nSize * sizeof(BYTE));

	int nFrameWidth = rectROI.width;
	int nFrameHeight = rectROI.height;

	int nMinSearch = nFindStartEndLineX - (nFindStartEndLineSearchRangeX / 2);
	int nMaxSearch = nFindStartEndLineX + (nFindStartEndLineSearchRangeX / 2);

	nMinSearch = (nMinSearch < 0) ? 0 : (nFrameWidth < nMinSearch) ? nFrameWidth - (nFindStartEndLineSearchRangeX / 2) : nMinSearch;
	nMaxSearch = (nMaxSearch < 0) ? nFindStartEndLineSearchRangeX / 2 : (nFrameWidth < nMaxSearch) ? nFrameWidth - 1 : nMaxSearch;

	int nRange = nMaxSearch - nMinSearch;
	int nFrameY = 0;

	if (nRange == 0)
		return FALSE;

	// 3. Find Threshold Point
	std::vector<cv::Point> vecFindLineFitPoints;
	vecFindLineFitPoints.resize(nFindStartEndLineSearchRangeX, cv::Point(-1, -1));

	const int nConst_Pitch = 5;
	const int nConst_ContinueCount = 3;

#ifdef USE_TBB
	tbb::parallel_for(0, nFindStartEndLineSearchRangeX, [&](int nX)
#else
	for (int nX = 0; nX < nFindStartEndLineSearchRangeX; nX++)
#endif
	{
		int nContinueCount = 0;

		for (int nY = 0; nY < nFrameHeight - nConst_Pitch; nY++)
		{
			int nGrayValue_Current = (int)pBuffer[(nY * nFrameWidth) + (nX + nMinSearch)];
			int nGrayValue_Compare = (int)pBuffer[((nY + nConst_Pitch) * nFrameWidth) + (nX + nMinSearch)];
			int nGray_Diff = abs(nGrayValue_Current - nGrayValue_Compare);

			if (nThreshold < nGray_Diff)
				nContinueCount++;
			else
				nContinueCount = 0;

			if (nConst_ContinueCount <= nContinueCount)
			{
				vecFindLineFitPoints[nX] = cv::Point(nX + nMinSearch, nY + nConst_Pitch - nContinueCount + nFrameY);
				break;
			}
		}
#ifdef USE_TBB
	});
#else
}

#endif


	int nSum = 0;
	int nCount = 0;

	for (int i = 0; i < (int)vecFindLineFitPoints.size(); i++)
	{
		if (0 <= vecFindLineFitPoints[i].y)
		{
			nSum += vecFindLineFitPoints[i].y;
			nCount++;
		}
	}

	if (nCount == 0)
		return FALSE;

	// Average..
	int nFindLine = nSum / nCount;

	// Line Fitting..
	const int nFindFit = 3;
	int nStartSum = 0;
	int nEndSum = 0;
	for (int i = 0; i < nFindFit; i++)
	{
		int nStartIdx = i;
		int nEndIdx = vecFindLineFitPoints.size() - 1 - i;

		nStartSum += vecFindLineFitPoints[nStartIdx].y;
		nEndSum += vecFindLineFitPoints[nEndIdx].y;
	}
	float fStartAvgr = ((float)nStartSum) / ((float)nFindFit);
	float fEndAvgr = ((float)nEndSum) / ((float)nFindFit);
	float fSlope = (fEndAvgr - fStartAvgr) / vecFindLineFitPoints.size();

	cv::Point2f ptFitLinePoint_1 = cv::Point2f(0, (((-vecFindLineFitPoints[0].x) * fSlope + fStartAvgr)));
	cv::Point2f ptFitLinePoint_2 = cv::Point2f(nFrameWidth - 1, ((((double)(nFrameWidth - 1) - vecFindLineFitPoints[0].x) * fSlope + fStartAvgr)));

	vecPtsLine.push_back(cv::Point2f(ptFitLinePoint_1.x + rectROI.x, ptFitLinePoint_1.y + rectROI.y));
	vecPtsLine.push_back(cv::Point2f(ptFitLinePoint_2.x + rectROI.x, ptFitLinePoint_2.y + rectROI.y));

	return TRUE;
}

BOOL CSealingInspectCore::FindLine_Bottom_Top_Average(CSealingInspectRecipe_SideCam* pRecipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROI, cv::Mat& matROI, std::vector<cv::Point2f>& vecPtsLine)
{
	int nFindStartEndLineX = 0;
	int nFindStartEndLineSearchRangeX = 0;
	int nThreshold = 0;

	switch (nFrame)
	{
	case 1:
		nFindStartEndLineX = pRecipeSideCam->m_recipeFrame1.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam->m_recipeFrame1.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam->m_recipeFrame1.m_nFindStartEndXThresholdGray;
		break;
	case 2:
		nFindStartEndLineX = pRecipeSideCam->m_recipeFrame2.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam->m_recipeFrame2.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam->m_recipeFrame2.m_nFindStartEndXThresholdGray;
		break;
	case 3:
		nFindStartEndLineX = pRecipeSideCam->m_recipeFrame3.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam->m_recipeFrame3.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam->m_recipeFrame3.m_nFindStartEndXThresholdGray;
		break;
	case 4:
		nFindStartEndLineX = pRecipeSideCam->m_recipeFrame4.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam->m_recipeFrame4.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam->m_recipeFrame4.m_nFindStartEndXThresholdGray;
		break;
	}

	int nSize = matROI.total() * matROI.elemSize();

	LPBYTE pBuffer = new BYTE[nSize];
	std::memcpy(pBuffer, matROI.data, nSize * sizeof(BYTE));

	int nFrameWidth = rectROI.width;
	int nFrameHeight = rectROI.height;

	int nSearchFrameCount = 1;
	int nSearchFrameSize = nFrameHeight * nSearchFrameCount;

	int nMinSearch = nFindStartEndLineX - (nFindStartEndLineSearchRangeX / 2);
	int nMaxSearch = nFindStartEndLineX + (nFindStartEndLineSearchRangeX / 2);

	nMinSearch = (nMinSearch < 0) ? 0 : (nFrameWidth < nMinSearch) ? nFrameWidth - (nFindStartEndLineSearchRangeX / 2) : nMinSearch;
	nMaxSearch = (nMaxSearch < 0) ? nFindStartEndLineSearchRangeX / 2 : (nFrameWidth < nMaxSearch) ? nFrameWidth - 1 : nMaxSearch;

	int nRange = nMaxSearch - nMinSearch;
	int nFrameY = 0;

	if (nRange == 0)
		return FALSE;

	// 3. Find Threshold Point
	std::vector<cv::Point> vecFindLineFitPoints;
	vecFindLineFitPoints.resize(nFindStartEndLineSearchRangeX, cv::Point(-1, -1));


	const int nPitch = 5;

#ifdef USE_TBB
	tbb::parallel_for(0, nFindStartEndLineSearchRangeX, [&](int nX)
#else
	for (int nX = 0; nX < nFindStartEndLineSearchRangeX; nX++)
#endif
	{
		for (int nY = nSearchFrameSize - 1; nPitch < nY; nY--)
		{
			int nGrayValue_Current = (int)pBuffer[(nY * nFrameWidth) + (nX + nMinSearch)];
			int nGrayValue_Compare = (int)pBuffer[((nY - nPitch) * nFrameWidth) + (nX + nMinSearch)];
			int nGray_Diff = abs(nGrayValue_Current - nGrayValue_Compare);

			if (nThreshold < nGray_Diff)
			{
				vecFindLineFitPoints[nX] = cv::Point(nX + nMinSearch, nY - nPitch + nFrameY);
				break;
			}
		}
#ifdef USE_TBB
	});
#else
}
#endif

	int nSum = 0;
	int nCount = 0;

	// Line Fitting..
	const int nFindFit = 3;
	int nStartSum = 0;
	int nEndSum = 0;
	for (int i = 0; i < nFindFit; i++)
	{
		int nStartIdx = i;
		int nEndIdx = vecFindLineFitPoints.size() - 1 - i;

		nStartSum += vecFindLineFitPoints[nStartIdx].y;
		nEndSum += vecFindLineFitPoints[nEndIdx].y;
	}
	float fStartAvgr = ((float)nStartSum) / ((float)nFindFit);
	float fEndAvgr = ((float)nEndSum) / ((float)nFindFit);
	float fSlope = (fEndAvgr - fStartAvgr) / vecFindLineFitPoints.size();

	cv::Point2f ptFitLinePoint_1 = cv::Point2f(0, (int)(((-vecFindLineFitPoints[0].x) * fSlope + fStartAvgr) + 0.5));
	cv::Point2f ptFitLinePoint_2 = cv::Point2f(nFrameWidth - 1, (int)((((double)(nFrameWidth - 1) - vecFindLineFitPoints[0].x) * fSlope + fStartAvgr) + 0.5));

	vecPtsLine.push_back(cv::Point2f(ptFitLinePoint_1.x + rectROI.x, ptFitLinePoint_1.y + rectROI.y));
	vecPtsLine.push_back(cv::Point2f(ptFitLinePoint_2.x + rectROI.x, ptFitLinePoint_2.y + rectROI.y));
}

cv::Point2f CSealingInspectCore::FindIntersectionPoint_LineCircle(cv::Point2i pt, cv::Point2f centerPt, double dRadius)
{
	cv::Point2f IntsecPt;
	double t;

	double dx = centerPt.x - pt.x;
	double dy = centerPt.y - pt.y;

	double A = dx * dx + dy * dy;
	double B = 2 * (dx * (pt.x - centerPt.x) + dy * (pt.y - centerPt.y));
	double C = (pt.x - centerPt.x) * (pt.x - centerPt.x) + (pt.y - centerPt.y) * (pt.y - centerPt.y) - dRadius * dRadius;

	double delta = B * B - 4 * A * C;
	if (delta > 0) {
		t = ((-1) * B + std::sqrt(delta)) / (2 * A);
		IntsecPt = cv::Point2f(pt.x + t * dx, pt.y + t * dy);
	}

	return IntsecPt;
}

double CSealingInspectCore::CalculateDistancePointToCircle(cv::Point2i pt, cv::Point2f centerPt, double dRadius)
{
	double dDistance = 0.0;

	int nDelta_X = pt.x - centerPt.x;
	int nDelta_Y = pt.y - centerPt.y;

	dDistance = std::abs(std::sqrt(std::pow(nDelta_X, 2) + std::pow(nDelta_Y, 2)) - dRadius);

	return dDistance;
}

cv::Point2i CSealingInspectCore::CalculateIntersectionPointCoordinate(cv::Point2i pt, cv::Point2f centerPt, double dRadius, double dDist)
{
	cv::Point2i ptIntersection;

	int ndX = std::abs(centerPt.x - pt.x);
	int ndY = std::abs(centerPt.y - pt.y);
	double hypotenuse = std::sqrt(ndX * ndX + ndY * ndY);

	double angRad = std::atan2(centerPt.y - pt.y, centerPt.x - pt.x);
	double angDeg = RAD2DEG(angRad);
	int dx = 0, dy = 0;

	dx = std::abs(dDist * std::cos(angRad));
	dy = std::abs(dDist * std::sin(angRad));

	/*if ((angDeg > 0.01 && angDeg < 90) || (angDeg < -0.01 && angDeg > -90))
	{
		dx = dDist * std::cos(angRad);
		dy = dDist * std::sin(angRad);
	}
	else if ((angDeg > 90 && angDeg < 180) || (angDeg < -90 && angDeg > -180))
	{
		dx = dDist * std::sin(angRad);
		dy = dDist * std::cos(angRad);
	}
	else {

	}*/

	if (hypotenuse > dRadius)
	{
		if (pt.x > centerPt.x && pt.y < centerPt.y) {
			ptIntersection.x = pt.x - dx;
			ptIntersection.y = pt.y + dy;
		}
		else if (pt.x > centerPt.x && pt.y > centerPt.y) {
			ptIntersection.x = pt.x - dx;
			ptIntersection.y = pt.y - dy;
		}
		else if (pt.x < centerPt.x && pt.y < centerPt.y) {
			ptIntersection.x = pt.x + dx;
			ptIntersection.y = pt.y + dy;
		}
		else if (pt.x < centerPt.x && pt.y > centerPt.y) {
			ptIntersection.x = pt.x + dx;
			ptIntersection.y = pt.y - dy;
		}
	}
	else {
		if (pt.x > centerPt.x && pt.y < centerPt.y) {
			ptIntersection.x = pt.x + dx;
			ptIntersection.y = pt.y - dy;
		}
		else if (pt.x > centerPt.x && pt.y > centerPt.y) {
			ptIntersection.x = pt.x + dx;
			ptIntersection.y = pt.y + dy;
		}
		else if (pt.x < centerPt.x && pt.y < centerPt.y) {
			ptIntersection.x = pt.x - dx;
			ptIntersection.y = pt.y - dy;
		}
		else if (pt.x < centerPt.x && pt.y > centerPt.y) {
			ptIntersection.x = pt.x - dx;
			ptIntersection.y = pt.y + dy;
		}
	}

	return ptIntersection;
}

void CSealingInspectCore::Draw_MinEnclosing(cv::Mat& matSrc, std::vector<std::vector<cv::Point>>& vecContours, cv::Point2f centers, float radius)
{
	//cv::drawContours(mat, contours, (int)i, cv::Scalar(255, 0, 255), 1, cv::LINE_AA, hierarchy, 0);
	cv::circle(matSrc, centers, (int)radius, GREEN_COLOR, 1);
	cv::circle(matSrc, centers, 3, PINK_COLOR, cv::FILLED, cv::LINE_AA);
	char ch[256];
	sprintf_s(ch, sizeof(ch), "R = %.2f pxl", radius);
	std::string text(ch);
	cv::putText(matSrc, text, cv::Point(centers.x - 100, centers.y - radius - 10), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);

	char ch2[256];
	sprintf_s(ch2, sizeof(ch2), "(x: %.1f, y: %.1f)", centers.x, centers.y);
	cv::putText(matSrc, ch2, cv::Point(centers.x - 300, centers.y), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);
}

void CSealingInspectCore::Draw_HoughCircle(cv::Mat& matSrc, std::vector<cv::Vec3f>& vecCircles, std::vector<cv::Point2i>& vecPts)
{
	for (size_t i = 0; i < vecCircles.size(); i++)
	{
		cv::Point center(cvRound(vecCircles[i][0]), cvRound(vecCircles[i][1]));
		int radius = cvRound(vecCircles[i][2]);
		cv::circle(matSrc, center, 3, RED_COLOR, -1);
		cv::circle(matSrc, center, radius, YELLOW_COLOR, 1);

		char ch2[256];
		sprintf_s(ch2, sizeof(ch2), "(x: %d, y: %d)", center.x, center.y);
		cv::putText(matSrc, ch2, cv::Point(center.x + 50, center.y), cv::FONT_HERSHEY_PLAIN, 1.5, BLUE_COLOR);
	}

}

void CSealingInspectCore::DrawDistance(cv::Mat& mat, std::vector<cv::Point> vecPts, std::vector<cv::Point> vecIntsecPts)
{
	for (int i = 0; i < vecPts.size(); i++) {
		cv::line(mat, vecPts[i], vecIntsecPts[i], GREEN_COLOR, 1, cv::LINE_AA);
		cv::circle(mat, vecIntsecPts[i], 1, ORANGE_COLOR, 1, cv::FILLED);
		cv::circle(mat, vecPts[i], 1, ORANGE_COLOR, 1, cv::FILLED);
	}
}

void CSealingInspectCore::DrawPositionNG(cv::Mat& mat, std::vector<int>& vecPosNG, std::vector<cv::Point> vecPts)
{
	// draw pos NG

		/*for (auto it = vecPosNG.begin(); it != vecPosNG.end(); ++it) {
			int x = vecPts[vecPosNG[*it]].x - 15;
			int y = vecPts[vecPosNG[*it]].y - 15;
			int width = 30;
			int height = 30;
			cv::Rect rec(x, y, width, height);
			cv::rectangle(mat, rec, RED_COLOR, 1, cv::LINE_AA);
		}*/
	for (int i = 0; i < vecPosNG.size(); i++) {
		int x = vecPts[vecPosNG[i]].x - 15;
		int y = vecPts[vecPosNG[i]].y - 15;
		int width = 30;
		int height = 30;
		cv::Rect rec(x, y, width, height);
		cv::rectangle(mat, rec, RED_COLOR, 1, cv::LINE_AA);
	}

	vecPosNG.clear();
}

void CSealingInspectCore::DrawROIFindLine(cv::Mat& mat, cv::Rect rectROI, std::vector<cv::Point2f> vecPtsLine)
{
	cv::rectangle(mat, rectROI, BLUE_COLOR, 2, cv::LINE_AA);
	if (!vecPtsLine.empty())
		cv::line(mat, vecPtsLine.at(0), vecPtsLine.at(1), YELLOW_COLOR, 2, cv::LINE_AA);
}

void CSealingInspectCore::DrawROIFindPoints(cv::Mat& mat, cv::Rect rectROI, std::vector<cv::Point> vecMeasurePt, std::vector<cv::Point2f> vecClosesPt)
{
	cv::rectangle(mat, rectROI, BLUE_COLOR, 2, cv::LINE_AA);
	for (int i = 0; i < vecMeasurePt.size(); i++)
	{
		cv::circle(mat, vecMeasurePt[i], 1, RED_COLOR, 1, cv::LINE_AA);
		cv::circle(mat, vecClosesPt[i], 1, RED_COLOR, 1, cv::LINE_AA);
		cv::line(mat, vecMeasurePt[i], vecClosesPt[i], GREEN_COLOR, 1, cv::LINE_AA);
	}
}

void CSealingInspectCore::ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	/*if (m_bSimulator == TRUE)
		return;*/

	if (mat.empty())
		return;

	CRecipe_TopCam_Frame1 recipeTopCamFrame1 = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1;

	int nFrame = 0;

	// params minEnclosing
	int nThresholdBinary = recipeTopCamFrame1.m_nThresholdBinaryMinEnclosing;
	int nRadiusInnerMin = recipeTopCamFrame1.m_nRadiusInner_Min;
	int nRadiusInnerMax = recipeTopCamFrame1.m_nRadiusInner_Max;
	int nContourSizeMin = recipeTopCamFrame1.m_nContourSizeMinEnclosingCircle_Min;
	int nContourSizeMax = recipeTopCamFrame1.m_nContourSizeMinEnclosingCircle_Max;

	// params HoughCircle
	int nThresholdCannyHoughCircle = recipeTopCamFrame1.m_nThresholdBinaryCannyHoughCircle;
	int nRadiusOuterMin = recipeTopCamFrame1.m_nRadiusOuter_Min;
	int nRadiusOuterMax = recipeTopCamFrame1.m_nRadiusOuter_Max;
	int nDistRadiusDiffMin = recipeTopCamFrame1.m_nDistanceRadiusDiffMin;
	double dIncrementAngle = recipeTopCamFrame1.m_dIncrementAngle;

	// Roi
	double nThresholdCanny1_MakeROI = recipeTopCamFrame1.m_dThresholdCanny1_MakeROI;
	double nThresholdCanny2_MakeROI = recipeTopCamFrame1.m_dThresholdCanny2_MakeROI;
	int nWidthROI_Hor = recipeTopCamFrame1.m_nROIWidth_Hor;
	int nHeightROI_Hor = recipeTopCamFrame1.m_nROIHeight_Hor;
	int nWidthROI_Ver = recipeTopCamFrame1.m_nROIWidth_Ver;
	int nHeightROI_Ver = recipeTopCamFrame1.m_nROIHeight_Ver;

	// judgement
	int nDeltaRadiusTolerance = recipeTopCamFrame1.m_nDeltaRadiusOuterInner;
	double nDistanceMeasureToleranceMin = recipeTopCamFrame1.m_dDistanceMeasurementTolerance_Min;
	double nDistanceMeasureToleranceMax = recipeTopCamFrame1.m_dDistanceMeasurementTolerance_Max;
	int nNumberOfDistNGMax = recipeTopCamFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	BOOL bUseAdvancedAlgorithms = recipeTopCamFrame1.m_bUseAdvancedAlgorithms;

	// MinEnclosingCircle
	std::vector<std::vector<cv::Point> > vecContours;
	std::vector<cv::Vec4i> vecHierarchy;
	std::vector<cv::Point2f> vecCenters;
	std::vector<float> vecRadius;

	// HoughCircle
	std::vector<cv::Vec3f> vecCircles;
	std::vector<cv::Point2i> vecPoints;
	std::vector<cv::Point2i> vecPtsIntersection;
	std::vector<cv::Point2f> vecIntsecPtsFound;

	// params for calculate Distance
	cv::Point2f center_Inner;
	cv::Point2f center_Outer;
	double dRadius_Inner;
	double dRadius_Outer;
	std::vector<double> vecDist;
	std::vector<int> vecPosNG;

	BOOL bFindCircle_MinEnclosing = FALSE;
	BOOL bFindCircle_HoughCircle = FALSE;
	BOOL nRet = FALSE;

	cv::Mat matProcess, matGray;
	cv::cvtColor(mat, matGray, cv::COLOR_BGR2GRAY);
	mat.copyTo(matProcess);

	// Find inner circle
	bFindCircle_MinEnclosing = FindCircle_MinEnclosing(&matProcess, nThresholdBinary, nContourSizeMin, nContourSizeMax, nRadiusInnerMin, nRadiusInnerMax, vecContours, vecHierarchy, vecCenters, vecRadius, center_Inner, dRadius_Inner);

	if (bFindCircle_MinEnclosing == FALSE) {
		// set buffer
		nRet &= bFindCircle_MinEnclosing;
		m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = nRet;
		return;
	}

	m_ptCenter_Inner = center_Inner;
	m_dRadius_Inner = dRadius_Inner;
	Draw_MinEnclosing(mat, vecContours, center_Inner, dRadius_Inner);


	// Find outer circle
	bFindCircle_HoughCircle = FindCircle_HoughCircle(&matProcess, vecCircles, vecPoints, nThresholdCannyHoughCircle, nDistRadiusDiffMin, nRadiusOuterMin, nRadiusOuterMax, dIncrementAngle);

	if (bFindCircle_HoughCircle == FALSE) {
		// set buffer
		nRet &= bFindCircle_HoughCircle;
		m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = nRet;
		return;
	}

	Draw_HoughCircle(mat, vecCircles, vecPoints);
	dRadius_Outer = (vecCircles[0][2]);
	center_Outer = cv::Point2f((vecCircles[0][0]), (vecCircles[0][1]));
	m_ptCenter_Outer = center_Outer;
	m_dRadius_Outer = dRadius_Outer;

	if (vecPoints.size() != 0)
	{
		// calculate distance
		FindDistanceAll_OuterToInner(vecPoints, vecPtsIntersection, vecIntsecPtsFound, center_Inner, dRadius_Inner, vecDist);
		DrawDistance(mat, vecPoints, vecPtsIntersection);
	}

	// Judgement

	int nDeltaRadius = (int)(dRadius_Outer - dRadius_Inner);

	nRet = nDeltaRadius < nDeltaRadiusTolerance ? TRUE : FALSE;

	nRet &= JudgementInspectDistanceMeasurement(vecDist, vecPosNG, nDistanceMeasureToleranceMin, nDistanceMeasureToleranceMax);

	if (!vecPosNG.empty())
		DrawPositionNG(mat, vecPosNG, vecPoints);

	if (bUseAdvancedAlgorithms == TRUE)
	{
		// Inspection advanced
		std::vector<cv::Rect> vecRectROI;
		std::vector<cv::Mat> vecMatROI;

		std::vector<cv::Point> vecMeaPts;
		std::vector<double> vecDistPointToCircle;

		// create ROI from center point of circle Inner
		MakeROIAdvancedAlgorithms(recipeTopCamFrame1, vecRectROI, center_Inner, dRadius_Inner);

		if (!vecRectROI.empty())
		{
			for (int i = 0; i < vecRectROI.size(); i++)
			{
				FindMeasurePointsAtPosMinMax(&recipeTopCamFrame1, &matProcess, vecRectROI[i], vecMeaPts, i);

				if (!vecMeaPts.empty()) {
					for (auto it = vecMeaPts.begin(); it != vecMeaPts.end(); ++it)
					{
						double distance = CalculateDistancePointToCircle(*it, center_Outer, dRadius_Outer);
						vecDistPointToCircle.push_back(distance);

						cv::circle(mat, *it, 1, RED_COLOR, 1, cv::LINE_AA);
					}
				}
			}
		}
		nRet &= JudgementInspectDistanceMeasurement_AdvancedAlgorithms(vecDistPointToCircle, vecPosNG, nDistanceMeasureToleranceMin, nDistanceMeasureToleranceMax, nNumberOfDistNGMax);

		if (!vecPosNG.empty())
			DrawPositionNG(mat, vecPosNG, vecPoints);
	}

	// save image
	/*char chPath[255] = {};
	sprintf_s(chPath, "%s\\%s%d%s_%s%d%s", "D:\\Sealing_Folder\\Cavity1\\DefectImage", "Core", m_nCoreIdx, "TopCam1", "Frame", nFrame, ".bmp");
	cv::imwrite(chPath, mat);*/

	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = nRet;
}

void CSealingInspectCore::ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	/*if (m_bSimulator == TRUE)
		return;*/

	if (mat.empty())
		return;

	cv::Mat matProcess;
	mat.copyTo(matProcess);

	int nFrame = 1;

	CRecipe_TopCam_Frame2 recipeTopCamFrame2 = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame2;

	std::vector<cv::Rect> vecRectROI;
	std::vector<cv::Mat> vecMatROI;

	BOOL nRet = FALSE;

	if (m_ptCenter_Inner.x == 0 && m_ptCenter_Inner.y == 0 && m_dRadius_Inner == 0.0)
	{
		// set buffer
		nRet &= FALSE;
		m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame2 = nRet;
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bInspectComplete = TRUE;

		return;
	}

	MakeROITopCamFrame2(&recipeTopCamFrame2, vecRectROI, &matProcess, m_ptCenter_Inner, m_dRadius_Inner);

	for (auto it = vecRectROI.begin(); it != vecRectROI.end(); ++it)
	{
		cv::rectangle(mat, *it, PINK_COLOR, 2, cv::LINE_AA);
	}

	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame2 = TRUE;
	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bInspectComplete = TRUE;
}

void CSealingInspectCore::ProcessFrame_SideCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, int nFrameIdx, cv::Mat& mat)
{
	/*if (m_bSimulator == TRUE)
		return;*/

	if (mat.empty())
		return;

	// create a clone of 
	cv::Mat pMatProcess;
	mat.copyTo(pMatProcess);

	double dDistanceMax = 0.0;
	double dDistanceMin = 0.0;
	int nNumberOfNGMax = 0;

	// recipe side cam
	
	CSealingInspectRecipe_SideCam recipeSideCam = pRecipe->m_sealingInspRecipe_SideCam[nBufferProcessorIdx];

	switch (nFrameIdx)
	{
	case 1:
		dDistanceMin = recipeSideCam.m_recipeFrame1.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame1.m_dDistanceMeasurementTolerance_Max;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		break;
	case 2:
		dDistanceMin = recipeSideCam.m_recipeFrame2.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame2.m_dDistanceMeasurementTolerance_Max;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		break;
	case 3:
		dDistanceMin = recipeSideCam.m_recipeFrame3.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame3.m_dDistanceMeasurementTolerance_Max;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		break;
	case 4:
		dDistanceMin = recipeSideCam.m_recipeFrame4.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame4.m_dDistanceMeasurementTolerance_Max;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		break;
	}

	// 2. Process
	cv::Rect rectROIFindLine;

	cv::Rect rectROIFindPts;

	std::vector<cv::Point2f> vecPtLineTop;
	std::vector<cv::Point2f> vecPtLineBottom;
	std::vector<cv::Point> vecMeasurePt;
	std::vector<cv::Point2f> vecClosesPt;

	std::vector<double> vecDist;
	std::vector<int> vecPosNG;

	BOOL bFindLine_TopBottom = FALSE;
	BOOL bFindMeasurePt = FALSE;
	BOOL nRet = TRUE;

	MakeROIFindLine(recipeSideCam, nFrameIdx, rectROIFindLine);
	MakeROIFindPoints(recipeSideCam, nFrameIdx, rectROIFindPts);

	bFindLine_TopBottom = FindLine_Top_Bottom_Average(recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindLine, vecPtLineTop);
	//FindLine_Bottom_Top_Average(&recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindPts, matROIFindPts, vecPtsLineBottom);

	if (bFindLine_TopBottom == FALSE)
	{
		// 3. Set buff
		nRet &= bFindLine_TopBottom;
		m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);
		switch (nFrameIdx)
		{
		case 1:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
			break;
		case 2:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
			break;
		case 3:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
			break;
		case 4:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
			break;
		}

		return;
	}


	bFindMeasurePt = FindMeasurePoints_SideCam(recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindPts, vecMeasurePt);
	//FindMeasurePointsAtPosDistMinMax_SideCam(&recipeSideCam, &matROIFindPts, nFrameIdx, rectROIFindPts, vecMeasurePt);

	//FindClosesPointAndDistancePointToLine(vecMeasurePt, vecPtLineTop, vecClosesPt, vecDist);

	if (bFindMeasurePt == FALSE)
	{
		// 3. Set buff
		nRet &= bFindMeasurePt;
		m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);
		switch (nFrameIdx)
		{
		case 1:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
			break;
		case 2:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
			break;
		case 3:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
			break;
		case 4:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
			break;
		}

		return;
	}


	cv::Point2f closesPt;
	float fDist = 0.0;
	for (auto it : vecMeasurePt)
	{
		CalculateDistancePointToLine(it, vecPtLineTop[0], vecPtLineTop[1], closesPt, fDist);
		vecClosesPt.push_back(closesPt);
		vecDist.push_back(fDist);
	}

	nRet &= JudgementInspectDistanceMeasurement_AdvancedAlgorithms(vecDist, vecPosNG, dDistanceMin, dDistanceMax, nNumberOfNGMax);

	DrawROIFindLine(mat, rectROIFindLine, vecPtLineTop);
	//DrawROIFindLine(mat, rectROIFindPts, vecPtsLineBottom);
	DrawROIFindPoints(mat, rectROIFindPts, vecMeasurePt, vecClosesPt);
	DrawPositionNG(mat, vecPosNG, vecMeasurePt);

	// 3. Set buff

	m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);

	switch (nFrameIdx)
	{
	case 1:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
		break;
	case 2:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
		break;
	case 3:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
		break;
	case 4:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
		break;
	}
}

