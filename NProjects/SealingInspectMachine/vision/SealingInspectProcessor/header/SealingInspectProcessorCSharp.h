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
	__declspec(dllexport) BYTE*                     GetBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame);
								                    
	__declspec(dllexport) BYTE*                     GetBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame);

	__declspec(dllexport) BYTE*                     GetResultBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame);
								                    
	__declspec(dllexport) BYTE*                     GetResultBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame);
											        
	__declspec(dllexport) bool                      LoadImageBuffer_SIDE(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath);

	__declspec(dllexport) bool                      LoadImageBuffer_TOP(CSealingInspectProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath);

	__declspec(dllexport) bool                      LoadAllImageBuffer(CSealingInspectProcessor* pProcessor, char* strDirPath, char* strImageType);
											        
	__declspec(dllexport) bool                      ClearBufferImage_SIDE(CSealingInspectProcessor* pProcessor, int nBuff);

	__declspec(dllexport) bool                      ClearBufferImage_TOP(CSealingInspectProcessor* pProcessor, int nBuff);

	__declspec(dllexport) bool                      Inspect_TopCam_Simulation(CSealingInspectProcessor* pProcessor,int nCoreIdx, int nCamIdx, int nFrame);

	__declspec(dllexport) bool                      Inspect_SideCam_Simulation(CSealingInspectProcessor* pProcessor, int nCoreIdx, int nCamIdx, int nFrame);
#pragma endregion

	/************************** Hik Cam **************************/
	__declspec(dllexport) bool                      ContinuousGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      SoftwareTriggerHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      StopGrabHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                     GetBufferImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                      SetTriggerModeHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nMode);

	__declspec(dllexport) bool                      SetTriggerSourceHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, int nSource);

	__declspec(dllexport) bool                      SaveImageHikCam(CSealingInspectProcessor* pProcessor, int nCamIdx, char* strImageSavePath);

	/************************** Get Result **************************/
	__declspec(dllexport) bool                      InspectStart(CSealingInspectProcessor* pProcessor, int nThreadCount, emInspectCavity nInspCavity, BOOL isSimulator);

	__declspec(dllexport) bool                      InspectStop(CSealingInspectProcessor* pProcessor,emInspectCavity nInspCavity);

	__declspec(dllexport) bool                      GetInspectionResult(CSealingInspectProcessor* pProcessor, int nCoreIdx, CSealingInspectResult* pSealingInspResult);

	__declspec(dllexport) bool                      GetInspectStatus(CSealingInspectProcessor* pProcessor, int nInspStatusIdx, CSealingInspectStatus* pSealingInspStatus);

	__declspec(dllexport) bool                      SetSealingInspectSimulationIO(CSealingInspectProcessor* pProcessor, int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO);

	__declspec(dllexport) bool                      SetCavityInfo(CSealingInspectProcessor* pProcessor, char* strLoadingTime);

	__declspec(dllexport) bool                      SetProcessStatus1(CSealingInspectProcessor* pProcessor, BOOL bProcessStatus);

	__declspec(dllexport) bool                      SetProcessStatus2(CSealingInspectProcessor* pProcessor, BOOL bProcessStatus);

	__declspec(dllexport) bool                      SetGrabFrameSideCam(CSealingInspectProcessor* pProcessor, int nCoreIdx, BOOL bTrigger);

	/************************** Load Setting and Recipe **************************/
	__declspec(dllexport) bool                      LoadSystemSettings(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                      LoadRecipe(CSealingInspectProcessor* pProcessor, CSealingInspectRecipe* pRecipe);

	__declspec(dllexport) bool                      ReloadSystenSettings(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      ReloadRecipe(CSealingInspectProcessor* pProcessor);

	/************************** Save Setting and Recipe **************************/
	__declspec(dllexport) bool                      SaveSystemSetting(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting);

	__declspec(dllexport) bool                      SaveLightSetting(CSealingInspectProcessor* pProcessor, CSealingInspectSystemSetting* pSysSetting, int nLightIdx);

	__declspec(dllexport) bool                      SaveRecipe(CSealingInspectProcessor* pProcessor, CSealingInspectRecipe* pRecipe, char* sPosCam, int nFrameIdx);

	/************************** Test Inspect **************************/
	__declspec(dllexport) bool                      TestInspectCavity1(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      TestInspectCavity2(CSealingInspectProcessor* pProcessor);

	__declspec(dllexport) bool                      TestTCPSocket(CSealingInspectProcessor* pProcessor);

	/************************** Callback **************************/
	__declspec(dllexport) void                      RegCallbackLogFunc(CSealingInspectProcessor* pProcessor, CallbackLogFunc* pFunc);

	__declspec(dllexport) void			            RegCallBackInspectCavity1CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectCavity1Complete* pFunc);

	__declspec(dllexport) void			            RegCallBackInspectCavity2CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectCavity2Complete* pFunc);

	__declspec(dllexport) void			            RegCallBackInspectTopCam1CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectTopCam1Complete* pFunc);

	__declspec(dllexport) void			            RegCallBackInspectTopCam2CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackInspectTopCam2Complete* pFunc);

	__declspec(dllexport) void			            RegCallBackGrabFrameSideCam1CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackGrabFrameSideCam1Complete* pFunc);

	__declspec(dllexport) void			            RegCallBackGrabFrameSideCam2CompleteFunc(CSealingInspectProcessor* pProcessor, CallbackGrabFrameSideCam2Complete* pFunc);
};