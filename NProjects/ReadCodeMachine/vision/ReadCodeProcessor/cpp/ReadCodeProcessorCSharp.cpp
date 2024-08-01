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

	CReadCodeBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	pBaslerCam->SetIsStreaming(TRUE);

	int retVal = pBaslerCam->StartGrab(nCamIdx);
	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

bool SingleGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	CReadCodeBaslerCam* pBaslerCam = pProcessor->GetBaslerCamControl();
	if (pBaslerCam == NULL)
		return false;

	pBaslerCam->SetIsStreaming(TRUE);

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

	pBaslerCam->SetIsStreaming(FALSE);

	if (retVal == 0) return false;
	else if (retVal == 1) return true;
}

BYTE* GetImageBufferBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return NULL;

	return pProcessor->GetImageBufferBaslerCam(nCamIdx);
}

bool InspectStart(CReadCodeProcessor* pProcessor, BOOL isSimulator)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(isSimulator);
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
