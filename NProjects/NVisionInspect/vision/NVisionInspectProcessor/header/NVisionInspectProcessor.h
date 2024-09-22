#pragma once
#include <iostream>
#include <vector>

#include "NVisionInspect_HikCam.h"
#include "NVisionInspectCore.h"

#include "NVisionInspect_BaslerCam.h"

#include "SharedMemoryBuffer.h"
#include "LogView.h"
#include "Config.h"

#include "rapidxml.hpp"
#include "RapidXMLSTD.hpp"
#include "rapidxml_utils.hpp"
#include "rapidxml_print.hpp"

#define TEST_NO_CAMERA
#undef TEST_NO_CAMERA

typedef void _stdcall CallbackLogFunc(char* strLogMsg);
typedef void _stdcall CallbackAlarmFunc(char* strAlarmMessage);
typedef void _stdcall CallbackInspectComplete(int nCamIdx, BOOL bSetting);
typedef void _stdcall CallbackLocatorTrainComplete(int nCamIdx);
typedef void _stdcall CallbackInspectComplete_FakeCam(emInspectTool inspTool);

class AFX_EXT_CLASS CNVisionInspectProcessor : public INVisionInspectHikCamToParent,
	                                           public INVisionInspectBaslerCamToParent,
	                                           public INVisionInspectCoreToParent
{
public:
	CNVisionInspectProcessor();
	~CNVisionInspectProcessor();

public:
	BOOL         Initialize();
	BOOL         Destroy();
	CString      GetCurrentPathApp();

	virtual      CNVisionInspectRecipe*                GetRecipeControl() { return m_pNVisionInspectRecipe; }
	virtual      CNVisionInspectRecipe_FakeCam*        GetRecipe_FakeCamControl() { return m_pNVisionInspectRecipe_FakeCam; }
	virtual      CNVisionInspectSystemSetting*         GetSystemSettingControl() { return m_pNVisionInspectSystemSetting; }
	virtual      CNVisionInspectCameraSetting*         GetCameraSettingControl(int nCamIdx) { return m_pNVisionInspectCameraSetting[nCamIdx]; };
	virtual      CNVisionInspect_FakeCameraSetting*    GetFakeCameraSettingControl() { return m_pNVisionInspect_FakeCamSetting; };
	virtual      CNVisionInspectResult*                GetResultControl() { return m_pNVisionInspectResult; }
	virtual      CNVisionInspectResult_FakeCam*        GetResult_FakeCamControl() { return m_pNVisionInspectResult_FakeCam; }
	virtual      CNVisionInspectStatus*                GetStatusControl(int nCamIdx) { return m_pNVisionInspectStatus[nCamIdx]; }
	             CNVisionInspect_HikCam*               GetHikCamControl() { return m_pNVisionInspectHikCam; }
				 CNVisionInspect_BaslerCam*            GetBaslerCamControl() { return m_pNVisionInspectBaslerCam; }
				 std::vector<int>                      GetVecCameras() { return m_vecCameras; }

public:
	BOOL                       InspectStart(int nThreadCount, int nCamCount);
	BOOL                       InspectStop(int nCamCount);
	BOOL                       Inspect_Reality(emCameraBrand camBrand, int nCamIdx, LPBYTE pBuff);
	BOOL                       Inspect_Simulator(emCameraBrand camBrand, int nCamIdx);
	BOOL                       SetTriggerMode(int nCamIdx, int nMode);
	BOOL                       SetTriggerSource(int nCamIdx, int nSource);
	BOOL                       SetExposureTime(int nCamIdx, double dExpTime);
	BOOL                       SetAnalogGain(int nCamIdx, double dGain);
	BOOL                       SaveImage(int nCamIdx);


	static void                ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nFrameIdx, emCameraBrand camBrand, LPVOID param);
	void                       ReceivedImageCallbackEx(int nGrabberIdx, int nFrameIdx, emCameraBrand camBrand, LPVOID pBuffer);

public:
	BOOL                       LoadSystemSettings(CNVisionInspectSystemSetting* pSystemSetting);
	BOOL                       LoadRecipe(int nCamCount, CNVisionInspectRecipe* pRecipe);
	BOOL                       LoadRecipe_FakeCam(CNVisionInspectRecipe_FakeCam* pRecipeFakeCam);
	BOOL                       LoadCameraSettings(CNVisionInspectCameraSetting* pCameraSetting);
	BOOL                       LoadFakeCameraSettings(CNVisionInspect_FakeCameraSetting* pFakeCameraSetting);

	BOOL                       SaveSystemSetting(CNVisionInspectSystemSetting* pSystemSetting);
	BOOL                       SaveRecipe(int nCamIdx, CNVisionInspectRecipe* pRecipe);
	BOOL                       SaveRecipe_FakeCam(CNVisionInspectRecipe_FakeCam* pRecipeFakeCam);
	BOOL                       SaveCameraSettings(int nCamIdx, CNVisionInspectCameraSetting* pCameraSetting);
	BOOL                       SaveFakeCameraSettings(CNVisionInspect_FakeCameraSetting* pFakeCameraSetting);

public:
	void                       RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc);
	void                       RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                       RegCallbackAlarmFunc(CallbackAlarmFunc* pFunc);
	void                       RegCallbackLocatorTrainCompleteFunc(CallbackLocatorTrainComplete* pFunc);
	void                       RegCallbackInspComplete_FakeCamFunc(CallbackInspectComplete_FakeCam* pFunc);

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

public:
	LPBYTE                            GetResultBuffer(int nBuff, int nFrame);
	virtual BOOL                      SetResultBuffer(int nBuff, int nFrame, BYTE* buff);
	BOOL                              GetInspectionResult(CNVisionInspectResult* pNVisionInspRes);
	BOOL                              GetInspectToolResult_FakeCam(CNVisionInspectResult_FakeCam* pInspRes_FakeCam);
	LPBYTE                            GetImageBufferHikCam(int nCamIdx);
	LPBYTE                            GetImageBufferBaslerCam(int nCamIdx);

	BOOL                              LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath);
	BOOL                              LocatorTool_Train(int nCamIdx);
	BOOL                              LocatorToolSimulator_Train(int nSimuBuff, int nFrame);
	BOOL                              SelectROI(int nCamIdx, int nROIIdx, int nFrom); /*0: From Image, 1: From Camera*/
	virtual LPBYTE                    GetSimulatorBuffer(int nBuff, int nFrame);

	void                              CallInspectTool(emInspectTool inspTool);

private:
	virtual void				      AlarmMessage(CString strAlarmMessage);
	virtual void				      SystemMessage(const TCHAR* lpstrFormat, ...);
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateResultBuffer();
	BOOL                              CreateSimulatorBuffer();
	virtual void		              InspectComplete(int nCamIdx, BOOL bSetting);
	virtual void                      LocatorTrainComplete(int nCamIdx);
	virtual void                      InspectComplete_FakeCam(emInspectTool eInspTool);

private:

	CallbackInspectComplete*                    m_pCallbackInsCompleteFunc;

	CallbackLogFunc*                            m_pCallbackLogFunc;

	CallbackAlarmFunc*                          m_pCallbackAlarmFunc;

	CallbackLocatorTrainComplete*               m_pCallbackLocatorTrainCompleteFunc;

	CallbackInspectComplete_FakeCam*            m_pCallbackInspComplete_FakeCamFunc;

	// UI						                            
	CLogView*                                   m_pLogView;

	// System Setting			                   
	CNVisionInspectSystemSetting*               m_pNVisionInspectSystemSetting;

	// Recipe					               
	CNVisionInspectRecipe*                      m_pNVisionInspectRecipe;
	CNVisionInspectRecipe_FakeCam*              m_pNVisionInspectRecipe_FakeCam;

	// Result					               
	CNVisionInspectResult*                      m_pNVisionInspectResult;
	CNVisionInspectResult_FakeCam*              m_pNVisionInspectResult_FakeCam;

	// Hik cam				               
	CNVisionInspect_HikCam*                     m_pNVisionInspectHikCam;

	// Basler cam				               
	CNVisionInspect_BaslerCam*                  m_pNVisionInspectBaslerCam;

	// Camera Setting
	CNVisionInspectCameraSetting*               m_pNVisionInspectCameraSetting[MAX_CAMERA_INSPECT_COUNT];
	CNVisionInspect_FakeCameraSetting*          m_pNVisionInspect_FakeCamSetting;

	// Status
	CNVisionInspectStatus*                      m_pNVisionInspectStatus[MAX_CAMERA_INSPECT_COUNT];

	// Core
	CNVisionInspectCore*                        m_pNVisionInspectCore[MAX_CAMERA_INSPECT_COUNT];

	// Buffer Image
	CSharedMemoryBuffer*                        m_pSimulatorBuffer[MAX_CAMERA_INSPECT_COUNT];

	// Result Buffer			               
	CSharedMemoryBuffer*                        m_pResultBuffer[MAX_CAMERA_INSPECT_COUNT];

	// Result						                        
	CCriticalSection                            m_csInspResult;
	CCriticalSection                            m_csInspToolResult_FakeCam;

	// system settings file path	           
	CString                                     m_csSysSettingsPath;
	CString                                     m_csRecipePath;
	CString                                     m_csRecipeFakeCamPath;
	CString                                     m_csCamSettingPath;
	CString                                     m_csFakeCamSettingPath;
											    
	cv::Mat                                     m_matBGR;
	cv::Mat                                     m_matRGB;

	std::vector<int>                            m_vecCameras; /*pos 0: number of Hik Cam, pos 1: number of Basler Cam*/
};
