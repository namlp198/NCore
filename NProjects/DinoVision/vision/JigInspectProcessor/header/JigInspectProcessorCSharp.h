#pragma once

#include "JigInspectProcessor.h"

extern "C"
{
	__declspec(dllexport)                 CJigInspectProcessor* CreateJigInspectProcessor();

	__declspec(dllexport) void            DeleteJigInspectProcessor(CJigInspectProcessor* pProcessor);

	__declspec(dllexport) bool            Initialize(CJigInspectProcessor* pProcessor);

	__declspec(dllexport) bool            InspectStart(CJigInspectProcessor* pProcessor, int nThreadCount, int nCamIdx);

	__declspec(dllexport) bool            SingleGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            StartGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            StopGrabDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            ConnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            DisconnectDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*           GetBufferDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*           GetResultBufferImageDinoCam(CJigInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool            GetInspectionResult(CJigInspectProcessor* pProcessor, int nCamIdx, CJigInspectResults* pJigInspResult);

	__declspec(dllexport) void			  RegCallBackInspectCompleteFunc(CJigInspectProcessor* pProcessor, CallbackInspectComplete* pFunc);
	/**********************************
    - Register Inspection Complete CallBack
    - Parameter : Instance Pointer, CallBack Func Pointer
    **********************************/
};