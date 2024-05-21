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
#include "SharedMemoryBuffer.h"
#include "LogView.h"

#define TEST_NO_CAMERA
#undef TEST_NO_CAMERA

typedef void _stdcall CallbackLogFunc(char* strLogMsg);
typedef void _stdcall CallbackAlarm(emInspectCavity nSetInsp, char* strAlarmMessage);
typedef void _stdcall CallbackInspectComplete();

class AFX_EXT_CLASS CSealingInspectProcessor : public ISealingInspectHikCamToParent,
											   public ISealingInspectCoreToParent
{
public:
	CSealingInspectProcessor();
	~CSealingInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadSystemSetting();
	BOOL LoadRecipe();

public:
	BOOL InspectStart(int nThreadCount, emInspectCavity nInspCavity, BOOL isSimulator);
	BOOL InspectStop(emInspectCavity nInspCavity);

public:
	virtual LPBYTE                    GetBufferImage_SIDE(int nBuff, int nFrame);
	BOOL                              LoadImageBuffer_SIDE(int nBuff, int nFrame, CString strFilePath);
	virtual LPBYTE                    GetBufferImage_TOP(int nBuff, int nFrame);
	BOOL                              LoadImageBuffer_TOP(int nBuff, int nFrame, CString strFilePath);

	BOOL                              LoadAllImageBuffer(CString strDirPath, CString strImageType);

	BOOL                              ClearBufferImage_SIDE(int nBuff);
	BOOL                              ClearBufferImage_TOP(int nBuff);

public:
	virtual BOOL                      SetTopCamResultBuffer(int nBuff, int nFrame, BYTE* buff);
	virtual BOOL                      SetSideCamResultBuffer(int nBuff, int nFrame, BYTE* buff);

public:
	// CallBack
	void                              RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void                              RegCallbackAlarm(CallbackAlarm* pFunc);
	void                              RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc);

public:
	// getter
	CSealingInspectHikCam*            GetHikCamControl() { return m_pSealingInspHikCam; }

public:
	virtual void							InspectComplete(emInspectCavity nSetInsp);
	virtual CSealingInspectRecipe*          GetRecipe() { return m_pSealingInspRecipe; }
	virtual CSealingInspectSystemSetting*   GetSystemSetting() { return m_pSealingInspSystemSetting; }

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);

private:
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateBuffer_SIDE();
	BOOL                              CreateBuffer_TOP();

private:

	// Image Buffer
	CSharedMemoryBuffer*              m_pImageBuffer_Top[MAX_TOPCAM_COUNT];
								      
	CSharedMemoryBuffer*              m_pImageBuffer_Side[MAX_SIDECAM_COUNT];
								      
	CallbackLogFunc*                  m_pCallbackLogFunc;
	CallbackAlarm*                    m_pCallbackAlarm;
	CallbackInspectComplete*          m_pCallbackInsCompleteFunc;
								      
	// UI						      
	CLogView*                         m_pLogView;
								      
	// Hik cam					      
	CSealingInspectHikCam*            m_pSealingInspHikCam;
								      
	// Inspect Core				      
	CSealingInspectCore*              m_pSealingInspCore[NUMBER_OF_SET_INSPECT];
								      
	// System Setting			      
	CSealingInspectSystemSetting*     m_pSealingInspSystemSetting;

	// Recipe
	CSealingInspectRecipe*            m_pSealingInspRecipe;

	// Result
	CSealingInspectResult*            m_pSealingInspResult;
};