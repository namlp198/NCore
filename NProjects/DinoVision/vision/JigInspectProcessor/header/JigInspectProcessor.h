#pragma once
#include <iostream>
#include <fstream>
#include <vector>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>

#include "SharedMemoryBuffer.h"
#include "JigInspectDinoCam.h"
#include "JigInspectConfig.h"
#include "JigInspectRecipe.h"
#include "JigInspectResults.h"
#include "JigInspectDefine.h"

#include "rapidxml.hpp"
#include "RapidXMLSTD.hpp"
#include "rapidxml_utils.hpp"
#include "rapidxml_print.hpp"

typedef void _stdcall CallbackInspectComplete();

class AFX_EXT_CLASS CJigInspectProcessor : IJigInspectDinoCamToParent
{
public:
	CJigInspectProcessor();
	~CJigInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadSysConfigurations(CJigInspectSystemConfig* pSysConfig);
	BOOL LoadCamConfigurations(int nCamIdx, CJigInspectCameraConfig* pCamConfig);
	BOOL LoadRecipe(int nCamIdx, CJigInspectRecipe* pRecipe);
	BOOL SaveSysConfigurations(CJigInspectSystemConfig* pSysConfig);
	BOOL SaveCamConfigurations(int nCamIdx, CJigInspectCameraConfig* pCamConfig);
	BOOL SaveRecipe(int nCamIdx, CJigInspectRecipe* pRecipe);
	BOOL CreateBuffer();

public:
	BOOL                    InspectStart(int nCamIdx);
	virtual LPBYTE          GetFrameImage(int nCamIdx, UINT nFrameIndex);
	virtual LPBYTE          GetBufferImage(int nCamIdx, UINT nY);

public:
	//getter
	CJigInspectDinoCam*                 GetDinoCamControl() { return m_pInspDinoCam; }

	virtual CJigInspectRecipe*          GetRecipe(int nCamIdx) { return m_pJigInspRecipe[nCamIdx]; }
	virtual CJigInspectCameraConfig*    GetCameraConfig(int nCamIdx) { return m_pJigInspCamConfig[nCamIdx]; }
	virtual CJigInspectSystemConfig*    GetSystemConfig() { return m_pJigInspSysConfig; }
	virtual CJigInspectResults*         GetJigInspectResult(int nCamIdx) { return m_pJigInspResutls[nCamIdx]; }
	virtual void				        InspectComplete();

public:
	BOOL                                GetInspectionResult(int nCamIdx, CJigInspectResults* pJigInspRes);

public:
	// Callback
	void       RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc);

public:
	CString    GetCurrentApp();
	BOOL       SetCamConfigPath();
	BOOL       SetRecipePath(int nCamIdx, CString recipeName);

private:

	CallbackInspectComplete*         m_pCallbackInsCompleteFunc;

	// Image Buffer
	CSharedMemoryBuffer*             m_pImageBuffer[MAX_CAMERA_INSP_COUNT];

	CJigInspectSystemConfig*         m_pJigInspSysConfig;

	CJigInspectCameraConfig*         m_pJigInspCamConfig[MAX_CAMERA_INSP_COUNT];

	CJigInspectRecipe*               m_pJigInspRecipe[MAX_CAMERA_INSP_COUNT];

	CJigInspectResults*              m_pJigInspResutls[MAX_CAMERA_INSP_COUNT];

	CJigInspectDinoCam*              m_pInspDinoCam;

	CString                          m_csSysConfigPath;
	CString                          m_csCamConfigPath[MAX_CAMERA_INSP_COUNT];
	CString                          m_csRecipePath[MAX_CAMERA_INSP_COUNT];
};