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

	CJigInspectRecipe recipe;
	recipe = *m_pInterface->GetRecipe(nCamIdx);

	// 3. READ RECIPE
	USES_CONVERSION;
	char templateImgPath[1024];
	const char* tempImgName = W2A(recipe.m_sImageTemplate);
	sprintf_s(templateImgPath, "%s%s%s", W2A(m_pInterface->GetCameraConfig(nCamIdx)->m_sImageTemplatePath), "\\", tempImgName);

	double dMatchingRateLimit = recipe.m_dMatchingRate * 100;
	int nCenterX = recipe.m_nCenterX;
	int nCenterY = recipe.m_nCenterY;
	int nROIWidth = recipe.m_nROIWidth;
	int nROIHeight = recipe.m_nROIHeight;
	int nNumberOfArray = recipe.m_nNumberOfArray;


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
#ifdef DRAW_RESULT
		cv::putText(mat, "Can't Find Template", cv::Point(30, 50), cv::FONT_HERSHEY_SIMPLEX, 1.0, cv::Scalar(0, 0, 255), 2, cv::LINE_AA);
		m_pResultImageBuffer_BGR[nCamIdx]->SetFrameImage(0, mat.data);
#endif // DRAW_RESULT

		m_pResultImageBuffer[nCamIdx]->SetFrameImage(0, matGray.data);
		m_pInterface->GetJigInspectResult(nCamIdx)->m_bInspectCompleted = TRUE;
		m_pInterface->GetJigInspectResult(nCamIdx)->m_bResultOKNG = FALSE;
		m_pInterface->GetJigInspectResult(nCamIdx)->m_TemplateMatchingResult.m_bResult = FALSE;
		m_pInterface->InspectComplete();
		return FALSE;
	}

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


	// 8. judge and store result
#ifdef DRAW_RESULT
	cv::rectangle(mat, cv::Rect(ptLeftTop.x, ptLeftTop.y, recipe.m_nRectWidth, recipe.m_nRectHeight), cv::Scalar(255, 0, 0), 2, cv::LINE_AA);
	for (int i = 0; i < nNumberOfArray; i++) {
		cv::Rect rect1(nOriginROI0_X, nOriginROI0_Y, nROIWidth / 5, nROIHeight);
		cv::Rect rect2(nOriginROI1_X, nOriginROI1_Y, nROIWidth / 5, nROIHeight);
		cv::rectangle(mat, rect1, cv::Scalar(0, 255, 0), 2, cv::LINE_AA);
		cv::rectangle(mat, rect2, cv::Scalar(0, 255, 0), 2, cv::LINE_AA);
		nOriginROI0_X += nROIWidth / 5;
		nOriginROI1_X += nROIWidth / 5;
	}
	m_pResultImageBuffer_BGR[nCamIdx]->SetFrameImage(0, mat.data);
#endif // DRAW_RESULT

	m_pResultImageBuffer[nCamIdx]->SetFrameImage(0, matGray.data);
	m_pInterface->GetJigInspectResult(nCamIdx)->m_bInspectCompleted = TRUE;
	m_pInterface->GetJigInspectResult(nCamIdx)->m_bResultOKNG = TRUE;

	// 9. inform inspect completed.
	m_pInterface->InspectComplete();

	return TRUE;
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
	sprintf_s(sImageTemplatePath, "%s%s", W2A(m_pInterface->GetCameraConfig(nCamIdx)->m_sImageTemplatePath),"\\template_0.png");

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
