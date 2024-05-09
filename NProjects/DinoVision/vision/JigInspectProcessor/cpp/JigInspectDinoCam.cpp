#include "pch.h"
#include "JigInspectDinoCam.h"

CJigInspectDinoCam::CJigInspectDinoCam(IJigInspectDinoCamToParent* pInterface)
{
	m_pInterface = pInterface;

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pUsbCamera[i] = NULL;
		m_pCameraCurrentFrameIdx[i] = 0;
		m_pUsbCamera[i] = NULL;
	}
}

CJigInspectDinoCam::~CJigInspectDinoCam()
{
	Destroy();
}

BOOL CJigInspectDinoCam::Initialize()
{
	int deviceCount = EnumerateDevices();
	if (deviceCount == 0)
		return FALSE;
	int devices = deviceCount < MAX_CAMERA_INSP_COUNT ? deviceCount : MAX_CAMERA_INSP_COUNT;

	for (int i = 0; i < deviceCount; i++)
	{
		int nChannels = m_pInterface->GetCameraConfig(i)->m_nChannels;
		DWORD dwFrameWidth = (DWORD)m_pInterface->GetCameraConfig(i)->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pInterface->GetCameraConfig(i)->m_nFrameHeight;
		DWORD dwFrameCount = MAX_BUFFER_FRAME;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nChannels;

		// Camera
		m_pUsbCamera[i] = new CFramGrabber_UsbCam(i);
		m_pUsbCamera[i]->SetFrameWidth(dwFrameWidth);
		m_pUsbCamera[i]->SetFrameHeight(dwFrameHeight);
		m_pUsbCamera[i]->SetFrameSize(dwFrameSize);
		m_pUsbCamera[i]->SetFrameCount(dwFrameCount);
		m_pUsbCamera[i]->SetChannels(nChannels);
		m_pUsbCamera[i]->SetId(m_vIdDevices[i]);

		m_pUsbCamera[i]->Initialize();

		if (m_pUsbCamera[i]->Connect())
		{
			m_pUsbCamera[i]->SetId(m_mapDevices[i].id);
			m_pUsbCamera[i]->SetDeviceName(m_mapDevices[i].deviceName);
			m_pUsbCamera[i]->SetDevicePath(m_mapDevices[i].devicePath);
			m_pUsbCamera[i]->SingleGrab();
		}
		CreateResultBuffer(i, m_pUsbCamera[i]);

		//m_pUsbCamera[i]->Disconnect();
	}

	return TRUE;
}

BOOL CJigInspectDinoCam::Destroy()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pUsbCamera[i] != NULL)
		{
			m_pUsbCamera[i]->StopGrab();
			Sleep(500);
			m_pUsbCamera[i]->Disconnect();
			delete m_pUsbCamera[i], m_pUsbCamera[i] = NULL;
		}
	}

	return TRUE;
}

int CJigInspectDinoCam::EnumerateDevices()
{
	DeviceEnumerator de;

	m_mapDevices = de.getVideoDevicesMap();
	for (auto const& device : m_mapDevices)
	{
		m_vIdDevices.push_back(device.first);
	}
	return m_vIdDevices.size();
}

LPBYTE CJigInspectDinoCam::GetBufferImage(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return NULL;

	CSingleLock localLock(&m_csCameraFrameIdx[nCamIdx]);
	localLock.Lock();

	int nCurrentFrameIdx = m_pCameraCurrentFrameIdx[nCamIdx];

	localLock.Unlock();

	return m_pUsbCamera[nCamIdx]->GetBufferImage();
}

LPBYTE CJigInspectDinoCam::GetResultBufferImage(int nCamIdx)
{
	if (m_pResultImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pResultImageBuffer[nCamIdx]->GetBufferImage(0);
}

LPBYTE CJigInspectDinoCam::GetResultBufferImage_BGR(int nCamIdx)
{
	if (m_pResultImageBuffer_BGR[nCamIdx] == NULL)
		return NULL;

	return m_pResultImageBuffer_BGR[nCamIdx]->GetBufferImage(0);
}


int CJigInspectDinoCam::StartGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;
	m_pUsbCamera[nCamIdx]->StartGrab();

	return 1;
}

int CJigInspectDinoCam::StopGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->StopGrab();

	return 1;
}

int CJigInspectDinoCam::SingleGrab(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->SingleGrab();

	return 1;
}

int CJigInspectDinoCam::Connect(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->Connect();

	return 1;
}

int CJigInspectDinoCam::Disconnect(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	m_pUsbCamera[nCamIdx]->Disconnect();

	return 1;
}

BOOL CJigInspectDinoCam::CreateResultBuffer(int nCamIdx, CFramGrabber_UsbCam* pUsbCam)
{
	BOOL bRetValue = FALSE;

	if (m_pResultImageBuffer[nCamIdx] != NULL)
	{
		m_pResultImageBuffer[nCamIdx]->DeleteSharedMemory();
		delete m_pResultImageBuffer[nCamIdx];
		m_pResultImageBuffer[nCamIdx] = NULL;
	}
	m_pResultImageBuffer[nCamIdx] = new CSharedMemoryBuffer;

	// save gray image should be just need to a channel for save image.
	DWORD frameSize = pUsbCam->GetFrameWidth() * pUsbCam->GetFrameHeight() * 1;
	m_pResultImageBuffer[nCamIdx]->SetFrameWidth(pUsbCam->GetFrameWidth());
	m_pResultImageBuffer[nCamIdx]->SetFrameHeight(pUsbCam->GetFrameHeight());
	m_pResultImageBuffer[nCamIdx]->SetFrameCount(pUsbCam->GetFramCount());
	m_pResultImageBuffer[nCamIdx]->SetFrameSize(frameSize);

	DWORD64 dw64Size = (DWORD64)pUsbCam->GetFramCount() * frameSize;

	CString strMemory;
	strMemory.Format(_T("%s_%d"), "Buffer_UsbCam");

	bRetValue = m_pResultImageBuffer[nCamIdx]->CreateSharedMemory(strMemory, dw64Size);

#ifdef DRAW_RESULT
	if (m_pResultImageBuffer_BGR[nCamIdx] != NULL)
	{
		m_pResultImageBuffer_BGR[nCamIdx]->DeleteSharedMemory();
		delete m_pResultImageBuffer_BGR[nCamIdx];
		m_pResultImageBuffer_BGR[nCamIdx] = NULL;
	}

	m_pResultImageBuffer_BGR[nCamIdx] = new CSharedMemoryBuffer;

	// save color image
	DWORD frameSize_BGR = pUsbCam->GetFrameWidth() * pUsbCam->GetFrameHeight() * 3;
	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameWidth(pUsbCam->GetFrameWidth());
	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameHeight(pUsbCam->GetFrameHeight());
	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameCount(pUsbCam->GetFramCount());
	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameSize(frameSize_BGR);

	DWORD64 dw64Size_BGR = (DWORD64)pUsbCam->GetFramCount() * frameSize_BGR;

	CString strMemory_BGR;
	strMemory_BGR.Format(_T("%s_%d"), "Buffer_UsbCam_BGR");

	bRetValue = m_pResultImageBuffer_BGR[nCamIdx]->CreateSharedMemory(strMemory_BGR, dw64Size_BGR);
#endif // DRAW_RESULT

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dw64Size)) / 1000000.0));


	return bRetValue;
}

BOOL CJigInspectDinoCam::InspectStart(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	// 1. grab image
	m_pUsbCamera[nCamIdx]->SingleGrab();

	// 2. get buffer
	LPBYTE buff = m_pUsbCamera[nCamIdx]->GetBufferImage();
	cv::Mat mat(m_pUsbCamera[nCamIdx]->GetFrameHeight(), m_pUsbCamera[nCamIdx]->GetFrameWidth(), CV_8UC3, buff);

	if (mat.empty())
		return FALSE;

	BOOL bRet = FALSE;

	CJigInspectRecipe recipe;
	CJigInspectSystemConfig config;
	CJigInspectCameraConfig camConfig;
	recipe = *m_pInterface->GetRecipe(nCamIdx);
	config = *m_pInterface->GetSystemConfig();
	camConfig = *m_pInterface->GetCameraConfig(nCamIdx);

	// 3. read RECIPE
	USES_CONVERSION;
	char templateImgPath[1024];
	const char* tempImgName = W2A(recipe.m_sImageTemplate);
	sprintf_s(templateImgPath, "%s%s%s", W2A(camConfig.m_sImageTemplatePath), "\\", tempImgName);

	double dMatchingRateLimit = recipe.m_dMatchingRate * 100;
	int nCenterX = recipe.m_nCenterX;
	int nCenterY = recipe.m_nCenterY;
	int nROIWidth = recipe.m_nROIWidth;
	int nROIHeight = recipe.m_nROIHeight;
	int nNumberOfArray = recipe.m_nNumberOfArray;
	int nThresholdWidth_Min = recipe.m_nThresholdWidthMin;
	int nThresholdWidth_Max = recipe.m_nThresholdWidthMax;
	int nThresholdHeight_Min = recipe.m_nThresholdHeightMin;
	int nThresholdHeight_Max = recipe.m_nThresholdHeightMax;

	cv::Mat matGray;
	cv::cvtColor(mat, matGray, cv::COLOR_BGR2GRAY);

	// 4. Template matching

	cv::Mat templateImg = cv::imread(templateImgPath, CV_8UC1);

	cv::Mat C = cv::Mat::zeros(matGray.rows - templateImg.rows + 1, matGray.cols - templateImg.cols + 1, CV_32FC1);

	double dMin = 0.0;
	double dMatchingRate = 0.0;
	cv::Point ptLeftTop;
	cv::Point ptFindResult;

	cv::matchTemplate(matGray, templateImg, C, cv::TM_CCOEFF_NORMED);
	cv::minMaxLoc(C, &dMin, &dMatchingRate, NULL, &ptLeftTop);

	ptFindResult.x = float(ptLeftTop.x) + (templateImg.cols / 2.0);
	ptFindResult.y = float(ptLeftTop.y) + (templateImg.rows / 2.0);
	dMatchingRate = dMatchingRate * 100.0;

	if (dMatchingRate < dMatchingRateLimit)
	{
		bRet = FALSE;
#ifdef DRAW_RESULT
		cv::putText(mat, "Can't Find Template", cv::Point(30, 50), cv::FONT_HERSHEY_SIMPLEX, 1.0, cv::Scalar(0, 0, 255), 2, cv::LINE_AA);
		m_pResultImageBuffer_BGR[nCamIdx]->SetFrameImage(0, mat.data);
#endif // DRAW_RESULT

		m_pResultImageBuffer[nCamIdx]->SetFrameImage(0, matGray.data);
		m_pInterface->GetJigInspectResult(nCamIdx)->m_bInspectCompleted = TRUE;
		m_pInterface->GetJigInspectResult(nCamIdx)->m_bResultOKNG = FALSE;
		m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_bResult = FALSE;
		m_pInterface->InspectComplete();
		return bRet;
	}

	bRet = TRUE;

	// 5. calculate deviations compared to standard center position.
	int nDeltaCenterX = ptFindResult.x - recipe.m_nCenterX;
	int nDeltaCenterY = ptFindResult.y - recipe.m_nCenterY;

	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nLeft = ptLeftTop.x;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nTop = ptLeftTop.y;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nCenterX = ptFindResult.x;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nCenterY = ptFindResult.y;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_dMatchingRate = dMatchingRate;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nDelta_X = nDeltaCenterX;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_nDelta_Y = nDeltaCenterY;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_dDif_Angle = 0.0;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_bResult = TRUE;

	// 6. calculate ROI0 and ROI1
	int nOriginROI0_X = ptFindResult.x - recipe.m_nOffsetROI0_X;
	int nOriginROI0_Y = ptFindResult.y - recipe.m_nOffsetROI0_Y;
	int nOriginROI1_X = ptFindResult.x + recipe.m_nOffsetROI1_X;
	int nOriginROI1_Y = ptFindResult.y - recipe.m_nOffsetROI1_Y;

	// 7. Start inspect ROI
	int nROIWidth_Unit = nROIWidth / nNumberOfArray;
	cv::Mat ROIUnit(nROIHeight, nROIWidth_Unit, CV_8UC1);
	std::map<int, int> mapNGPosition;
	for (int row = 0; row < 1; row++) {
		for (int col = 0; col < NUMBER_OF_ROI * nNumberOfArray; col++) {

			if (col < 5) {
				for (int i = 0; i < ROIUnit.rows; i++) {
					memcpy(&ROIUnit.data[i * ROIUnit.step1()], &matGray.data[(i + nOriginROI0_Y) * matGray.step1() + nOriginROI0_X], ROIUnit.cols);
				}

				bRet &= FindBoundingRect(mat, ROIUnit, nOriginROI0_X, nOriginROI0_Y, row, col, mapNGPosition, recipe, config);

				nOriginROI0_X += nROIWidth_Unit;
			}
			else {
				for (int i = 0; i < ROIUnit.rows; i++) {
					memcpy(&ROIUnit.data[i * ROIUnit.step1()], &matGray.data[(i + nOriginROI1_Y) * matGray.step1() + nOriginROI1_X], ROIUnit.cols);
				}

				bRet &= FindBoundingRect(mat, ROIUnit, nOriginROI1_X, nOriginROI1_Y, row, col, mapNGPosition, recipe, config);

				nOriginROI1_X += nROIWidth_Unit;
			}

#ifdef SAVE_IMAGE_TEST
			char sImageTemplatePath[1024] = {};
			sprintf_s(sImageTemplatePath, "%s%s%d%s", W2A(camConfig.m_sImageTemplatePath), "\\test_", col + 1, ".png");
			cv::imwrite(sImageTemplatePath, ROIUnit);
#endif // SAVE_IMAGE_TEST

		}
	}

	// 8. judge and store result
#ifdef DRAW_RESULT
	cv::rectangle(mat, cv::Rect(ptLeftTop.x, ptLeftTop.y, recipe.m_nRectWidth, recipe.m_nRectHeight), cv::Scalar(255, 0, 0), 2, cv::LINE_AA);
	cv::circle(mat, ptFindResult, 3, cv::Scalar(255, 0, 255), cv::FILLED);

	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameImage(0, mat.data);
#endif // DRAW_RESULT

	if (config.m_bSaveImage) {
		uint64_t ms = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		char sImageName[1024] = {};
		sprintf_s(sImageName, "%s%s%d%s", W2A(camConfig.m_sImageSavePath), "\\img_", ms, ".png");

		cv::imwrite(sImageName, mat);
	}

	m_pResultImageBuffer[nCamIdx]->SetFrameImage(0, matGray.data);
	m_pInterface->GetJigInspectResult(nCamIdx)->m_bInspectCompleted = TRUE;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_bResultOKNG = bRet;

	// 9. inform inspect completed.
	m_pInterface->InspectComplete();

	return bRet;
}

BOOL CJigInspectDinoCam::FindBoundingRect(cv::Mat& matDraw, cv::Mat& matROIUnit, int nX, int nY,
	int nRowROIUnitPos, int nColROIUnitPos, std::map<int, int>& mapNGPosition, CJigInspectRecipe recipe, CJigInspectSystemConfig config)
{
	// THRESHOLD
	int nWidthMin = recipe.m_nThresholdWidthMin;
	int nWidthMax = recipe.m_nThresholdWidthMax;
	int nHeightMin = recipe.m_nThresholdHeightMin;
	int nHeightMax = recipe.m_nThresholdHeightMax;
	int nKSizeX_Open = recipe.m_nKSizeX_Open;
	int nKSizeY_Open = recipe.m_nKSizeY_Open;
	int nKSizeX_Close = recipe.m_nKSizeX_Close;
	int nKSizeY_Close = recipe.m_nKSizeY_Close;
	int nContourSizeMin = recipe.m_nContourSizeMin;
	int nContourSizeMax = recipe.m_nContourSizeMax;
	int nThreshBinary = recipe.m_nThresholdBinary;

	// pre-processing
	cv::Mat matBGR;
	cv::cvtColor(matROIUnit, matBGR, cv::COLOR_GRAY2BGR);

	cv::Mat matBinary, matSobel, matEle;

	std::vector<cv::Rect>             vecRect;
	std::vector<cv::RotatedRect>      vecRotRect;

#ifdef METHOD01
	cv::Sobel(matROIUnit, matSobel, CV_8U, 0, 1, 3, 1.0, 0.0, cv::BORDER_DEFAULT);
	cv::threshold(matSobel, matBinary, 0, 255, cv::THRESH_OTSU + cv::THRESH_BINARY);
#endif // METHOD01

#ifdef METHOD02
	cv::threshold(matROIUnit, matBinary, nThreshBinary, 255, cv::THRESH_BINARY);

#endif // METHOD02

	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(nKSizeX_Open, nKSizeY_Open));
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_OPEN, matEle);

	matEle = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(nKSizeX_Close, nKSizeY_Close)); // default: KSizeX = 12, KSizeY = 5
	cv::morphologyEx(matBinary, matBinary, cv::MORPH_CLOSE, matEle);

#ifdef SAVE_IMAGE_TEST
	USES_CONVERSION;
	char sImagePreProcessingPath[1024] = {};
	sprintf_s(sImagePreProcessingPath, "%s%s%d%s", W2A(m_pInterface->GetCameraConfig(0)->m_sImageTemplatePath), "\\pre-processing_", nColROIUnitPos + 1, ".png");
	cv::imwrite(sImagePreProcessingPath, matBinary);
#endif // SAVE_IMAGE_TEST

	std::vector<std::vector<cv::Point>> contours;
	cv::findContours(matBinary, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
	std::vector<std::vector<cv::Point>> contours_poly(contours.size());

	BOOL bRet = FALSE;
	BOOL bRetWidth = FALSE;
	BOOL bRetHeight = TRUE;
	for (int i = 0; i < contours.size(); i++)
	{
		if (contours[i].size() > nContourSizeMin && contours[i].size() < nContourSizeMax)
		{
			cv::approxPolyDP(cv::Mat(contours[i]), contours_poly[i], 3, true);

#ifdef _FIND_RECT_ROTATE_ALGORITHM

			cv::Rect approxRect(cv::boundingRect(cv::Mat(contours_poly[i])));

			bRet = (approxRect.width > approxRect.height) ? TRUE : FALSE;

			bRetWidth = (nWidthMin <= approxRect.width) ? TRUE : FALSE;
			bRetWidth &= (approxRect.width <= nWidthMax) ? TRUE : FALSE;
			bRetHeight = (nHeightMin <= approxRect.height) ? TRUE : FALSE;
			bRetHeight &= (approxRect.height <= nHeightMax) ? TRUE : FALSE;

			bRet &= (bRetWidth & bRetHeight);

			vecRect.push_back(approxRect);
			cv::Point p1(approxRect.x + (approxRect.width / 2), approxRect.y + 1);
			cv::Point p2(approxRect.x + (approxRect.width / 2), approxRect.y + approxRect.height - 2);
			cv::Rect rectROI(nX, nY, matROIUnit.cols, matROIUnit.rows);

			char result[100] = {};
			sprintf_s(result, "%d|%d", approxRect.width, approxRect.height);

			if (config.m_bShowDetail) {
				cv::line(matBGR, p1, p2, cv::Scalar(255, 0, 255), 2, cv::LINE_AA);
				cv::line(matDraw, cv::Point(p1.x + nX, p1.y + nY), cv::Point(p2.x + nX, p2.y + nY), cv::Scalar(255, 0, 255), 2, cv::LINE_AA);

				cv::rectangle(matBGR, cv::Rect(approxRect.x, approxRect.y, approxRect.width, approxRect.height), cv::Scalar(0, 255, 255), 1, cv::LINE_8);
				cv::rectangle(matDraw, cv::Rect(approxRect.x + nX, approxRect.y + nY, approxRect.width, approxRect.height), cv::Scalar(0, 255, 255), 1, cv::LINE_8);
			}

			if (bRet == TRUE) {
				cv::rectangle(matDraw, rectROI, cv::Scalar(0, 255, 0), 2, cv::LINE_AA);
				if (config.m_bShowDetail)
					cv::putText(matDraw, result, cv::Point(nX + 2, nY - 5), cv::FONT_HERSHEY_PLAIN, 0.7, cv::Scalar(0, 255, 0), 1, cv::LINE_AA);
			}
			else {
				cv::rectangle(matDraw, rectROI, cv::Scalar(0, 0, 255), 2, cv::LINE_AA);
				if (config.m_bShowDetail)
					cv::putText(matDraw, result, cv::Point(nX + 2, nY - 5), cv::FONT_HERSHEY_PLAIN, 0.7, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
			}

#endif // FIND_RECT_ROTATE_ALGORITHM

#ifdef FIND_RECT_ROTATE_ALGORITHM
			// Find min rect
			cv::RotatedRect rotatedRect = cv::minAreaRect(cv::Mat(contours_poly[i]));

			bRet = (rotatedRect.size.width > rotatedRect.size.height) ? TRUE : FALSE;

			bRetWidth = (nWidthMin <= rotatedRect.size.width) ? TRUE : FALSE;
			bRetWidth &= (rotatedRect.size.width <= nWidthMax) ? TRUE : FALSE;
			bRetHeight = (nHeightMin <= rotatedRect.size.height) ? TRUE : FALSE;
			bRetHeight &= (rotatedRect.size.height <= nHeightMax) ? TRUE : FALSE;

			bRet &= (bRetWidth & bRetHeight);

			vecRotRect.push_back(rotatedRect);


			cv::Point2f vertices2f[4];
			cv::Point vertices[4];
			rotatedRect.points(vertices2f);
			for (int i = 0; i < 4; ++i) {
				vertices[i] = cv::Point(vertices2f[i].x + nX, vertices2f[i].y + nY);
			}

			for (int i = 0; i < 4; i++) {
				cv::line(matDraw, vertices[i], vertices[(i + 1) % 4], cv::Scalar(0, 255, 255), 1);
			}

			cv::Point p1(rotatedRect.center.x, rotatedRect.center.y - rotatedRect.size.height / 2);
			cv::Point p2(rotatedRect.center.x, rotatedRect.center.y + rotatedRect.size.height / 2);
			cv::Rect rectROI(nX, nY, matROIUnit.cols, matROIUnit.rows);

			/*char result[100] = {};
			sprintf_s(result, "%d|%d", rotatedRect.size.width, rotatedRect.size.height);*/

			//cv::line(matBGR, p1, p2, cv::Scalar(255, 0, 255), 2, cv::LINE_AA);
			//cv::line(matDraw, cv::Point(p1.x + nX, p1.y + nY), cv::Point(p2.x + nX, p2.y + nY), cv::Scalar(255, 0, 255), 2, cv::LINE_AA);

			if (bRet == TRUE) {
				cv::rectangle(matDraw, rectROI, cv::Scalar(0, 255, 0), 2, cv::LINE_AA);
				//cv::putText(matDraw, result, cv::Point(nX + 2, nY - 5), cv::FONT_HERSHEY_PLAIN, 0.7, cv::Scalar(0, 255, 0), 1, cv::LINE_AA);
			}
			else {
				cv::rectangle(matDraw, rectROI, cv::Scalar(0, 0, 255), 2, cv::LINE_AA);
				//cv::putText(matDraw, result, cv::Point(nX + 2, nY - 5), cv::FONT_HERSHEY_PLAIN, 0.7, cv::Scalar(0, 0, 255), 1, cv::LINE_AA);
			}
			/*char chTemp[256] = {};
			sprintf_s(chTemp, "angle: %.2f", rotatedRect.angle);
			cv::putText(matCopy, chTemp, cv::Point(vertices[0].x, vertices[0].y + 20), cv::FONT_HERSHEY_PLAIN, 1, cv::Scalar(255, 0, 255), 1, cv::LINE_AA);*/

		}

		}
#endif // FIND_RECT_ROTATE_ALGORITHM
	}
}
#ifdef SAVE_IMAGE_TEST
char sImageResultPath[1024] = {};
sprintf_s(sImageResultPath, "%s%s%d%s", W2A(m_pInterface->GetCameraConfig(0)->m_sImageTemplatePath), "\\result_", nColROIUnitPos + 1, ".png");
cv::imwrite(sImageResultPath, matBGR);
#endif // SAVE_IMAGE_TEST

return bRet;
}

void CJigInspectDinoCam::DrawAxis(cv::Mat& img, cv::Point p, cv::Point q, cv::Scalar colour, const float scale)
{
	//! [visualization1]
	double angle = atan2((double)p.y - q.y, (double)p.x - q.x); // angle in radians
	double hypotenuse = sqrt((double)(p.y - q.y) * (p.y - q.y) + (p.x - q.x) * (p.x - q.x));

	// Here we lengthen the arrow by a factor of scale
	q.x = (int)(p.x - scale * hypotenuse * cos(angle));
	q.y = (int)(p.y - scale * hypotenuse * sin(angle));
	line(img, p, q, colour, 1, cv::LINE_AA);

	// create the arrow hooks
	p.x = (int)(q.x + 9 * cos(angle + CV_PI / 4));
	p.y = (int)(q.y + 9 * sin(angle + CV_PI / 4));
	line(img, p, q, colour, 1, cv::LINE_AA);

	p.x = (int)(q.x + 9 * cos(angle - CV_PI / 4));
	p.y = (int)(q.y + 9 * sin(angle - CV_PI / 4));
	line(img, p, q, colour, 1, cv::LINE_AA);
	//! [visualization1]
}

BOOL CJigInspectDinoCam::GrabImageForLocatorTool(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_INSP_COUNT <= nCamIdx)
		return 0;

	if (m_pUsbCamera[nCamIdx] == NULL)
		return 0;

	// 1. grab image
	m_pUsbCamera[nCamIdx]->SingleGrab();

	// 2. get buffer
	LPBYTE buff = m_pUsbCamera[nCamIdx]->GetBufferImage();
	cv::Mat mat(m_pUsbCamera[nCamIdx]->GetFrameHeight(), m_pUsbCamera[nCamIdx]->GetFrameWidth(), CV_8UC3, buff);

	cv::Mat matGray;
	cv::cvtColor(mat, matGray, cv::COLOR_BGR2GRAY);

	m_pResultImageBuffer[nCamIdx]->SetFrameImage(0, matGray.data);

	return TRUE;
}

BOOL CJigInspectDinoCam::LocatorTrain(int nCamIdx, CJigInspectRecipe* pRecipe)
{
	// 1. get buffer
	LPBYTE buff = m_pResultImageBuffer[nCamIdx]->GetBufferImage(0);
	cv::Mat mat(m_pUsbCamera[nCamIdx]->GetFrameHeight(), m_pUsbCamera[nCamIdx]->GetFrameWidth(), CV_8UC1, buff);

	if (mat.empty())
		return FALSE;

	USES_CONVERSION;
	char sImageGrayPath[1024] = {};
	sprintf_s(sImageGrayPath, "%s%s", W2A(m_pInterface->GetCameraConfig(nCamIdx)->m_sImageSavePath), "\\gray.png");

	//cv::imwrite(sImageGrayPath, mat);

	CJigInspectRecipe recipe;
	recipe = *(pRecipe);

	int nX = recipe.m_nRectX;
	int nY = recipe.m_nRectY;
	int nWidth = recipe.m_nRectWidth;
	int nHeight = recipe.m_nRectHeight;

	// 2. Get image template
	cv::Mat templateImg(nHeight, nWidth, CV_8UC1);
	for (size_t i = 0; i < templateImg.rows; i++)
	{
		memcpy(&templateImg.data[i * templateImg.step1()], &mat.data[(i + nY) * mat.step1() + nX], templateImg.cols);
	}

	char sImageTemplatePath[1024] = {};
	sprintf_s(sImageTemplatePath, "%s%s", W2A(m_pInterface->GetCameraConfig(nCamIdx)->m_sImageTemplatePath), "\\template_0.png");

	cv::imwrite(sImageTemplatePath, templateImg);

	// 3. Find center
	// Template matching
	cv::Mat C = cv::Mat::zeros(mat.rows - templateImg.rows + 1, mat.cols - templateImg.cols + 1, CV_32FC1);

	double dMin = 0.0;
	double dMatchingRate = 0.0;
	cv::Point ptLeftTop;
	cv::Point ptFindResult;

	cv::matchTemplate(mat, templateImg, C, cv::TM_CCOEFF_NORMED);
	cv::minMaxLoc(C, &dMin, &dMatchingRate, NULL, &ptLeftTop);

	ptFindResult.x = float(ptLeftTop.x) + (templateImg.cols / 2.0);
	ptFindResult.y = float(ptLeftTop.y) + (templateImg.rows / 2.0);
	dMatchingRate = dMatchingRate * 100.0;

	double dMatchingRateLimit = recipe.m_dMatchingRate;

	if (dMatchingRate < dMatchingRateLimit)
	{
		return FALSE;
	}

	recipe.m_nCenterX = ptFindResult.x;
	recipe.m_nCenterY = ptFindResult.y;

	// 4. Save result
	*m_pInterface->GetRecipe(nCamIdx) = recipe;


	return TRUE;
}

