#pragma once

#include "ReadCodeProcessor.h"

extern "C"
{
	/************************** Declare AND Delete **************************/

	__declspec(dllexport) CReadCodeProcessor*     CreateReadCodeProcessor();

	__declspec(dllexport) void                    DeleteReadCodeProcessor(CReadCodeProcessor* pProcessor);

	__declspec(dllexport) bool                    Initialize(CReadCodeProcessor* pProcessor);

	/************************** Basler Cam **************************/
	
	__declspec(dllexport) bool                    ContinuousGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                    SingleGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                    StopGrabBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) BYTE*                   GetImageBufferBaslerCam(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                    SetTriggerMode(CReadCodeProcessor* pProcessor, int nCamIdx, int nMode);

	__declspec(dllexport) bool                    SetTriggerSource(CReadCodeProcessor* pProcessor, int nCamIdx, int nSource);

	__declspec(dllexport) bool                    SetExposureTime(CReadCodeProcessor* pProcessor, int nCamIdx, double dExpTime);

	__declspec(dllexport) bool                    SetAnalogGain(CReadCodeProcessor* pProcessor, int nCamIdx, double dGain);

	__declspec(dllexport) bool                    SaveImage(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                    LocatorTool_Train(CReadCodeProcessor* pProcessor, int nCamIdx);

	__declspec(dllexport) bool                    LocatorToolSimulator_Train(CReadCodeProcessor* pProcessor, int nSimuBuff, int nFrame);

	/************************** Operation **************************/

	__declspec(dllexport) bool                    InspectStart(CReadCodeProcessor* pProcessor, int nThreadCount, BOOL isSimulator);

	__declspec(dllexport) bool                    InspectStop(CReadCodeProcessor* pProcessor);

	__declspec(dllexport) BYTE*                   GetResultBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame);

	__declspec(dllexport) bool                    GetInspectionResult(CReadCodeProcessor* pProcessor, int nCoreIdx, CReadCodeResult* pReadCodepResult);

	__declspec(dllexport) bool                    LoadSimulatorBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame, char* pFilePath);

	__declspec(dllexport) BYTE*                   GetSimulatorBuffer(CReadCodeProcessor* pProcessor, int nBuff, int nFrame);

	/************************** Callback **************************/

	__declspec(dllexport) void			          RegCallBackInspectCompleteFunc(CReadCodeProcessor* pProcessor, CallbackInspectComplete* pFunc);

	__declspec(dllexport) void			          RegCallbackLogFunc(CReadCodeProcessor* pProcessor, CallbackLogFunc* pFunc);

	__declspec(dllexport) void			          RegCallbackAlarm(CReadCodeProcessor* pProcessor, CallbackAlarm* pFunc);

	__declspec(dllexport) void			          RegCallbackLocatorTrainedFunc(CReadCodeProcessor* pProcessor, CallbackLocatorTrained* pFunc);

	/************************** Load Setting and Recipe **************************/

	__declspec(dllexport) bool                    LoadSystemSettings(CReadCodeProcessor* pProcessor, CReadCodeSystemSetting* pSysSetting);

	__declspec(dllexport) bool                    LoadRecipe(CReadCodeProcessor* pProcessor, CReadCodeRecipe* pRecipe);

	__declspec(dllexport) bool                    ReloadSystenSettings(CReadCodeProcessor* pProcessor);

	__declspec(dllexport) bool                    ReloadRecipe(CReadCodeProcessor* pProcessor);

	/************************** Save Setting and Recipe **************************/

	__declspec(dllexport) bool                    SaveSystemSetting(CReadCodeProcessor* pProcessor, CReadCodeSystemSetting* pSysSetting);

	__declspec(dllexport) bool                    SaveRecipe(CReadCodeProcessor* pProcessor, CReadCodeRecipe* pRecipe);
};