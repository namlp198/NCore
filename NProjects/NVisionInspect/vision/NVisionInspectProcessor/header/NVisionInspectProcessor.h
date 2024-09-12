#pragma once
#include <iostream>
#include <vector>

#include "NVisionInspect_HikCam.h"
#include "NVisionInspectCore.h"

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
typedef void _stdcall CallbackInspectComplete(BOOL bSetting);
typedef void _stdcall CallbackLocatorTrainComplete(int nCamIdx);

class AFX_EXT_CLASS CNVisionInspectProcessor : public INVisionInspectHikCamToParent,
	                                           public INVisionInspectCoreToParent
{ 
public:
	CNVisionInspectProcessor();
	~CNVisionInspectProcessor();

public:
	BOOL         Initialize();
	BOOL         Destroy();
	CString      GetCurrentPathApp();

	virtual      CNVisionInspectRecipe*          GetRecipeControl() { return m_pNVisionInspectRecipe; }
	virtual      CNVisionInspectSystemSetting*   GetSystemSettingControl() { return m_pNVisionInspectSystemSetting; }
	virtual      CNVisionInspectCameraSetting*   GetCameraSettingControl(int nCamIdx) { return m_pNVisionInspectCameraSetting[nCamIdx]; };
	virtual      CNVisionInspectResult*          GetResultControl(int nCoreIdx) { return m_pNVisionInspectResult[nCoreIdx]; }
	virtual      CNVisionInspectStatus*          GetStatusControl(int nCoreIdx) { return m_pNVisionInspectStatus[nCoreIdx]; }

public:
	BOOL                       InspectStart(int nThreadCount, BOOL bSimulator);
	BOOL                       InspectStop();
	BOOL                       Inspect_Reality(int nCamIdx, LPBYTE pBuff);
	BOOL                       SetTriggerMode(int nCamIdx, int nMode);
	BOOL                       SetTriggerSource(int nCamIdx, int nSource);
	BOOL                       SetExposureTime(int nCamIdx, double dExpTime);
	BOOL                       SetAnalogGain(int nCamIdx, double dGain);
	BOOL                       SaveImage(int nCamIdx);


	/*static void              ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nFrameIdx, LPVOID param);
	void                       ReceivedImageCallbackEx(int nGrabberIdx, int nFrameIdx, LPVOID pBuffer);*/

public:
	BOOL                       LoadSystemSettings(CNVisionInspectSystemSetting* pSystemSetting);
	BOOL                       LoadRecipe(CNVisionInspectRecipe* pRecipe);
	BOOL                       LoadCameraSettings(CNVisionInspectCameraSetting* pCameraSetting);
	BOOL                       SaveSystemSetting(CNVisionInspectSystemSetting* pSystemSetting);
	BOOL                       SaveRecipe(CNVisionInspectRecipe* pRecipe);
	BOOL                       SaveCameraSettings(CNVisionInspectCameraSetting* pCameraSetting, int nCamIdx);

	BOOL                       ReloadSystemSetting();
	BOOL                       ReloadRecipe();

public:
	void                       RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc);
	void                       RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                       RegCallbackAlarmFunc(CallbackAlarmFunc* pFunc);
	void                       RegCallbackLocatorTrainCompleteFunc(CallbackLocatorTrainComplete* pFunc);

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

public:
	LPBYTE                            GetResultBuffer(int nBuff, int nFrame);
	virtual BOOL                      SetResultBuffer(int nBuff, int nFrame, BYTE* buff);
	BOOL                              GetInspectionResult(int nCoreIdx, CNVisionInspectResult* pReadCodeInspRes);


	CNVisionInspect_HikCam*           GetHikCamControl() { return m_pNVisionInspectHikCam; }
	LPBYTE                            GetImageBufferHikCam(int nCamIdx);

	BOOL                              LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath);
	BOOL                              LocatorTool_Train(int nCamIdx);
	BOOL                              LocatorToolSimulator_Train(int nSimuBuff, int nFrame);
	BOOL                              SelectROI(int nCamIdx, int nROIIdx, int nFrom); /*0: From Image, 1: From Camera*/
	virtual LPBYTE                    GetSimulatorBuffer(int nBuff, int nFrame);

private:
	virtual void				      AlarmMessage(CString strAlarmMessage);
	virtual void				      SystemMessage(const TCHAR* lpstrFormat, ...);
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateResultBuffer();
	BOOL                              CreateSimulatorBuffer();
	virtual void		              InspectComplete(BOOL bSetting);
	virtual void                      LocatorTrainComplete(int nCamIdx);

private:

	CallbackInspectComplete*                    m_pCallbackInsCompleteFunc;

	CallbackLogFunc*                            m_pCallbackLogFunc;

	CallbackAlarmFunc*                          m_pCallbackAlarmFunc;

	CallbackLocatorTrainComplete*               m_pCallbackLocatorTrainCompleteFunc;

	// UI						                            
	CLogView*                                   m_pLogView;

	// Core
	CNVisionInspectCore*                        m_pNVisionInspectCore[MAX_CAMERA_INSPECT_COUNT];

	// System Setting			                   
	CNVisionInspectSystemSetting*               m_pNVisionInspectSystemSetting;

	// Recipe					               
	CNVisionInspectRecipe*                      m_pNVisionInspectRecipe;

	// Camera Setting
	CNVisionInspectCameraSetting*               m_pNVisionInspectCameraSetting[MAX_CAMERA_INSPECT_COUNT];

	// Result					               
	CNVisionInspectResult*                      m_pNVisionInspectResult[MAX_CAMERA_INSPECT_COUNT];

	// Status
	CNVisionInspectStatus*                      m_pNVisionInspectStatus[MAX_CAMERA_INSPECT_COUNT];

	// Basler cam				               
	CNVisionInspect_HikCam*                     m_pNVisionInspectHikCam;

	// Buffer Image
	CSharedMemoryBuffer*                        m_pSimulatorBuffer[MAX_CAMERA_INSPECT_COUNT];

	// Result Buffer			               
	CSharedMemoryBuffer*                        m_pResultBuffer[MAX_CAMERA_INSPECT_COUNT];

	// Result						                        
	CCriticalSection                            m_csInspResult[MAX_CAMERA_INSPECT_COUNT];

	// system settings file path	           
	CString                                     m_csSysSettingsPath;
	CString                                     m_csRecipePath;
	CString                                     m_csCam1SettingPath;
											    
	cv::Mat                                     m_matBGR;
	cv::Mat                                     m_matRGB;
};