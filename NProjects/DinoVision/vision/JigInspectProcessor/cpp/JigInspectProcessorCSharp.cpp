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

    BOOL bRet = pProcessor->InspectStart(nThreadCount, nCamIdx);
    if (bRet == FALSE) return false;
    else               return true;
}

bool InspectStop(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return false;

    BOOL bRet = pProcessor->InspectStop(nCamIdx);
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

BYTE* GetBufferDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx)
{
    if (pProcessor == NULL)
        return NULL;

    CJigInspectDinoCam* pDinoCam = pProcessor->GetDinoCamControl();
    if (pDinoCam == NULL)
        return NULL;

    return pDinoCam->GetBufferImage(nCamIdx);
}
