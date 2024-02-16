#include "pch.h"
#include "ImageProcessor.h"

ImageProcessor::ImageProcessor()
{
}

ImageProcessor::~ImageProcessor()
{
}

void ImageProcessor::FindLineWithHoughLine_Offline(std::string pathImg, cv::Rect rectROI)
{
    //cv::namedWindow("source", cv::WINDOW_AUTOSIZE);
    cv::Mat mat = cv::imread(pathImg, cv::IMREAD_COLOR);
    if (mat.empty())
    {
        std::cout << "Load image error" << std::endl;
        return;
    }

    // Find line
    FindLineTool finder;
    std::vector<cv::Point2f> vPoints;
    finder.FindLineWithHoughLine(&mat, rectROI, vPoints);
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

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")))
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer[nBuff]->GetFrameCount();

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

	ZeroMemory(pBuffer, nFrameWidth * nFrameHeight * nFrameCount);

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
		strMemory.Format(_T("%s_%d"), "Buffer", i);

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

BOOL ImageProcessor::Initialize()
{
	// Create Image Buffer..
	if (CreateBuffer() == FALSE)
	{
		return FALSE;
	}
}
