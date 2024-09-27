#include "pch.h"
#include "NVisionInspectCore.h"

CNVisionInspectCore::CNVisionInspectCore(INVisionInspectCoreToParent* pInterface)
{
	m_pInterface = pInterface;
	m_bSimulator = FALSE;
}

CNVisionInspectCore::~CNVisionInspectCore()
{
}

void CNVisionInspectCore::CreateInspectThread(int nThreadCount)
{
	DeleteInspectThread();

	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	m_vecProcessedFrame.clear();
	m_vecProcessedFrame.resize(MAX_FRAME_WAIT, FALSE);

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;
		m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CNVisionInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], nThreadIdx));

		localLock.Unlock();
	}
}

void CNVisionInspectCore::DeleteInspectThread()
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

void CNVisionInspectCore::WorkThreadProcessArray(PVOID pParameter)
{
	CNVisionInspectCoreThreadData* pThreadData = static_cast<CNVisionInspectCoreThreadData*>(pParameter);

	if (pThreadData == NULL) return;

	UINT nThreadOrder = pThreadData->m_nThreadIdx;
	RunningThread_INSPECT((int)nThreadOrder);
}

void CNVisionInspectCore::RunningThread_INSPECT(int nThreadIndex)
{
	if (m_pInterface == NULL)
		return;

	int nGrabberIdx = 0;
	CNVisionInspectRecipe* recipe = m_pInterface->GetRecipeControl();
	CNVisionInspectSystemSetting* sysSetting = m_pInterface->GetSystemSettingControl();

	while (m_bRunningThread[nThreadIndex] == TRUE)
	{
		// for avoid UI Freezing
		Sleep(1);

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

	//m_pInterface->InspectComplete(FALSE);
}

void CNVisionInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++) {
		CSingleLock lockLocal(&m_csWorkThreadArray[i]);
		lockLocal.Lock();
		m_bRunningThread[i] = FALSE;
		lockLocal.Unlock();
	}
}

void CNVisionInspectCore::StartThread(int nThreadCount)
{
	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		m_bRunningThread[nThreadIdx] = TRUE;

		m_pWorkThreadArray[nThreadIdx]->WorkThreadProcess(new CNVisionInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], nThreadIdx));
		localLock.Unlock();
	}
}

void CNVisionInspectCore::Inspect_Simulation(emCameraBrand camBrand, int nCamIdx, int nBuff, int nFrame)
{
	LPBYTE pBuff = m_pInterface->GetSimulatorBuffer(nBuff, nFrame);

	if (pBuff == NULL)
		return;

	if (nCamIdx < 0 || nCamIdx >= MAX_CAMERA_INSPECT_COUNT)
		return;

	int nCamIndex = 0;
	int nHikCamCount = 0;

	switch (camBrand)
	{
	case CameraBrand_Hik:
		nCamIndex = nCamIdx;
		break;
	case CameraBrand_Basler:
		nHikCamCount = m_pInterface->GetVecCameras().at(0); // Index 0 is number of Hik Cam
		nCamIndex = nHikCamCount + nCamIdx;
		break;
	case CameraBrand_Jai:
		break;
	case CameraBrand_IRayple:
		break;
	}

	ProcessFrame(nCamIndex, pBuff);
}

void CNVisionInspectCore::Inspect_Reality(emCameraBrand camBrand, int nCamIdx, LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	if (nCamIdx < 0 || nCamIdx >= MAX_CAMERA_INSPECT_COUNT)
		return;

	int nCamIndex = 0;
	int nHikCamCount = 0;

	switch (camBrand)
	{
	case CameraBrand_Hik:
		nCamIndex = nCamIdx;
		break;
	case CameraBrand_Basler:
		nHikCamCount = m_pInterface->GetVecCameras().at(0); // Index 0 is number of Hik Cam
		nCamIndex = nHikCamCount + nCamIdx;
		break;
	case CameraBrand_Jai:
		break;
	case CameraBrand_IRayple:
		break;
	}

	ProcessFrame(nCamIndex, pBuffer);
}

void CNVisionInspectCore::LocatorTool_Train(int nCamIdx, LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	CNVisionInspectCameraSetting* pCamSetting = m_pInterface->GetCameraSettingControl(nCamIdx);
	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();

	int nBuff = 0;
	int nFrame = 0;

	cv::Mat matGray, matBGR;
	cv::Mat matSrc(pCamSetting->m_nFrameHeight, pCamSetting->m_nFrameWidth, CV_8UC3, pBuffer);

	cv::cvtColor(matSrc, matGray, cv::COLOR_BGR2GRAY);
	matSrc.copyTo(matBGR);

	//cv::imshow("gray", matGray);

	int nRectInner_X = 0;
	int nRectInner_Y = 0;
	int nRectInner_Width = 0;
	int nRectInner_Height = 0;

	int nRectOuter_X = 0;
	int nRectOuter_Y = 0;
	int nRectOuter_Width = 0;
	int nRectOuter_Height = 0;

	double dMatchingRateLimit = 0.0;

	switch (nCamIdx)
	{
	case 0:
	{
		nRectInner_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerX;
		nRectInner_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerY;
		nRectInner_Width = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Width;
		nRectInner_Height = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Height;

		nRectOuter_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterX;
		nRectOuter_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterY;
		nRectOuter_Width = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Width;
		nRectOuter_Height = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Height;

		dMatchingRateLimit = pRecipe->m_NVisionInspRecipe_Cam1.m_dTemplateMatchingRate;
		break;
	}
	case 1:
	{
		break;
	}
	case 2:
	{
		break;
	}
	case 3:
	{
		break;
	}
	case 4:
	{
		break;
	}
	case 5:
	{
		break;
	}
	case 6:
	{
		break;
	}
	case 7:
	{
		break;
	}
	}

	// 2. Get image template
	cv::Mat imgTemplate(nRectInner_Height, nRectInner_Width, CV_8UC1);
	for (size_t i = 0; i < imgTemplate.rows; i++)
		memcpy(&imgTemplate.data[i * imgTemplate.step1()], &matGray.data[(i + nRectInner_Y) * matGray.step1() + nRectInner_X], imgTemplate.cols);

	//cv::imshow("template", imgTemplate);

	// 3. Crop Image Outer
	cv::Mat imgOuter(nRectOuter_Height, nRectOuter_Width, CV_8UC1);
	for (int i = 0; i < imgOuter.rows; i++)
		memcpy(&imgOuter.data[i * imgOuter.step1()], &matGray.data[(i + nRectOuter_Y) * matGray.step1() + nRectOuter_X], imgOuter.cols);

	SaveTemplateImage(imgTemplate, nCamIdx);

	//m_pInterface->AlarmMessage(_T("Saved Image Template Success!"));

	// make inner ROI, outer ROI and draw
	cv::Rect rectInner(nRectInner_X, nRectInner_Y, nRectInner_Width, nRectInner_Height);
	cv::Rect rectOuter(nRectOuter_X, nRectOuter_Y, nRectOuter_Width, nRectOuter_Height);
	cv::rectangle(matBGR, rectInner, GREEN_COLOR, 2, cv::LINE_AA);
	cv::rectangle(matBGR, rectOuter, BLUE_COLOR, 2, cv::LINE_AA);

	// 3. Find center
	// Template matching
	cv::Mat C = cv::Mat::zeros(imgOuter.rows - imgTemplate.rows + 1, imgOuter.cols - imgTemplate.cols + 1, CV_32FC1);

	double dMin = 0.0;
	double dMatchingRate = 0.0;
	cv::Point ptLeftTop;
	cv::Point ptFindResult;

	cv::matchTemplate(imgOuter, imgTemplate, C, cv::TM_CCOEFF_NORMED);
	cv::minMaxLoc(C, &dMin, &dMatchingRate, NULL, &ptLeftTop);

	ptFindResult.x = float(ptLeftTop.x) + (imgTemplate.cols / 2.0);
	ptFindResult.y = float(ptLeftTop.y) + (imgTemplate.rows / 2.0);
	dMatchingRate = dMatchingRate * 100.0;

	if (dMatchingRate < dMatchingRateLimit)
	{
		cv::putText(matBGR, "Can't Find Template", cv::Point(matBGR.rows - 20, 80), cv::FONT_HERSHEY_SIMPLEX, 2.0, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
		m_pInterface->SetResultBuffer(0, 0, matBGR.data);

		return;
	}

	int ptResult_X = ptFindResult.x + nRectOuter_X;
	int ptResult_Y = ptFindResult.y + nRectOuter_Y;

	switch (nCamIdx)
	{
	case 0:
	{
		pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX = ptResult_X;
		pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY = ptResult_Y;
		break;
	}
	case 1:
	{
		break;
	}
	case 2:
	{
		break;
	}
	case 3:
	{
		break;
	}
	case 4:
	{
		break;
	}
	case 5:
	{
		break;
	}
	case 6:
	{
		break;
	}
	case 7:
	{
		break;
	}
	}

	// draw center pt result
	cv::circle(matBGR, cv::Point(ptResult_X, ptResult_Y), 3, YELLOW_COLOR, cv::FILLED);

	// draw center pt text
	char ch2[256];
	sprintf_s(ch2, sizeof(ch2), "(x: %d, y: %d)", ptResult_X, ptResult_Y);
	cv::putText(matBGR, ch2, cv::Point(ptResult_X + 10, ptResult_Y - 10), cv::FONT_HERSHEY_PLAIN, 1.0, CYAN_COLOR);

	// set result to buffer
	m_pInterface->SetResultBuffer(nBuff, nFrame, matBGR.data);

	// inform that locator trained succsess
	m_pInterface->LocatorTrainComplete(nCamIdx);
}

void CNVisionInspectCore::HSVTrain(int nCamIdx, int nFrame, CNVisionInspectRecipe_HSV* pRecipeHSV)
{
	Sleep(5);

	if (nCamIdx < 0)
		return;

	LPBYTE pBuffer = NULL;

	// Fake Cam
	if (nCamIdx >= m_pInterface->GetSystemSettingControl()->m_nNumberOfInspectionCamera)
	{
		pBuffer = m_pInterface->GetSimulatorBuffer_FakeCam(nFrame);
		if (pBuffer == NULL)
			return;

		int nWidth = m_pInterface->GetFakeCameraSettingControl()->m_nFrameWidth;
		int nHeight = m_pInterface->GetFakeCameraSettingControl()->m_nFrameHeight;

		cv::Mat matSrc(nHeight, nWidth, CV_8UC3, pBuffer);

		cv::Mat matHSV, matResultHSV, maskHSV;

		int nHueMin = pRecipeHSV->m_nHueMin;
		int nHueMax = pRecipeHSV->m_nHueMax;
		int nSatMin = pRecipeHSV->m_nSaturationMin;
		int nSatMax = pRecipeHSV->m_nSaturationMax;
		int nValMin = pRecipeHSV->m_nValueMin;
		int nValMax = pRecipeHSV->m_nValueMax;

		cv::Scalar minHSV, maxHSV;
		cv::cvtColor(matSrc, matHSV, cv::COLOR_BGR2HSV);
		matResultHSV = cv::Mat::zeros(matSrc.rows, matSrc.cols, CV_8UC3);

		minHSV = cv::Scalar(nHueMin, nSatMin, nValMin);
		maxHSV = cv::Scalar(nHueMax, nSatMax, nValMax);

		cv::inRange(matHSV, minHSV, maxHSV, maskHSV);
		cv::bitwise_and(matSrc, matSrc, matResultHSV, maskHSV);

		m_pInterface->SetResultBuffer_FakeCam(nFrame, matResultHSV.data);
		m_pInterface->HSVTrainComplete(nCamIdx);

		Sleep(1);
	}
}

void CNVisionInspectCore::MakeROI(int nCamIdx, int nROIIdx, LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	if (nCamIdx == -1)
		return;

	if (nROIIdx == -1)
		return;

	cv::Mat matGray, matBGR, matROI;
	cv::Rect rectROI;

	int nWidth = m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameWidth;
	int nHeight = m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameHeight;
	cv::Mat matSrc(nHeight, nWidth, CV_8UC3, pBuffer);

	cv::cvtColor(matSrc, matGray, cv::COLOR_BGR2GRAY);
	matSrc.copyTo(matBGR);

	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();

	int nROI_X = 0;
	int nROI_Y = 0;
	int nROI_OffsetX = 0;
	int nROI_OffsetY = 0;
	int nROI_Width = 0;
	int nROI_Height = 0;
	int nTempCntPt_X = 0;
	int nTempCntPt_Y = 0;

	BOOL bROI_UseOffset = FALSE;

	switch (nCamIdx)
	{
	case 0:
	{
		nTempCntPt_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX;
		nTempCntPt_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY;

		switch (nROIIdx)
		{
		case 1:
		{
			nROI_Width = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Width;
			nROI_Height = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Height;
			bROI_UseOffset = pRecipe->m_NVisionInspRecipe_Cam1.m_bROI1UseOffset;

			if (bROI_UseOffset == TRUE)
			{
				nROI_OffsetX = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Offset_X;
				nROI_OffsetY = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Offset_Y;

				rectROI.x = nTempCntPt_X + nROI_OffsetX;
				rectROI.y = nTempCntPt_Y + nROI_OffsetY;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			else if (bROI_UseOffset == FALSE)
			{
				nROI_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_X;
				nROI_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI1_Y;

				rectROI.x = nROI_X;
				rectROI.y = nROI_Y;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			break;
		}
		case 2:
		{
			nROI_Width = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Width;
			nROI_Height = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Height;
			bROI_UseOffset = pRecipe->m_NVisionInspRecipe_Cam1.m_bROI2UseOffset;

			if (bROI_UseOffset == TRUE)
			{
				nROI_OffsetX = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Offset_X;
				nROI_OffsetY = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Offset_Y;

				rectROI.x = nTempCntPt_X + nROI_OffsetX;
				rectROI.y = nTempCntPt_Y + nROI_OffsetY;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			else if (bROI_UseOffset == FALSE)
			{
				nROI_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_X;
				nROI_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI2_Y;

				rectROI.x = nROI_X;
				rectROI.y = nROI_Y;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			break;
		}
		case 3:
		{
			nROI_Width = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Width;
			nROI_Height = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Height;
			bROI_UseOffset = pRecipe->m_NVisionInspRecipe_Cam1.m_bROI3UseOffset;

			if (bROI_UseOffset == TRUE)
			{
				nROI_OffsetX = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Offset_X;
				nROI_OffsetY = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Offset_Y;

				rectROI.x = nTempCntPt_X + nROI_OffsetX;
				rectROI.y = nTempCntPt_Y + nROI_OffsetY;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			else if (bROI_UseOffset == FALSE)
			{
				nROI_X = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_X;
				nROI_Y = pRecipe->m_NVisionInspRecipe_Cam1.m_nROI3_Y;

				rectROI.x = nROI_X;
				rectROI.y = nROI_Y;
				rectROI.width = nROI_Width;
				rectROI.height = nROI_Height;
			}
			break;
		}
		// more
		}
		break;
	}
	case 1:
	{
		break;
	}
	case 2:
	{
		break;
	}
	case 3:
	{
		break;
	}
	case 4:
	{
		break;
	}
	case 5:
	{
		break;
	}
	case 6:
	{
		break;
	}
	case 7:
	{
		break;
	}
	}

	// create ROI
	matROI = cv::Mat(rectROI.height, rectROI.width, CV_8UC1);
	for (int i = 0; i < matROI.rows; i++)
		memcpy(&matROI.data[i * matROI.step1()], &matGray.data[(i + rectROI.y) * matGray.step1() + rectROI.x], matROI.cols);

	SaveROIImage(matROI, nCamIdx, nROIIdx);

	cv::rectangle(matBGR, rectROI, PINK_COLOR, 1, cv::LINE_AA);

	m_pMat = matSrc;
	m_pMatROI = matROI;
	m_pRectROI = rectROI;

	m_pInterface->SetResultBuffer(nCamIdx, 0, matBGR.data);
}

void CNVisionInspectCore::MakeROI_FakeCam(LPBYTE pBuffer)
{
	cv::Mat matGray, matBGR, matROI;
	cv::Rect rectROI;

	int nWidth = m_pInterface->GetFakeCameraSettingControl()->m_nFrameWidth;
	int nHeight = m_pInterface->GetFakeCameraSettingControl()->m_nFrameHeight;
	cv::Mat matSrc(nHeight, nWidth, CV_8UC3, pBuffer);

	cv::cvtColor(matSrc, matGray, cv::COLOR_BGR2GRAY);
	matSrc.copyTo(matBGR);

	CNVisionInspectRecipe_FakeCam* pRecipeFakeCam = m_pInterface->GetRecipe_FakeCamControl();

	int nROI_X = pRecipeFakeCam->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_ROI_X;
	int nROI_Y = pRecipeFakeCam->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_ROI_Y;
	int nROI_Width = pRecipeFakeCam->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_ROI_Width;
	int nROI_Height = pRecipeFakeCam->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_ROI_Height;

	rectROI.x = nROI_X;
	rectROI.y = nROI_Y;
	rectROI.width = nROI_Width;
	rectROI.height = nROI_Height;

	// create ROI
	matROI = cv::Mat(rectROI.height, rectROI.width, CV_8UC1);
	for (int i = 0; i < matROI.rows; i++)
		memcpy(&matROI.data[i * matROI.step1()], &matGray.data[(i + rectROI.y) * matGray.step1() + rectROI.x], matROI.cols);


	USES_CONVERSION;
	char chROIImageFakeCamPath[1000] = {};
	sprintf_s(chROIImageFakeCamPath, "%s%s_%s%s", W2A(m_pInterface->GetFakeCameraSettingControl()->m_sROIsPath), "ROI", "FakeCam", ".png");

	CString strFilePath(chROIImageFakeCamPath);
	SaveROIImage(matROI, strFilePath);

	cv::rectangle(matBGR, rectROI, PINK_COLOR, 1, cv::LINE_AA);

	matSrc.copyTo(m_pMat);
	matROI.copyTo(m_pMatROI);
	m_pRectROI = rectROI;

	m_pInterface->SetResultBuffer_FakeCam(0, matBGR.data);
}

BOOL CNVisionInspectCore::Algorithm_CountPixel()
{
	if (m_pMat.empty())
		return FALSE;

	if (m_pMatROI.empty())
		return FALSE;

	cv::Mat matBGR;
	m_pMat.copyTo(matBGR);

	int nMinThreshold = m_pInterface->GetRecipe_FakeCamControl()->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_GrayThreshold_Min;
	int nMaxThreshold = m_pInterface->GetRecipe_FakeCamControl()->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_GrayThreshold_Max;
	int nMinNumberOfPixel = m_pInterface->GetRecipe_FakeCamControl()->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_PixelCount_Min;
	int nMaxNumberOfPixel = m_pInterface->GetRecipe_FakeCamControl()->m_NVisionInspectRecipe_CountPixel.m_nCountPixel_PixelCount_Max;

	int count = 0;
	for (int row = 0; row < m_pMatROI.rows; row++)
	{
		for (int col = 0; col < m_pMatROI.cols; col++)
		{
			int gray = m_pMatROI.at<uchar>(row, col);
			if (gray >= nMinThreshold && gray <= nMaxThreshold)
			{
				matBGR.at<cv::Vec3b>(row + m_pRectROI.y, col + m_pRectROI.x)[0] = 0;
				matBGR.at<cv::Vec3b>(row + m_pRectROI.y, col + m_pRectROI.x)[1] = 128;
				matBGR.at<cv::Vec3b>(row + m_pRectROI.y, col + m_pRectROI.x)[2] = 255;
				count++;
			}
		}
	}
	// NG
	if (count < nMinNumberOfPixel || count > nMaxNumberOfPixel)
	{
		cv::rectangle(matBGR, m_pRectROI, RED_COLOR, 2, cv::LINE_AA);

		char chTemp[256] = {};
		sprintf_s(chTemp, "%i", count);
		cv::putText(matBGR, chTemp, cv::Point(m_pRectROI.x + m_pRectROI.width / 2 - 10, m_pRectROI.y + m_pRectROI.height / 2 + 10), cv::FONT_HERSHEY_PLAIN, 1.5, RED_COLOR, 2, cv::LINE_AA);

		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_bInspectCompleted = TRUE;
		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_bResultStatus = FALSE;
		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_fNumberOfPixel = count;

		m_pInterface->SetResultBuffer_FakeCam(0, matBGR.data);
		m_pInterface->InspectComplete_FakeCam(InspectTool_CountPixel);

		return TRUE;
	}
	// OK
	else if (count > nMinNumberOfPixel && count < nMaxNumberOfPixel)
	{
		cv::rectangle(matBGR, m_pRectROI, GREEN_COLOR, 2, cv::LINE_AA);

		char chTemp[256] = {};
		sprintf_s(chTemp, "%i", count);
		cv::putText(matBGR, chTemp, cv::Point(m_pRectROI.x + m_pRectROI.width / 2 - 10, m_pRectROI.y + m_pRectROI.height / 2 + 10), cv::FONT_HERSHEY_PLAIN, 1.5, GREEN_COLOR, 2, cv::LINE_AA);

		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_bInspectCompleted = TRUE;
		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_bResultStatus = TRUE;
		m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResCntPxl.m_fNumberOfPixel = count;

		m_pInterface->SetResultBuffer_FakeCam(0, matBGR.data);
		m_pInterface->InspectComplete_FakeCam(InspectTool_CountPixel);

		return TRUE;
	}
}

BOOL CNVisionInspectCore::Algorithm_Decode()
{
	if (m_pMat.empty())
		return FALSE;

	if (m_pMatROI.empty())
		return FALSE;

	if (m_pRectROI.empty())
		return FALSE;

	int nFrame = 0;

	cv::Mat matBGR;
	m_pMat.copyTo(matBGR);

	BOOL bRet = FALSE;
	CString csRet;

	CNVisionInspectResult_Decode* pDecodeRes = &m_pInterface->GetResult_FakeCamControl()->m_NVisonInspectResDecode;

	auto barcodes = ReadBarcodes(m_pMatROI);

	if (!barcodes.empty())
	{
		if (barcodes.size() == m_pInterface->GetRecipe_FakeCamControl()->m_NVisionInspectRecipe_Decode.m_nMaxCodeCount)
		{
			bRet = TRUE;
		}
		else
		{
			bRet = FALSE;
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matBGR, m_pRectROI, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}
	else
	{
		bRet = FALSE;
	}

	if (bRet == TRUE)
	{
		cv::rectangle(matBGR, m_pRectROI, GREEN_COLOR, 2, cv::LINE_AA);

		pDecodeRes->m_bInspectCompleted = TRUE;
		pDecodeRes->m_bResultStatus = TRUE;
		ZeroMemory(pDecodeRes->m_sResultString, sizeof(pDecodeRes->m_sResultString));
		wsprintf(pDecodeRes->m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);
	}
	else
	{
		cv::rectangle(matBGR, m_pRectROI, RED_COLOR, 2, cv::LINE_AA);

		pDecodeRes->m_bInspectCompleted = TRUE;
		pDecodeRes->m_bResultStatus = FALSE;
	}

	m_pInterface->SetResultBuffer_FakeCam(nFrame, matBGR.data);
	m_pInterface->InspectComplete_FakeCam(InspectTool_Decode);

	return bRet;
}

#pragma region Functions handle Frame Cam
void CNVisionInspectCore::ProcessFrame(int nCamIdx, LPBYTE pBuffer)
{
	switch (nCamIdx)
	{
	case 0: ProcessFrame_Cam1(pBuffer); break;
	case 1: ProcessFrame_Cam2(pBuffer); break;
	case 2: ProcessFrame_Cam3(pBuffer); break;
	case 3: ProcessFrame_Cam4(pBuffer); break;
	case 4: ProcessFrame_Cam5(pBuffer); break;
	case 5: ProcessFrame_Cam6(pBuffer); break;
	case 6: ProcessFrame_Cam7(pBuffer); break;
	case 7: ProcessFrame_Cam8(pBuffer); break;
	}
}
void CNVisionInspectCore::ProcessFrame_Cam1(LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	int nCamIdx = 0;
	int nFrame = 0;

	CNVisionInspectCameraSetting* pCamSetting = m_pInterface->GetCameraSettingControl(nCamIdx);
	CNVisionInspectStatus* pStatus = m_pInterface->GetStatusControl(nCamIdx);
	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();
	CNVisionInspectResult* pResult = m_pInterface->GetResultControl();

	cv::Mat matSrc(pCamSetting->m_nFrameHeight, pCamSetting->m_nFrameWidth, CV_8UC1);
	cv::Mat matResult(pCamSetting->m_nFrameHeight, pCamSetting->m_nFrameWidth, CV_8UC3);

	DWORD dFrameSize = pCamSetting->m_nFrameHeight * pCamSetting->m_nFrameWidth * pCamSetting->m_nChannels;
	memcpy(matSrc.data, (LPBYTE)pBuffer, dFrameSize);

	/*char pathSaveImage[200] = { };
	sprintf_s(pathSaveImage, "%s%s_%d.bmp", "D:\\entry\\NCore\\NProjects\\ReadCodeMachine\\bin\\SaveImages\\", "Code_", nNextFrameIdx);
	cv::imwrite(pathSaveImage, matSrc);*/

	cv::cvtColor(matSrc, matResult, cv::COLOR_GRAY2BGR);

	BOOL bRet = FALSE;
	CString csRet;

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		if (barcodes.size() == pRecipe->m_NVisionInspRecipe_Cam1.m_nMaxCodeCount)
		{
			bRet = TRUE;
		}
		else
		{
			bRet = FALSE;
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, m_pRectROI, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	m_pInterface->SetResultBuffer(nCamIdx, nFrame, matResult.data); // cause the buffer just have a frame should be frame index = 0

	pResult->m_NVisionInspRes_Cam1.m_bInspectCompleted = TRUE;
	pResult->m_NVisionInspRes_Cam1.m_bResultStatus = bRet;
	ZeroMemory(pResult->m_NVisionInspRes_Cam1.m_sResultString, sizeof(pResult->m_NVisionInspRes_Cam1.m_sResultString));
	wsprintf(pResult->m_NVisionInspRes_Cam1.m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);

	if (pStatus->GetInspectRunning() == TRUE)
		m_pInterface->InspectComplete(nCamIdx, FALSE);
	else
		m_pInterface->InspectComplete(nCamIdx, TRUE);
}
void CNVisionInspectCore::ProcessFrame_Cam2(LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	int nCamIdx = 1;
	int nFrame = 0;

	CNVisionInspectCameraSetting* pCamSetting = m_pInterface->GetCameraSettingControl(nCamIdx);
	CNVisionInspectStatus* pStatus = m_pInterface->GetStatusControl(nCamIdx);
	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();
	CNVisionInspectResult* pResult = m_pInterface->GetResultControl();

	cv::Mat matSrc(pCamSetting->m_nFrameHeight, pCamSetting->m_nFrameWidth, CV_8UC1);
	cv::Mat matResult(pCamSetting->m_nFrameHeight, pCamSetting->m_nFrameWidth, CV_8UC3);

	DWORD dFrameSize = pCamSetting->m_nFrameHeight * pCamSetting->m_nFrameWidth * pCamSetting->m_nChannels;
	memcpy(matSrc.data, (LPBYTE)pBuffer, dFrameSize);

	cv::cvtColor(matSrc, matResult, cv::COLOR_GRAY2BGR);

	BOOL bRet = FALSE;
	CString csRet;

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		if (barcodes.size() == pRecipe->m_NVisionInspRecipe_Cam2.m_nMaxCodeCount)
		{
			bRet = TRUE;
		}
		else
		{
			bRet = FALSE;
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, m_pRectROI, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	/*char pathSaveImage[200] = { };
	sprintf_s(pathSaveImage, "%s%s_%d.bmp", "D:\\entry\\NCore\\NProjects\\NVisionInspect\\bin\\VisionSettings\\SaveImage\\FullImage\\Cam2\\", "ImageBasler", nCamIdx);
	cv::imwrite(pathSaveImage, matResult);*/

	m_pInterface->SetResultBuffer(nCamIdx, nFrame, matResult.data); // cause the buffer just have a frame should be frame index = 0

	pResult->m_NVisionInspRes_Cam2.m_bInspectCompleted = TRUE;
	pResult->m_NVisionInspRes_Cam2.m_bResultStatus = bRet;
	ZeroMemory(pResult->m_NVisionInspRes_Cam2.m_sResultString, sizeof(pResult->m_NVisionInspRes_Cam2.m_sResultString));
	wsprintf(pResult->m_NVisionInspRes_Cam2.m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);

	if (pStatus->GetInspectRunning() == TRUE)
		m_pInterface->InspectComplete(nCamIdx, FALSE);
	else
		m_pInterface->InspectComplete(nCamIdx, TRUE);
}
void CNVisionInspectCore::ProcessFrame_Cam3(LPBYTE pBuffer)
{
}
void CNVisionInspectCore::ProcessFrame_Cam4(LPBYTE pBuffer)
{
}
void CNVisionInspectCore::ProcessFrame_Cam5(LPBYTE pBuffer)
{
}
void CNVisionInspectCore::ProcessFrame_Cam6(LPBYTE pBuffer)
{
}
void CNVisionInspectCore::ProcessFrame_Cam7(LPBYTE pBuffer)
{
}
void CNVisionInspectCore::ProcessFrame_Cam8(LPBYTE pBuffer)
{
}
#pragma endregion

void CNVisionInspectCore::SaveTemplateImage(cv::Mat& matTemplate, int nCamIdx)
{
	if (matTemplate.empty())
		return;

	USES_CONVERSION;
	char chImgTemplatePath[1000] = {};
	sprintf_s(chImgTemplatePath, "%s%s", W2A(m_pInterface->GetCameraSettingControl(nCamIdx)->m_sTemplateImagePath), "template.png");

	cv::imwrite(chImgTemplatePath, matTemplate);
}

void CNVisionInspectCore::SaveROIImage(cv::Mat& matROI, int nCamIdx, int nROIIdx)
{
	if (matROI.empty())
		return;

	USES_CONVERSION;
	char chROIImagePath[1000] = {};
	sprintf_s(chROIImagePath, "%s%s_%d%s", W2A(m_pInterface->GetCameraSettingControl(nCamIdx)->m_sROIsPath), "ROI", nROIIdx, ".png");

	cv::imwrite(chROIImagePath, matROI);
}

void CNVisionInspectCore::SaveROIImage(cv::Mat& matROI, CString strFilePath)
{
	if (matROI.empty())
		return;

	USES_CONVERSION;
	char chROIImageFakeCamPath[1000] = {};
	sprintf_s(chROIImageFakeCamPath, "%s", W2A(strFilePath));

	cv::imwrite(chROIImageFakeCamPath, matROI);
}
