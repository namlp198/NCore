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
	virtual LPBYTE                    GetBufferImage_SIDE(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer_SIDE(int nBuff, CString strFilePath);
	virtual LPBYTE                    GetBufferImage_TOP(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer_TOP(int nBuff, CString strFilePath);

	BOOL                              LoadAllImageBuffer(CString strDirPath, CString strImageType);

	BOOL                              ClearBufferImage_SIDE(int nBuff);
	BOOL                              ClearBufferImage_TOP(int nBuff);

public:
	// CallBack
	void RegCallbackLogFunc(CallbackLogFunc* pFunc);

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
	CSharedMemoryBuffer*           m_pImageBuffer_Top[MAX_IMAGE_BUFFER_TOP];

	CSharedMemoryBuffer*           m_pImageBuffer_Side[MAX_IMAGE_BUFFER_SIDE];

	CallbackLogFunc*               m_pCallbackLogFunc;

	// UI
	CLogView*                      m_pLogView;
};