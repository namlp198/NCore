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

	__declspec(dllexport) bool                           TrainLocator_TemplateMatching(CTempInspectProcessor* pProcessor, int nCamIdx, CRectForTrainLocTool* rectForTrainLoc);

	__declspec(dllexport) bool                           CountPixelAlgorithm_Train(CTempInspectProcessor* pProcessor,int nCamIdx, CParamCntPxlAlgorithm* pParamCntPxlTrain);

	__declspec(dllexport) bool                           CalculateAreaAlgorithm_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CParamCalAreaAlgorithm* pParamTrainCalArea);

	__declspec(dllexport) BYTE*                          GetTemplateImage(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                          GetResultROIBuffer_Train(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                          GetResultImageBuffer(CTempInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                           GetDataTrained_TemplateMatching(CTempInspectProcessor* pProcessor, int nCamIdx, CLocatorToolResult* dataTrained);

	__declspec(dllexport) bool                           GetResultCntPxl_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CAlgorithmsCountPixelResult* pCntPxlTrainRes);

	__declspec(dllexport) bool                           GetResultCalArea_Train(CTempInspectProcessor* pProcessor, int nCamIdx, CAlgorithmsCalculateAreaResult* pCalAreaTrainRes);

};