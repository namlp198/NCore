#include "pch.h"
#include "JigInspectProcessorCSharp.h"

CJigInspectProcessor* CreateJigInspectProcessor()
{
    return new CJigInspectProcessor();
}

void DeleteJigInspectProcessor(CJigInspectProcessor* pProcessor)
{
    if (pProcessor == NULL)
        return;

    delete pProcessor;
    pProcessor = NULL;
}

bool Initialize(CJigInspectProcessor* pProcessor)
{
    if (pProcessor == NULL)
        return false;

    BOOL bRet = pProcessor->Initialize();
    if (bRet == FALSE) return false;
    else               return true;
}

bool InspectStart(CJigInspectProcessor* pProcessor, int nThreadCount, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    BOOL bRet = pProcessor->InspectStart(nCamIdx);
    if (bRet == FALSE) return false;
    else               return true;
}

bool SingleGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return false;

    int retVal = pDinoCam->SingleGrab(nCamIdx);
    if (retVal == 0) return false;
    else if (retVal == 1) return true;
}

bool StartGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return false;

    int retVal = pDinoCam->StartGrab(nCamIdx);
    if (retVal == 0) return false;
    else if (retVal == 1) return true;
}

bool StopGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return false;

    int retVal = pDinoCam->StopGrab(nCamIdx);
    if (retVal == 0) return false;
    else if (retVal == 1) return true;
}

bool ConnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return false;

    int retVal = pDinoCam->Connect(nCamIdx);
    if (retVal == 0) return false;
    else if (retVal == 1) return true;
}

bool DisconnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return false;

    int retVal = pDinoCam->Disconnect(nCamIdx);
    if (retVal == 0) return false;
    else if (retVal == 1) return true;
}

BYTE* GetBufferDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return NULL;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return NULL;

    return pDinoCam->GetBufferImage(nCamIdx);
}

BYTE* GetResultBufferImageDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return NULL;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return NULL;

    return pDinoCam->GetResultBufferImage(nCamIdx);
}

bool GetInspectionResult(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectResults* pJigInspResult)
{
    if (pProcessor == NULL)
        return false;

    BOOL bRet = pProcessor->GetInspectionResult(nCamIdx, pJigInspResult);
    if (bRet == FALSE) return false;
    else               return true;
}

bool LoadSysConfigurations(CJigInspectProcessor* pProcessor, CJigInspectSystemConfig* pSysConfig)
{
    if (pProcessor == NULL)
        return false;

    *(pSysConfig) = *(pProcessor->GetSystemConfig());

    return true;
}

bool LoadCamConfigurations(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectCameraConfig* pCamConfig)
{
    if (pProcessor == NULL)
        return false;

    *(pCamConfig) = *(pProcessor->GetCameraConfig(nCamIdx));

    return true;
}

bool LoadRecipe(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectRecipe* pRecipe)
{
    if (pProcessor == NULL)
        return false;

    *(pRecipe) = *(pProcessor->GetRecipe(nCamIdx));

    return true;
}

bool SaveSysConfigurations(CJigInspectProcessor* pProcessor, CJigInspectSystemConfig* pSysConfig)
{
    if (pProcessor == NULL)
        return false;

    BOOL bRet = pProcessor->SaveSysConfigurations(pSysConfig);
    if (bRet == FALSE) return false;
    else               return true;
}

void RegCallBackInspectCompleteFunc(CJigInspectProcessor* pProcessor, CallbackInspectComplete* pFunc)
{
    if (pProcessor == NULL)
        return;

    pProcessor->RegCallbackInscompleteFunc(pFunc);
}
