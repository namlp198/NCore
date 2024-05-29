#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

#include "SealingInspectHikCam.h"
#include "SealingInspectCore.h"
#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectResult.h"
#include "SealingInspectSystemSetting.h"
#include "SealingInspect_Simulation_IO.h"
#include "SharedMemoryBuffer.h"
#include "LogView.h"
#include "ListBoxLog.h"
#include "TCPSocket.h"

#include "rapidxml.hpp"
#include "RapidXMLSTD.hpp"
#include "rapidxml_utils.hpp"
#include "rapidxml_print.hpp"

#define TEST_NO_CAMERA
#undef TEST_NO_CAMERA

typedef void _stdcall CallbackLogFunc(char* strLogMsg);
typedef void _stdcall CallbackAlarm(emInspectCavity nSetInsp, char* strAlarmMessage);
typedef void _stdcall CallbackInspectComplete(emInspectCavity emInspCavity, BOOL bSetting);

class AFX_EXT_CLASS CSealingInspectProcessor : public ISealingInspectHikCamToParent,
	public ISealingInspectCoreToParent
{
public:
	CSealingInspectProcessor();
	~CSealingInspectProcessor();

public:
	BOOL         Initialize();
	BOOL         Destroy();
	CString      GetCurrentPathApp();
	BOOL         LoadSystemSetting(CSealingInspectSystemSetting* pSystemSetting);
	BOOL         LoadLightSetting(CSealingInspectSystemSetting* pSystemSetting);
	BOOL         LoadRecipe(CSealingInspectRecipe* pRecipe);
	BOOL         SaveSystemSetting(CSealingInspectSystemSetting* pSystemSetting);
	BOOL         SaveLightSetting(CSealingInspectSystemSetting* pSystemSetting, int nLightIdx);
	BOOL         SaveRecipe(CSealingInspectRecipe* pRecipe, CString sPosCam, int nFrameIdx);

public:
	BOOL InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL bSimulator);
	BOOL InspectStop(emInspectCavity nInspCavity);

	BOOL TestInspectCavity1();
	BOOL TestInspectCavity2();

	BOOL Inspect_TopCam_Simulation(int nCamIdx, int nFrame);

	BOOL           TestTCPSocket();
	static void    TestTcpSocketCallback(char* pMsg, int nMsglen, void* param);
	void           TestTcpSocketCallbackEx(char* pMsg, int nMsglen);

public:
	// Image buffer
	virtual LPBYTE                    GetBufferImage_SIDE(int nBuff, int nFrame);
	BOOL                              LoadImageBuffer_SIDE(int nBuff, int nFrame, CString strFilePath);
	virtual LPBYTE                    GetBufferImage_TOP(int nBuff, int nFrame);
	BOOL                              LoadImageBuffer_TOP(int nBuff, int nFrame, CString strFilePath);

	// Get Result buffer
	virtual LPBYTE                    GetResultBuffer_SIDE(int nBuff, int nFrame);
	virtual LPBYTE                    GetResultBuffer_TOP(int nBuff, int nFrame);

	BOOL                              LoadAllImageBuffer(CString strDirPath, CString strImageType);

	BOOL                              ClearBufferImage_SIDE(int nBuff);
	BOOL                              ClearBufferImage_TOP(int nBuff);

public:
	virtual BOOL                      SetResultBuffer_SIDE(int nBuff, int nFrame, BYTE* buff);
	virtual BOOL                      SetResultBuffer_TOP(int nBuff, int nFrame, BYTE* buff);

public:
	// CallBack
	void                              RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                              RegCallbackAlarm(CallbackAlarm* pFunc);
	void                              RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc);

public:
	// getter
	CSealingInspectHikCam*           GetHikCamControl() { return m_pSealingInspHikCam; }
	CSealingInspectResult*           GetSealingInspectResultControl(int nResIdx) { return m_pSealingInspResult[nResIdx]; }
	CSealingInspect_Simulation_IO*   GetSealingInspectSimulationIO(int nCoreIdx) { return m_pSealingInspect_Simulation_IO[nCoreIdx]; }

	BOOL                             SetSealingInspectSimulationIO(int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO);

public:
	virtual void							InspectComplete(emInspectCavity nSetInsp, BOOL bSetting);
	BOOL                                    GetInspectionResult(int nCoreIdx, CSealingInspectResult* pSealingInspRes);
	virtual CSealingInspectRecipe*          GetRecipe() { return m_pSealingInspRecipe; }
	virtual CSealingInspectSystemSetting*   GetSystemSetting() { return m_pSealingInspSystemSetting; }

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

	void                              SetCavityInfo(CString strLoadingTime);

private:
	void						      SystemMessage(CString strMessage);
	// create image buffer
	BOOL                              CreateBuffer_SIDE();
	BOOL                              CreateBuffer_TOP();
	// create result buffer
	BOOL                              CreateResultBuffer_SIDE();
	BOOL                              CreateResultBuffer_TOP();
	void                              MakeDirectory();
	virtual BOOL				      CheckDirectory(const TCHAR szPathName[], BOOL bDelete = FALSE);
	BOOL						      DeleteFolder(const CString strFolder);

private:

	// Image Buffer
	CSharedMemoryBuffer*                       m_pImageBuffer_Top[MAX_TOPCAM_COUNT];
								               
	CSharedMemoryBuffer*                       m_pImageBuffer_Side[MAX_SIDECAM_COUNT];

	// Result Buffer
	CSharedMemoryBuffer*                       m_pResultBuffer_Top[MAX_TOPCAM_COUNT];
						                       
	CSharedMemoryBuffer*                       m_pResultBuffer_Side[MAX_SIDECAM_COUNT];
								               
	CallbackLogFunc*                           m_pCallbackLogFunc;
	CallbackAlarm*                             m_pCallbackAlarm;
	CallbackInspectComplete*                   m_pCallbackInsCompleteFunc;
								               
	// UI						               
	CLogView*                                  m_pLogView;
								               
	// Hik cam					               
	CSealingInspectHikCam*                     m_pSealingInspHikCam;
								               
	// Inspect Core				               
	CSealingInspectCore*                       m_pSealingInspCore[NUMBER_OF_SET_INSPECT];
								      
	// System Setting			      
	CSealingInspectSystemSetting*              m_pSealingInspSystemSetting;

	// Recipe
	CSealingInspectRecipe*                     m_pSealingInspRecipe;
									           
	// Result						           
	CCriticalSection                           m_csInspResult[NUMBER_OF_SET_INSPECT];
	CSealingInspectResult*                     m_pSealingInspResult[NUMBER_OF_SET_INSPECT];
									           
	// Simulation IO				           
	CCriticalSection                           m_csSimulation_IO[NUMBER_OF_SET_INSPECT];
	CSealingInspect_Simulation_IO*             m_pSealingInspect_Simulation_IO[NUMBER_OF_SET_INSPECT];
									           
	// system settings file path	           
	CString                                    m_csSysSettingsPath;
	CString                                    m_csRecipePath;
	CString                                    m_csLightSettingPath;
	CTime                                      m_timeLoadingTime;
	CString                                    m_strFullImagePath;
	CString                                    m_strDefectImagePath;

	CTCPSocket*                                m_pTcpSocket;
};