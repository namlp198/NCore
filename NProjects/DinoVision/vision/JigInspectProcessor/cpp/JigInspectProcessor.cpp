#include "pch.h"
#include "JigInspectProcessor.h"

CJigInspectProcessor::CJigInspectProcessor()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pImageBuffer[i] = NULL;
	}
}

CJigInspectProcessor::~CJigInspectProcessor()
{
	Destroy();
}

BOOL CJigInspectProcessor::Initialize()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspConfig[i] != NULL)
			delete m_pJigInspConfig[i], m_pJigInspConfig[i] == NULL;
		m_pJigInspConfig[i] = new CJigInspectConfig;
	}

	// Load Sys Config
	if (LoadConfigurations() == FALSE)
		return FALSE;
	
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspRecipe[i] != NULL)
			delete m_pJigInspRecipe[i], m_pJigInspRecipe[i] == NULL;
		m_pJigInspRecipe[i] = new CJigInspectRecipe;
	}

	//// Load Recipe
	//if (LoadRecipe() == FALSE)
	//{
	//	return FALSE;
	//}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspResutls[i] != NULL)
			delete m_pJigInspResutls[i], m_pJigInspResutls[i] == NULL;
		m_pJigInspResutls[i] = new CJigInspectResults;
	}

	// Create Image Buffer..
	if (CreateBuffer() == FALSE)
		return FALSE;
	
	if (m_pInspDinoCam != NULL)
	{
		delete m_pInspDinoCam, m_pInspDinoCam = NULL;
	}
	m_pInspDinoCam = new CJigInspectDinoCam(this);
	m_pInspDinoCam->Initialize();
}

BOOL CJigInspectProcessor::Destroy()
{
	if (m_pInspDinoCam != NULL)
		delete m_pInspDinoCam, m_pInspDinoCam = NULL;

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspConfig[i] != NULL)
			delete m_pJigInspConfig[i], m_pJigInspConfig[i] == NULL;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspRecipe[i] != NULL)
			delete m_pJigInspRecipe[i], m_pJigInspRecipe[i] == NULL;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pJigInspResutls[i] != NULL)
			delete m_pJigInspResutls[i], m_pJigInspResutls[i] == NULL;
	}

	return TRUE;
}

BOOL CJigInspectProcessor::LoadConfigurations()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pJigInspConfig[i]->m_csSensorType = _T("color");
		m_pJigInspConfig[i]->m_nChannels = 3;
		m_pJigInspConfig[i]->m_nFrameWidth = 640;
		m_pJigInspConfig[i]->m_nFrameHeight = 480;
	}

	return TRUE;
}

BOOL CJigInspectProcessor::CreateBuffer()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		USES_CONVERSION;
		char cSensorType[10] = {};
		sprintf_s(cSensorType, "%s", W2A(m_pJigInspConfig[i]->m_csSensorType));


		int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
		int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

		BOOL bRetValue = FALSE;

		DWORD dwFrameWidth = (DWORD)m_pJigInspConfig[i]->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)m_pJigInspConfig[i]->m_nFrameHeight;
		DWORD dwFrameCount = 0;
		DWORD dwFrameSize = dwFrameWidth * dwFrameHeight * nFrameChannels;

		DWORD64 dwTotalFrameCount = 0;

		if (m_pImageBuffer[i] != NULL)
		{
			m_pImageBuffer[i]->DeleteSharedMemory();
			delete m_pImageBuffer[i];
			m_pImageBuffer[i] = NULL;
		}

		m_pImageBuffer[i] = new CSharedMemoryBuffer;

		dwFrameCount = MAX_FRAME_COUNT;

		dwTotalFrameCount += dwFrameCount;

		m_pImageBuffer[i]->SetFrameWidth(dwFrameWidth);
		m_pImageBuffer[i]->SetFrameHeight(dwFrameHeight);
		m_pImageBuffer[i]->SetFrameCount(dwFrameCount);
		m_pImageBuffer[i]->SetFrameSize(dwFrameSize);

		DWORD64 dw64Size = (DWORD64)dwFrameCount * dwFrameSize;

		CString strMemory;
		strMemory.Format(_T("%s_%d"), MAP_FILE_NAME_INS_BUFFER, i);

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

		CString strLogMessage;
		strLogMessage.Format(_T("Total Create Memory : %.2f GB"), (((double)(dwFrameSize * dwTotalFrameCount)) / 1000000000.0));
	}

	return TRUE;
}

BOOL CJigInspectProcessor::InspectStart(int nCamIdx)
{
	if (nCamIdx < 0 || MAX_CAMERA_COUNT <= nCamIdx)
		return 0;

	if (m_pInspDinoCam == NULL)
		return FALSE;

	m_pInspDinoCam->InspectStart(nCamIdx);
}

LPBYTE CJigInspectProcessor::GetFrameImage(int nCamIdx, UINT nFrameIndex)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetFrameImage((DWORD)nFrameIndex);
}

LPBYTE CJigInspectProcessor::GetBufferImage(int nCamIdx, UINT nY)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetBufferImage(nY);
}

void CJigInspectProcessor::InspectComplete()
{
	if (m_pCallbackInsCompleteFunc == NULL)
		return;

	m_pCallbackInsCompleteFunc();
}

BOOL CJigInspectProcessor::GetInspectionResult(int nCamIdx, CJigInspectResults* pJigInspRes)
{
	pJigInspRes->m_bInspectCompleted = m_pJigInspResutls[nCamIdx]->m_bInspectCompleted;
	pJigInspRes->m_bResultOKNG = m_pJigInspResutls[nCamIdx]->m_bResultOKNG;

	return TRUE;
}

void CJigInspectProcessor::RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc)
{
	m_pCallbackInsCompleteFunc = pFunc;
}