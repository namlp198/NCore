#include "pch.h"
#include "TempInspectProcessorCSharp.h"

CTempInspectProcessor* CreateTempInspectProcessor()
{
    return new CTempInspectProcessor();
}

void DeleteTempInspectProcessor(CTempInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return;

	delete pProcessor;
	pProcessor = NULL;
}

bool Initialize(CTempInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->Initialize();
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStart(CTempInspectProcessor* pProcessor, int nThreadCount, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStart(nThreadCount, nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}

bool InspectStop(CTempInspectProcessor* pProcessor, int nCamIdx)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->InspectStop(nCamIdx);
	if (bRet == FALSE) return false;
	else               return true;
}
