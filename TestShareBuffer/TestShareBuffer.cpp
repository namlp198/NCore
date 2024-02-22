
#include "pch.h"
#include <iostream>
#include "SharedMemoryBuffer.h"
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <atlconv.h>

#ifdef _DEBUG
#pragma comment(lib, "opencv_core480d.lib")
#pragma comment(lib, "opencv_features2d480d.lib")
#pragma comment(lib, "opencv_flann480d.lib")
#pragma comment(lib, "opencv_highgui480d.lib")
#pragma comment(lib, "opencv_imgcodecs480d.lib")
#pragma comment(lib, "opencv_imgproc480d.lib")
#pragma comment(lib, "opencv_videoio480d.lib")
#pragma comment(lib, "opencv_ml480d.lib")
#pragma comment(lib, "opencv_dnn480d.lib")
#pragma comment(lib, "opencv_ccalib480d.lib")
#pragma comment(lib, "opencv_calib3d480d.lib")
#pragma comment(lib, "opencv_aruco480d.lib")
#else
#pragma comment(lib, "opencv_core480.lib")
#pragma comment(lib, "opencv_features2d480.lib")
#pragma comment(lib, "opencv_flann480.lib")
#pragma comment(lib, "opencv_highgui480.lib")
#pragma comment(lib, "opencv_imgcodecs480.lib")
#pragma comment(lib, "opencv_imgproc480.lib")
#pragma comment(lib, "opencv_videoio480.lib")
#pragma comment(lib, "opencv_ml480.lib")
#pragma comment(lib, "opencv_dnn480.lib")
#pragma comment(lib, "opencv_ccalib480.lib")
#pragma comment(lib, "opencv_calib3d480.lib")
#pragma comment(lib, "opencv_aruco480.lib")

//#pragma comment(lib, "NToolCore_Release64.lib")
#endif

#define MAX_BUFF 1
#define FRAME_WIDTH 512
#define FRAME_HEIGHT 512
#define FRAME_COUNT 1

CSharedMemoryBuffer* m_pImageBuffer[MAX_BUFF];

int main()
{
    // Create buffer
	BOOL bRetValue = FALSE;

	DWORD dwFrameWidth = (DWORD)FRAME_WIDTH;
	DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT;
	DWORD dwFrameCount = 1;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * 3;

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
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000.0));
	CT2CA pszConvertedAnsiString(strLogMessage);
	std::string strStd(pszConvertedAnsiString);
	std::cout << strStd << std::endl;

	CString strImagePath = _T("D:\\entry\\src\\APP_DIP_Cpp\\All_Projects\\ImageTest\\lena.png");

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_COLOR);

	if (pOpenImage.empty())
		return FALSE;

	LPBYTE pBuffer1 = m_pImageBuffer[0]->GetSharedBuffer();
	int nCopyHeight = (dwFrameHeight * dwFrameCount < pOpenImage.rows) ? dwFrameHeight * dwFrameCount : pOpenImage.rows;
	int nCopyWidth = (dwFrameWidth < pOpenImage.cols) ? dwFrameWidth : pOpenImage.cols;
	//ZeroMemory(pBuffer1, dwFrameCount * dwFrameSize);

	/*for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer1 + (i * dwFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);*/

	//memcpy(pBuffer1, pOpenImage.data, dwFrameCount * dwFrameSize);

	//ZeroMemory(m_pImageBuffer[0], dwFrameCount * dwFrameSize);
	m_pImageBuffer[0]->SetFrameImage(0, pOpenImage.data);

	LPBYTE pBuffer2 = m_pImageBuffer[0]->GetBufferImage(0);
	cv::Mat matCopy(dwFrameHeight, dwFrameWidth, CV_8UC3, pBuffer2);
	cv::imshow("lena", matCopy);

	//cv::imshow("lena", pOpenImage);
	cv::waitKey(0);

    return 0;
}
