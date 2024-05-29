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

	m_pInterface->InspectComplete(emInspectCavity_Cavity2, FALSE);
}

void CSealingInspectCore::Inspect_TopCam_Simulation(int nCamIdx, int nFrame)
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

	cv::Mat mat(FRAME_HEIGHT_SIDECAM, FRAME_WIDTH_SIDECAM, CV_8UC3, pImageBuffer);

	if (nFrame == 1) {
		ProcessFrame1_TopCam(recipe, nCamIdx, nTopCam1_ResultBuffer, mat);
	}
	else if (nFrame == 2) {
		ProcessFrame2_TopCam(recipe, nCamIdx, nTopCam2_ResultBuffer, mat);
	}

	if(nCamIdx == 0)
		m_pInterface->InspectComplete(emInspectCavity_Cavity1, TRUE);
	else if(nCamIdx == 1)
		m_pInterface->InspectComplete(emInspectCavity_Cavity2, TRUE);
}

void CSealingInspectCore::Inspect_SideCam_Simulation(int nCamIdx, int nFrame)
{
}

void CSealingInspectCore::FindCircle_MinEnclosing(cv::Mat& matSrc, cv::Mat& matProcess, int nThresholdBinary, int nRadiusInnerMin, int nRadiusInnerMax,
	                                              std::vector<std::vector<cv::Point>> vecContours, std::vector<cv::Vec4i> vecHierarchy, 
	                                              std::vector<cv::Point2f> vecCenters, std::vector<float> vecRadius)
{
	
	cv::Mat matGray, matBinary;
	cv::cvtColor(matProcess, matGray, cv::COLOR_BGR2GRAY);
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
		if (contours[i].size() < 1000)
			continue;

		// approximate contour become the polygon
		approxPolyDP(contours[i], contours_poly[i], 3, true);
		// find minimal enclosing circle of contours
		minEnclosingCircle(contours_poly[i], centers[i], radius[i]);
	}

	for (size_t i = 0; i < contours.size(); i++)
	{
		cv::Scalar color = cv::Scalar(0, 0, 255);
		//cv::drawContours(drawing, contours_poly, (int)i, color);
		if ((int)radius[i] > nRadiusInnerMin && (int)radius[i] < nRadiusInnerMax)
		{
			//cv::drawContours(mat, contours, (int)i, cv::Scalar(255, 0, 255), 1, cv::LINE_AA, hierarchy, 0);
			cv::circle(matSrc, centers[i], (int)radius[i], cv::Scalar(0, 255, 0), 1);
			cv::circle(matSrc, centers[i], 3, cv::Scalar(255, 0, 255), cv::FILLED, cv::LINE_AA);
			char ch[256];
			sprintf_s(ch, sizeof(ch), "R = %.2f pxl", radius[i]);
			std::string text(ch);
			cv::putText(matSrc, text, cv::Point(centers[i].x - 100, centers[i].y - radius[i] - 10), cv::FONT_HERSHEY_PLAIN, 1.5, color);

			char ch2[256];
			sprintf_s(ch2, sizeof(ch2), "(x: %.1f, y: %.1f)", centers[i].x, centers[i].y);
			cv::putText(matSrc, ch2, cv::Point(centers[i].x - 300, centers[i].y), cv::FONT_HERSHEY_PLAIN, 1.5, color);
		}
	}

	/*vecContours = contours;
	vecHierarchy = hierarchy;
	vecCenters = centers;
	vecRadius = radius;*/
}

BOOL CSealingInspectCore::FindCircle_HoughCircle(cv::Mat& matSrc, cv::Mat& matProcess, std::vector<cv::Vec3f> circles, std::vector<cv::Point2i> vecPts)
{
	return 0;
}

void CSealingInspectCore::ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	int nThresholdBinary = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nThresholdBinaryMinEnclosing;
	int nRadiusInnerMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusInner_Min;
	int nRadiusInnerMax = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusInner_Max;

	// copy mat
	cv::Mat matCpy;
	mat.copyTo(matCpy);

	//cv::imshow("mat copy", matCpy);

	std::vector<std::vector<cv::Point> > contours;
	std::vector<cv::Vec4i> hierarchy;
	std::vector<cv::Point2f> centers;
	std::vector<float> radius;

	FindCircle_MinEnclosing(mat, matCpy, nThresholdBinary, nRadiusInnerMin, nRadiusInnerMax, contours, hierarchy, centers, radius);

	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, 0, mat.data);

	char ch[255] = {};
	sprintf_s(ch, "%s%s%d%s", "E:\\SealingImage\\FullImage\\20240522\\SealingAllInspect_12345\\", "TopCam", (nBufferProcessorIdx + 1), "_Frame1.bmp");
	cv::imwrite(ch, mat);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = TRUE;
}

void CSealingInspectCore::ProcessFrame2_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, 1, mat.data);

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

	m_pInterface->SetResultBuffer_SIDE(nBufferProcessorIdx, nFrameIdx, pImageBuffer);

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

