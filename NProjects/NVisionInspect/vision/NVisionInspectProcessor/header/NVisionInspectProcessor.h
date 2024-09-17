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

	virtual      CNVisionInspectRecipe*          GetRecipeControl() { return m_pNVisionInspectRecipe; }
	virtual      CNVisionInspectSystemSetting*   GetSystemSettingControl() { return m_pNVisionInspectSystemSetting; }
	virtual      CNVisionInspectCameraSetting*   GetCameraSettingControl(int nCamIdx) { return m_pNVisionInspectCameraSetting[nCamIdx]; };
	virtual      CNVisionInspectResult*          GetResultControl() { return m_pNVisionInspectResult; }
	virtual      CNVisionInspectStatus*          GetStatusControl(int nCamIdx) { return m_pNVisionInspectStatus[nCamIdx]; }
	             CNVisionInspect_HikCam*         GetHikCamControl() { return m_pNVisionInspectHikCam; }
				 std::vector<int>                GetVecCameras() { return m_vecCameras; }

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
	BOOL                       LoadCameraSettings(CNVisionInspectCameraSetting* pCameraSetting);
	BOOL                       SaveSystemSetting(CNVisionInspectSystemSetting* pSystemSetting);
	BOOL                       SaveRecipe(int nCamIdx, CNVisionInspectRecipe* pRecipe);
	BOOL                       SaveCameraSettings(int nCamIdx, CNVisionInspectCameraSetting* pCameraSetting);

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
	virtual void		              InspectComplete(int nCamIdx, BOOL bSetting);
	virtual void                      LocatorTrainComplete(int nCamIdx);

private:

	CallbackInspectComplete*                    m_pCallbackInsCompleteFunc;

	CallbackLogFunc*                            m_pCallbackLogFunc;

	CallbackAlarmFunc*                          m_pCallbackAlarmFunc;

	CallbackLocatorTrainComplete*               m_pCallbackLocatorTrainCompleteFunc;

	// UI						                            
	CLogView*                                   m_pLogView;

	// System Setting			                   
	CNVisionInspectSystemSetting*               m_pNVisionInspectSystemSetting;

	// Recipe					               
	CNVisionInspectRecipe*                      m_pNVisionInspectRecipe;

	// Result					               
	CNVisionInspectResult*                      m_pNVisionInspectResult;

	// Hik cam				               
	CNVisionInspect_HikCam*                     m_pNVisionInspectHikCam;

	// Basler cam				               
	CNVisionInspect_BaslerCam*                  m_pNVisionInspectBaslerCam;

	// Camera Setting
	CNVisionInspectCameraSetting*               m_pNVisionInspectCameraSetting[MAX_CAMERA_INSPECT_COUNT];

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

	// system settings file path	           
	CString                                     m_csSysSettingsPath;
	CString                                     m_csRecipePath;
	CString                                     m_csCamSettingPath;
											    
	cv::Mat                                     m_matBGR;
	cv::Mat                                     m_matRGB;

	std::vector<int>                            m_vecCameras; /*pos 0: number of Hik Cam, pos 1: number of Basler Cam*/
};
