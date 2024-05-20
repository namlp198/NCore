#pragma once

#include "SealingInspectProcessor.h"

extern "C"
{
	/************************** Declare AND Delete **************************/
	__declspec(dllexport) CSealingInspectProcessor* CreateSealingInspectProcessor();

	__declspec(dllexport) void                      DeleteSealingInspectProcessor(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      Initialize(CSealingInspectProcessor* pProcessor);

	/************************** Simulator **************************/
#pragma region Offline simulation
	__declspec(dllexport) BYTE*                     GetBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nY);
								                    
	__declspec(dllexport) BYTE*                     GetBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nY);
											        
	__declspec(dllexport) bool                      LoadImageBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool                      LoadImageBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, char* pFilePath);

	__declspec(dllexport) bool                      LoadAllImageBuffer(CSealingInspectProcessor* pProcessor, char* strDirPath, char* strImageType);
											        
	__declspec(dllexport) bool                      ClearBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff);

	__declspec(dllexport) bool                      ClearBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff);
#pragma endregion

	/************************** Hik Cam **************************/
	__declspec(dllexport) bool                      ContinuousGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      SingleGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      StopGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                     GetBufferImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      SetTriggerModeHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nMode);

	__declspec(dllexport) bool                      SetTriggerSourceHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nSource);

	/************************** Callback **************************/
	__declspec(dllexport) void                      RegCallbackLogFunc(CSealingInspectProcessor* pProcessor, CallbackLogFunc* pFunc);

	__declspec(dllexport) void			            RegCallBackInspectCompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectComplete* pFunc);
};