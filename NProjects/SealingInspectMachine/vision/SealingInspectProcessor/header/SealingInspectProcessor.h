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
#include "SealingInspectStatus.h"
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
typedef void _stdcall CallbackInspectCavity1Complete(BOOL bSetting);
typedef void _stdcall CallbackInspectCavity2Complete(BOOL bSetting);
typedef void _stdcall CallbackInspectTopCam1Complete(BOOL bSetting);
typedef void _stdcall CallbackInspectTopCam2Complete(BOOL bSetting);

typedef void _stdcall CallbackGrabFrameSideCam1Complete(BOOL bSetting);
typedef void _stdcall CallbackGrabFrameSideCam2Complete(BOOL bSetting);

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

	BOOL         ReloadSystemSetting();
	BOOL         ReloadRecipe();

public:
	BOOL InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL bSimulator);
	BOOL InspectStop(emInspectCavity nInspCavity);

	BOOL TestInspectCavity1();
	BOOL TestInspectCavity2();

	BOOL Inspect_TopCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame);
	BOOL Inspect_SideCam_Simulation(int nCoreIdx, int nCamIdx, int nFrame);

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
	LPBYTE                            GetBufferImageHikCam(int nCamIdx);
	BOOL                              SaveImageHikCam(int nCamIdx, CString strImageSavePath);

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
	void                              RegCallbackInsCavity1completeFunc(CallbackInspectCavity1Complete* pFunc);
	void                              RegCallbackInsCavity2completeFunc(CallbackInspectCavity2Complete* pFunc);
	void                              RegCallbackInsTopCam1CompleteFunc(CallbackInspectTopCam1Complete* pFunc);
	void                              RegCallbackInsTopCam2CompleteFunc(CallbackInspectTopCam2Complete* pFunc);

	void                              RegCallbackGrabFrameSideCam1CompleteFunc(CallbackGrabFrameSideCam1Complete* pFunc);
	void                              RegCallbackGrabFrameSideCam2CompleteFunc(CallbackGrabFrameSideCam2Complete* pFunc);

public:
	// getter
	CSealingInspectHikCam*           GetHikCamControl() { return m_pSealingInspHikCam; }
	CSealingInspectResult*           GetSealingInspectResultControl(int nResIdx) { return m_pSealingInspResult[nResIdx]; }
	CSealingInspect_Simulation_IO*   GetSealingInspectSimulationIO(int nCoreIdx) { return m_pSealingInspect_Simulation_IO[nCoreIdx]; }

	BOOL                             SetSealingInspectSimulationIO(int nCoreIdx, CSealingInspect_Simulation_IO* sealingInspSimulationIO);

	void                             SetProcessStatus1(BOOL bProcessStatus1);
	BOOL                             GetProcessStatus1() { return m_bProcessStatus1; }
	void                             SetProcessStatus2(BOOL bProcessStatus2);
	BOOL                             GetProcessStatus2() { return m_bProcessStatus2; }
	void                             SetGrabFrameSideCam(int nCoreIdx, BOOL bGrab);
	BOOL                             GetGrabFrameSideCam(int nCoreId) { return m_bGrabFrameSideCam[nCoreId]; }

public:
	virtual void							InspectCavity1Complete(BOOL bSetting);
	virtual void							InspectCavity2Complete(BOOL bSetting);
	virtual void							InspectTopCam1Complete(BOOL bSetting);
	virtual void							InspectTopCam2Complete(BOOL bSetting);
	virtual void                            GrabFrameSideCam1Complete(BOOL bSetting);
	virtual void                            GrabFrameSideCam2Complete(BOOL bSetting);
	BOOL                                    GetInspectionResult(int nCoreIdx, CSealingInspectResult* pSealingInspRes);
	virtual CSealingInspectRecipe*          GetRecipe() { return m_pSealingInspRecipe; }
	virtual CSealingInspectSystemSetting*   GetSystemSetting() { return m_pSealingInspSystemSetting; }
	virtual void                            SetInspectStatus(int nCavity, BOOL bInspRunning) { m_pSealingInspStatus[nCavity]->m_bInspRunning = bInspRunning; };
	virtual BOOL                            GetInspectStatus(int nCavity, CSealingInspectStatus* pInspStatus);

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
	CallbackInspectCavity1Complete*            m_pCallbackInsCavity1CompleteFunc;
	CallbackInspectCavity2Complete*            m_pCallbackInsCavity2CompleteFunc;
	CallbackInspectTopCam1Complete*            m_pCallbackInsTopCam1CompleteFunc;
	CallbackInspectTopCam2Complete*            m_pCallbackInsTopCam2CompleteFunc;

	CallbackGrabFrameSideCam1Complete*         m_pCallbackGrabFrameSideCam1CompleteFunc;
	CallbackGrabFrameSideCam2Complete*         m_pCallbackGrabFrameSideCam2CompleteFunc;
								               
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

	// Status
	CSealingInspectStatus*                     m_pSealingInspStatus[NUMBER_OF_SET_INSPECT];
									           
	// Result						           
	CCriticalSection                           m_csInspResult[NUMBER_OF_SET_INSPECT];
	CSealingInspectResult*                     m_pSealingInspResult[NUMBER_OF_SET_INSPECT];
									           
	// Simulation IO				           
	CCriticalSection                           m_csSimulation_IO[NUMBER_OF_SET_INSPECT];
	CSealingInspect_Simulation_IO*             m_pSealingInspect_Simulation_IO[NUMBER_OF_SET_INSPECT];

	// Process Status
	CCriticalSection                           m_csProcessStatus1;
	CCriticalSection                           m_csProcessStatus2;
									           
	// system settings file path	           
	CString                                    m_csSysSettingsPath;
	CString                                    m_csRecipePath;
	CString                                    m_csLightSettingPath;
	CTime                                      m_timeLoadingTime;
	CString                                    m_strFullImagePath;
	CString                                    m_strDefectImagePath;

	CTCPSocket*                                m_pTcpSocket;
	cv::Mat                                    m_matBGR;

	BOOL                                       m_bProcessStatus1;
	BOOL                                       m_bProcessStatus2;

	BOOL                                       m_bGrabFrameSideCam[NUMBER_OF_SET_INSPECT];
};