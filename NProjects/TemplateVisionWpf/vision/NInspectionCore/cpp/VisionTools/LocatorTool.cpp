#include "pch.h"
#include "LocatorTool.h"

CLocatorTool::CLocatorTool()
{
	m_pImageBuffer = NULL;
}

CLocatorTool::~CLocatorTool()
{
	if (m_pImageBuffer != NULL)
		delete m_pImageBuffer, m_pImageBuffer = NULL;
	if (m_pTemplateImageBuffer != NULL)
		delete m_pTemplateImageBuffer, m_pTemplateImageBuffer = NULL;
}

LPBYTE CLocatorTool::GetImageBuffer()
{
	if (m_pImageBuffer == NULL)
		return nullptr;

	return m_pImageBuffer->GetBufferImage(0);
}

LPBYTE CLocatorTool::GetTemplateImageBuffer()
{
	//CSingleLock localLock(&m_crsGetTemplateImage);
	//localLock.Lock();
	if (m_pTemplateImageBuffer == NULL)
		return nullptr;
	
	return m_pTemplateImageBuffer;
	//localLock.Unlock();
}

LPBYTE CLocatorTool::GetResultImageBuffer()
{
	if (m_matResultImage.empty())
		return nullptr;

	return (LPBYTE)m_matResultImage.data;
}

BOOL CLocatorTool::GetDataTrained_TemplateMatching(CLocatorToolResult* pDataTrained)
{
	pDataTrained->m_nX = m_locResult.m_nX;
	pDataTrained->m_nY = m_locResult.m_nY;
	pDataTrained->m_dMatchingRate = m_locResult.m_dMatchingRate;
	pDataTrained->m_bResult = m_locResult.m_bResult;
	pDataTrained->m_nDelta_x = m_locResult.m_nDelta_x;
	pDataTrained->m_nDelta_y = m_locResult.m_nDelta_y;
	pDataTrained->m_dDif_Angle = m_locResult.m_dDif_Angle;

	return TRUE;
}

BOOL CLocatorTool::SetImageBuffer(LPBYTE pBuff)
{
	if (pBuff == NULL)
		return FALSE;

	m_pImageBuffer->SetFrameImage(0, pBuff);
}

BOOL CLocatorTool::Run()
{
	return NVision_FindLocator_TemplateMatching();
}

BOOL CLocatorTool::Initialize(CameraInfo* pCamInfo)
{
	int width = pCamInfo->m_nFrameWidth;
	int height = pCamInfo->m_nFrameHeight;

	USES_CONVERSION;
	char cSensorType[10] = {};
	sprintf_s(cSensorType, "%s", W2A(pCamInfo->m_csSensorType));


	int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
	int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

	// Buffer
	if (m_pImageBuffer != NULL)
	{
		m_pImageBuffer->DeleteSharedMemory();
		delete m_pImageBuffer;
		m_pImageBuffer = NULL;
	}

	DWORD dwFrameWidth = (DWORD)pCamInfo->m_nFrameWidth;
	DWORD dwFrameHeight = (DWORD)pCamInfo->m_nFrameHeight;
	DWORD dwFrameCount = MAX_FRAME_COUNT;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nFrameChannels;

	m_pImageBuffer = new CSharedMemoryBuffer;
	m_pImageBuffer->SetFrameWidth(dwFrameWidth);
	m_pImageBuffer->SetFrameHeight(dwFrameHeight);
	m_pImageBuffer->SetFrameCount(dwFrameCount);
	m_pImageBuffer->SetFrameSize(dwFrameSize);

	DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

	CString strMemory;
	strMemory.Format(_T("%s_%d"), "Buffer_LocTool", 0);
	m_pImageBuffer->CreateSharedMemory(strMemory, dw64Size);

	return TRUE;
}

BOOL CLocatorTool::SaveImageTemplate(cv::Mat* pSaveImage, CString strFileTitle)
{
	if (pSaveImage == NULL)
		return FALSE;



	return TRUE;
}

void CLocatorTool::DrawAxis(cv::Mat& img, cv::Point p, cv::Point q, cv::Scalar colour,const float scale)
{
	//! [visualization1]
	double angle = atan2((double)p.y - q.y, (double)p.x - q.x); // angle in radians
	double hypotenuse = sqrt((double)(p.y - q.y) * (p.y - q.y) + (p.x - q.x) * (p.x - q.x));

	// Here we lengthen the arrow by a factor of scale
	q.x = (int)(p.x - scale * hypotenuse * cos(angle));
	q.y = (int)(p.y - scale * hypotenuse * sin(angle));
	cv::line(img, p, q, colour, 1, cv::LINE_AA);

	// create the arrow hooks
	p.x = (int)(q.x + 9 * cos(angle + CV_PI / 4));
	p.y = (int)(q.y + 9 * sin(angle + CV_PI / 4));
	cv::line(img, p, q, colour, 1, cv::LINE_AA);

	p.x = (int)(q.x + 9 * cos(angle - CV_PI / 4));
	p.y = (int)(q.y + 9 * sin(angle - CV_PI / 4));
	cv::line(img, p, q, colour, 1, cv::LINE_AA);
	//! [visualization1]
}

void CLocatorTool::DrawCenterPt(cv::Mat& img, cv::Point p, cv::Scalar colour)
{
	int seg = 10;
	int squareEdge = (int)(seg * tan(CV_PI / 4));

	cv::circle(img, p, 3, colour, cv::FILLED);

	cv::Point pt1(p.x + seg, p.y - squareEdge);
	cv::Point pt2(p.x - seg, p.y + squareEdge);
	cv::line(img, pt1, pt2, colour, 1.0, cv::LINE_AA);

	cv::Point pt3(p.x - seg, p.y - squareEdge);
	cv::Point pt4(p.x + seg, p.y + squareEdge);
	cv::line(img, pt3, pt4, colour, 1.0, cv::LINE_AA);
}

BOOL CLocatorTool::NVision_FindLocator_TemplateMatching()
{
	// implement algorithm at here

	// when the algorithm handle is done then assign the result received for the CLocatorToolResult object

	return TRUE;
}

BOOL CLocatorTool::NVision_FindLocator_TemplateMatching_TRAIN(int nCamIdx, BYTE* pBuff, CameraInfo* camInfo, CString templatePath, CRectForTrainLocTool* paramTrainLoc)
{
	/*CString csData;
	csData.Format(_T("rectOutside_X:%d, rectOutside_Y:%d, rectOutsie_Width:%d, rectOutsie_Height:%d\nrectInside_X:%d, rectInside_Y:%d, rectInsie_Width:%d, rectInsie_Height:%d"),
		paramTrainLoc->m_nRectOut_X, paramTrainLoc->m_nRectOut_Y, paramTrainLoc->m_nRectOut_Width, paramTrainLoc->m_nRectOut_Height, paramTrainLoc->m_nRectIn_X, paramTrainLoc->m_nRectIn_Y, paramTrainLoc->m_nRectIn_Width, paramTrainLoc->m_nRectIn_Height);
	AfxMessageBox(csData);*/

	int nFrameWidth = camInfo->m_nFrameWidth;
	int nFrameHeight = camInfo->m_nFrameHeight;
	int channels = camInfo->m_nChannels;

	if (channels == 3)
		m_matResultImage = cv::Mat(nFrameHeight, nFrameWidth, CV_8UC3, pBuff);
	else if (channels == 1)
		m_matResultImage = cv::Mat(nFrameHeight, nFrameWidth, CV_8UC1, pBuff);

	if (m_matResultImage.empty())
		return FALSE;

	cv::Mat matGray;
	cv::cvtColor(m_matResultImage, matGray, cv::COLOR_BGR2GRAY);

	cv::Mat m_matImageROI = cv::Mat(paramTrainLoc->m_nRectOut_Height, paramTrainLoc->m_nRectOut_Width, CV_8UC1);
	for (int i = 0; i < m_matImageROI.rows; i++)
		memcpy(&m_matImageROI.data[i * m_matImageROI.step1()], &matGray.data[(i + (paramTrainLoc->m_nRectOut_Y)) * matGray.step1() + (paramTrainLoc->m_nRectOut_X)], m_matImageROI.cols);

	if (m_matImageROI.empty())
		return FALSE;

	m_matImageTemplate = cv::Mat(paramTrainLoc->m_nRectIn_Height, paramTrainLoc->m_nRectIn_Width, CV_8UC1);
	for (int i = 0; i < m_matImageTemplate.rows; i++)
		memcpy(&m_matImageTemplate.data[i * m_matImageTemplate.step1()], &matGray.data[(i + (paramTrainLoc->m_nRectIn_Y)) * matGray.step1() + (paramTrainLoc->m_nRectIn_X)], m_matImageTemplate.cols);

	if (m_matImageTemplate.empty())
		return FALSE;
	// copy image template to buffer
	/*m_pTemplateImageBuffer = new BYTE[m_matImageTemplate.total()];
	memcpy(m_pTemplateImageBuffer, m_matImageTemplate.data, m_matImageTemplate.total());*/
	SetImageBuffer(pBuff);


	USES_CONVERSION;
	CString strPath;
	strPath.Format(templatePath + _T("\\template_%d.bmp"), nCamIdx);
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strPath));
	cv::imwrite(strTemp, m_matImageTemplate);

	// Template matching
	cv::Mat C = cv::Mat::zeros(m_matImageROI.rows - m_matImageTemplate.rows + 1, m_matImageROI.cols - m_matImageTemplate.cols + 1, CV_32FC1);

	double dMin = 0.0;
	double dMatchingRate = 0.0;
	cv::Point ptLeftTop;
	cv::Point ptFindResult;

	cv::matchTemplate(m_matImageROI, m_matImageTemplate, C, cv::TM_CCOEFF_NORMED);
	cv::minMaxLoc(C, &dMin, &dMatchingRate, NULL, &ptLeftTop);

	ptFindResult.x = float(ptLeftTop.x) + (m_matImageTemplate.cols / 2.0);
	ptFindResult.y = float(ptLeftTop.y) + (m_matImageTemplate.rows / 2.0);
	dMatchingRate = dMatchingRate * 100.0;

	double dMatchingRateLimit = paramTrainLoc->m_dMatchingRateLimit;

	if (dMatchingRate < dMatchingRateLimit)
	{
		m_locResult.m_bResult = FALSE;
		return FALSE;
	}

	m_locResult.m_bResult = TRUE;
	m_locResult.m_nX = ptFindResult.x + paramTrainLoc->m_nRectOut_X;
	m_locResult.m_nY = ptFindResult.y + paramTrainLoc->m_nRectOut_Y;
	m_locResult.m_dMatchingRate = dMatchingRate;
	m_locResult.m_nDelta_x = 0;
	m_locResult.m_nDelta_y = 0;
	m_locResult.m_dDif_Angle = 0.0;

	// draw result
	/*CString csMatchingRate;
	csMatchingRate.Format(_T("Matching Rate: %.3f"), dMatchingRate);
	char strMatchingRate[1024] = {};
	sprintf_s(strMatchingRate, "%s", W2A(csMatchingRate));

	cv::Point cnt(m_locResult.m_nX, m_locResult.m_nY);
	cv::Rect rect(paramTrainLoc->m_nRectOut_X, paramTrainLoc->m_nRectOut_Y, paramTrainLoc->m_nRectOut_Width, paramTrainLoc->m_nRectOut_Height);
	cv::rectangle(m_matResultImage, rect, cv::Scalar(0, 255, 0), 1);
	DrawAxis(m_matResultImage, cnt, cv::Point(cnt.x + 300, cnt.y), cv::Scalar(255, 255, 0));
	DrawAxis(m_matResultImage, cnt, cv::Point(cnt.x, cnt.y + 300), cv::Scalar(255, 255, 0));
	DrawCenterPt(m_matResultImage, cnt, cv::Scalar(0, 255, 255));
	cv::putText(m_matResultImage, strMatchingRate, cv::Point(50, 30), cv::FONT_HERSHEY_SIMPLEX, 1.0, cv::Scalar(0, 0, 255), 2);*/

	m_matImageROI.release();
	/*m_matImageTemplate.release();
	m_matResultImage.release();*/

	return TRUE;
}
