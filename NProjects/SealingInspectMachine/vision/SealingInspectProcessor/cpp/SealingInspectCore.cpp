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
	CSealingInspectSystemSetting* sysSetting = m_pInterface->GetSystemSetting();

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam1_BufferHikCamIdx = 0;
	int nSideCam1_BufferHikCamIdx = 2;

	int nTopCam1_BufferProcessor = 0;
	int nSideCam1_BufferProcessor = 0;

	int nGotoPos1Time = sysSetting->m_nGoToPos1Time_Cavity1;
	int nGotoPos2Time = sysSetting->m_nGoToPos2Time_Cavity1;
	int nGotoPos3Time = sysSetting->m_nGoToPos3Time_Cavity1;
	int nGotoPos4Time = sysSetting->m_nGoToPos4Time_Cavity1;
	int nGotoPos5Time = sysSetting->m_nGoToPos5Time_Cavity1;
	int nGotoPos6Time = sysSetting->m_nGoToPos6Time_Cavity1;
	int nGotoPos7Time = sysSetting->m_nGoToPos7Time_Cavity1;
	int nGotoPos8Time = sysSetting->m_nGoToPos8Time_Cavity1;
	int nGotoPos9Time = sysSetting->m_nGoToPos9Time_Cavity1;
	int nGotoPos10Time = sysSetting->m_nGoToPos10Time_Cavity1;

	int nOffsetTimePos1 = sysSetting->m_nOffsetTime_Pos1_Cavity1;
	int nOffsetTimePos2 = sysSetting->m_nOffsetTime_Pos2_Cavity1;
	int nOffsetTimePos3 = sysSetting->m_nOffsetTime_Pos3_Cavity1;
	int nOffsetTimePos4 = sysSetting->m_nOffsetTime_Pos4_Cavity1;
	int nOffsetTimePos5 = sysSetting->m_nOffsetTime_Pos5_Cavity1;
	int nOffsetTimePos6 = sysSetting->m_nOffsetTime_Pos6_Cavity1;
	int nOffsetTimePos7 = sysSetting->m_nOffsetTime_Pos7_Cavity1;
	int nOffsetTimePos8 = sysSetting->m_nOffsetTime_Pos8_Cavity1;
	int nOffsetTimePos9 = sysSetting->m_nOffsetTime_Pos9_Cavity1;
	int nOffsetTimePos10 = sysSetting->m_nOffsetTime_Pos10_Cavity1;

	while (m_bRunningThread[nThreadIndex] == TRUE)
	{
		// for avoid UI Freezing
		Sleep(1);

		if (m_bSimulator == TRUE) {
			// LOCK PROCESS
			while (m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS != TRUE)
			{
				Sleep(50);
			}
			m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS = FALSE;
		}
		else if (m_bSimulator == FALSE)
		{
			while (m_pInterface->GetProcessStatus1() != TRUE)
			{
				Sleep(50);
			}
			m_pInterface->SetProcessStatus1(FALSE);
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

		Sleep(500);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(200);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam1_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam1_BufferHikCamIdx, nTopCam1_BufferProcessor, matTopResult);

		m_pInterface->InspectTopCam1Complete(FALSE);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

		/*int nCountFrame = 0;
		while (nCountFrame < 10)
		{
			if (m_pInterface->GetGrabFrameSideCam(m_nCoreIdx) == TRUE)
			{
				nCountFrame++;
				hikCamControl->SetFrameWaitProcess_SideCam(nSideCam1_BufferHikCamIdx);

				m_pInterface->GrabFrameSideCam1Complete(FALSE);
			}
		}*/

#ifdef TEST_INSPECT_CAVITY_1

		Sleep(nGotoPos1Time);

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			switch (i)
			{
			case 0:
				Sleep(nGotoPos1Time - nOffsetTimePos1);
				break;
			case 1:
				Sleep(nGotoPos2Time - nGotoPos1Time - nOffsetTimePos2);
				break;
			case 2:
				Sleep(nGotoPos3Time - nGotoPos2Time - nOffsetTimePos3);
				break;
			case 3:
				Sleep(nGotoPos4Time - nGotoPos3Time - nOffsetTimePos4);
				break;
			case 4:
				Sleep(nGotoPos5Time - nGotoPos4Time - nOffsetTimePos5);
				break;
			case 5:
				Sleep(nGotoPos6Time - nGotoPos5Time - nOffsetTimePos6);
				break;
			case 6:
				Sleep(nGotoPos7Time - nGotoPos6Time - nOffsetTimePos7);
				break;
			case 7:
				Sleep(nGotoPos8Time - nGotoPos7Time - nOffsetTimePos8);
				break;
			case 8:
				Sleep(nGotoPos9Time - nGotoPos8Time - nOffsetTimePos9);
				break;
			case 9:
				Sleep(nGotoPos10Time - nGotoPos9Time - nOffsetTimePos10);
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

		m_pInterface->InspectCavity1Complete(FALSE);
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

	m_pInterface->InspectCavity1Complete(FALSE);
}

void CSealingInspectCore::RunningThread_INSPECT_CAVITY2(int nThreadIndex)
{
	if (m_pInterface == NULL)
		return;

	// 1. Load Recipe

	CSealingInspectRecipe* recipe = m_pInterface->GetRecipe();
	CSealingInspectSystemSetting* sysSetting = m_pInterface->GetSystemSetting();

	/*
	1. At processor class have 2 pointer for TOP and SIDE
	2. At HikCam class just have 1 array 4 pointer, order is these pointer with 0, 1 index correspond TOPCAM1, TOPCAM2
	and 2, 3 index correspond SIDECAM1, SIDECAM2
	*/
	int nTopCam2_BufferHikCamIdx = 1;
	int nSideCam2_BufferHikCamIdx = 3;

	int nTopCam2_BufferProcessor = 1;
	int nSideCam2_BufferProcessor = 1;

	int nGotoPos1Time = sysSetting->m_nGoToPos1Time_Cavity1;
	int nGotoPos2Time = sysSetting->m_nGoToPos2Time_Cavity1;
	int nGotoPos3Time = sysSetting->m_nGoToPos3Time_Cavity1;
	int nGotoPos4Time = sysSetting->m_nGoToPos4Time_Cavity1;
	int nGotoPos5Time = sysSetting->m_nGoToPos5Time_Cavity1;
	int nGotoPos6Time = sysSetting->m_nGoToPos6Time_Cavity1;
	int nGotoPos7Time = sysSetting->m_nGoToPos7Time_Cavity1;
	int nGotoPos8Time = sysSetting->m_nGoToPos8Time_Cavity1;
	int nGotoPos9Time = sysSetting->m_nGoToPos9Time_Cavity1;
	int nGotoPos10Time = sysSetting->m_nGoToPos10Time_Cavity1;

	int nOffsetTimePos1 = sysSetting->m_nOffsetTime_Pos1_Cavity1;
	int nOffsetTimePos2 = sysSetting->m_nOffsetTime_Pos2_Cavity1;
	int nOffsetTimePos3 = sysSetting->m_nOffsetTime_Pos3_Cavity1;
	int nOffsetTimePos4 = sysSetting->m_nOffsetTime_Pos4_Cavity1;
	int nOffsetTimePos5 = sysSetting->m_nOffsetTime_Pos5_Cavity1;
	int nOffsetTimePos6 = sysSetting->m_nOffsetTime_Pos6_Cavity1;
	int nOffsetTimePos7 = sysSetting->m_nOffsetTime_Pos7_Cavity1;
	int nOffsetTimePos8 = sysSetting->m_nOffsetTime_Pos8_Cavity1;
	int nOffsetTimePos9 = sysSetting->m_nOffsetTime_Pos9_Cavity1;
	int nOffsetTimePos10 = sysSetting->m_nOffsetTime_Pos10_Cavity1;

	while (m_bRunningThread[nThreadIndex == TRUE])
	{
		// for avoid UI Freezing
		Sleep(1);

		if (m_bSimulator == TRUE) {
			// LOCK PROCESS
			while (m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS != TRUE)
			{
				Sleep(50);
			}
			m_pInterface->GetSealingInspectSimulationIO(m_nCoreIdx)->m_bLOCK_PROCESS = FALSE;
		}
		else if (m_bSimulator == FALSE)
		{
			while (m_pInterface->GetProcessStatus2() != TRUE)
			{
				Sleep(50);
			}
			m_pInterface->SetProcessStatus2(FALSE);
		}

		// 2. turn on ring light

		// 3. wait for the signal lighting cluster go to the position capture.

		// 4. grab frame
		CSealingInspectHikCam* hikCamControl = m_pInterface->GetHikCamControl();

		cv::Mat matBayerRG(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC1);
		cv::Mat matTopResult = cv::Mat::zeros(FRAME_HEIGHT_TOPCAM, FRAME_WIDTH_TOPCAM, CV_8UC3);

		Sleep(500);

		// 5. grab frame 1
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 6. process frame 1 (top cam 1)
		ProcessFrame1_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		// 7. turn on 4 bar light

		Sleep(100);

		// 8. grab frame
		if (hikCamControl->GetGrabBufferImage(nTopCam2_BufferHikCamIdx, matBayerRG.data) == FALSE)
			return;

		cv::cvtColor(matBayerRG, matTopResult, cv::COLOR_BayerBG2BGR);

		// 9. process frame 2 (top cam 1)
		ProcessFrame2_TopCam(recipe, nTopCam2_BufferHikCamIdx, nTopCam2_BufferProcessor, matTopResult);

		m_pInterface->InspectTopCam2Complete(FALSE);

		// 10. Read the PLC signal for grab frame, then store in frame wait process list.

#ifdef TEST_INSPECT_CAVITY_1

		Sleep(nGotoPos1Time);

		for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++)
		{
			switch (i)
			{
			case 0:
				Sleep(nGotoPos1Time - nOffsetTimePos1);
				break;
			case 1:
				Sleep(nGotoPos2Time - nGotoPos1Time - nOffsetTimePos2);
				break;
			case 2:
				Sleep(nGotoPos3Time - nGotoPos2Time - nOffsetTimePos3);
				break;
			case 3:
				Sleep(nGotoPos4Time - nGotoPos3Time - nOffsetTimePos4);
				break;
			case 4:
				Sleep(nGotoPos5Time - nGotoPos4Time - nOffsetTimePos5);
				break;
			case 5:
				Sleep(nGotoPos6Time - nGotoPos5Time - nOffsetTimePos6);
				break;
			case 6:
				Sleep(nGotoPos7Time - nGotoPos6Time - nOffsetTimePos7);
				break;
			case 7:
				Sleep(nGotoPos8Time - nGotoPos7Time - nOffsetTimePos8);
				break;
			case 8:
				Sleep(nGotoPos9Time - nGotoPos8Time - nOffsetTimePos9);
				break;
			case 9:
				Sleep(nGotoPos10Time - nGotoPos9Time - nOffsetTimePos10);
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

		m_pInterface->InspectCavity2Complete(FALSE);
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

	m_pInterface->InspectCavity2Complete(FALSE);
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

	m_pInterface->InspectTopCam1Complete(FALSE);

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

	m_pInterface->InspectCavity1Complete(FALSE);
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

	m_pInterface->InspectTopCam2Complete(FALSE);

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

	m_pInterface->InspectCavity2Complete(FALSE);
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
		m_pInterface->InspectCavity1Complete(TRUE);
	else if (nCoreIdx == 1)
		m_pInterface->InspectCavity2Complete(TRUE);
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
		m_pInterface->InspectCavity1Complete(TRUE);
	else if (nCoreIdx == 1)
		m_pInterface->InspectCavity2Complete(TRUE);
}

BOOL CSealingInspectCore::FindCircle_MinEnclosing(cv::Mat* matProcess, int nThresholdBinary, int nContourSizeMin, int nContourSizeMax, int nRadiusInnerMin, int nRadiusInnerMax, std::vector<std::vector<cv::Point>>& vecContours, std::vector<cv::Vec4i>& vecHierarchy, cv::Point2f& center, double& dRadius, int nIdx)
{
	if (matProcess->empty())
		return FALSE;

	cv::Mat matGray, matBinary;
	if (matProcess->channels() > 1)
		cv::cvtColor(*matProcess, matGray, cv::COLOR_BGR2GRAY);
	else
		matProcess->copyTo(matGray);

	cv::threshold(matGray, matBinary, nThresholdBinary, 255, cv::THRESH_BINARY_INV);
	/*char text[100] = {};
	sprintf_s(text, "%s_%d", "binary", nIdx);
	cv::imshow(text, matBinary);*/

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

	return TRUE;
}

BOOL CSealingInspectCore::FindCircle_HoughCircle(cv::Mat* matProcess, std::vector<cv::Vec3f>& vecCircles, std::vector<cv::Point2i>& vecPts, int nThresholdCanny, int minDist, int nParam1, int nParam2, int nRadiusOuterMin, int nRadiusOuterMax, double dIncreAngle)
{
	cv::Mat gray, blur, binary, canny, matResize;

	cv::cvtColor(*matProcess, gray, cv::COLOR_BGR2GRAY);

	cv::GaussianBlur(gray, blur, cv::Size(3, 3), 0.7, 0.7);
	cv::Canny(blur, canny, 50, nThresholdCanny);

	/*cv::resize(canny, matResize, cv::Size(1000, 750));
	cv::imshow("mat canny", matResize);*/

	// Apply the Hough Transform to find the circles
	cv::HoughCircles(gray, vecCircles, cv::HOUGH_GRADIENT, 1, minDist, nParam1, nParam2, nRadiusOuterMin, nRadiusOuterMax);

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

BOOL CSealingInspectCore::FindCircle_HoughCircle_2(cv::Mat* matProcess, std::vector<cv::Vec3f>& vecCircles, int nThresholdCanny1, int nThresholdCanny2, int minDist, int nParam1, int nParam2, int nRadiusOuterMin, int nRadiusOuterMax)
{
	cv::Mat gray, blur, binary, canny, matResize;

	if (matProcess->channels() > 1)
		cv::cvtColor(*matProcess, gray, cv::COLOR_BGR2GRAY);
	else
		matProcess->copyTo(gray);

	/*cv::GaussianBlur(gray, blur, cv::Size(3, 3), 0.7, 0.7);
	cv::Canny(blur, canny, nThresholdCanny1, nThresholdCanny2);*/

	/*cv::resize(canny, matResize, cv::Size(1000, 750));
	cv::imshow("mat canny", matResize);*/

	// Apply the Hough Transform to find the circles
	cv::HoughCircles(gray, vecCircles, cv::HOUGH_GRADIENT, 1, minDist, nParam1, nParam2, nRadiusOuterMin, nRadiusOuterMax);

	if (vecCircles.size() == 0)
		return FALSE;

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
		memcpy(&pImageDataROI.data[i * pImageDataROI.step1()], &matGray.data[(i + rectROI.y) * matGray.step1() + rectROI.x], pImageDataROI.cols);

	//cv::imshow("ROI", pImageDataROI);

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
	MakeCannyEdgeImage(&pImageDataROI, matCanny, dTheshold1, dTheshold2);

	/*char chCanny[10] = {};
	sprintf_s(chCanny, "%s_%d", "Canny", nROIIdx);
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
	//cv::imshow("ROI Pt", matROIFindMearuePt);

	double dTheshold1 = 0.0;
	double dTheshold2 = 0.0;
	int nWidthROI = rectROI.width;
	int nHeightROI = rectROI.height;
	BOOL nUseAdvancedAlgorithms = FALSE;

	switch (nFrame)
	{
	case 0:
	case 1:
		dTheshold1 = pRecipeSideCam.m_recipeFrame1.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame1.m_dThresholdCanny2_MakeROI;
		nUseAdvancedAlgorithms = pRecipeSideCam.m_recipeFrame1.m_bUseAdvancedAlgorithms;
		break;
	case 2:
	case 3:
	case 4:
		dTheshold1 = pRecipeSideCam.m_recipeFrame2.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame2.m_dThresholdCanny2_MakeROI;
		nUseAdvancedAlgorithms = pRecipeSideCam.m_recipeFrame2.m_bUseAdvancedAlgorithms;
		break;
	case 5:
	case 6:
		dTheshold1 = pRecipeSideCam.m_recipeFrame3.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame3.m_dThresholdCanny2_MakeROI;
		nUseAdvancedAlgorithms = pRecipeSideCam.m_recipeFrame3.m_bUseAdvancedAlgorithms;
		break;
	case 7:
	case 8:
	case 9:
		dTheshold1 = pRecipeSideCam.m_recipeFrame4.m_dThresholdCanny1_MakeROI;
		dTheshold2 = pRecipeSideCam.m_recipeFrame4.m_dThresholdCanny2_MakeROI;
		nUseAdvancedAlgorithms = pRecipeSideCam.m_recipeFrame4.m_bUseAdvancedAlgorithms;
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
		int nLargest_Idx = 0;
		int nLargest_Contour = 0;

		findContours(roiCanny, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

		if (nUseAdvancedAlgorithms == TRUE)
		{
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
				for (auto k : vecdistMinIndices)
				{
					cv::Point pt(contours[i].at(k).x + nROI_X, contours[i].at(k).y + rectROI.y);
					vecMeaPts.push_back(pt);
				}
				for (auto k : vecdistMaxIndices)
				{
					cv::Point pt(contours[i].at(k).x + nROI_X, contours[i].at(k).y + rectROI.y);
					vecMeaPts.push_back(pt);
				}
			}
		}
		else
		{
			if (contours.empty())
				break;

			for (int i = 0; i < contours.size(); i++) {
				if (contours[i].size() < 30)
					continue;

				if (contours[i].size() > nLargest_Contour) {
					nLargest_Contour = contours[i].size();
					nLargest_Idx = i;
				}
			}

			if (contours[nLargest_Idx].size() < 0)
				break;

			int nAverX = 0;
			int nAverY = 0;
			int k = contours[nLargest_Idx].size();
			for (int i = 0; i < contours[nLargest_Idx].size(); i++)
			{
				nAverX += contours[nLargest_Idx][i].x;
				nAverY += contours[nLargest_Idx][i].y;
			}
			nAverX = nAverX / k;
			nAverY = nAverY / k;

			cv::Point pt(nAverX + nROI_X, nAverY + rectROI.y);
			vecMeaPts.push_back(pt);
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
	case 0:
	case 1:
		nROI_X = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame1.m_nROI_Top[3];
		break;

	case 2:
	case 3:
	case 4:
		nROI_X = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame2.m_nROI_Top[3];
		break;


	case 5:
	case 6:
		nROI_X = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame3.m_nROI_Top[3];
		break;

	case 7:
	case 8:
	case 9:
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
	case 0:
	case 1:
		nROI_X = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame1.m_nROI_Bottom[3];
		break;

	case 2:
	case 3:
	case 4:
		nROI_X = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame2.m_nROI_Bottom[3];
		break;

	case 5:
	case 6:
		nROI_X = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[0];
		nROI_Y = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[1];
		nROIWidth = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[2];
		nROIHeight = pRecipeSideCam.m_recipeFrame3.m_nROI_Bottom[3];
		break;

	case 7:
	case 8:
	case 9:
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

BOOL CSealingInspectCore::MakeROI_AdvancedAlgorithms(CSealingInspectRecipe_SideCam recipeSideCam, cv::Mat* pMatProcess, int nFrame, cv::Rect& rectROIFindLIne, cv::Rect& rectROIFindPts)
{
	if (pMatProcess->empty())
		return FALSE;

	int nThresholdBinary = 0;
	int nOffset_Y_Top = 0;
	int nOffset_Y_Bottom = 0;
	int nROIWidth_Top = 0;
	int nROIHeight_Top = 0;
	int nROIWidth_Bottom = 0;
	int nROIHeight_Bottom = 0;

	switch (nFrame)
	{
	case 0:
	case 1:
		nThresholdBinary = recipeSideCam.m_recipeFrame1.m_nThresholdBinaryFindROI;
		nOffset_Y_Top = recipeSideCam.m_recipeFrame1.m_nOffetY_Top;
		nOffset_Y_Bottom = recipeSideCam.m_recipeFrame1.m_nOffetY_Bottom;
		nROIWidth_Top = recipeSideCam.m_recipeFrame1.m_nROI_Top[2];
		nROIHeight_Top = recipeSideCam.m_recipeFrame1.m_nROI_Top[3];
		nROIWidth_Bottom = recipeSideCam.m_recipeFrame1.m_nROI_Bottom[2];
		nROIHeight_Bottom = recipeSideCam.m_recipeFrame1.m_nROI_Bottom[3];
		break;

	case 2:
	case 3:
	case 4:
		nThresholdBinary = recipeSideCam.m_recipeFrame2.m_nThresholdBinaryFindROI;
		nOffset_Y_Top = recipeSideCam.m_recipeFrame2.m_nOffetY_Top;
		nOffset_Y_Bottom = recipeSideCam.m_recipeFrame2.m_nOffetY_Bottom;
		nROIWidth_Top = recipeSideCam.m_recipeFrame2.m_nROI_Top[2];
		nROIHeight_Top = recipeSideCam.m_recipeFrame2.m_nROI_Top[3];
		nROIWidth_Bottom = recipeSideCam.m_recipeFrame2.m_nROI_Bottom[2];
		nROIHeight_Bottom = recipeSideCam.m_recipeFrame2.m_nROI_Bottom[3];
		break;

	case 5:
	case 6:
		nThresholdBinary = recipeSideCam.m_recipeFrame3.m_nThresholdBinaryFindROI;
		nOffset_Y_Top = recipeSideCam.m_recipeFrame3.m_nOffetY_Top;
		nOffset_Y_Bottom = recipeSideCam.m_recipeFrame3.m_nOffetY_Bottom;
		nROIWidth_Top = recipeSideCam.m_recipeFrame3.m_nROI_Top[2];
		nROIHeight_Top = recipeSideCam.m_recipeFrame3.m_nROI_Top[3];
		nROIWidth_Bottom = recipeSideCam.m_recipeFrame3.m_nROI_Bottom[2];
		nROIHeight_Bottom = recipeSideCam.m_recipeFrame3.m_nROI_Bottom[3];
		break;

	case 7:
	case 8:
	case 9:
		nThresholdBinary = recipeSideCam.m_recipeFrame4.m_nThresholdBinaryFindROI;
		nOffset_Y_Top = recipeSideCam.m_recipeFrame4.m_nOffetY_Top;
		nOffset_Y_Bottom = recipeSideCam.m_recipeFrame4.m_nOffetY_Bottom;
		nROIWidth_Top = recipeSideCam.m_recipeFrame4.m_nROI_Top[2];
		nROIHeight_Top = recipeSideCam.m_recipeFrame4.m_nROI_Top[3];
		nROIWidth_Bottom = recipeSideCam.m_recipeFrame4.m_nROI_Bottom[2];
		nROIHeight_Bottom = recipeSideCam.m_recipeFrame4.m_nROI_Bottom[3];
		break;
	}

	cv::Mat matGray, matBinary, matResize, matEle;
	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	cv::threshold(matGray, matBinary, nThresholdBinary, 255, cv::THRESH_BINARY);

	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5));
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_OPEN, matEle);
	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(7, 7));
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_CLOSE, matEle);

	int nLargest_contour_Idx = 0;
	std::vector<std::vector<cv::Point>> contours;
	cv::findContours(matBinary, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
	//std::vector<std::vector<cv::Point>> contours_poly(contours.size());

	for (int i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() < 4000)
			continue;

		nLargest_contour_Idx = i;
		break;
	}

	cv::Rect boundingRect = cv::boundingRect(contours[nLargest_contour_Idx]);
	cv::Rect rectFindLine = cv::Rect(boundingRect.x, boundingRect.y - nOffset_Y_Top, nROIWidth_Top, nROIHeight_Top);
	cv::Rect rectFindPt = cv::Rect(boundingRect.x, boundingRect.y + nOffset_Y_Bottom, nROIWidth_Bottom, nROIHeight_Bottom);

	rectROIFindLIne = rectFindLine;
	rectROIFindPts = rectFindPt;

	/*cv::rectangle(*pMatProcess, boundingRect, RED_COLOR, 2, cv::LINE_AA);
	cv::rectangle(*pMatProcess, rectFindLine, BLUE_COLOR, 2, cv::LINE_AA);
	cv::rectangle(*pMatProcess, rectFindPt, BLUE_COLOR, 2, cv::LINE_AA);
	cv::resize(*pMatProcess, matResize, cv::Size(1000, 750));
	cv::imshow("Bin", matResize);*/

	return TRUE;
}

BOOL CSealingInspectCore::MakeCannyEdgeImage(cv::Mat* pImageData, cv::Mat& pEdgeImageData, double dThreshold1, double dThreshold2, int nGaussianMask /* = 3 */)
{
	if (pImageData->empty())
		return FALSE;

	cv::Mat matBlur;

	if (nGaussianMask < 3)
		matBlur = *(pImageData);
	else
		cv::GaussianBlur(*(pImageData), matBlur, cv::Size(nGaussianMask, nGaussianMask), 1.0);

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
	//cv::imshow("ROI", matROIFindLine);
	int nFindStartEndLineX = 0;
	int nFindStartEndLineSearchRangeX = 0;
	int nThreshold = 0;

	switch (nFrame)
	{
	case 0:
	case 1:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame1.m_nFindStartEndXThresholdGray;
		break;

	case 2:
	case 3:
	case 4:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame2.m_nFindStartEndXThresholdGray;
		break;

	case 5:
	case 6:
		nFindStartEndLineX = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndX;
		nFindStartEndLineSearchRangeX = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndSearchRangeX;
		nThreshold = pRecipeSideCam.m_recipeFrame3.m_nFindStartEndXThresholdGray;
		break;

	case 7:
	case 8:
	case 9:
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

BOOL CSealingInspectCore::FindBlob_SealingSurface(CRecipe_TopCam_Frame2 pRecipeTopCamFrame2, cv::Mat* pMatProcess, cv::Point2f ptCenter, double dRadius, std::vector<std::vector<cv::Point>>& mContours)
{
	if (pMatProcess->empty())
		return FALSE;

	//cv::Mat matHSV, matResultHSV, maskHSV;

	cv::Mat matBinary, matGray, matResize;
	cv::Mat matSubtract, matSubtract1, matSubtract2, mask1, mask2, matEle;
	cv::Mat matResizeMaskCircle, matCopyMaskCircle;

	cv::cvtColor(*pMatProcess, matGray, cv::COLOR_BGR2GRAY);

	int nThresholdBinary = pRecipeTopCamFrame2.m_nThresholdBinary;

	int nContourSize_Min_FindBlob = pRecipeTopCamFrame2.m_nContourSizeFindBlob_Min;
	int nContourSize_Max_FindBlob = pRecipeTopCamFrame2.m_nContourSizeFindBlob_Max;

	cv::Mat maskCircle(pMatProcess->size(), CV_8UC1, cv::Scalar(0));
	cv::circle(maskCircle, ptCenter, dRadius, cv::Scalar(255), cv::FILLED);

	matGray.copyTo(matCopyMaskCircle, maskCircle);

	/*cv::resize(matCopyMaskCircle, matResizeMaskCircle, cv::Size(1000, 750));
	cv::imshow("mask circle", matResizeMaskCircle);*/

	/*cv::Scalar minHSV, maxHSV;
	cv::cvtColor(*pMatProcess, matHSV, cv::COLOR_BGR2HSV);
	matResultHSV = cv::Mat::zeros(pMatProcess->rows, pMatProcess->cols, CV_8UC3);

	int nHMin = 55;
	int nHMax = 180;
	int nSMin = 0;
	int nSMax = 255;
	int nVMin = 70;
	int nVMax = 255;

	minHSV = cv::Scalar(nHMin, nSMin, nVMin);
	maxHSV = cv::Scalar(nHMax, nSMax, nVMax);

	cv::inRange(matHSV, minHSV, maxHSV, maskHSV);
	cv::bitwise_and(*pMatProcess, *pMatProcess, matResultHSV, maskHSV);*/


	cv::threshold(matCopyMaskCircle, matBinary, nThresholdBinary, 255, cv::THRESH_BINARY);

	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(7, 7));
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_CLOSE, matEle);

	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(12, 12));
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_OPEN, matEle);

	/*cv::resize(matBinary, matResize, cv::Size(1000, 750));
	cv::imshow("binary", matResize);*/


	mask1 = cv::Mat(matBinary.size(), CV_8UC1, cv::Scalar(0));
	mask2 = cv::Mat(matBinary.size(), CV_8UC1, cv::Scalar(0));

	int largest_contour_idx = 0;
	int largest_contour = 0;
	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;
	std::vector<int> vecIdx;

	// list all polygons found.
	//std::vector<std::vector<cv::Point> > contours_poly(contours.size());

	cv::findContours(matBinary, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
	for (size_t i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() < nContourSize_Min_FindBlob || contours[i].size() > nContourSize_Max_FindBlob)
			continue;

		vecIdx.push_back(i);
		if (vecIdx.size() == 2)
			break;

	}

	if (contours[vecIdx[0]].size() > contours[vecIdx[1]].size())
	{
		cv::fillPoly(mask1, contours[vecIdx[0]], WHITE_COLOR);
		matGray.copyTo(matSubtract1, mask1);

		cv::fillPoly(mask2, contours[vecIdx[1]], WHITE_COLOR);
		matGray.copyTo(matSubtract2, mask2);

		cv::subtract(matSubtract1, matSubtract2, matSubtract);
	}
	else
	{
		cv::fillPoly(mask1, contours[vecIdx[1]], WHITE_COLOR);
		matGray.copyTo(matSubtract1, mask1);

		cv::fillPoly(mask2, contours[vecIdx[0]], WHITE_COLOR);
		matGray.copyTo(matSubtract2, mask2);

		cv::subtract(matSubtract1, matSubtract2, matSubtract);
	}

	BOOL bRet = TRUE;

	bRet &= FindBlob(pRecipeTopCamFrame2, &matSubtract, mContours);

	/*cv::drawContours(matMask, contours, largest_contour_idx, cv::Scalar(255), 2, 8, hierarchy);
	cv::imshow("mask", matMask);*/

	/*cv::Mat rgb[3];
	cv::split(*pMatProcess, rgb);

	cv::Mat rgba[4] = { rgb[0], rgb[1], rgb[2], matMask };
	cv::merge(rgba, 4, matSubtract);

	cv::imshow("subtract", matSubtract);*/

	/*cv::Mat matResize1, matResize2;
	cv::resize(matSubtract1, matResize1, cv::Size(1000, 750));
	cv::imshow("sub1", matResize1);

	cv::resize(matSubtract2, matResize2, cv::Size(1000, 750));
	cv::imshow("sub2", matResize2);

	cv::Mat matResize3;
	cv::resize(matSub, matResize3, cv::Size(1000, 750));
	cv::imshow("sub", matResize3);*/

	return bRet;
}

BOOL CSealingInspectCore::FindBlob(CRecipe_TopCam_Frame2 pRecipeTopCamFrame2, cv::Mat* pMatProcess, std::vector<std::vector<cv::Point>>& mContours)
{
	if (pMatProcess->empty())
		return FALSE;

	int nThresholdBinary_FindBlobWhite = pRecipeTopCamFrame2.m_nThreshBinary_FindBlobWhite;
	int nThresholdBinary_Max_FindBlobWhite = pRecipeTopCamFrame2.m_nThreshBinary_FindBlobWhite_Max;
	int nThresholdBinary_FindBlobBlack = pRecipeTopCamFrame2.m_nThreshBinary_FindBlobBlack;
	int nThresholdBinary_Max_FindBlobBlack = pRecipeTopCamFrame2.m_nThreshBinary_FindBlobBlack_Max;
	int nMaxBlobCount = pRecipeTopCamFrame2.m_nBlobCount_Max;
	int nBlobArea_Min = pRecipeTopCamFrame2.m_dBlobArea_Min;
	int nBlobArea_Max = pRecipeTopCamFrame2.m_dBlobArea_Max;

	// start inspection

	cv::Mat matResize;
	cv::Mat matFindBlobWhite;
	cv::Mat matFindBlobBlack;
	cv::threshold(*pMatProcess, matFindBlobWhite, nThresholdBinary_FindBlobWhite, nThresholdBinary_Max_FindBlobWhite, cv::THRESH_BINARY);
	cv::threshold(*pMatProcess, matFindBlobBlack, nThresholdBinary_FindBlobBlack, nThresholdBinary_Max_FindBlobBlack, cv::THRESH_BINARY_INV);
	cv::resize(matFindBlobWhite, matResize, cv::Size(1000, 750));
	//cv::imshow("findBlobWhite", matResize);
	//cv::imshow("findBlobBlack", matResize);

	std::vector<std::vector<cv::Point>> contours_white;
	std::vector<std::vector<cv::Point>> contours_black;
	std::vector<cv::Vec4i> hierarchy;

	int nCountBlobWhite = 0;
	int nCountBlobBlack = 0;

	cv::findContours(matFindBlobWhite, contours_white, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
	for (size_t i = 0; i < contours_white.size(); i++)
	{
		double areaBlob = cv::contourArea(contours_white[i]);

		if (nBlobArea_Min < areaBlob && areaBlob < nBlobArea_Max)
		{
			nCountBlobWhite++;
			mContours.push_back(contours_white[i]);
		}
	}
	if (nCountBlobWhite > nMaxBlobCount)
		return FALSE;

	/*cv::findContours(matFindBlobBlack, contours_black, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
	for (size_t i = 0; i < contours_black.size(); i++)
	{
		double areaBlob = cv::contourArea(contours_black[i]);

		if (areaBlob < nBlobArea_Min || areaBlob > nBlobArea_Max)
		{
			nCountBlobWhite++;
			mContours.push_back(contours_black[i]);
		}
		if (nCountBlobWhite > nMaxBlobCount)
			return FALSE;
	}*/

	return TRUE;
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
	/*char ch[256];
	sprintf_s(ch, sizeof(ch), "R = %.2f pxl", radius);
	std::string text(ch);
	cv::putText(matSrc, text, cv::Point(centers.x - 100, centers.y - radius - 10), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);*/

	/*char ch2[256];
	sprintf_s(ch2, sizeof(ch2), "(x: %.1f, y: %.1f)", centers.x, centers.y);
	cv::putText(matSrc, ch2, cv::Point(centers.x - 300, centers.y), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);*/
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
	try
	{
		for (int i = 0; i < vecPosNG.size(); i++) {
			int x = vecPts[vecPosNG[i]].x - 5;
			int y = vecPts[vecPosNG[i]].y - 5;
			int width = 10;
			int height = 10;
			cv::Rect rec(x, y, width, height);
			cv::rectangle(mat, rec, RED_COLOR, 1, cv::LINE_AA);
		}
	}
	catch (...) {

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

void CSealingInspectCore::DrawRotateRect(cv::Mat* pMat, std::vector<cv::Point> vertices, BOOL bStatus)
{
	if (pMat->empty())
		return;

	if (vertices.empty())
		return;

	if (bStatus == TRUE)
	{
		for (int i = 0; i < 4; i++) {
			cv::line(*pMat, vertices[i], vertices[(i + 1) % 4], GREEN_COLOR, 1, cv::LINE_8);
		}
	}
	else
	{
		for (int i = 0; i < 4; i++) {
			cv::line(*pMat, vertices[i], vertices[(i + 1) % 4], RED_COLOR, 1, cv::LINE_8);
		}
	}

}

BOOL CSealingInspectCore::FindSealingOverflow(cv::Mat* pMatProcess, cv::Rect rectROI, cv::Rect rectFindSealingOverflow, std::vector<cv::Point>& vertices_FindSealingOverflow, double angle, int nThreshold, int nContourSizeMax, double dAreaContourMax)
{
	if (pMatProcess->empty())
		return FALSE;

	cv::RotatedRect rotateRect(cv::Point2f(rectFindSealingOverflow.x + rectFindSealingOverflow.width / 2, rectFindSealingOverflow.y + rectFindSealingOverflow.height / 2), cv::Size(rectFindSealingOverflow.width, rectFindSealingOverflow.height), angle);
	cv::Point2f verticies2f[4];
	rotateRect.points(verticies2f);
	for (int i = 0; i < 4; i++) {
		vertices_FindSealingOverflow.push_back(cv::Point(verticies2f[i].x + rectROI.x, verticies2f[i].y + rectROI.y));
	}

	cv::Size rotatedRectSize = rotateRect.size;
	if (angle < -45.) {
		angle += 90.0;
		cv::swap(rotatedRectSize.width, rotatedRectSize.height);
	}
	cv::Mat M, rotated, cropped, matBinary;
	// get the rotation matrix
	M = cv::getRotationMatrix2D(rotateRect.center, angle, 1.0);
	// perform the affine transformation
	cv::warpAffine(*pMatProcess, rotated, M, pMatProcess->size(), cv::INTER_CUBIC);
	// crop the result
	cv::getRectSubPix(rotated, rotatedRectSize, rotateRect.center, cropped);

	/*char text[100] = {};
	sprintf_s(text, "%s_%.f", "Cropped", angle);
	cv::imshow(text, cropped);*/

	cv::threshold(cropped, matBinary, nThreshold, 255, cv::THRESH_BINARY);
	//cv::imshow(text, matBinary);
	std::vector<std::vector<cv::Point>> contours;
	cv::findContours(matBinary, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
	for (int i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() < nContourSizeMax)
			continue;

		if (cv::contourArea(contours[i]) > dAreaContourMax)
			return FALSE;
	}

	return TRUE;
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
	int nParam1 = recipeTopCamFrame1.m_nHoughCircleParam1;
	int nParam2 = recipeTopCamFrame1.m_nHoughCircleParam2;
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
	double dDistanceMeasureToleranceRefer = recipeTopCamFrame1.m_dDistanceMeasurementTolerance_Refer;
	double dDistanceMeasureToleranceMin = recipeTopCamFrame1.m_dDistanceMeasurementTolerance_Min;
	double dDistanceMeasureToleranceMax = recipeTopCamFrame1.m_dDistanceMeasurementTolerance_Max;
	double dPxlSize = recipeTopCamFrame1.m_dRatioPxlUm;
	int nNumberOfDistNGMax = recipeTopCamFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	BOOL bUseAdvancedAlgorithms = recipeTopCamFrame1.m_bUseAdvancedAlgorithms;

	// MinEnclosingCircle
	std::vector<std::vector<cv::Point> > vecContours;
	std::vector<cv::Vec4i> vecHierarchy;

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
	bFindCircle_MinEnclosing = FindCircle_MinEnclosing(&matProcess, nThresholdBinary, nContourSizeMin, nContourSizeMax, nRadiusInnerMin, nRadiusInnerMax, vecContours, vecHierarchy, center_Inner, dRadius_Inner, 0);

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
	bFindCircle_HoughCircle = FindCircle_HoughCircle(&matProcess, vecCircles, vecPoints, nThresholdCannyHoughCircle, nDistRadiusDiffMin, nParam1, nParam2, nRadiusOuterMin, nRadiusOuterMax, dIncrementAngle);

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

	double dPxlMin = ConvertUmToPixel(dDistanceMeasureToleranceRefer, dPxlSize) - ConvertUmToPixel(dDistanceMeasureToleranceMin, dPxlSize);
	double dPxlMax = ConvertUmToPixel(dDistanceMeasureToleranceRefer, dPxlSize) + ConvertUmToPixel(dDistanceMeasureToleranceMax, dPxlSize);

	//nRet &= JudgementInspectDistanceMeasurement(vecDist, vecPosNG, nPxlMin, nPxlMax);

	for (int i = 0; i < vecDist.size(); i++) {
		if (vecDist[i] < dPxlMin || vecDist[i] > dPxlMax) {
			nRet &= FALSE;
			vecPosNG.push_back(i);
		}
	}

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

				cv::rectangle(mat, vecRectROI[i], BLUE_COLOR, 1, cv::LINE_AA);
			}
		}
		nRet &= JudgementInspectDistanceMeasurement_AdvancedAlgorithms(vecDistPointToCircle, vecPosNG, dPxlMin, dPxlMax, nNumberOfDistNGMax);

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

	cv::Mat matProcess, matGray;
	mat.copyTo(matProcess);

	cv::cvtColor(matProcess, matGray, cv::COLOR_BGR2GRAY);

	int nFrame = 1;

	CRecipe_TopCam_Frame2 recipeTopCamFrame2 = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame2;

	int nSelectMethod = recipeTopCamFrame2.m_nSelectMethodFindCircle;
	int nThresholdBinary_FindCircle = recipeTopCamFrame2.m_nThresholdBinary_FindCircle;
	int nContourSizeMin = recipeTopCamFrame2.m_nContourSizeMin;
	int nContourSizeMax = recipeTopCamFrame2.m_nContourSizeMax;
	int nRadiusMin = recipeTopCamFrame2.m_nRadiusMin;
	int nRadiusMax = recipeTopCamFrame2.m_nRadiusMax;
	int nThresholdCanny1 = recipeTopCamFrame2.m_nThreshold1_Canny;
	int nThresholdCanny2 = recipeTopCamFrame2.m_nThreshold2_Canny;
	int nParam1 = recipeTopCamFrame2.m_nParam1_HoughCircle;
	int nParam2 = recipeTopCamFrame2.m_nParam2_HoughCircle;
	int nMinDist = recipeTopCamFrame2.m_nMinDist_HoughCircle;

	int nOffsetROIFindMeaPt1H_X = recipeTopCamFrame2.Offset_ROIFindMeasurePoint1H_X;
	int nOffsetROIFindMeaPt1H_Y = recipeTopCamFrame2.Offset_ROIFindMeasurePoint1H_Y;
	int nOffsetROIFindMeaPt5H_X = recipeTopCamFrame2.Offset_ROIFindMeasurePoint5H_X;
	int nOffsetROIFindMeaPt5H_Y = recipeTopCamFrame2.Offset_ROIFindMeasurePoint5H_Y;
	int nOffsetROIFindMeaPt7H_X = recipeTopCamFrame2.Offset_ROIFindMeasurePoint7H_X;
	int nOffsetROIFindMeaPt7H_Y = recipeTopCamFrame2.Offset_ROIFindMeasurePoint7H_Y;
	int nOffsetROIFindMeaPt11H_X = recipeTopCamFrame2.Offset_ROIFindMeasurePoint11H_X;
	int nOffsetROIFindMeaPt11H_Y = recipeTopCamFrame2.Offset_ROIFindMeasurePoint11H_Y;

	int nWidthROI_FindSealingOverflow = recipeTopCamFrame2.m_nWidth_ROIFindSealingOverflow;
	int nHeightROI_FindSealingOverflow = recipeTopCamFrame2.m_nHeight_ROIFindSealingOverflow;

	int nOffset_FindSealingOverflow_X_1H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Hoz;
	int nOffset_FindSealingOverflow_Y_1H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Hoz;
	int nOffset_FindSealingOverflow_X_1H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_1H_Ver;
	int nOffset_FindSealingOverflow_Y_1H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_1H_Ver;

	int nOffset_FindSealingOverflow_X_5H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Hoz;
	int nOffset_FindSealingOverflow_Y_5H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Hoz;
	int nOffset_FindSealingOverflow_X_5H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_5H_Ver;
	int nOffset_FindSealingOverflow_Y_5H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_5H_Ver;

	int nOffset_FindSealingOverflow_X_7H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Hoz;
	int nOffset_FindSealingOverflow_Y_7H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Hoz;
	int nOffset_FindSealingOverflow_X_7H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_7H_Ver;
	int nOffset_FindSealingOverflow_Y_7H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_7H_Ver;

	int nOffset_FindSealingOverflow_X_11H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Hoz;
	int nOffset_FindSealingOverflow_Y_11H_Hoz = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Hoz;
	int nOffset_FindSealingOverflow_X_11H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_X_11H_Ver;
	int nOffset_FindSealingOverflow_Y_11H_Ver = recipeTopCamFrame2.m_nOffset_ROIFindSealingOverflow_Y_11H_Ver;

	int nThreshold_FindSealingOverflow = recipeTopCamFrame2.m_nThresholdBinary_FindSealingOverflow;
	int nContourSizeMax_FindSealingOverflow = recipeTopCamFrame2.m_nContourSize_FindSealingOverflow_Max;
	double dAreaContourMax_FindSealingOverflow = recipeTopCamFrame2.m_dAreaContour_FindSealingOverflow_Max;

	double dDistanceRefer = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Refer;
	double dDistanceMin = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Min;
	double dDistanceMax = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Max;

	std::vector<cv::Rect> vecRectROI;
	std::vector<cv::Mat> vecMatROI;
	std::vector<std::vector<cv::Point>> vecContours_FindCircle;
	std::vector<cv::Vec4i> vecHierarchy_FindCircle;
	std::vector<cv::Point2f> vecMeasurePt;
	std::vector<double> vecDistance;
	std::vector<cv::Point2i> vecPtsIntersection;
	std::vector<cv::Vec3f> vecCircles;

	std::vector<cv::Point2f> vecPtCenter;
	std::vector<double> vecRadius;
	cv::Point2f ptCenter;
	double dRadius;

	std::vector<std::vector<cv::Point>> vecContours_FindBlob;

	int nCounterNG = 0;
	BOOL bFindCircle = FALSE;
	BOOL bMeasureWidth = TRUE;
	BOOL bFindSealingOverflow_Hoz = TRUE;
	BOOL bFindSealingOverflow_Ver = TRUE;
	BOOL nRet = TRUE;

	if (recipeTopCamFrame2.m_nUseCheckSurface == 1)
	{
		// FindBlob  on surface
		nRet &= FindBlob_SealingSurface(recipeTopCamFrame2, &matProcess, m_ptCenter_Inner, m_dRadius_Inner, vecContours_FindBlob);

		if (!vecContours_FindBlob.empty()) {
			for (int i = 0; i < vecContours_FindBlob.size(); i++)
			{
				//cv::drawContours(mat, mContours, i, YELLOW_COLOR, 1, cv::LINE_8);
				cv::Rect rect = cv::boundingRect(vecContours_FindBlob[i]);
				cv::Rect rectDraw(rect.x, rect.y, 1.5 * rect.width, 1.5 * rect.height);
				cv::rectangle(mat, rectDraw, RED_COLOR, 1, cv::LINE_AA);
			}
		}
	}

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

	for (int nROIIdx = 0; nROIIdx < vecRectROI.size(); nROIIdx++)
	{
		int nOffsetX = 0;
		int nOffsetY = 0;
		int nOffset_FindSealingOverflow_X_Hoz = 0;
		int nOffset_FindSealingOverflow_Y_Hoz = 0;
		int nOffset_FindSealingOverflow_X_Ver = 0;
		int nOffset_FindSealingOverflow_Y_Ver = 0;

		cv::Rect rectFindPt;
		cv::Rect rectMeasureWidth;

		cv::Rect rectFindSealingOverflow_Hoz;
		cv::Rect rectFindSealingOverflow_Ver;
		double dAngle_Hoz = 0.0;
		double dAngle_Ver = 0.0;

		cv::Mat matROI_BGR = matProcess(vecRectROI[nROIIdx]);
		if (matROI_BGR.empty())
			return;
		//cv::imshow("ROI_BGR", matROI_BGR);

		cv::Mat pImageDataROI(vecRectROI[nROIIdx].height, vecRectROI[nROIIdx].width, CV_8UC1);
		for (int i = 0; i < pImageDataROI.rows; i++)
			memcpy(&pImageDataROI.data[i * pImageDataROI.step1()], &matGray.data[(i + vecRectROI[nROIIdx].y) * matGray.step1() + vecRectROI[nROIIdx].x], pImageDataROI.cols);

		if (nSelectMethod == 1)
		{
			bFindCircle = FindCircle_MinEnclosing(&pImageDataROI, nThresholdBinary_FindCircle, nContourSizeMin, nContourSizeMax, nRadiusMin, nRadiusMax, vecContours_FindBlob, vecHierarchy_FindCircle, ptCenter, dRadius, nROIIdx);
		}
		else if (nSelectMethod == 2)
		{
			bFindCircle = FindCircle_HoughCircle_2(&pImageDataROI, vecCircles, nThresholdCanny1, nThresholdCanny2, nMinDist, nParam1, nParam2, nRadiusMin, nRadiusMax);
		}

		if (bFindCircle == FALSE)
		{
			nRet &= bFindCircle;
			return;
		}

		if (bFindCircle == TRUE && nSelectMethod == 2 && !vecCircles.empty())
		{
			ptCenter = cv::Point2f(vecCircles[0][0], vecCircles[0][1]);
			dRadius = vecCircles[0][2];
		}

		vecPtCenter.push_back(ptCenter);
		vecRadius.push_back(dRadius);

#ifdef ALGORITHM_2_TOPCAM_FRAME2

		switch (nROIIdx)
		{
		case 0:
			nOffsetX = nOffsetROIFindMeaPt1H_X;
			nOffsetY = nOffsetROIFindMeaPt1H_Y;
			nOffset_FindSealingOverflow_X_Hoz = nOffset_FindSealingOverflow_X_1H_Hoz;
			nOffset_FindSealingOverflow_Y_Hoz = nOffset_FindSealingOverflow_Y_1H_Hoz;
			nOffset_FindSealingOverflow_X_Ver = nOffset_FindSealingOverflow_X_1H_Ver;
			nOffset_FindSealingOverflow_Y_Ver = nOffset_FindSealingOverflow_Y_1H_Ver;
			dAngle_Hoz = 5;
			dAngle_Ver = 70;
			rectMeasureWidth = cv::Rect(ptCenter.x - nOffsetX, ptCenter.y + nOffsetY, 40, 40);
			rectFindSealingOverflow_Hoz = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Hoz, ptCenter.y + nOffset_FindSealingOverflow_Y_Hoz, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			rectFindSealingOverflow_Ver = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Ver, ptCenter.y + nOffset_FindSealingOverflow_Y_Ver, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			break;
		case 1:
			nOffsetX = nOffsetROIFindMeaPt5H_X;
			nOffsetY = nOffsetROIFindMeaPt5H_Y;
			nOffset_FindSealingOverflow_X_Hoz = nOffset_FindSealingOverflow_X_5H_Hoz;
			nOffset_FindSealingOverflow_Y_Hoz = nOffset_FindSealingOverflow_Y_5H_Hoz;
			nOffset_FindSealingOverflow_X_Ver = nOffset_FindSealingOverflow_X_5H_Ver;
			nOffset_FindSealingOverflow_Y_Ver = nOffset_FindSealingOverflow_Y_5H_Ver;
			dAngle_Hoz = -5;
			dAngle_Ver = -70;
			rectMeasureWidth = cv::Rect(ptCenter.x - nOffsetX, ptCenter.y - nOffsetY, 40, 40);
			rectFindSealingOverflow_Hoz = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Hoz, ptCenter.y - nOffset_FindSealingOverflow_Y_Hoz, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			rectFindSealingOverflow_Ver = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Ver, ptCenter.y - nOffset_FindSealingOverflow_Y_Ver, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			break;
		case 2:
			nOffsetX = nOffsetROIFindMeaPt7H_X;
			nOffsetY = nOffsetROIFindMeaPt7H_Y;
			nOffset_FindSealingOverflow_X_Hoz = nOffset_FindSealingOverflow_X_7H_Hoz;
			nOffset_FindSealingOverflow_Y_Hoz = nOffset_FindSealingOverflow_Y_7H_Hoz;
			nOffset_FindSealingOverflow_X_Ver = nOffset_FindSealingOverflow_X_7H_Ver;
			nOffset_FindSealingOverflow_Y_Ver = nOffset_FindSealingOverflow_Y_7H_Ver;
			dAngle_Hoz = 5;
			dAngle_Ver = 70;
			rectMeasureWidth = cv::Rect(ptCenter.x + nOffsetX, ptCenter.y - nOffsetY, 40, 40);
			rectFindSealingOverflow_Hoz = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Hoz, ptCenter.y - nOffset_FindSealingOverflow_Y_Hoz, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			rectFindSealingOverflow_Ver = cv::Rect(ptCenter.x + nOffset_FindSealingOverflow_X_Ver, ptCenter.y - nOffset_FindSealingOverflow_Y_Ver, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			break;
		case 3:
			nOffsetX = nOffsetROIFindMeaPt11H_X;
			nOffsetY = nOffsetROIFindMeaPt11H_Y;
			nOffset_FindSealingOverflow_X_Hoz = nOffset_FindSealingOverflow_X_11H_Hoz;
			nOffset_FindSealingOverflow_Y_Hoz = nOffset_FindSealingOverflow_Y_11H_Hoz;
			nOffset_FindSealingOverflow_X_Ver = nOffset_FindSealingOverflow_X_11H_Ver;
			nOffset_FindSealingOverflow_Y_Ver = nOffset_FindSealingOverflow_Y_11H_Ver;
			dAngle_Hoz = 3;
			dAngle_Ver = -70;
			rectMeasureWidth = cv::Rect(ptCenter.x + nOffsetX, ptCenter.y + nOffsetY, 40, 40);
			rectFindSealingOverflow_Hoz = cv::Rect(ptCenter.x - nOffset_FindSealingOverflow_X_Hoz, ptCenter.y + nOffset_FindSealingOverflow_Y_Hoz, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			rectFindSealingOverflow_Ver = cv::Rect(ptCenter.x + nOffset_FindSealingOverflow_X_Ver, ptCenter.y + nOffset_FindSealingOverflow_Y_Ver, nWidthROI_FindSealingOverflow, nHeightROI_FindSealingOverflow);
			break;
		}

		// 1. MEASURE WIDTH

#pragma region Measure Width
		cv::Mat matHSV, matResultHSV, maskHSV, matEle, matHSV2Gray, matBinary;
		cv::Scalar minHSV, maxHSV;
	    cv::cvtColor(matROI_BGR, matHSV, cv::COLOR_BGR2HSV);
	    matResultHSV = cv::Mat::zeros(matROI_BGR.rows, matROI_BGR.cols, CV_8UC3);
	    
		// Tolerance
		double dWidth_Refer_um = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Refer;
		double dWidth_Min_um = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Min;
		double dWidth_Max_um = recipeTopCamFrame2.m_dDistanceMeasurementTolerance_Max;
		double dPxlSize_um = recipeTopCamFrame2.m_dRatioPxlUm;

		double dWidth_Refer_Pxl = ConvertUmToPixel(dWidth_Refer_um, dPxlSize_um);
		double dWidth_Min_Pxl = dWidth_Refer_Pxl - ConvertUmToPixel(dWidth_Min_um, dPxlSize_um);
		double dWidth_Max_Pxl = dWidth_Refer_Pxl + ConvertUmToPixel(dWidth_Max_um, dPxlSize_um);

		int nHMin = recipeTopCamFrame2.m_nHMin;
	    int nHMax = recipeTopCamFrame2.m_nHMax;
	    int nSMin = recipeTopCamFrame2.m_nSMin;
	    int nSMax = recipeTopCamFrame2.m_nSMax;
	    int nVMin = recipeTopCamFrame2.m_nVMin;
	    int nVMax = recipeTopCamFrame2.m_nVMax;
		int nThresholdMeasureWidth = recipeTopCamFrame2.m_nThresholdBinary_MeasureWidth;
	    
	    minHSV = cv::Scalar(nHMin, nSMin, nVMin);
	    maxHSV = cv::Scalar(nHMax, nSMax, nVMax);
	    
	    cv::inRange(matHSV, minHSV, maxHSV, maskHSV);
	    cv::bitwise_and(matROI_BGR, matROI_BGR, matResultHSV, maskHSV);
		//cv::imshow("HSV", matResultHSV);

		cv::cvtColor(matResultHSV, matHSV2Gray, cv::COLOR_BGR2GRAY);
		cv::threshold(matHSV2Gray, matBinary, nThresholdMeasureWidth, 255, cv::THRESH_BINARY);
		//cv::imshow("Binary", matBinary);

		/*matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(7, 7));
		cv::morphologyEx(matBinary, matBinary, cv::MORPH_CLOSE, matEle);

		matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(12, 12));
		cv::morphologyEx(matBinary, matBinary, cv::MORPH_OPEN, matEle);*/

		
		// rotate rect
		/*cv::RotatedRect rotateRect(cv::Point2f(rectMeasureWidth.x + rectMeasureWidth.width / 2, rectMeasureWidth.y + rectMeasureWidth.height / 2), cv::Size(rectMeasureWidth.width, rectMeasureWidth.height), 30);
		cv::Point2f verticies2f[4];
		rotateRect.points(verticies2f);
		cv::Point vertices[4];
		for (int i = 0; i < 4; i++) {
			vertices[i] = verticies2f[i];
		}

		cv::Mat maskMeasureWidth(matBinary.rows, matBinary.cols, CV_8UC1, cv::Scalar(0));
		cv::fillConvexPoly(maskMeasureWidth, vertices, 4, cv::Scalar(255), cv::LINE_AA);
		matBinary.copyTo(matMeasureWidth, maskMeasureWidth);
		cv::cvtColor(matMeasureWidth, matMeasureWidth_BGR, cv::COLOR_GRAY2BGR);*/
		

		// Find rect rotate

		cv::Mat matMeasureWidth, matMeasureWidth_BGR;
		cv::Point vertices[4];
		std::vector<cv::Point> vertices_original;
		int nCounterNG_MeasureWidth = 0;

		matMeasureWidth = matBinary(rectMeasureWidth);
		if (matMeasureWidth.empty())
			return;

		matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5));
		cv::morphologyEx(matBinary, matBinary, cv::MORPH_DILATE, matEle);
		matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5));
		cv::morphologyEx(matBinary, matBinary, cv::MORPH_CLOSE, matEle);

		//cv::cvtColor(matMeasureWidth, matMeasureWidth_BGR, cv::COLOR_GRAY2BGR);

		std::vector<std::vector<cv::Point>> contours;
		cv::findContours(matMeasureWidth, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
		std::vector<std::vector<cv::Point>> contours_poly(contours.size());

		for (int i = 0; i < contours.size(); i++)
		{
			if (contours[i].size() < 30)
				continue;

			cv::approxPolyDP(cv::Mat(contours[i]), contours_poly[i], 3, true);

			cv::RotatedRect rotatedRect = cv::minAreaRect(cv::Mat(contours_poly[i]));

			// JUDGEMENT
			if (rotatedRect.size.height < dWidth_Min_Pxl || rotatedRect.size.height > dWidth_Max_Pxl)
			{
				nCounterNG_MeasureWidth++;
			}

			cv::Point2f vertices2f[4];
			rotatedRect.points(vertices2f);
			for (int i = 0; i < 4; ++i) {
				vertices[i] = cv::Point(vertices2f[i].x, vertices2f[i].y);
				vertices_original.push_back(cv::Point(vertices2f[i].x + rectMeasureWidth.x + vecRectROI[nROIIdx].x, vertices2f[i].y + rectMeasureWidth.y + vecRectROI[nROIIdx].y));
			}
		}

		/*for (int i = 0; i < 4; i++) {
			cv::line(matMeasureWidth_BGR, vertices[i], vertices[(i + 1) % 4], GREEN_COLOR, 1, cv::LINE_8);
		}
		cv::imshow("ROI Measure Width", matMeasureWidth_BGR);*/

		if (nCounterNG_MeasureWidth > 0)
		{
			bMeasureWidth = FALSE;
			nCounterNG++;
		}
#pragma endregion

		// 2. PROCESS SEALING OVERFLOW

#pragma region Find Sealing Overflow

		std::vector<cv::Point> vertices_hoz;
		std::vector<cv::Point> vertices_ver;

		bFindSealingOverflow_Hoz = FindSealingOverflow(&pImageDataROI, vecRectROI[nROIIdx], rectFindSealingOverflow_Hoz, vertices_hoz, dAngle_Hoz, nThreshold_FindSealingOverflow, nContourSizeMax_FindSealingOverflow, dAreaContourMax_FindSealingOverflow);
		bFindSealingOverflow_Ver = FindSealingOverflow(&pImageDataROI, vecRectROI[nROIIdx], rectFindSealingOverflow_Ver, vertices_ver, dAngle_Ver, nThreshold_FindSealingOverflow, nContourSizeMax_FindSealingOverflow, dAreaContourMax_FindSealingOverflow);

		if (bFindSealingOverflow_Hoz == FALSE || bFindSealingOverflow_Ver == FALSE)
		{
			nCounterNG++;
		}

		//cv::rectangle(mat, cv::Rect(rectFindSealingOverflow_Hoz.x + vecRectROI[nROIIdx].x, rectFindSealingOverflow_Hoz.y + vecRectROI[nROIIdx].y, rectFindSealingOverflow_Hoz.width, rectFindSealingOverflow_Hoz.height), ORANGE_COLOR, 1, cv::LINE_8);
		//cv::imshow("find sealing overflow", matROI_BGR);
#pragma endregion

#pragma region Draw Result

		DrawRotateRect(&mat, vertices_original, bMeasureWidth);
		DrawRotateRect(&mat, vertices_hoz, bFindSealingOverflow_Hoz);
		DrawRotateRect(&mat, vertices_ver, bFindSealingOverflow_Ver);

#pragma endregion

#endif

#ifdef ALGORITHM_1_TOPCAM_FRAME2

		switch (nROIIdx)
		{
		case 0:
			nOffsetX = nOffsetROIFindMeaPt1H_X;
			nOffsetY = nOffsetROIFindMeaPt1H_Y;
			rectFindPt = cv::Rect(ptCenter.x - nOffsetX, ptCenter.y + nOffsetY, 100, 60);
			break;
		case 1:
			nOffsetX = nOffsetROIFindMeaPt5H_X;
			nOffsetY = nOffsetROIFindMeaPt5H_Y;
			rectFindPt = cv::Rect(ptCenter.x - nOffsetX, ptCenter.y - nOffsetY, 100, 60);
			break;
		case 2:
			nOffsetX = nOffsetROIFindMeaPt7H_X;
			nOffsetY = nOffsetROIFindMeaPt7H_Y;
			rectFindPt = cv::Rect(ptCenter.x + nOffsetX, ptCenter.y - nOffsetY, 100, 60);
			break;
		case 3:
			nOffsetX = nOffsetROIFindMeaPt11H_X;
			nOffsetY = nOffsetROIFindMeaPt11H_Y;
			rectFindPt = cv::Rect(ptCenter.x + nOffsetX, ptCenter.y + nOffsetY, 100, 60);
			break;
		}

		cv::Mat pImageFindPt(rectFindPt.height, rectFindPt.width, CV_8UC1);
		for (int i = 0; i < pImageFindPt.rows; i++)
			memcpy(&pImageFindPt.data[i * pImageFindPt.step1()], &pImageDataROI.data[(i + rectFindPt.y) * pImageDataROI.step1() + rectFindPt.x], pImageFindPt.cols);

		cv::Mat matFindPt_Binary, matEle, matCanny;
		cv::threshold(pImageFindPt, matFindPt_Binary, 100, 255, cv::THRESH_BINARY);
		matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(5, 5));
		cv::morphologyEx(matFindPt_Binary, matFindPt_Binary, cv::MORPH_CLOSE, matEle);

		MakeCannyEdgeImage(&matFindPt_Binary, matCanny, 30, 30);

		/*char text[100] = {};
		sprintf_s(text, "%s_%d", "Canny", nROIIdx);
		cv::imshow("Binary", matFindPt_Binary);
		cv::imshow(text, matCanny);*/

		std::vector<std::vector<cv::Point> > contours;
		std::vector<cv::Vec4i> hierarchy;
		int nLargest_idx = 0;
		int nLargest_Contour = 0;

		cv::findContours(matCanny, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);
		for (int i = 0; i < contours.size(); i++) {
			if (contours[i].size() > nLargest_Contour) {
				nLargest_Contour = contours[i].size();
				nLargest_idx = i;
			}
		}

		int nAverX = 0;
		int nAverY = 0;
		int k = contours[nLargest_idx].size();
		for (int i = 0; i < contours[nLargest_idx].size(); i++)
		{
			nAverX += contours[nLargest_idx][i].x;
			nAverY += contours[nLargest_idx][i].y;
		}
		nAverX = nAverX / k;
		nAverY = nAverY / k;

		vecMeasurePt.push_back(cv::Point2f(nAverX, nAverY));

		int dx = (nAverX + rectFindPt.x) - (int)ptCenter.x;
		int dy = (nAverY + rectFindPt.y) - (int)ptCenter.y;
		double dDistance = std::sqrt(dx * dx + dy * dy);

		// Jugdement
		if (dDistance < (dDistanceRefer - dDistanceMin) || dDistance >(dDistanceRefer + dDistanceMax))
		{
			nCounterNG++;
		}

		char distText[100] = {};
		sprintf_s(distText, "%.2f", dDistance); 
		cv::Point2f ptMeasureOnOriginal(nAverX + vecRectROI[nROIIdx].x + rectFindPt.x, nAverY + vecRectROI[nROIIdx].y + rectFindPt.y);

		// draw
		cv::circle(mat, ptMeasureOnOriginal, 3, RED_COLOR, cv::FILLED);
		cv::putText(mat, distText, ptMeasureOnOriginal, cv::FONT_HERSHEY_SIMPLEX, 1.0, GREEN_COLOR, 1);
		cv::line(mat, ptMeasureOnOriginal, ptCenterOnOriginal, GREEN_COLOR, 1, cv::LINE_AA);

#endif // DEALGORITHM_1_TOPCAM_FRAME2BUG

#pragma region Draw All Results

		cv::Point2f ptCenterOnOriginal(ptCenter.x + vecRectROI[nROIIdx].x, ptCenter.y + vecRectROI[nROIIdx].y);
		cv::circle(mat, ptCenterOnOriginal, dRadius, RED_COLOR, 1, cv::LINE_AA);
		cv::circle(mat, ptCenterOnOriginal, 3, RED_COLOR, cv::FILLED);
		cv::Rect rectMeasure(rectMeasureWidth.x + vecRectROI[nROIIdx].x, rectMeasureWidth.y + vecRectROI[nROIIdx].y, rectMeasureWidth.width, rectMeasureWidth.height);
		
		//cv::rectangle(mat, rectMeasure, ORANGE_COLOR, 1, cv::LINE_AA);
		
		//cv::rectangle(mat, cv::Rect(rectFindPt.x + vecRectROI[nROIIdx].x, rectFindPt.y + vecRectROI[nROIIdx].y, rectFindPt.width, rectFindPt.height), ORANGE_COLOR, 1, cv::LINE_AA);
#pragma endregion

	}

	if (nCounterNG > 0)
	{
		nRet &= FALSE;
	}

	/*if (vecMeasurePt.empty())
		return;

	if (vecMeasurePt.size() == vecPtCenter.size())
	{
		for (int i = 0; i < vecMeasurePt.size(); i++) {
			double distance = CalculateDistancePointToCircle(vecMeasurePt[i], vecPtCenter[i], vecRadius[i]);
			vecDistance.push_back(distance);
			vecPtsIntersection.push_back(CalculateIntersectionPointCoordinate(vecMeasurePt[i], vecPtCenter[i], vecRadius[i], distance));
		}
	}*/

	// draw
	for (auto it = vecRectROI.begin(); it != vecRectROI.end(); ++it)
	{
		cv::rectangle(mat, *it, PINK_COLOR, 2, cv::LINE_AA);
	}

	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, nFrame, mat.data);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame2 = nRet;
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

	if (m_pInterface->GetSystemSetting()->m_bSimulation == TRUE)
		nFrameIdx -= 1;

	double dDistanceRefer = 0.0;
	double dDistanceMax = 0.0;
	double dDistanceMin = 0.0;
	double dPxlSize = 0.0;
	int nNumberOfNGMax = 0;
	int nUseFindROIAdvanced = 0;

	// recipe side cam

	CSealingInspectRecipe_SideCam recipeSideCam = pRecipe->m_sealingInspRecipe_SideCam[nBufferProcessorIdx];

	switch (nFrameIdx)
	{

		// use recipe of frame 1
	case 0:
	case 1:
		dDistanceRefer = recipeSideCam.m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer;
		dDistanceMin = recipeSideCam.m_recipeFrame1.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame1.m_dDistanceMeasurementTolerance_Max;
		dPxlSize = recipeSideCam.m_recipeFrame1.m_dRatioPxlUm;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame1.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		nUseFindROIAdvanced = recipeSideCam.m_recipeFrame1.b_bUseFindROIAdvancedAlgorithms;
		break;

		// use recipe of frame 2
	case 2:
	case 3:
	case 4:
		dDistanceRefer = recipeSideCam.m_recipeFrame2.m_dDistanceMeasurementTolerance_Refer;
		dDistanceMin = recipeSideCam.m_recipeFrame2.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame2.m_dDistanceMeasurementTolerance_Max;
		dPxlSize = recipeSideCam.m_recipeFrame2.m_dRatioPxlUm;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame2.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		nUseFindROIAdvanced = recipeSideCam.m_recipeFrame2.b_bUseFindROIAdvancedAlgorithms;
		break;

		// use recipe of frame 3
	case 5:
	case 6:
		dDistanceRefer = recipeSideCam.m_recipeFrame3.m_dDistanceMeasurementTolerance_Refer;
		dDistanceMin = recipeSideCam.m_recipeFrame3.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame3.m_dDistanceMeasurementTolerance_Max;
		dPxlSize = recipeSideCam.m_recipeFrame3.m_dRatioPxlUm;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame3.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		nUseFindROIAdvanced = recipeSideCam.m_recipeFrame3.b_bUseFindROIAdvancedAlgorithms;
		break;

		// use recipe of frame 4
	case 7:
	case 8:
	case 9:
		dDistanceRefer = recipeSideCam.m_recipeFrame4.m_dDistanceMeasurementTolerance_Refer;
		dDistanceMin = recipeSideCam.m_recipeFrame4.m_dDistanceMeasurementTolerance_Min;
		dDistanceMax = recipeSideCam.m_recipeFrame4.m_dDistanceMeasurementTolerance_Max;
		dPxlSize = recipeSideCam.m_recipeFrame4.m_dRatioPxlUm;
		nNumberOfNGMax = recipeSideCam.m_recipeFrame4.m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
		nUseFindROIAdvanced = recipeSideCam.m_recipeFrame4.b_bUseFindROIAdvancedAlgorithms;
		break;

	// ignore frame 8
	/*case 8:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame9 = TRUE;
		return;*/
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

	if (nUseFindROIAdvanced == 1)
	{
		MakeROI_AdvancedAlgorithms(recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindLine, rectROIFindPts);
	}
	else if (nUseFindROIAdvanced == 0) {
		MakeROIFindLine(recipeSideCam, nFrameIdx, rectROIFindLine);
		MakeROIFindPoints(recipeSideCam, nFrameIdx, rectROIFindPts);
	}

	/*cv::rectangle(mat, rectROIFindLine, BLUE_COLOR, 2, cv::LINE_AA);
	cv::rectangle(mat, rectROIFindPts, BLUE_COLOR, 2, cv::LINE_AA);*/

	bFindLine_TopBottom = FindLine_Top_Bottom_Average(recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindLine, vecPtLineTop);
	//FindLine_Bottom_Top_Average(&recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindPts, matROIFindPts, vecPtsLineBottom);

	if (bFindLine_TopBottom == FALSE)
	{
		// 3. Set buff
		nRet &= bFindLine_TopBottom;
		m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);
		switch (nFrameIdx)
		{
		case 0:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
			break;
		case 1:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
			break;
		case 2:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
			break;
		case 3:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
			break;
		case 4:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame5 = nRet;
			break;
		case 5:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame6 = nRet;
			break;
		case 6:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame7 = nRet;
			break;
		case 7:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame8 = nRet;
			break;
		case 8:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame9 = nRet;
			break;
		case 9:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame10 = nRet;
			break;
		}

		return;
	}


	bFindMeasurePt = FindMeasurePoints_SideCam(recipeSideCam, &pMatProcess, nFrameIdx, rectROIFindPts, vecMeasurePt);
	//FindMeasurePointsAtPosDistMinMax_SideCam(&recipeSideCam, &matROIFindPts, nFrameIdx, rectROIFindPts, vecMeasurePt);

	FindClosesPointAndDistancePointToLine(vecMeasurePt, vecPtLineTop, vecClosesPt, vecDist);

	if (bFindMeasurePt == FALSE)
	{
		// 3. Set buff
		nRet &= bFindMeasurePt;
		m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);
		switch (nFrameIdx)
		{
		case 0:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
			break;
		case 1:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
			break;
		case 2:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
			break;
		case 3:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
			break;
		case 4:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame5 = nRet;
			break;
		case 5:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame6 = nRet;
			break;
		case 6:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame7 = nRet;
			break;
		case 7:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame8 = nRet;
			break;
		case 8:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame9 = nRet;
			break;
		case 9:
			m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame10 = nRet;
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

	double dDistPxlMin = ConvertUmToPixel(dDistanceRefer, dPxlSize) - ConvertUmToPixel(dDistanceMin, dPxlSize);
	double dDistPxlMax = ConvertUmToPixel(dDistanceRefer, dPxlSize) + ConvertUmToPixel(dDistanceMax, dPxlSize);
	int nCounter = 0;

	//nRet &= JudgementInspectDistanceMeasurement_AdvancedAlgorithms(vecDist, vecPosNG, dDistPxlMin, dDistPxlMax, nNumberOfNGMax);
	for (int i = 0; i < vecDist.size(); i++) {
		if (vecDist[i] > dDistPxlMax || vecDist[i] < dDistPxlMin) {
			//nRet &= FALSE;
			nCounter++;
			vecPosNG.push_back(i);
		}
	}
	if (nCounter > nNumberOfNGMax)
		nRet &= FALSE;

	DrawROIFindLine(mat, rectROIFindLine, vecPtLineTop);
	//DrawROIFindLine(mat, rectROIFindPts, vecPtsLineBottom);
	DrawROIFindPoints(mat, rectROIFindPts, vecMeasurePt, vecClosesPt);
	//DrawPositionNG(mat, vecPosNG, vecMeasurePt);

	// 3. Set buff

	m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, mat.data);

	switch (nFrameIdx)
	{
	case 0:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame1 = nRet;
		break;
	case 1:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame2 = nRet;
		break;
	case 2:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame3 = nRet;
		break;
	case 3:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame4 = nRet;
		break;
	case 4:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame5 = nRet;
		break;
	case 5:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame6 = nRet;
		break;
	case 6:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame7 = nRet;
		break;
	case 7:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame8 = nRet;
		break;
	case 8:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame9 = nRet;
		break;
	case 9:
		m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_SideCam.m_bStatusFrame10 = nRet;
		break;
	}
}

