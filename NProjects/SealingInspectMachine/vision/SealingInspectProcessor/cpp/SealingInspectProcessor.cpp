#include "pch.h"
#include "SealingInspectProcessor.h"

CSealingInspectProcessor::CSealingInspectProcessor()
{
	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++) {
		if (m_pImageBuffer_Side[i] != NULL)
			delete m_pImageBuffer_Side[i], m_pImageBuffer_Side[i] = NULL;
	}
	for (int i = 0; i < MAX_IMAGE_BUFFER_TOPCAM; i++) {
		if (m_pImageBuffer_Top[i] != NULL)
			delete m_pImageBuffer_Top[i], m_pImageBuffer_Top[i] = NULL;
	}
}

CSealingInspectProcessor::~CSealingInspectProcessor()
{
	Destroy();
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

	// 1. Create Image Buffer Color..
	if (CreateBuffer_SIDE() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// 2. Create Image Buffer Mono..
	if (CreateBuffer_TOP() == FALSE)
	{
		SystemMessage(_T("Create Memory Fail!"));
		return FALSE;
	}

	// 3. Load System Setting
	if (m_pSealingInspSystemSetting != NULL)
		delete m_pSealingInspSystemSetting, m_pSealingInspSystemSetting = NULL;
	m_pSealingInspSystemSetting = new CSealingInspectSystemSetting;

	// 4. Load Recipe
	if (m_pSealingInspRecipe != NULL)
		delete m_pSealingInspRecipe, m_pSealingInspRecipe = NULL;
	m_pSealingInspRecipe = new CSealingInspectRecipe;

	// 5. Inspect Result Data
	if (m_pSealingInspResult != NULL)
		delete m_pSealingInspResult, m_pSealingInspResult = NULL;
	m_pSealingInspResult = new CSealingInspectResult;

	// 6. Hik Cam
	if (m_pSealingInspHikCam != NULL)
		delete m_pSealingInspHikCam, m_pSealingInspHikCam = NULL;
	m_pSealingInspHikCam = new CSealingInspectHikCam(this);
#ifndef TEST_NO_CAMERA
	m_pSealingInspHikCam->Initialize();
#endif

	// 7. Inspect Core
	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspCore[i] != NULL)
			delete m_pSealingInspCore[i], m_pSealingInspCore[i] = NULL;
		m_pSealingInspCore[i] = new CSealingInspectCore(this);
	}

	return TRUE;
}

BOOL CSealingInspectProcessor::Destroy()
{
	for (int i = 0; i < MAX_IMAGE_BUFFER_SIDECAM; i++) {
		delete m_pImageBuffer_Side[i], m_pImageBuffer_Side[i] = NULL;
	}
	for (int i = 0; i < MAX_IMAGE_BUFFER_TOPCAM; i++) {
		delete m_pImageBuffer_Top[i], m_pImageBuffer_Top[i] = NULL;
	}

	if (m_pSealingInspHikCam != NULL)
		delete m_pSealingInspHikCam, m_pSealingInspHikCam = NULL;

	for (int i = 0; i < NUMBER_OF_SET_INSPECT; i++) {
		if (m_pSealingInspCore[i] != NULL)
			delete m_pSealingInspCore[i], m_pSealingInspCore[i] = NULL;
	}

	if (m_pSealingInspSystemSetting != NULL)
		delete m_pSealingInspSystemSetting, m_pSealingInspSystemSetting = NULL;

	if (m_pSealingInspRecipe != NULL)
		delete m_pSealingInspRecipe, m_pSealingInspRecipe = NULL;

	if (m_pSealingInspResult != NULL)
		delete m_pSealingInspResult, m_pSealingInspResult = NULL;

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

BOOL CSealingInspectProcessor::InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL isSimulator)
{
	// NUMBER_OF_SET_INSPECT = 2
	int nCoreIdx = 0;
	int nTopCamIdx = 0;
	int nSideCamIdx = 0;
	emInspectCavity nInspCav = emUNKNOWN;

	switch (nInspCavity)
	{
	case emInspectCavity_Cavity1:

		nTopCamIdx = 0;
		nSideCamIdx = 2;
		nCoreIdx = 0;
		nInspCav = emInspectCavity_Cavity1;
		break;
	case emInspectCavity_Cavity2:
		nTopCamIdx = 1;
		nSideCamIdx = 3;
		nCoreIdx = 1;
		nInspCav = emInspectCavity_Cavity2;
		break;
	}

	// start grabbing top cam 1 and side cam 1
	m_pSealingInspHikCam->StartGrab(nTopCamIdx);
	m_pSealingInspHikCam->StartGrab(nSideCamIdx);

	// create thread inspect cavity 1
	m_pSealingInspCore[nCoreIdx]->SetSimulatorMode(isSimulator);
	m_pSealingInspCore[nCoreIdx]->CreateInspectThread(nThreadCount, nInspCav);

	return TRUE;
}

BOOL CSealingInspectProcessor::InspectStop(emInspectCavity nInspCavity)
{
	int nCoreIdx = 0;

	if (nInspCavity == emInspectCavity_Cavity1)
		nCoreIdx = 0;
	else if (nInspCavity == emInspectCavity_Cavity2)
		nCoreIdx = 1;
	else {
		nCoreIdx = -1;
		return FALSE;
	}

	m_pSealingInspCore[nCoreIdx]->DeleteInspectThread();
	return TRUE;
}

#pragma region Offine Simulation
LPBYTE CSealingInspectProcessor::GetBufferImage_SIDE(int nBuff, int nFrame)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return NULL;

	return m_pImageBuffer_Side[nBuff]->GetFrameImage(nFrame);
}
LPBYTE CSealingInspectProcessor::GetBufferImage_TOP(int nBuff, int nFrame)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return NULL;

	return m_pImageBuffer_Top[nBuff]->GetFrameImage(nFrame);
}

BOOL CSealingInspectProcessor::LoadImageBuffer_SIDE(int nBuff, int nFrame, CString strFilePath)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer_Side[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer_Side[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer_Side[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBuffer_Side[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_COLOR);

	if (pOpenImage.empty())
		return FALSE;

	/*if (pOpenImage.type() != CV_8UC1)
		return FALSE;

	LPBYTE pBuffer = m_pImageBuffer[nBuff]->GetSharedBuffer();

	int nCopyHeight = (nFrameHeight * nFrameCount < pOpenImage.rows) ? nFrameHeight * nFrameCount : pOpenImage.rows;
	int nCopyWidth = (nFrameWidth < pOpenImage.cols) ? nFrameWidth : pOpenImage.cols;

	ZeroMemory(pBuffer, nFrameSize * nFrameCount);

	for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer + (i * nFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);*/

	m_pImageBuffer_Side[nBuff]->SetFrameImage(nFrame, pOpenImage.data);

	return TRUE;
}
BOOL CSealingInspectProcessor::LoadImageBuffer_TOP(int nBuff, int nFrame, CString strFilePath)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	if (strFilePath.IsEmpty() == TRUE)
		return FALSE;

	CString strExt = strFilePath.Right(3);

	strExt.MakeUpper();

	if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")) != 0 && strExt.CompareNoCase(_T("TIF")) != 0)
		return FALSE;

	CString strImagePath = strFilePath;

	int nFrameWidth = m_pImageBuffer_Top[nBuff]->GetFrameWidth();
	int nFrameHeight = m_pImageBuffer_Top[nBuff]->GetFrameHeight();
	int nFrameCount = m_pImageBuffer_Top[nBuff]->GetFrameCount();
	int nFrameSize = m_pImageBuffer_Top[nBuff]->GetFrameSize();

	USES_CONVERSION;
	char strTemp[1024] = {};
	sprintf_s(strTemp, "%s", W2A(strImagePath));

	cv::Mat pOpenImage = cv::imread(strTemp, cv::IMREAD_COLOR);

	if (pOpenImage.empty())
		return FALSE;

	/*if (pOpenImage.type() != CV_8UC1)
		return FALSE;

	LPBYTE pBuffer = m_pImageBuffer[nBuff]->GetSharedBuffer();

	int nCopyHeight = (nFrameHeight * nFrameCount < pOpenImage.rows) ? nFrameHeight * nFrameCount : pOpenImage.rows;
	int nCopyWidth = (nFrameWidth < pOpenImage.cols) ? nFrameWidth : pOpenImage.cols;

	ZeroMemory(pBuffer, nFrameSize * nFrameCount);

	for (int i = 0; i < nCopyHeight; i++)
		memcpy(pBuffer + (i * nFrameWidth), &pOpenImage.data[i * pOpenImage.step1()], nCopyWidth);*/

	m_pImageBuffer_Top[nBuff]->SetFrameImage(nFrame, pOpenImage.data);

	return TRUE;
}

BOOL CSealingInspectProcessor::LoadAllImageBuffer(CString strDirPath, CString strImageType)
{
	if (strDirPath.IsEmpty() == TRUE)
		return FALSE;

	for (int nSideIdx = 0; nSideIdx < MAX_SIDECAM_COUNT; nSideIdx++) {

		for (int nFrame = 0; nFrame < MAX_IMAGE_BUFFER_SIDECAM; nFrame++)
		{
			CString strExt = strImageType;
			strExt.MakeUpper();

			if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")))
				continue;

			CString strImagePath;

			strImagePath.Format(_T("%s\\%s%d_%s%d.%s"), strDirPath, _T("SideCam"),(nSideIdx + 1), _T("Frame"), (nFrame + 1), strImageType);

			LoadImageBuffer_SIDE(nSideIdx, nFrame, strImagePath);
		}
	}

	for (int nTopIdx = 0; nTopIdx < MAX_TOPCAM_COUNT; nTopIdx++) {

		for (int nFrame = 0; nFrame < MAX_IMAGE_BUFFER_TOPCAM; nFrame++)
		{
			CString strExt = strImageType;
			strExt.MakeUpper();

			if (strExt.CompareNoCase(_T("JPG")) != 0 && strExt.CompareNoCase(_T("BMP")) != 0 && strExt.CompareNoCase(_T("PNG")))
				continue;

			CString strImagePath;

			strImagePath.Format(_T("%s\\%s%d_%s%d.%s"), strDirPath, _T("TopCam"), (nTopIdx + 1), _T("Frame"), (nFrame + 1), strImageType);

			LoadImageBuffer_TOP(nTopIdx, nFrame, strImagePath);
		}
	}
}

BOOL CSealingInspectProcessor::CreateBuffer_SIDE()
{
	BOOL bRetValue_Side = FALSE;

	DWORD dwFrameWidth_Side = (DWORD)FRAME_WIDTH_SIDECAM;
	DWORD dwFrameHeight_Side = (DWORD)FRAME_HEIGHT_SIDECAM;
	DWORD dwFrameCount_Side = 0;
	DWORD dwFrameSize_Side = dwFrameWidth_Side * dwFrameHeight_Side * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_SIDECAM_COUNT; i++)
	{
		if (m_pImageBuffer_Side[i] != NULL)
		{
			m_pImageBuffer_Side[i]->DeleteSharedMemory();
			delete m_pImageBuffer_Side[i];
			m_pImageBuffer_Side[i] = NULL;
		}

		m_pImageBuffer_Side[i] = new CSharedMemoryBuffer;

		dwFrameCount_Side = (DWORD)MAX_IMAGE_BUFFER_SIDECAM;

		dwTotalFrameCount += dwFrameCount_Side;

		m_pImageBuffer_Side[i]->SetFrameWidth(dwFrameWidth_Side);
		m_pImageBuffer_Side[i]->SetFrameHeight(dwFrameHeight_Side);
		m_pImageBuffer_Side[i]->SetFrameCount(dwFrameCount_Side);
		m_pImageBuffer_Side[i]->SetFrameSize(dwFrameSize_Side);

		DWORD64 dw64Size_Side = (DWORD64)dwFrameCount_Side * dwFrameSize_Side;

		CString strMemory_Side;
		strMemory_Side.Format(_T("%s_%d"), "BufferOffline_Color_Side", i);

		bRetValue_Side = m_pImageBuffer_Side[i]->CreateSharedMemory(strMemory_Side, dw64Size_Side);

		if (bRetValue_Side == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Side [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Side, (int)dwFrameHeight_Side, (int)dwFrameCount_Side, (((double)(dwFrameSize_Side * dwFrameCount_Side)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Side * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}
BOOL CSealingInspectProcessor::CreateBuffer_TOP()
{
	BOOL bRetValue_Top = FALSE;

	DWORD dwFrameWidth_Top = (DWORD)FRAME_WIDTH_TOPCAM;
	DWORD dwFrameHeight_Top = (DWORD)FRAME_HEIGHT_TOPCAM;
	DWORD dwFrameCount_Top = 0;
	DWORD dwFrameSize_Top = dwFrameWidth_Top * dwFrameHeight_Top * (DWORD)NUMBER_OF_CHANNEL;

	DWORD64 dwTotalFrameCount = 0;

	for (int i = 0; i < MAX_TOPCAM_COUNT; i++)
	{
		if (m_pImageBuffer_Top[i] != NULL)
		{
			m_pImageBuffer_Top[i]->DeleteSharedMemory();
			delete m_pImageBuffer_Top[i];
			m_pImageBuffer_Top[i] = NULL;
		}

		m_pImageBuffer_Top[i] = new CSharedMemoryBuffer;

		dwFrameCount_Top = (DWORD)MAX_IMAGE_BUFFER_TOPCAM;

		dwTotalFrameCount += dwFrameCount_Top;

		m_pImageBuffer_Top[i]->SetFrameWidth(dwFrameWidth_Top);
		m_pImageBuffer_Top[i]->SetFrameHeight(dwFrameHeight_Top);
		m_pImageBuffer_Top[i]->SetFrameCount(dwFrameCount_Top);
		m_pImageBuffer_Top[i]->SetFrameSize(dwFrameSize_Top);

		DWORD64 dw64Size_Top = (DWORD64)dwFrameCount_Top * dwFrameSize_Top;

		CString strMemory_Top;
		strMemory_Top.Format(_T("%s_%d"), "BufferOffline_Color_Top", i);

		bRetValue_Top = m_pImageBuffer_Top[i]->CreateSharedMemory(strMemory_Top, dw64Size_Top);

		if (bRetValue_Top == FALSE)
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Fail.. : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
			return FALSE;
		}
		else
		{
			CString strLogMessage;
			strLogMessage.Format(_T("Top [%d] Create Memory Info : W[%d]xH[%d]xC[%d]=%.2f GB"), i, (int)dwFrameWidth_Top, (int)dwFrameHeight_Top, (int)dwFrameCount_Top, (((double)(dwFrameSize_Top * dwFrameCount_Top)) / 1000000000.0));
			SystemMessage(strLogMessage);
		}
	}

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(dwFrameSize_Top * dwTotalFrameCount)) / 1000000.0));
	SystemMessage(strLogMessage);
	return TRUE;
}

BOOL CSealingInspectProcessor::ClearBufferImage_SIDE(int nBuff)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	BOOL nRet = FALSE;
	nRet = m_pImageBuffer_Side[nBuff]->ClearBufferImage();

	return nRet;
}
BOOL CSealingInspectProcessor::ClearBufferImage_TOP(int nBuff)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	BOOL nRet = FALSE;
	nRet = m_pImageBuffer_Top[nBuff]->ClearBufferImage();

	return nRet;
}

BOOL CSealingInspectProcessor::SetTopCamResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pImageBuffer_Top[nBuff] == NULL)
		return FALSE;

	return m_pImageBuffer_Top[nBuff]->SetFrameImage(nFrame, buff);
}

BOOL CSealingInspectProcessor::SetSideCamResultBuffer(int nBuff, int nFrame, BYTE* buff)
{
	if (m_pImageBuffer_Side[nBuff] == NULL)
		return FALSE;

	return m_pImageBuffer_Side[nBuff]->SetFrameImage(nFrame, buff);
}

#pragma endregion

void CSealingInspectProcessor::RegCallbackLogFunc(CallbackLogFunc* pFunc)
{
	m_pCallbackLogFunc = pFunc;
}

void CSealingInspectProcessor::RegCallbackAlarm(CallbackAlarm* pFunc)
{
	m_pCallbackAlarm = pFunc;
}

void CSealingInspectProcessor::RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}

void CSealingInspectProcessor::InspectComplete(emInspectCavity nSetInsp)
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	m_pCallbackInsCompleteFunc();
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
