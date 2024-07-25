#pragma once

#include <iostream>
#include <vector>

#include "ReadCodeRecipe.h"
#include "ReadCodeDefine.h"
#include "ReadCodeBaslerCam.h"
#include "ReadCodeResult.h"
#include "ReadCodeSystemSetting.h"

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

class AFX_EXT_CLASS CReadCodeProcessor : public IReadCodeBaslerCamToParent
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

public:
	BOOL                       InspectStart(BOOL bSimulator);
	BOOL                       InspectStop();

public:
	BOOL         LoadSystemSettings(CReadCodeSystemSetting* pSystemSetting);
	BOOL         LoadRecipe(CReadCodeRecipe* pRecipe);
	BOOL         SaveSystemSetting(CReadCodeSystemSetting* pSystemSetting);
	BOOL         SaveRecipe(CReadCodeRecipe* pRecipe);

	BOOL         ReloadSystemSetting();
	BOOL         ReloadRecipe();

public:
	void                       RegCallbackInsCompleteFunc(CallbackInspectComplete* pFunc);
	void                       RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                       RegCallbackAlarm(CallbackAlarm* pFunc);
	virtual void		       InspectComplete(BOOL bSetting);

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

public:
	LPBYTE                            GetResultBuffer(int nBuff, int nFrame);
	virtual BOOL                      SetResultBuffer(int nBuff, int nFrame, BYTE* buff);
	BOOL                              GetInspectionResult(int nCoreIdx, CReadCodeResult* pReadCodeInspRes);
	virtual CReadCodeResult*          GetInspectionResultControl(int nCoreIdx) { return m_pReadCodeResult[nCoreIdx]; }

	CReadCodeBaslerCam*               GetBaslerCamControl() { return m_pReadCodeBaslerCam; }
	LPBYTE                            GetImageBufferBaslerCam(int nCamIdx);

	BOOL                              LoadSimulatorBuffer(int nBuff, int nFrame, CString strFilePath);
	virtual LPBYTE                    GetSimulatorBuffer(int nBuff, int nFrame);

private:
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateResultBuffer();
	BOOL                              CreateSimulatorBuffer();

private:

	CallbackInspectComplete*                   m_pCallbackInsCompleteFunc;
								               
	CallbackLogFunc*                           m_pCallbackLogFunc;
								               
	CallbackAlarm*                             m_pCallbackAlarm;
								               
	// UI						                            
	CLogView*                                  m_pLogView;
								               
	// System Setting			                   
	CReadCodeSystemSetting*                    m_pReadCodeSystemSetting;
								               
	// Recipe					               
	CReadCodeRecipe*                           m_pReadCodeRecipe;
								               
	// Result					               
	CReadCodeResult*                           m_pReadCodeResult[NUMBER_OF_SET_INSPECT];
								               
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

	cv::Mat                                    m_matBGR;
};