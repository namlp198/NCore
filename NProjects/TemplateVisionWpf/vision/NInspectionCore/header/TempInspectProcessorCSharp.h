#pragma once

#include "TempInspectProcessor.h"

extern "C"
{
	__declspec(dllexport) CTempInspectProcessor*         CreateTempInspectProcessor();

	__declspec(dllexport) void                           DeleteTempInspectProcessor(CTempInspectProcessor* pProcessor);
												         
	__declspec(dllexport) bool                           Initialize(CTempInspectProcessor* pProcessor);

	__declspec(dllexport) bool                           TestRun(CTempInspectProcessor* pProcessor);
};