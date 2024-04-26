#pragma once

#include "JigInspectProcessor.h"

extern "C"
{
	/******************** Initialize ********************/
	__declspec(dllexport)                 CJigInspectProcessor* CreateJigInspectProcessor();

	__declspec(dllexport) void            DeleteJigInspectProcessor(CJigInspectProcessor* pProcessor);

	__declspec(dllexport) bool            Initialize(CJigInspectProcessor* pProcessor);

	/******************** Inspect ********************/
	__declspec(dllexport) bool            InspectStart(CJigInspectProcessor* pProcessor, int nThreadCount, int nCamIdx);

	__declspec(dllexport) bool            GrabImageForLocatorTool(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            LocatorTrain(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectRecipe* pRecipe);

	/******************** Connect and grab ********************/
	__declspec(dllexport) bool            SingleGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            StartGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            StopGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            ConnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            DisconnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	/******************** Get result and buffer ********************/
	__declspec(dllexport) BYTE*           GetBufferDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*           GetResultBufferImageDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            GetInspectionResult(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectResults* pJigInspResult);

	/******************** Load settings ********************/
	__declspec(dllexport) bool            LoadSysConfigurations(CJigInspectProcessor* pProcessor, CJigInspectSystemConfig* pSysConfig);

	__declspec(dllexport) bool            LoadCamConfigurations(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectCameraConfig* pCamConfig);

	__declspec(dllexport) bool            LoadRecipe(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectRecipe* pRecipe);

	/******************** Save settings ********************/
	__declspec(dllexport) bool            SaveSysConfigurations(CJigInspectProcessor* pProcessor, CJigInspectSystemConfig* pSysConfig);

	__declspec(dllexport) bool            SaveCamConfigurations(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectCameraConfig* pCamConfig);

	__declspec(dllexport) bool            SaveRecipe(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectRecipe* pRecipeConfig);

	/******************** Callback ********************/
	__declspec(dllexport) void			  RegCallBackInspectCompleteFunc(CJigInspectProcessor* pProcessor, CallbackInspectComplete* pFunc);
	/**********************************
    - Register Inspection Complete CallBack
    - Parameter : Instance Pointer, CallBack Func Pointer
    **********************************/
};