#pragma once

#include "TempInspectProcessor.h"

extern "C"
{
	__declspec(dllexport) CTempInspectProcessor*         CreateTempInspectProcessor();

	__declspec(dllexport) void                           DeleteTempInspectProcessor(CTempInspectProcessor* pProcessor);
												         
	__declspec(dllexport) bool                           Initialize(CTempInspectProcessor* pProcessor);

	__declspec(dllexport) bool                           InspectStart(CTempInspectProcessor* pProcessor, int nThreadCount, int nCamIdx);

	__declspec(dllexport) bool                           InspectStop(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                           ContinuousGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                           SingleGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                           StopGrabHikCam(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                          GetBufferImageHikCam(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                           SetTriggerModeHikCam(CTempInspectProcessor* pProcessor, int nCamIdx, int nMode);

	__declspec(dllexport) bool                           SetTriggerSourceHikCam(CTempInspectProcessor* pProcessor, int nCamIdx, int nSource);
};