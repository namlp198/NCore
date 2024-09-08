#include "pch.h"
#include "NVisionInspectProcessorCSharp.h"

CNVisionInspectProcessor* CreateNVisionInspectProcessor()
{
    return new CNVisionInspectProcessor();
}

void DeleteNVisionInspectProcessor(CNVisionInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CNVisionInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

bool ContinuousGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	int nCoreIdx = nCamIdx;
	
	CNVisionInspect_HikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	int nCoreIdx = nCamIdx;
	
	CNVisionInspect_HikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;

	/*int retVal = pHikCam->SoftwareTrigger(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;*/
}

bool StopGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;
	
	int nCoreIdx = nCamIdx;

	CNVisionInspect_HikCam* pHikCam = pProcessor->GetHikCamControl();
	if (pHikCam == NULL)
		return false;

	int retVal = pHikCam->StopGrab(nCamIdx);

	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetImageBufferHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetImageBufferHikCam(nCamIdx);
}

bool SetTriggerMode(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nMode)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetTriggerMode(nCamIdx, nMode);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetTriggerSource(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nSource)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetTriggerSource(nCamIdx, nSource);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetExposureTime(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dExpTime)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetExposureTime(nCamIdx, dExpTime);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetAnalogGain(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dGain)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetAnalogGain(nCamIdx, dGain);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveImage(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveImage(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LocatorTool_Train(CNVisionInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LocatorTool_Train(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LocatorToolSimulator_Train(CNVisionInspectProcessor* pProcessor, int nSimuBuff, int nFrame)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LocatorToolSimulator_Train(nSimuBuff, nFrame);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStart(CNVisionInspectProcessor* pProcessor, int nThreadCount, BOOL isSimulator)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, isSimulator);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CNVisionInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop();
	if (bRet == FALSE) return false;
	else               return true;
}

BYTE* GetResultBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultBuffer(nBuff, nFrame);
}

bool GetInspectionResult(CNVisionInspectProcessor* pProcessor, int nCoreIdx, CNVisionInspectResult* pReadCodepResult)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetInspectionResult(nCoreIdx, pReadCodepResult);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LoadSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadSimulatorBuffer(nBuff, nFrame, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

BYTE* GetSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetSimulatorBuffer(nBuff, nFrame);
}

void RegCallBackInspectCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackInspectComplete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsCompleteFunc(pFunc);
}

void RegCallBackLogFunc(CNVisionInspectProcessor* pProcessor, CallbackLogFunc* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLogFunc(pFunc);
}

void RegCallBackAlarmFunc(CNVisionInspectProcessor* pProcessor, CallbackAlarmFunc* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackAlarmFunc(pFunc);
}

void RegCallBackLocatorTrainCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackLocatorTrainComplete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLocatorTrainCompleteFunc(pFunc);
}

bool LoadSystemSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	*(pSysSetting) = *(pProcessor->GetSystemSettingControl());
	return true;
}

bool LoadCameraSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectCameraSetting* pCamSetting, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	*(pCamSetting) = *(pProcessor->GetCameraSettingControl(nCamIdx));
	return true;
}

bool LoadRecipe(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->LoadRecipe(pProcessor->GetRecipeControl());

	*(pRecipe) = *(pProcessor->GetRecipeControl());
	return true;
}

bool ReloadSystenSettings(CNVisionInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->ReloadSystemSetting();
	if (bRet == FALSE) return false;
	else               return true;
}

bool ReloadRecipe(CNVisionInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->ReloadRecipe();
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveSystemSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveSystemSetting(pSysSetting);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveCameraSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspectCameraSetting* pCamSetting, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveCameraSettings(pCamSetting, nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveRecipe(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveRecipe(pRecipe);
	if (bRet == FALSE) return false;
	else               return true;
}
