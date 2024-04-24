#pragma once
#include <iostream>
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

#define MAX_CAMERA_INSP_COUNT 1
#define FRAME_WIDTH 640
#define FRAME_HEIGHT 480
#define MAX_FRAME_COUNT 15

typedef void _stdcall CallbackInspectComplete();

class AFX_EXT_CLASS CJigInspectProcessor : IJigInspectDinoCamToParent
{
public:
	CJigInspectProcessor();
	~CJigInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadConfigurations();
	//BOOL LoadRecipe();
	BOOL CreateBuffer();

public:
	BOOL                    InspectStart(int nCamIdx);
	virtual LPBYTE          GetFrameImage(int nCamIdx, UINT nFrameIndex);
	virtual LPBYTE          GetBufferImage(int nCamIdx, UINT nY);

public:
	//getter
	CJigInspectDinoCam*           GetDinoCamControl() { return m_pInspDinoCam; }

	virtual CJigInspectRecipe*    GetRecipe(int nCamIdx) { return m_pJigInspRecipe[nCamIdx]; }
	virtual CJigInspectConfig*    GetSystemConfig(int nCamIdx) { return m_pJigInspConfig[nCamIdx]; }
	virtual CJigInspectResults*   GetJigInspectResult(int nCamIdx) { return m_pJigInspResutls[nCamIdx]; }
	virtual void				  InspectComplete();

public:
	BOOL                          GetInspectionResult(int nCamIdx, CJigInspectResults* pJigInspRes);

public:
	// Callback
	void RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc);

private:

	CallbackInspectComplete*         m_pCallbackInsCompleteFunc;

	// Image Buffer
	CSharedMemoryBuffer*             m_pImageBuffer[MAX_CAMERA_INSP_COUNT];

	CJigInspectConfig*               m_pJigInspConfig[MAX_CAMERA_INSP_COUNT];

	CJigInspectRecipe*               m_pJigInspRecipe[MAX_CAMERA_INSP_COUNT];

	CJigInspectResults*              m_pJigInspResutls[MAX_CAMERA_INSP_COUNT];

	CJigInspectDinoCam*              m_pInspDinoCam;
};