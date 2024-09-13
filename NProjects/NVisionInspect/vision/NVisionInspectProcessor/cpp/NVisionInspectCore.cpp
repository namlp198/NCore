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

void CNVisionInspectCore::Inspect_Simulation(int nCamIdx, int nBuff, int nFrame)
{
	LPBYTE pBuff = m_pInterface->GetSimulatorBuffer(nBuff, nFrame);

	if (pBuff == NULL)
		return;

	cv::Mat matSrc(FRAME_HEIGHT, FRAME_WIDTH, CV_8UC3);
	cv::Mat matResult(FRAME_HEIGHT, FRAME_WIDTH, CV_8UC3);

	DWORD dFrameSize = FRAME_HEIGHT * FRAME_WIDTH * NUMBER_OF_CHANNEL_BGR;
	memcpy(matSrc.data, pBuff, dFrameSize);

	matSrc.copyTo(matResult);

	cv::cvtColor(matSrc, matSrc, cv::COLOR_BGR2GRAY);

	BOOL bRet = FALSE;
	CString csRet;

	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();
	CNVisionInspectResult* pResult = m_pInterface->GetResultControl();

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		switch (nCamIdx)
		{
		case 1:
			if (barcodes.size() == pRecipe->m_NVisionInspRecipe_Cam1.m_nMaxCodeCount)
			{
				bRet = TRUE;
			}
			else
			{
				bRet = FALSE;
			}

			break;
		}
		
		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	m_pInterface->SetResultBuffer(0, 0, matResult.data);

	switch (nCamIdx)
	{
	case 1:
	{
		pResult->m_NVisionInspRes_Cam1.m_bInspectCompleted = TRUE;
		pResult->m_NVisionInspRes_Cam1.m_bResultStatus = bRet;
		ZeroMemory(pResult->m_NVisionInspRes_Cam1.m_sResultString, sizeof(pResult->m_NVisionInspRes_Cam1.m_sResultString));
		wsprintf(pResult->m_NVisionInspRes_Cam1.m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);
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
	case 8:
	{
		break;
	}
	}
	m_pInterface->InspectComplete(nCamIdx, TRUE);
}

void CNVisionInspectCore::Inspect_Reality(int nCamIdx, LPBYTE pBuffer)
{
	ProcessFrame(nCamIdx, pBuffer);
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
	case 1:
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
	case 8:
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
	case 1:
	{
		pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX = ptResult_X;
		pRecipe->m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY = ptResult_Y;
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
	case 8:
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
	case 1:
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
	case 8:
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

	m_pInterface->SetResultBuffer(nCamIdx, 0, matBGR.data);
}

void CNVisionInspectCore::ProcessFrame(int nCamIdx, LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	// Start read code

	cv::Mat matSrc(m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameHeight, m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameWidth, CV_8UC1);
	cv::Mat matResult(m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameHeight, m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameWidth, CV_8UC3);

	DWORD dFrameSize = m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameHeight * m_pInterface->GetCameraSettingControl(nCamIdx)->m_nFrameWidth * m_pInterface->GetCameraSettingControl(nCamIdx)->m_nChannels;
	memcpy(matSrc.data, (LPBYTE)pBuffer, dFrameSize);

	/*char pathSaveImage[200] = { };
	sprintf_s(pathSaveImage, "%s%s_%d.bmp", "D:\\entry\\NCore\\NProjects\\ReadCodeMachine\\bin\\SaveImages\\", "Code_", nNextFrameIdx);
	cv::imwrite(pathSaveImage, matSrc);*/

	cv::cvtColor(matSrc, matResult, cv::COLOR_GRAY2BGR);

	BOOL bRet = FALSE;
	CString csRet;

	CNVisionInspectRecipe* pRecipe = m_pInterface->GetRecipeControl();
	CNVisionInspectResult* pResult = m_pInterface->GetResultControl();

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		switch (nCamIdx)
		{
		case 1:
		{
			if (barcodes.size() == pRecipe->m_NVisionInspRecipe_Cam1.m_nMaxCodeCount)
			{
				bRet = TRUE;
			}
			else
			{
				bRet = FALSE;
			}
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
		case 8:
		{
			break;
		}
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	m_pInterface->SetResultBuffer(nCamIdx, 0, matResult.data); // cause the buffer just have a frame should be frame index = 0

	switch (nCamIdx)
	{
	case 1:
	{
		pResult->m_NVisionInspRes_Cam1.m_bInspectCompleted = TRUE;
		pResult->m_NVisionInspRes_Cam1.m_bResultStatus = bRet;
		ZeroMemory(pResult->m_NVisionInspRes_Cam1.m_sResultString, sizeof(pResult->m_NVisionInspRes_Cam1.m_sResultString));
		wsprintf(pResult->m_NVisionInspRes_Cam1.m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);
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
	case 8:
	{
		break;
	}
	}

	m_pInterface->InspectComplete(nCamIdx, FALSE);
}

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
