#include "pch.h"
#include "ReadCodeProcessorCSharp.h"

CReadCodeProcessor* CreateReadCodeProcessor()
{
    return new CReadCodeProcessor();
}

void DeleteReadCodeProcessor(CReadCodeProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CReadCodeProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

bool ContinuousGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->GetReadCodeStatusControl(0)->SetStreaming(TRUE);

	CReadCodeBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	int retVal = pBaslerCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->GetReadCodeStatusControl(0)->SetStreaming(TRUE);

	CReadCodeBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	int retVal = pBaslerCam->SingleGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool StopGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CReadCodeBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	int retVal = pBaslerCam->StopGrab(nCamIdx);

	if (retVal == 0) return false;
	else if (retVal == 1) 
	{ 
		pProcessor->GetReadCodeStatusControl(0)->SetStreaming(FALSE); 
		return true; 
	}
}

BYTE* GetImageBufferBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetImageBufferBaslerCam(nCamIdx);
}

bool SetTriggerMode(CReadCodeProcessor* pProcessor, int nCamIdx, int nMode)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetTriggerMode(nCamIdx, nMode);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetTriggerSource(CReadCodeProcessor* pProcessor, int nCamIdx, int nSource)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetTriggerSource(nCamIdx, nSource);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetExposureTime(CReadCodeProcessor* pProcessor, int nCamIdx, double dExpTime)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetExposureTime(nCamIdx, dExpTime);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SetAnalogGain(CReadCodeProcessor* pProcessor, int nCamIdx, double dGain)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SetAnalogGain(nCamIdx, dGain);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveImage(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveImage(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LocatorTool_Train(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LocatorTool_Train(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LocatorToolSimulator_Train(CReadCodeProcessor* pProcessor, int nSimuBuff, int nFrame)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->LocatorToolSimulator_Train(nSimuBuff, nFrame);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStart(CReadCodeProcessor* pProcessor, int nThreadCount, BOOL isSimulator)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, isSimulator);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CReadCodeProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop();
	if (bRet == FALSE) return false;
	else               return true;
}

BYTE* GetResultBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetResultBuffer(nBuff, nFrame);
}

bool GetInspectionResult(CReadCodeProcessor* pProcessor, int nCoreIdx, CReadCodeResult* pReadCodepResult)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->GetInspectionResult(nCoreIdx, pReadCodepResult);
	if (bRet == FALSE) return false;
	else               return true;
}

bool LoadSimulatorBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath)
{
	if (pProcessor == NULL)
		return false;

	CString strPath = (CString)pFilePath;
	BOOL bRetValue = pProcessor->LoadSimulatorBuffer(nBuff, nFrame, strPath);

	if (bRetValue == FALSE) return false;
	else                    return true;
}

BYTE* GetSimulatorBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetSimulatorBuffer(nBuff, nFrame);
}

void RegCallBackInspectCompleteFunc(CReadCodeProcessor* pProcessor, CallbackInspectComplete* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackInsCompleteFunc(pFunc);
}

void RegCallbackLogFunc(CReadCodeProcessor* pProcessor, CallbackLogFunc* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLogFunc(pFunc);
}

void RegCallbackAlarm(CReadCodeProcessor* pProcessor, CallbackAlarm* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackAlarm(pFunc);
}

void RegCallbackLocatorTrainedFunc(CReadCodeProcessor* pProcessor, CallbackLocatorTrained* pFunc)
{
	if (pProcessor == NULL)
		return;

	pProcessor->RegCallbackLocatorTrainedFunc(pFunc);
}

bool LoadSystemSettings(CReadCodeProcessor* pProcessor, CReadCodeSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	*(pSysSetting) = *(pProcessor->GetSystemSettingControl());
	return true;
}

bool LoadRecipe(CReadCodeProcessor* pProcessor, CReadCodeRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	pProcessor->LoadRecipe(pProcessor->GetRecipeControl());

	*(pRecipe) = *(pProcessor->GetRecipeControl());
	return true;
}

bool ReloadSystenSettings(CReadCodeProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->ReloadSystemSetting();
	if (bRet == FALSE) return false;
	else               return true;
}

bool ReloadRecipe(CReadCodeProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->ReloadRecipe();
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveSystemSetting(CReadCodeProcessor* pProcessor, CReadCodeSystemSetting* pSysSetting)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveSystemSetting(pSysSetting);
	if (bRet == FALSE) return false;
	else               return true;
}

bool SaveRecipe(CReadCodeProcessor* pProcessor, CReadCodeRecipe* pRecipe)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->SaveRecipe(pRecipe);
	if (bRet == FALSE) return false;
	else               return true;
}
