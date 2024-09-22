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

bool ContinuousGrab(CNVisionInspectProcessor* pProcessor, emCameraBrand camBrand, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	int retVal = 0;
	int nCoreIdx = nCamIdx;
	
	switch (camBrand)
	{
	case CameraBrand_Hik:
	{
		CNVisionInspect_HikCam* pHikCam = pProcessor->GetHikCamControl();
		if (pHikCam == NULL)
			return false;
		retVal = pHikCam->StartGrab(nCamIdx);
	}
		break;
	case CameraBrand_Basler:
	{
		CNVisionInspect_BaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
		if (pBaslerCam == NULL)
			return false;
		retVal = pBaslerCam->StartGrab(nCamIdx);
	}
		break;
	case CameraBrand_Jai:
		break;
	case CameraBrand_IRayple:
		break;
	}
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrab(CNVisionInspectProcessor* pProcessor, emCameraBrand camBrand, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;
	
	int retVal = 0;
	int nCoreIdx = nCamIdx;

	switch (camBrand)
	{
	case CameraBrand_Hik:
	{
		CNVisionInspect_HikCam* pHikCam = pProcessor->GetHikCamControl();
		if (pHikCam == NULL)
			return false;
		retVal = pHikCam->StopGrab(nCamIdx);
	}
	break;
	case CameraBrand_Basler:
	{
		CNVisionInspect_BaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
		if (pBaslerCam == NULL)
			return false;
		retVal = pBaslerCam->StopGrab(nCamIdx);
	}
	break;
	case CameraBrand_Jai:
		break;
	case CameraBrand_IRayple:
		break;
	}

	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetImageBuffer(CNVisionInspectProcessor* pProcessor, emCameraBrand camBrand, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	switch (camBrand)
	{
	case CameraBrand_Hik: return pProcessor->GetImageBufferHikCam(nCamIdx); break;
	case CameraBrand_Basler: return pProcessor->GetImageBufferBaslerCam(nCamIdx); break;
	case CameraBrand_Jai: break;
	case CameraBrand_IRayple: break;
	}
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

bool SelectROI(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nROIIdx, int nFrom)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SelectROI(nCamIdx, nROIIdx, nFrom);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStart(CNVisionInspectProcessor* pProcessor, int nThreadCount, int nCamCount)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, nCamCount);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CNVisionInspectProcessor* pProcessor, int nCamCount)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop(nCamCount);
	if (bRet == FALSE) return false;
	else               return true;
}

bool Inspect_Simulator(CNVisionInspectProcessor* pProcessor, emCameraBrand camBrand, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Inspect_Simulator(camBrand, nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

BYTE* GetResultBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultBuffer(nBuff, nFrame);
}

bool GetInspectionResult(CNVisionInspectProcessor* pProcessor, CNVisionInspectResult* pNVisionInsppResult)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetInspectionResult(pNVisionInsppResult);
	if (bRet == FALSE) return false;
	else               return true;
}

bool GetInspectToolResult_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectResult_FakeCam* pNVisionInsppRes_FakeCam)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetInspectToolResult_FakeCam(pNVisionInsppRes_FakeCam);
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

void CallInspectTool(CNVisionInspectProcessor* pProcessor, emInspectTool inspTool)
{
	if (pProcessor == NULL)
		return;

	pProcessor->CallInspectTool(inspTool);
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

void RegCallbackInspComplete_FakeCamFunc(CNVisionInspectProcessor* pProcessor, CallbackInspectComplete_FakeCam* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInspComplete_FakeCamFunc(pFunc);
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

bool LoadFakeCameraSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspect_FakeCameraSetting* pFakeCamSetting)
{
	if (pProcessor == NULL)
		return false;

	*(pFakeCamSetting) = *(pProcessor->GetFakeCameraSettingControl());
	return true;
}

bool LoadRecipe(CNVisionInspectProcessor* pProcessor, int nCamCount, CNVisionInspectRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LoadRecipe(nCamCount, pRecipe);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LoadRecipe_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe_FakeCam* pFakeCamRecipe)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LoadRecipe_FakeCam(pFakeCamRecipe);
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

bool SaveCameraSetting(CNVisionInspectProcessor* pProcessor, int nCamIdx, CNVisionInspectCameraSetting* pCamSetting)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveCameraSettings(nCamIdx, pCamSetting);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveFakeCameraSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspect_FakeCameraSetting* pFakeCamSetting)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveFakeCameraSettings(pFakeCamSetting);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveRecipe(CNVisionInspectProcessor* pProcessor, int nCamIdx, CNVisionInspectRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveRecipe(nCamIdx, pRecipe);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveRecipe_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe_FakeCam* pRecipeFakeCam)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveRecipe_FakeCam(pRecipeFakeCam);
	if (bRet == FALSE) return false;
	else               return true;
}
