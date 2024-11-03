#pragma once

#include "NVisionInspectProcessor.h"

extern "C"
{
	/************************** Declare AND Delete **************************/

	__declspec(dllexport) CNVisionInspectProcessor*     CreateNVisionInspectProcessor();

	__declspec(dllexport) void                          DeleteNVisionInspectProcessor(CNVisionInspectProcessor* pProcessor);

	__declspec(dllexport) bool                          Initialize(CNVisionInspectProcessor* pProcessor);

	/************************** Camera **************************/

	__declspec(dllexport) bool                          ContinuousGrab(CNVisionInspectProcessor* pProcessor,emCameraBrand camBrand, int nCamIdx);
												        
	__declspec(dllexport) bool                          StopGrab(CNVisionInspectProcessor* pProcessor,emCameraBrand camBrand, int nCamIdx);
												        
	__declspec(dllexport) BYTE*                         GetImageBuffer(CNVisionInspectProcessor* pProcessor,emCameraBrand camBrand, int nCamIdx);
												        
	__declspec(dllexport) bool                          SetTriggerMode(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nMode);
												        
	__declspec(dllexport) bool                          SetTriggerSource(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nSource);
												        
	__declspec(dllexport) bool                          SetExposureTime(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dExpTime);
												        
	__declspec(dllexport) bool                          SetAnalogGain(CNVisionInspectProcessor* pProcessor, int nCamIdx, double dGain);
												        
	__declspec(dllexport) bool                          SaveImage(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          LocatorTool_Train(CNVisionInspectProcessor* pProcessor, int nCamIdx);
												        
	__declspec(dllexport) bool                          LocatorToolSimulator_Train(CNVisionInspectProcessor* pProcessor, int nSimuBuff, int nFrame);

	__declspec(dllexport) bool                          LocatorToolFakeCam_Train(CNVisionInspectProcessor* pProcessor, int nFrame);

	__declspec(dllexport) bool                          SelectROI(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nROIIdx, int nFrom, int nROIX, int nROIY, int nROIWidth, int nROIHeight);

	/************************** Operation **************************/

	__declspec(dllexport) bool                          InspectStart(CNVisionInspectProcessor* pProcessor, int nThreadCount, int nCamCount);
												        
	__declspec(dllexport) bool                          InspectStop(CNVisionInspectProcessor* pProcessor, int nCamCount);

	__declspec(dllexport) bool                          Inspect_Simulator(CNVisionInspectProcessor* pProcessor, emCameraBrand camBrand, int nCamIdx);
												        
	__declspec(dllexport) BYTE*                         GetResultBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame);

	__declspec(dllexport) BYTE*                         GetResultBuffer_FakeCam(CNVisionInspectProcessor* pProcessor, int nFrame);
												        
	__declspec(dllexport) bool                          GetInspectionResult(CNVisionInspectProcessor* pProcessor,  CNVisionInspectResult* pNVisionInsppResult);

	__declspec(dllexport) bool                          GetInspectToolResult_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectResult_FakeCam* pNVisionInsppRes_FakeCam);

	__declspec(dllexport) bool                          LoadSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath);

	__declspec(dllexport) bool                          LoadSimulatorBuffer_FakeCam(CNVisionInspectProcessor* pProcessor, int nFrame, char* pFilePath);
												        
	__declspec(dllexport) BYTE*                         GetSimulatorBuffer(CNVisionInspectProcessor* pProcessor, int nBuff, int nFrame);

	__declspec(dllexport) BYTE*                         GetSimulatorBuffer_FakeCam(CNVisionInspectProcessor* pProcessor, int nFrame);

	__declspec(dllexport) void                          CallInspectTool(CNVisionInspectProcessor* pProcessor, emInspectTool inspTool);

	__declspec(dllexport) bool                          HSVTrain(CNVisionInspectProcessor* pProcessor, int nCamIdx, int nFrame, CNVisionInspectRecipe_HSV* pRecipeHSV);

	/************************** Callback **************************/

	__declspec(dllexport) void			                RegCallBackInspectCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackInspectComplete* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackLogFunc(CNVisionInspectProcessor* pProcessor, CallbackLogFunc* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackAlarmFunc(CNVisionInspectProcessor* pProcessor, CallbackAlarmFunc* pFunc);
												        
	__declspec(dllexport) void			                RegCallBackLocatorTrainCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackLocatorTrainComplete* pFunc);

	__declspec(dllexport) void			                RegCallbackInspComplete_FakeCamFunc(CNVisionInspectProcessor* pProcessor, CallbackInspectComplete_FakeCam* pFunc);

	__declspec(dllexport) void                          RegCallbackHSVTrainCompleteFunc(CNVisionInspectProcessor* pProcessor, CallbackHSVTrainComplete* pFunc);

	/************************** Load Setting and Recipe **************************/

	__declspec(dllexport) bool                          LoadSystemSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                          LoadCameraSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspectCameraSetting* pCamSetting, int nCamIdx);

	__declspec(dllexport) bool                          LoadFakeCameraSettings(CNVisionInspectProcessor* pProcessor, CNVisionInspect_FakeCameraSetting* pFakeCamSetting);
												        
	__declspec(dllexport) bool                          LoadRecipe(CNVisionInspectProcessor* pProcessor, int nCamCount, CNVisionInspectRecipe* pRecipe);

	__declspec(dllexport) bool                          LoadRecipe_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe_FakeCam* pFakeCamRecipe);


	/************************** Save Setting and Recipe **************************/

	__declspec(dllexport) bool                          SaveSystemSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                          SaveCameraSetting(CNVisionInspectProcessor* pProcessor, int nCamIdx, CNVisionInspectCameraSetting* pCamSetting);

	__declspec(dllexport) bool                          SaveFakeCameraSetting(CNVisionInspectProcessor* pProcessor, CNVisionInspect_FakeCameraSetting* pFakeCamSetting);
												        
	__declspec(dllexport) bool                          SaveRecipe(CNVisionInspectProcessor* pProcessor, int nCamIdx, CNVisionInspectRecipe* pRecipe);

	__declspec(dllexport) bool                          SaveRecipe_FakeCam(CNVisionInspectProcessor* pProcessor, CNVisionInspectRecipe_FakeCam* pRecipeFakeCam);
};