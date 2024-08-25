#pragma once

#include <iostream>
#include <vector>

#include "ReadCodeBaslerCam.h"
#include "ReadCodeCore.h"

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
typedef void _stdcall CallbackAlarm(char* strAlarmMessage);
typedef void _stdcall CallbackInspectComplete(BOOL bSetting);
typedef void _stdcall CallbackLocatorTrained(BOOL bSetting);

class AFX_EXT_CLASS CReadCodeProcessor : public IReadCodeBaslerCamToParent,
	                                     public IReadCodeCoreToParent
{
public:
	CReadCodeProcessor();
	~CReadCodeProcessor();

public:
	BOOL         Initialize();
	BOOL         Destroy();
	CString      GetCurrentPathApp();

	virtual      CReadCodeRecipe*           GetRecipeControl() { return m_pReadCodeRecipe; }
	virtual      CReadCodeSystemSetting*    GetSystemSettingControl() { return m_pReadCodeSystemSetting; }
	virtual      CReadCodeCameraSetting*    GetCameraSettingControl(int nCamIdx) { return m_pReadCodeCameraSetting[nCamIdx]; };

public:
	BOOL                       InspectStart(int nThreadCount, BOOL bSimulator);
	BOOL                       InspectStop();
	BOOL                       ProcessFrame(int nCamIdx, LPBYTE pBuff);
	BOOL                       SetTriggerMode(int nCamIdx, int nMode);
	BOOL                       SetTriggerSource(int nCamIdx, int nSource);
	BOOL                       SetExposureTime(int nCamIdx, double dExpTime);
	BOOL                       SetAnalogGain(int nCamIdx, double dGain);
	BOOL                       SaveImage(int nCamIdx);


	/*static void                ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx, int nFrameIdx, LPVOID param);
	void                       ReceivedImageCallbackEx(int nGrabberIdx, int nFrameIdx, LPVOID pBuffer);*/

public:
	BOOL                       LoadSystemSettings(CReadCodeSystemSetting* pSystemSetting);
	BOOL                       LoadRecipe(CReadCodeRecipe* pRecipe);
	BOOL                       LoadCameraSettings(CReadCodeCameraSetting* pCameraSetting);
	BOOL                       SaveSystemSetting(CReadCodeSystemSetting* pSystemSetting);
	BOOL                       SaveRecipe(CReadCodeRecipe* pRecipe);
	BOOL                       SaveCameraSettings(CReadCodeCameraSetting* pCameraSetting, int nCamIdx);
				               
	BOOL                       ReloadSystemSetting();
	BOOL                       ReloadRecipe();

public:
	void                       RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc);
	void                       RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                       RegCallbackAlarm(CallbackAlarm* pFunc);
	void                       RegCallbackLocatorTrainedFunc(CallbackLocatorTrained* pFunc);
	virtual void		       InspectComplete(BOOL bSetting);
	virtual void               LocatorTrained(BOOL bSetting);

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

public:
	LPBYTE                            GetResultBuffer(int nBuff, int nFrame);
	virtual BOOL                      SetResultBuffer(int nBuff, int nFrame, BYTE* buff);
	BOOL                              GetInspectionResult(int nCoreIdx, CReadCodeResult* pReadCodeInspRes);
	virtual CReadCodeResult*          GetReadCodeResultControl(int nCoreIdx) { return m_pReadCodeResult[nCoreIdx]; }
	virtual CReadCodeStatus*          GetReadCodeStatusControl(int nCoreIdx) { return m_pReadCodeStatus[nCoreIdx]; }


	CReadCodeBaslerCam*               GetBaslerCamControl() { return m_pReadCodeBaslerCam; }
	LPBYTE                            GetImageBufferBaslerCam(int nCamIdx);

	BOOL                              LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath);
	BOOL                              LocatorTool_Train(int nCamIdx);
	BOOL                              LocatorToolSimulator_Train(int nSimuBuff, int nFrame);
	virtual LPBYTE                    GetSimulatorBuffer(int nBuff, int nFrame);

private:
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateResultBuffer();
	BOOL                              CreateSimulatorBuffer();

private:

	CallbackInspectComplete*                   m_pCallbackInsCompleteFunc;
								               
	CallbackLogFunc*                           m_pCallbackLogFunc;
								               
	CallbackAlarm*                             m_pCallbackAlarm;

	CallbackLocatorTrained*                    m_pCallbackLocatorTrainedFunc;
								               
	// UI						                            
	CLogView*                                  m_pLogView;

	// Core
	CReadCodeCore*                             m_pReadCodeCore[MAX_CAMERA_INSPECT_COUNT];
								               
	// System Setting			                   
	CReadCodeSystemSetting*                    m_pReadCodeSystemSetting;
								               
	// Recipe					               
	CReadCodeRecipe*                           m_pReadCodeRecipe;

	// Camera Setting
	CReadCodeCameraSetting*                    m_pReadCodeCameraSetting[MAX_CAMERA_INSPECT_COUNT];
								               
	// Result					               
	CReadCodeResult*                           m_pReadCodeResult[MAX_CAMERA_INSPECT_COUNT];

	// Status
	CReadCodeStatus*                           m_pReadCodeStatus[MAX_CAMERA_INSPECT_COUNT];
								               
	// Basler cam				               
	CReadCodeBaslerCam*                        m_pReadCodeBaslerCam;
	
	// Buffer Image
	CSharedMemoryBuffer*                       m_pSimulatorBuffer[MAX_CAMERA_INSPECT_COUNT];

	// Result Buffer			               
	CSharedMemoryBuffer*                       m_pResultBuffer[MAX_CAMERA_INSPECT_COUNT];
								               
	// Result						                        
	CCriticalSection                           m_csInspResult[MAX_CAMERA_INSPECT_COUNT];

	// system settings file path	           
	CString                                    m_csSysSettingsPath;
	CString                                    m_csRecipePath;
	CString                                    m_csCam1SettingPath;

	cv::Mat                                    m_matBGR;
};