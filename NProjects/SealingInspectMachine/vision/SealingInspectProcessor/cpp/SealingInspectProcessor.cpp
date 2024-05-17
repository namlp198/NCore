#include "pch.h"
#include "SealingInspectProcessor.h"

CSealingInspectProcessor::CSealingInspectProcessor()
{
	for (int i = 0; i < MAX_IMAGE_BUFFER; i++) {
		if (m_pImageBufferColor_Side[i] != NULL)
			delete m_pImageBufferColor_Side[i], m_pImageBufferColor_Side[i] = NULL;
	}
}

CSealingInspectProcessor::~CSealingInspectProcessor()
{
}

BOOL CSealingInspectProcessor::Initialize()
{
	if (m_pLogView != NULL)
	{
		CString strSaveFilePath;
		strSaveFilePath.Format(_T("%s"), "");

		if (strSaveFilePath.IsEmpty() || strSaveFilePath.GetLength() == 0)
			strSaveFilePath = _T("D:\\SealingInspect_Image");

		CString strLogPath;
		strLogPath.Format(_T("%s\\Log"), strSaveFilePath);
		CString strLogName = _T("ImageInspection");
		m_pLogView->SetLogPath(strLogPath, strLogName, 1000);
	}

	SystemMessage(_T("*********** Start Vision Processor ***********"));

	// Create Image Buffer Color..
	if (CreateBuffer_Color() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// Create Image Buffer Mono..
	if (CreateBuffer_Mono() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	return TRUE;
}

BOOL CSealingInspectProcessor::Destroy()
{
	for (int i = 0; i < MAX_IMAGE_BUFFER; i++) {
		delete m_pImageBufferColor_Side[i], m_pImageBufferColor_Side[i] = NULL;
	}

	return TRUE;
}

BOOL CSealingInspectProcessor::LoadSystemSetting()
{
	return 0;
}

BOOL CSealingInspectProcessor::LoadRecipe()
{
	return 0;
}

LPBYTE CSealingInspectProcessor::GetBufferImage_Color(int nBuff, UINT nY)
{
	if (m_pImageBufferColor_Side[nBuff] == NULL)
		return NULL;

	return m_pImageBufferColor_Side[nBuff]->GetBufferImage(nY);
}

BOOL CSealingInspectProcessor::LoadImageBuffer_Color(int nBuff, CString strFilePath)
{
	if (m_pImageBufferColor_Side[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBufferColor_Side[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBufferColor_Side[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBufferColor_Side[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBufferColor_Side[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_COLOR);

	if (pOpenImage.empty())
		return FALSE;

	m_pImageBufferColor_Side[nBuff]->SetFrameImage(0, pOpenImage.data);

	return TRUE;
}

BOOL CSealingInspectProcessor::CreateBuffer_Color()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameWidth = (DWORD)FRAME_WIDTH_SIDE_CAM;
	DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT_SIDE_CAM;
	DWORD dwFrameCount = 0;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * (DWORD)CHANNEL_COLOR;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_IMAGE_BUFFER; i++)
	{
		if (m_pImageBufferColor_Side[i] != NULL)
		{
			m_pImageBufferColor_Side[i]->DeleteSharedMemory();
			delete m_pImageBufferColor_Side[i];
			m_pImageBufferColor_Side[i] = NULL;
		}

		m_pImageBufferColor_Side[i] = new CSharedMemoryBuffer;

		dwFrameCount = (DWORD)FRAME_COUNT;

		dwTotalFrameCount += dwFrameCount;

		m_pImageBufferColor_Side[i]->SetFrameWidth(dwFrameWidth);
		m_pImageBufferColor_Side[i]->SetFrameHeight(dwFrameHeight);
		m_pImageBufferColor_Side[i]->SetFrameCount(dwFrameCount);
		m_pImageBufferColor_Side[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferOffline_Color", i);

		bRetValue = m_pImageBufferColor_Side[i]->CreateSharedMemory(strMemory, dw64Size);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

LPBYTE CSealingInspectProcessor::GetBufferImage_Mono(int nBuff, UINT nY)
{
	if (m_pImageBufferMono_Side[nBuff] == NULL)
		return NULL;

	return m_pImageBufferMono_Side[nBuff]->GetBufferImage(nY);
}

BOOL CSealingInspectProcessor::LoadImageBuffer_Mono(int nBuff, CString strFilePath)
{
	if (m_pImageBufferMono_Side[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBufferMono_Side[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBufferMono_Side[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBufferMono_Side[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBufferMono_Side[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_GRAYSCALE);

	if (pOpenImage.empty())
		return FALSE;

	if (pOpenImage.type() != CV_8UC1)
		return FALSE;

	LPBYTE pBuffer = m_pImageBufferMono_Side[nBuff]->GetSharedBuffer();

	int nCopyHeight = (nFrameHeight * nFrameCount < pOpenImage.rows) ? nFrameHeight * nFrameCount : pOpenImage.rows;
	int nCopyWidth = (nFrameWidth < pOpenImage.cols) ? nFrameWidth : pOpenImage.cols;

	ZeroMemory(pBuffer, nFrameSize * nFrameCount);

	for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer + (i * nFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);

	return TRUE;
}

BOOL CSealingInspectProcessor::CreateBuffer_Mono()
{
	BOOL bRetValue = FALSE;

	DWORD dwFrameWidth = (DWORD)FRAME_WIDTH_SIDE_CAM;
	DWORD dwFrameHeight = (DWORD)FRAME_HEIGHT_SIDE_CAM;
	DWORD dwFrameCount = 0;
	DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * (DWORD)CHANNEL_MONO;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_IMAGE_BUFFER; i++)
	{
		if (m_pImageBufferMono_Side[i] != NULL)
		{
			m_pImageBufferMono_Side[i]->DeleteSharedMemory();
			delete m_pImageBufferMono_Side[i];
			m_pImageBufferMono_Side[i] = NULL;
		}

		m_pImageBufferMono_Side[i] = new CSharedMemoryBuffer;

		dwFrameCount = (DWORD)FRAME_COUNT;

		dwTotalFrameCount += dwFrameCount;

		m_pImageBufferMono_Side[i]->SetFrameWidth(dwFrameWidth);
		m_pImageBufferMono_Side[i]->SetFrameHeight(dwFrameHeight);
		m_pImageBufferMono_Side[i]->SetFrameCount(dwFrameCount);
		m_pImageBufferMono_Side[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), "BufferOffline_Mono", i);

		bRetValue = m_pImageBufferMono_Side[i]->CreateSharedMemory(strMemory, dw64Size);

		if (bRetValue == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth, (int)dwFrameHeight, (int)dwFrameCount, (((double)(dwFrameSize * dwFrameCount)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

BOOL CSealingInspectProcessor::ClearBufferImage(int nBuff)
{
	if (m_pImageBufferColor_Side[nBuff] == NULL)
		return FALSE;

	BOOL nRet = FALSE;
	nRet = m_pImageBufferColor_Side[nBuff]->ClearBufferImage();
	nRet &= m_pImageBufferMono_Side[nBuff]->ClearBufferImage();

	return nRet;
}

void CSealingInspectProcessor::RegCallbackLogFunc(CallbackLogFunc* pFunc)
{
	m_pCallbackLogFunc = pFunc;
}

void CSealingInspectProcessor::LogMessage(char* strMessage)
{
	if (strMessage == NULL)
		return;

	CString strLogMessage = (CString)strMessage;

	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strLogMessage);
}

void CSealingInspectProcessor::LogMessage(CString strMessage)
{
	if (m_pLogView != NULL)
		m_pLogView->DisplayMessage(strMessage);
}

void CSealingInspectProcessor::ShowLogView(BOOL bShow)
{
	if (m_pLogView == NULL)
		return;

#ifndef _DEBUG
	m_pLogView->ShowMode(bShow);
#endif // !_DEBUG
}

void CSealingInspectProcessor::SystemMessage(CString strMessage)
{
	if (m_pCallbackLogFunc == NULL)
	{
		USES_CONVERSION;
		char strMsgBuffer[1024] = {};
		sprintf_s(strMsgBuffer, "%s", W2A(strMessage));

		LogMessage(strMsgBuffer);

		printf("%s\r\n", strMsgBuffer);
	}
	else
	{
		USES_CONVERSION;
		char strMsgBuffer[1024] = {};
		sprintf_s(strMsgBuffer, "%s", W2A(strMessage));

		m_pCallbackLogFunc(strMsgBuffer);

		printf("%s\r\n", strMsgBuffer);
	}
}
