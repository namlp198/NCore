#pragma once

#include "NVisionInspectProcessor.h"

extern "C"
{
	/************************** Declare AND Delete **************************/

	__declspec(dllexport) CNVisionInspectProcessor*     CreateNVisionInspectProcessor();

	__declspec(dllexport) void                          DeleteNVisionInspectProcessor(CNVisionInspectProcessor* pProcessor);

	__declspec(dllexport) bool                          Initialize(CNVisionInspectProcessor* pProcessor);

	/************************** Hik Cam **************************/

	__declspec(dllexport) bool                          ContinuousGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          SingleGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          StopGrabHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) BYTE*                         GetImageBufferHikCam(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          SetTriggerMode(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nMode);
												        
	__declspec(dllexport) bool                          SetTriggerSource(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nSource);
												        
	__declspec(dllexport) bool                          SetExposureTime(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dExpTime);
												        
	__declspec(dllexport) bool                          SetAnalogGain(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dGain);
												        
	__declspec(dllexport) bool                          SaveImage(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          LocatorTool_Train(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          LocatorToolSimulator_Train(CNVisionInspectProcessor* pProcessor, int nSimuBuff, int nFrame);

	/************************** Operation **************************/

	__declspec(dllexport) bool                          InspectStart(CNVisionInspectProcessor* pProcessor, int nThreadCount, BOOL isSimulator);
												        
	__declspec(dllexport) bool                          InspectStop(CNVisionInspectProcessor* pProcessor);
												        
	__declspec(dllexport) BYTE*                         GetResultBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame);
												        
	__declspec(dllexport) bool                          GetInspectionResult(CNVisionInspectProcessor* pProcessor, int nCoreIdx, CNVisionInspectResult* pReadCodepResult);
												        
	__declspec(dllexport) bool                          LoadSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath);
												        
	__declspec(dllexport) BYTE*                         GetSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame);

	/************************** Callback **************************/

	__declspec(dllexport) void			                RegCallBackInspectCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackInspectComplete* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackLogFunc(CNVisionInspectProcessor* pProcessor, CallbackLogFunc* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackAlarmFunc(CNVisionInspectProcessor* pProcessor, CallbackAlarmFunc* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackLocatorTrainCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackLocatorTrainComplete* pFunc);

	/************************** Load Setting and Recipe **************************/

	__declspec(dllexport) bool                          LoadSystemSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                          LoadCameraSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectCameraSetting* pCamSetting, int nCamIdx);
												        
	__declspec(dllexport) bool                          LoadRecipe(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe* pRecipe);
												        
	__declspec(dllexport) bool                          ReloadSystenSettings(CNVisionInspectProcessor* pProcessor);
												        
	__declspec(dllexport) bool                          ReloadRecipe(CNVisionInspectProcessor* pProcessor);

	/************************** Save Setting and Recipe **************************/

	__declspec(dllexport) bool                          SaveSystemSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                          SaveCameraSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspectCameraSetting* pCamSetting, int nCamIdx);
												        
	__declspec(dllexport) bool                          SaveRecipe(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe* pRecipe);
};