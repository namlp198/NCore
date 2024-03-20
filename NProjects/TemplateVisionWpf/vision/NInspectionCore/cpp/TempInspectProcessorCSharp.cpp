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

bool TestRun(CTempInspectProcessor* pProcessor)
{
	if (pProcessor == NULL)
		return false;

	BOOL bRet = pProcessor->TestRun();
	if (bRet == FALSE) return false;
	else               return true;
}
