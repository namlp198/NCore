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

BOOL CSealingInspectCore::FindCircle_MinEnclosing(cv::Mat& matProcess, int nThresholdBinary, int nContourSizeMin, int nContourSizeMax,
	                                             std::vector<std::vector<cv::Point>>& vecContours,
	                                             std::vector<cv::Vec4i>& vecHierarchy, std::vector<cv::Point2f>& vecCenters,
	                                             std::vector<float>& vecRadius)
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
		if (contours[i].size() < nContourSizeMin || contours[i].size() > nContourSizeMax)
			continue;

		// approximate contour become the polygon
		approxPolyDP(contours[i], contours_poly[i], 3, true);
		// find minimal enclosing circle of contours
		minEnclosingCircle(contours_poly[i], centers[i], radius[i]);
	}

	vecContours = contours;
	vecHierarchy = hierarchy;
	vecCenters = centers;
	vecRadius = radius;

	return TRUE;
}

BOOL CSealingInspectCore::FindCircle_HoughCircle(cv::Mat& matProcess, 
	                                             std::vector<cv::Vec3f>& vecCircles, 
	                                             std::vector<cv::Point2i>& vecPts,
	                                             int minDist, int nRadiusOuterMin, int nRadiusOuterMax, double dIncreAngle)
{
	cv::Mat gray, blur, canny;

	cv::cvtColor(matProcess, gray, cv::COLOR_BGR2GRAY);

	cv::GaussianBlur(gray, blur, cv::Size(3, 3), 0.7, 0.7);
	cv::Canny(blur, canny, 255, 120);

	// Apply the Hough Transform to find the circles
	cv::HoughCircles(gray, vecCircles, cv::HOUGH_GRADIENT, 1, minDist, 200, 80, nRadiusOuterMin, nRadiusOuterMax);
	
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

BOOL CSealingInspectCore::FindDistanceAll_OuterToInner(std::vector<cv::Point2i>& vecPts, 
	                                                   std::vector<cv::Point2i>& vecPtsIntersection,
	                                                   std::vector<cv::Point2f>& vecIntsecPtsFound,
	                                                   cv::Point2f center, double radius,
	                                                   std::vector<double>& vecDistance)
{
	for (int i = 0; i < vecPts.size(); i++) {
		double distance = CalculateDistancePointToCircle(vecPts[i], center, radius);
		vecDistance.push_back(distance);
		vecPtsIntersection.push_back(CalculateIntersectionPointCoordinate(vecPts[i], center, distance));
		
		//vecIntsecPtsFound.push_back(FindIntersectionPoint_LineCircle(vecPts[i], center, radius));
	}

	return TRUE;
}

BOOL CSealingInspectCore::JudgementInspectDistanceMeasurement(std::vector<double>& vecDistance, 
	                                                          std::vector<int>& vecPosNG, 
	                                                          double nDistanceMin, double nDistanceMax)
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

void CSealingInspectCore::MakeROIAdvancedAlgorithms(cv::Mat mat, cv::Point centerPt, double dRadius, int nROIWidth_Hor, int nROIHeight_Hor, int nROIWidth_Ver, int nROIHeight_Ver)
{
	int nX_12H = centerPt.x - 60;
	int nY_12H = centerPt.y - (dRadius + 4);

	cv::Rect rect12H(nX_12H, nY_12H, nROIWidth_Hor, nROIHeight_Hor);
	cv::rectangle(mat, rect12H, BLUE_COLOR, 1, cv::LINE_AA);
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

cv::Point2i CSealingInspectCore::CalculateIntersectionPointCoordinate(cv::Point2i pt, cv::Point2f centerPt, double dDist)
{
	cv::Point2i ptIntersection;

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

	return ptIntersection;
}

void CSealingInspectCore::Draw_MinEnclosing(cv::Mat& matSrc, int nRadiusInnerMin, int nRadiusInnerMax, 
	                                              std::vector<std::vector<cv::Point>>& vecContours, 
	                                              std::vector<cv::Point2f>& vecCenters, 
	                                              std::vector<float>& vecRadius, cv::Point2f& center, double& dRadius)
{
	for (size_t i = 0; i < vecContours.size(); i++)
	{
		if ((int)vecRadius[i] > nRadiusInnerMin && (int)vecRadius[i] < nRadiusInnerMax)
		{
			center = vecCenters[i];
			dRadius = vecRadius[i];

			//cv::drawContours(mat, contours, (int)i, cv::Scalar(255, 0, 255), 1, cv::LINE_AA, hierarchy, 0);
			cv::circle(matSrc, vecCenters[i], (int)vecRadius[i], GREEN_COLOR, 1);
			cv::circle(matSrc, vecCenters[i], 3, PINK_COLOR, cv::FILLED, cv::LINE_AA);
			char ch[256];
			sprintf_s(ch, sizeof(ch), "R = %.2f pxl", vecRadius[i]);
			std::string text(ch);
			cv::putText(matSrc, text, cv::Point(vecCenters[i].x - 100, vecCenters[i].y - vecRadius[i] - 10), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);

			char ch2[256];
			sprintf_s(ch2, sizeof(ch2), "(x: %.1f, y: %.1f)", vecCenters[i].x, vecCenters[i].y);
			cv::putText(matSrc, ch2, cv::Point(vecCenters[i].x - 300, vecCenters[i].y), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR);
		}
	}
}

void CSealingInspectCore::Draw_HoughCircle(cv::Mat& matSrc, std::vector<cv::Vec3f>& vecCircles, 
	                                       std::vector<cv::Point2i>& vecPts)
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

void CSealingInspectCore::ProcessFrame1_TopCam(CSealingInspectRecipe* pRecipe, int nCamIdx, int nBufferProcessorIdx, cv::Mat& mat)
{
	// params minEnclosing
	int nThresholdBinary = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nThresholdBinaryMinEnclosing;
	int nRadiusInnerMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusInner_Min;
	int nRadiusInnerMax = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusInner_Max;
	int nContourSizeMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Min;
	int nContourSizeMax = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nContourSizeMinEnclosingCircle_Max;

	// params HoughCircle
	int nThresholdCanny = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nThresholdBinaryCannyHoughCircle;
	int nRadiusOuterMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusOuter_Min;
	int nRadiusOuterMax = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nRadiusOuter_Max;
	int nDistRadiusDiffMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nDistanceRadiusDiffMin;
	double dIncrementAngle = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_dIncrementAngle;

	// judgement
	int nDeltaRadiusTolerance = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nDeltaRadiusOuterInner;
	double nDistanceMeasureToleranceMin = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min;
	double nDistanceMeasureToleranceMax = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max;
	BOOL bUseAdvancedAlgorithms = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_bUseAdvancedAlgorithms;

	// params make ROI
	int nROIWidth_Hor = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nROIWidth_Hor;
	int nROIHeight_Hor = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nROIHeight_Hor;
	int nROIWidth_Ver = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nROIWidth_Ver;
	int nROIHeight_Ver = pRecipe->m_sealingInspRecipe_TopCam[nCamIdx].m_recipeFrame1.m_nROIHeight_Ver;

	// copy mat
	cv::Mat matCpy;
	mat.copyTo(matCpy);

	//cv::imshow("mat copy", matCpy);

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
	cv::Point2f center;
	double dRadius_Inner;
	std::vector<double> vecDist;
	std::vector<int> vecPosNG;

	// Find inner circle
	FindCircle_MinEnclosing(matCpy, nThresholdBinary, nContourSizeMin, nContourSizeMax, vecContours, vecHierarchy, vecCenters, vecRadius);
	Draw_MinEnclosing(mat, nRadiusInnerMin, nRadiusInnerMax, vecContours, vecCenters, vecRadius, center, dRadius_Inner);

	// Find outer circle
	FindCircle_HoughCircle(matCpy, vecCircles, vecPoints, nDistRadiusDiffMin, nRadiusOuterMin, nRadiusOuterMax, dIncrementAngle);
	Draw_HoughCircle(mat, vecCircles, vecPoints);

	// calculate distance
	FindDistanceAll_OuterToInner(vecPoints, vecPtsIntersection, vecIntsecPtsFound, center, dRadius_Inner, vecDist);

	if (bUseAdvancedAlgorithms == TRUE) 
	{
		// Inspection advanced
		MakeROIAdvancedAlgorithms(mat, center, dRadius_Inner, nROIWidth_Hor, nROIHeight_Hor, nROIWidth_Ver, nROIHeight_Ver);
	}

	// Judgement
	BOOL nRet = FALSE;
	double dRadius_Outer = (vecCircles[0][2]);
	int nDeltaRadius = (int)(dRadius_Outer - dRadius_Inner);

	nRet = nDeltaRadius < nDeltaRadiusTolerance ? TRUE : FALSE;
	
	nRet &= JudgementInspectDistanceMeasurement(vecDist, vecPosNG, nDistanceMeasureToleranceMin, nDistanceMeasureToleranceMax);

	// draw pos NG
	if (vecPosNG.size() > 0) {
		for (int i = 0; i < vecPosNG.size(); i++) {
			int x = vecPoints[vecPosNG[i]].x - 15;
			int y = vecPoints[vecPosNG[i]].y - 15;
			int width = 30;
			int height = 30;
			cv::Rect rec(x, y, width, height);
			cv::rectangle(mat, rec, RED_COLOR, 1, cv::LINE_AA);
		}
	}

	for (int i = 0; i < vecPoints.size(); i++) {
		cv::line(mat, vecPoints[i], vecPtsIntersection[i], GREEN_COLOR, 1, cv::LINE_AA);
		cv::circle(mat, vecPtsIntersection[i], 1, ORANGE_COLOR, 1, cv::FILLED);
		cv::circle(mat, vecPoints[i], 1, ORANGE_COLOR, 1, cv::FILLED);
	}

	// set buffer
	m_pInterface->SetResultBuffer_TOP(nBufferProcessorIdx, 0, mat.data);

	m_pInterface->GetSealingInspectResultControl(m_nCoreIdx)->m_sealingInspResult_TopCam.m_bStatusFrame1 = nRet;
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

