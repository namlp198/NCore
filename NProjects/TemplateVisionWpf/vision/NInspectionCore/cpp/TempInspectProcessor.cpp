#include "pch.h"
#include "TempInspectProcessor.h"

CTempInspectProcessor::CTempInspectProcessor()
{
	m_pHikCamera = NULL;
	m_pTempInspSysConfig = NULL;

	std::queue<int> emptyList;
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pTempInspCore[i] = NULL;
		m_pTempInspRecipe[i] = NULL;
		m_pTempInspStatus[i] = NULL;
		m_pTempInspResult[i] = NULL;
		m_pImageBuffer[i] = NULL;

		std::swap(m_queueInspectWaitList[i], emptyList);
	}
}

CTempInspectProcessor::~CTempInspectProcessor()
{
}

BOOL CTempInspectProcessor::Initialize()
{
	// Load Sys Config
	if (LoadSystemConfig() == FALSE)
	{
		return FALSE;
	}
	// Load Recipe
	if (LoadRecipe() == FALSE)
	{
		return FALSE;
	}

	// Create Image Buffer..
	if (CreateBuffer() == FALSE)
	{
		return FALSE;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pTempInspCore[i] != NULL)
		{
			delete m_pTempInspCore[i], m_pTempInspCore[i] = NULL;
		}
		m_pTempInspCore[i] = new CTempInspectCore(this);
	}

	// Test Run
	//m_pTempInspCore->StartInspect(0);

	if (m_pHikCamera != NULL)
	{
		m_pHikCamera->Destroy();
		delete m_pHikCamera, m_pHikCamera = NULL;
	}
	m_pHikCamera = new CTempInspectHikCam(this);
	m_pHikCamera->Initialize();

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pTempInspStatus[i] != NULL)
			delete m_pTempInspStatus[i], m_pTempInspStatus[i] = NULL;

		m_pTempInspStatus[i] = new CTempInspectStatus;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pTempInspResult[i] != NULL)
			delete m_pTempInspResult[i], m_pTempInspResult[i] = NULL;

		m_pTempInspResult[i] = new CTempInspectResult(this);
	}

	if (m_pLocToolTrain != NULL)
		delete m_pLocToolTrain, m_pLocToolTrain = NULL;
	m_pLocToolTrain = new CLocatorTool();

	if (m_pVsAlgorithm != NULL)
		delete m_pVsAlgorithm, m_pVsAlgorithm = NULL;
	m_pVsAlgorithm = new CVisionAlgorithms;

	return TRUE;
}

BOOL CTempInspectProcessor::Destroy()
{
	if (m_pHikCamera != NULL)
	{
		m_pHikCamera->Destroy();
		delete m_pHikCamera, m_pHikCamera = NULL;
	}

	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		if (m_pTempInspCore[i] != NULL)
			delete m_pTempInspCore[i], m_pTempInspCore[i] = NULL;
	}

	return TRUE;
}

BOOL CTempInspectProcessor::LoadSystemConfig()
{
	char buf[1024] = {};
	GetCurrentDirectoryA(1024, buf);
	std::string sysConfigPath = std::string(buf) + "\\config";
	m_pTempInspSysConfig = new CTempInspectSystemConfig((CString)sysConfigPath.c_str());
	m_pTempInspSysConfig->Initialize();

	return TRUE;
}

BOOL CTempInspectProcessor::LoadRecipe()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		m_pTempInspRecipe[i] = new CTempInspectRecipe;

		CString jobPath = m_pTempInspSysConfig->GetJobPath() + "\\" + m_pTempInspSysConfig->GetJobName() + ".xml";
		m_pTempInspRecipe[i]->SetJobPath(jobPath);
		m_pTempInspRecipe[i]->SetJobName(m_pTempInspSysConfig->GetJobName());

		m_pTempInspRecipe[i]->LoadRecipe(i);
	}
	return TRUE;
}

BOOL CTempInspectProcessor::CreateBuffer()
{
	for (int i = 0; i < MAX_CAMERA_INSP_COUNT; i++)
	{
		CameraInfo* pCamInfo = m_pTempInspRecipe[i]->GetCameraInfos();

		USES_CONVERSION;
		char cSensorType[10] = {};
		sprintf_s(cSensorType, "%s", W2A(pCamInfo->m_csSensorType));


		int nFrameDepth = strcmp(cSensorType, "color") == 0 ? 24 : 8;
		int nFrameChannels = strcmp(cSensorType, "color") == 0 ? 3 : 1;

		BOOL bRetValue = FALSE;

		DWORD dwFrameWidth = (DWORD)pCamInfo->m_nFrameWidth;
		DWORD dwFrameHeight = (DWORD)pCamInfo->m_nFrameHeight;
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

BOOL CTempInspectProcessor::InspectStart(int nThreadCount, int nCamIdx)
{
	if (m_pTempInspStatus[nCamIdx]->GetInspectRunning() == TRUE)
	{
		return FALSE;
	}
	m_pTempInspCore[nCamIdx]->SetCamIndex(nCamIdx);

	// create thread inspect
	m_pTempInspCore[nCamIdx]->CreateInspectThread(nThreadCount);

	// set status is running
	m_pTempInspStatus[nCamIdx]->SetInspectRunning(TRUE);

	return TRUE;
}

BOOL CTempInspectProcessor::InspectStop(int nCamIdx)
{
	if (m_pTempInspStatus[nCamIdx]->GetInspectRunning() == FALSE)
	{
		return FALSE;
	}

	m_pTempInspStatus[nCamIdx]->SetInspectRunning(FALSE);

	m_pTempInspCore[nCamIdx]->DeleteInspectThread();

	// Init Inspect Wait List..
	CSingleLock localLock(&m_csInspectWaitList[nCamIdx]);
	localLock.Lock();
	std::queue<int> emptyQueue;
	std::swap(m_queueInspectWaitList[nCamIdx], emptyQueue);
	localLock.Unlock();

	return TRUE;
}

void CTempInspectProcessor::ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nNextFrameIdx, LPVOID param)
{
	CTempInspectProcessor* pThis = (CTempInspectProcessor*)param;
	pThis->ReceivedImageCallbackEx(nGrabberIdx, nNextFrameIdx, pBuffer);
}

void CTempInspectProcessor::ReceivedImageCallbackEx(int nGrabberIdx, int nNextFrameIdx, LPVOID pBuffer)
{
	m_pTempInspStatus[nGrabberIdx]->SetInsertFrameIndex(nNextFrameIdx);
	int nFrameIdx = m_pTempInspStatus[nGrabberIdx]->GetInsertFrameIndex();

	if (pBuffer != NULL)
		m_pImageBuffer[nGrabberIdx]->SetFrameImage(nFrameIdx, (LPBYTE)pBuffer);

	// Push Inspect Wait List
	CSingleLock localLock(&m_csInspectWaitList[nGrabberIdx]);
	localLock.Lock();
	m_queueInspectWaitList[nGrabberIdx].push(nFrameIdx);
	localLock.Unlock();
}

void CTempInspectProcessor::InspectComplete(int nCamIdx)
{

}

LPBYTE CTempInspectProcessor::GetFrameImage(int nCamIdx, UINT nFrameIndex)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetFrameImage((DWORD)nFrameIndex);
}

LPBYTE CTempInspectProcessor::GetBufferImage(int nCamIdx, UINT nY)
{
	if (m_pImageBuffer[nCamIdx] == NULL)
		return NULL;

	return m_pImageBuffer[nCamIdx]->GetBufferImage(nY);
}

int CTempInspectProcessor::PopInspectWaitFrame(int nCamIdx)
{
	int nProcessFrame = 0;

	CSingleLock localLock(&m_csInspectWaitList[(int)nCamIdx]);
	localLock.Lock();

	if (m_queueInspectWaitList[(int)nCamIdx].empty())
		return -1;

	nProcessFrame = m_queueInspectWaitList[(int)nCamIdx].front();

	m_queueInspectWaitList[(int)nCamIdx].pop();

	return nProcessFrame;
}

BOOL CTempInspectProcessor::TrainLocator_TemplateMatching(int nCamIdx, CRectForTrainLocTool* rectForTrainLoc)
{
	if (m_pLocToolTrain == NULL)
		return FALSE;

	if (rectForTrainLoc == NULL)
		return FALSE;

	return m_pLocToolTrain->NVision_FindLocator_TemplateMatching_Train(nCamIdx, rectForTrainLoc);
}

BOOL CTempInspectProcessor::GetDataTrained_TemplateMatching(CLocatorToolResult* dataTrained)
{
	if (m_pLocToolTrain == NULL)
		return FALSE;
	return m_pLocToolTrain->GetDataTrained_TemplateMatching(dataTrained);
}

BOOL CTempInspectProcessor::CountPixelAlgorithm_Train(CParamCntPxlAlgorithm* pParamCntPxlTrain)
{
	if (m_pVsAlgorithm == NULL)
		return FALSE;
	if (pParamCntPxlTrain == NULL)
		return FALSE;

	return m_pVsAlgorithm->NVision_CountPixelAlgorithm_Train(pParamCntPxlTrain);
}

BOOL CTempInspectProcessor::CalculateAreaAlgorithm_Train(CParamCalAreaAlgorithm* pParamTrainCalArea)
{
	if (m_pVsAlgorithm == NULL)
		return FALSE;
	if (pParamTrainCalArea == NULL)
		return FALSE;

	return m_pVsAlgorithm->NVision_CalculateAreaAlgorithm_Train(pParamTrainCalArea);
}

BYTE* CTempInspectProcessor::GetResultROIBuffer_Train()
{
	if (m_pVsAlgorithm == NULL)
		return FALSE;

	return m_pVsAlgorithm->GetResultROIBuffer_Train();
}

BOOL CTempInspectProcessor::GetResultCntPxl_Train(CAlgorithmsCountPixelResult* pCntPxlTrainRes)
{
	if (m_pVsAlgorithm == NULL)
		return FALSE;

	if (pCntPxlTrainRes == NULL)
		return FALSE;

	return m_pVsAlgorithm->GetResultCntPxl_Train(pCntPxlTrainRes);
}

BOOL CTempInspectProcessor::GetResultCalArea_Train(CAlgorithmsCalculateAreaResult* pCalAreaTrainRes)
{
	if (m_pVsAlgorithm == NULL)
		return FALSE;

	if (pCalAreaTrainRes == NULL)
		return FALSE;

	return m_pVsAlgorithm->GetResultCalArea_Train(pCalAreaTrainRes);
}

BYTE* CTempInspectProcessor::GetImageTemplate()
{
	if (m_pLocToolTrain == NULL)
		return nullptr;

	m_pLocToolTrain->GetTemplateImageBuffer();
}
	
