#pragma once

#include "TempInspectProcessor.h"

extern "C"
{
	__declspec(dllexport) CTempInspectProcessor*         CreateTempInspectProcessor();

	__declspec(dllexport) void                           DeleteTempInspectProcessor(CTempInspectProcessor* pProcessor);
												         
	__declspec(dllexport) bool                           Initialize(CTempInspectProcessor* pProcessor);

	__declspec(dllexport) bool                           InspectStart(CTempInspectProcessor* pProcessor, int nThreadCount, int nCamIdx);

	__declspec(dllexport) bool                           InspectStop(CTempInspectProcessor* pProcessor, int nCamIdx);
};