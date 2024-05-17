#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include "SealingInspectCore_TopCam.h"
#include "SealingInspectCore_SideCam.h"
#include "SealingInspectDefine.h"
#include "SealingInspectRecipe.h"
#include "SealingInspectResult.h"
#include "SealingInspectSystemSetting.h"
#include "SharedMemoryBuffer.h"
#include "LogView.h"

typedef void _stdcall CallbackLogFunc(char* strLogMsg);

class AFX_EXT_CLASS CSealingInspectProcessor 
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
	virtual LPBYTE                    GetBufferImage_Color(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer_Color(int nBuff, CString strFilePath);

	virtual LPBYTE                    GetBufferImage_Mono(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer_Mono(int nBuff, CString strFilePath);

	BOOL                              ClearBufferImage(int nBuff);

public:
	// CallBack
	void RegCallbackLogFunc(CallbackLogFunc* pFunc);

public:
	void						      LogMessage(char* strMessage);
	void						      LogMessage(CString strMessage);
	void						      ShowLogView(BOOL bShow);


private:
	void						      SystemMessage(CString strMessage);
	BOOL                              CreateBuffer_Color();
	BOOL                              CreateBuffer_Mono();

private:
	// Image Buffer
	CSharedMemoryBuffer*           m_pImageBufferColor_Top[MAX_IMAGE_BUFFER];
	CSharedMemoryBuffer*           m_pImageBufferMono_Top[MAX_IMAGE_BUFFER];

	CSharedMemoryBuffer*           m_pImageBufferColor_Side[MAX_IMAGE_BUFFER];
	CSharedMemoryBuffer*           m_pImageBufferMono_Side[MAX_IMAGE_BUFFER];

	CallbackLogFunc*               m_pCallbackLogFunc;

	// UI
	CLogView*                      m_pLogView;
};