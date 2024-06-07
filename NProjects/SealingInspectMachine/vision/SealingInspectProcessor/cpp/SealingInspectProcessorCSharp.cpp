#include "pch.h"
#include "SealingInspectProcessorCSharp.h"

CSealingInspectProcessor* CreateSealingInspectProcessor()
{
	return new CSealingInspectProcessor();
}

void DeleteSealingInspectProcessor(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

#pragma region Offline simulation
BYTE* GetBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_SIDE(nBuff, nFrame);
}
BYTE* GetBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetBufferImage_TOP(nBuff, nFrame);
}

BYTE* GetResultBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultBuffer_SIDE(nBuff, nFrame);
}

BYTE* GetResultBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultBuffer_TOP(nBuff, nFrame);
}

bool LoadImageBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_SIDE(nBuff, nFrame, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}
bool LoadImageBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadImageBuffer_TOP(nBuff, nFrame, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool LoadAllImageBuffer(CSealingInspectProcessor* pProcessor, char* strDirPath, char* strImageType)
{
	if (pProcessor == NULL)
		return false;

	CString strDir = (CString)strDirPath;
	CString strImgType = (CString)strImageType;
	BOOL bRetValue = pProcessor->LoadAllImageBuffer(strDir, strImgType);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ClearBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage_SIDE(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}
bool ClearBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->ClearBufferImage_TOP(nBuff);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool Inspect_TopCam_Simulation(CSealingInspectProcessor* pProcessor, int nCoreIdx, int nCamIdx, int nFrame)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->Inspect_TopCam_Simulation(nCoreIdx, nCamIdx, nFrame);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool Inspect_SideCam_Simulation(CSealingInspectProcessor* pProcessor, int nCoreIdx, int nCamIdx, int nFrame)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRetValue = pProcessor->Inspect_SideCam_Simulation(nCoreIdx, nCamIdx, nFrame);
	if (bRetValue == FALSE) return false;
	else                    return true;
}

bool ContinuousGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SoftwareTriggerHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SoftwareTrigger(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StopGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetBufferImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	/*CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return NULL;*/

	return pProcessor->GetBufferImageHikCam(nCamIdx);
}

bool SetTriggerModeHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nMode)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerMode(nCamIdx, nMode);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SetTriggerSourceHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nSource)
{
	if (pProcessor == NULL)
		return false;

	CSealingInspectHikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->SetTriggerSource(nCamIdx, nSource);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SaveImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, char* strImageSavePath)
{
	if (pProcessor == NULL)
		return false;

	int retVal = pProcessor->SaveImageHikCam(nCamIdx, (CString)strImageSavePath);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool InspectStart(CSealingInspectProcessor* pProcessor, int nThreadCount, emInspectCavity nInspCavity, BOOL isSimulator)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, nInspCavity, isSimulator);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CSealingInspectProcessor* pProcessor, emInspectCavity nInspCavity)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop(nInspCavity);
	if (bRet == FALSE) return false;
	else               return true;
}

bool GetInspectionResult(CSealingInspectProcessor* pProcessor, int nCoreIdx, CSealingInspectResult* pSealingInspResult)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetInspectionResult(nCoreIdx, pSealingInspResult);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetSealingInspectSimulationIO(CSealingInspectProcessor* pProcessor, int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetSealingInspectSimulationIO(nCoreIdx, sealingInspSimulationIO);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetCavityInfo(CSealingInspectProcessor* pProcessor, char* strLoadingTime)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->SetCavityInfo((CString)strLoadingTime);

	return true;
}

bool SetProcessStatus(CSealingInspectProcessor* pProcessor, int nCoreIdx, BOOL bProcessStatus)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->SetProcessStatus(nCoreIdx, bProcessStatus);
	
	return true;
}

bool LoadSystemSettings(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	*(pSysSetting) = *(pProcessor->GetSystemSetting());
	return true;
}

bool LoadRecipe(CSealingInspectProcessor* pProcessor, CSealingInspectRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->LoadRecipe(pProcessor->GetRecipe());

	*(pRecipe) = *(pProcessor->GetRecipe());
	return true;
}

bool SaveSystemSetting(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveSystemSetting(pSysSetting);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveLightSetting(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting, int nLightIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveLightSetting(pSysSetting, nLightIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveRecipe(CSealingInspectProcessor* pProcessor, CSealingInspectRecipe* pRecipe, char* sPosCam, int nFrameIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveRecipe(pRecipe, (CString)sPosCam, nFrameIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool TestInspectCavity1(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->TestInspectCavity1();
	if (bRet == FALSE) return false;
	else               return true;
}

bool TestInspectCavity2(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->TestInspectCavity2();
	if (bRet == FALSE) return false;
	else               return true;
}

bool TestTCPSocket(CSealingInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->TestTCPSocket();
	if (bRet == FALSE) return false;
	else               return true;
}

#pragma endregion

void RegCallbackLogFunc(CSealingInspectProcessor* pProcessor, CallbackLogFunc* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLogFunc(pFunc);
}

void RegCallBackInspectCavity1CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectCavity1Complete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsCavity1completeFunc(pFunc);
}

void RegCallBackInspectCavity2CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectCavity2Complete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsCavity2completeFunc(pFunc);
}

void RegCallBackInspectTopCam1CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectTopCam1Complete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsTopCam1CompleteFunc(pFunc);
}

void RegCallBackInspectTopCam2CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectTopCam2Complete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsTopCam2CompleteFunc(pFunc);
}
