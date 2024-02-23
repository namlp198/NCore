#include "pch.h"
#include "ImageProcessor.h"
#include "SharedMemoryBuffer.h"

ImageProcessor::ImageProcessor()
{
	for (int i = 0; i < MAX_BUFF; i++)
	{
		m_pImageBuffer[i] = NULL;
	}
}

ImageProcessor::~ImageProcessor()
{
	Destroy();
}

BOOL ImageProcessor::FindLineWithHoughLine_Simul(int top, int left, int width, int height, int nBuff)
{
	// Get buffer
	LPBYTE pBuffer = GetBufferImage(nBuff, 0);

	int frameWidth = m_pImageBuffer[nBuff]->GetFrameWidth();
	int frameHeight = m_pImageBuffer[nBuff]->GetFrameHeight();
	cv::Mat mat(frameHeight, frameWidth, CV_8UC1, pBuffer);

    // Find line
    FindLineTool finder;
    finder.FindLineWithHoughLine(&mat, top, left, width, height, vPoints);

	// Set Frame into buffer
	/*LPBYTE pBuffer = m_pImageBuffer[nBuff]->GetSharedBuffer();
	ZeroMemory(pBuffer, frameWidth * frameHeight);
	for (int i = 0; i < frameHeight; i++)
		memcpy(pBuffer + (i * frameWidth), &mat.data[i * mat.step1()], frameWidth);
	
	m_pImageBuffer[nBuff]->SetFrameImage(0, pBuffer);*/
	return TRUE;
}

LPBYTE ImageProcessor::GetBufferImage(int nBuff, UINT nY)
{
    if (m_pImageBuffer[nBuff] == NULL)
        return NULL;

    return m_pImageBuffer[nBuff]->GetBufferImage(nY);
}

BOOL ImageProcessor::LoadImageBuffer(int nBuff, CString strFilePath)
{
	if (m_pImageBuffer[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBuffer[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_GRAYSCALE);

	if (pOpenImage.empty())
		return FALSE;

	if (pOpenImage.type() != CV_8UC1)
		return FALSE;

	LPBYTE pBuffer = m_pImageBuffer[nBuff]->GetSharedBuffer();

	int nCopyHeight = (nFrameHeight * nFrameCount < pOpenImage.rows) ? nFrameHeight * nFrameCount : pOpenImage.rows;
	int nCopyWidth = (nFrameWidth < pOpenImage.cols) ? nFrameWidth : pOpenImage.cols;

	ZeroMemory(pBuffer, nFrameSize * nFrameCount);

	for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer + (i * nFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);

	return TRUE;
}

BOOL ImageProcessor::CreateBuffer()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameWidth = (DWORD)FRAME_WIDTH;
	DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT;
	DWORD dwFrameCount = 0;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_BUFF; i++)
	{
		if (m_pImageBuffer[i] != NULL)
		{
			m_pImageBuffer[i]->DeleteSharedMemory();
			delete m_pImageBuffer[i];
			m_pImageBuffer[i] = NULL;
		}

		m_pImageBuffer[i] = new CSharedMemoryBuffer;

		dwFrameCount = (DWORD)FRAME_COUNT;

		dwTotalFrameCount += dwFrameCount;

		m_pImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferOffline", i);

		bRetValue = m_pImageBuffer[i]->CreateSharedMemory(strMemory, dw64Size);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f GB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000000.0));

	return TRUE;
}

BOOL ImageProcessor::ClearBufferImage(int nBuff)
{
	if (m_pImageBuffer[nBuff] == NULL)
		return FALSE;

	return m_pImageBuffer[nBuff]->ClearBufferImage();
}

BOOL ImageProcessor::Initialize()
{
	// Create Image Buffer..
	if (CreateBuffer() == FALSE)
	{
		return FALSE;
	}

	// Inspection Hik Cam
	/*if (m_pInspHikCam != NULL)
	{
		m_pInspHikCam->Destroy();
		delete m_pInspHikCam, m_pInspHikCam = NULL;
	}
	m_pInspHikCam = new InspectionHikCam;
	m_pInspHikCam->Initialize();*/

	// Inspection Basler Cam
	/*if (m_pInspBaslerCam != NULL)
	{
		m_pInspBaslerCam->Destroy();
		delete m_pInspBaslerCam, m_pInspBaslerCam = NULL;
	}
	m_pInspBaslerCam = new CInspectionBaslerCam;
	m_pInspBaslerCam->Initialize();*/

	// Inspection Basler Camera New
	if (m_pInspBaslerCam_New != NULL)
	{
		m_pInspBaslerCam_New->Destroy();
		delete m_pInspBaslerCam_New, m_pInspBaslerCam_New = NULL;
	}
	m_pInspBaslerCam_New = new CInspectionBaslerCam_New;
	m_pInspBaslerCam_New->Initialize();

	return TRUE;
}

BOOL ImageProcessor::Destroy()
{
	for (int i = 0; i < MAX_BUFF; i++)
	{
		if (m_pImageBuffer[i] != NULL)
		{
			m_pImageBuffer[i]->DeleteSharedMemory();
			delete m_pImageBuffer[i];
			m_pImageBuffer[i] = NULL;
		}
	}

	if (m_pInspHikCam != NULL)
	{
		m_pInspHikCam->Destroy();
		delete m_pInspHikCam;
		m_pInspHikCam = NULL;
	}

	if (m_pInspBaslerCam != NULL)
	{
		m_pInspBaslerCam->Destroy();
		delete m_pInspBaslerCam;
		m_pInspBaslerCam = NULL;
	}

	if (m_pInspBaslerCam_New != NULL)
	{
		m_pInspBaslerCam_New->Destroy();
		delete m_pInspBaslerCam_New;
		m_pInspBaslerCam_New = NULL;
	}

	return TRUE;
}

BOOL ImageProcessor::GetInspectData(InspectResult* pInspectData)
{
	pInspectData->m_nX1 = vPoints[0].x;
	pInspectData->m_nY1 = vPoints[0].y;
	pInspectData->m_nX2 = vPoints[1].x;
	pInspectData->m_nY2 = vPoints[1].y;
	return TRUE;
}

