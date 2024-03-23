#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "TempInspectRecipe.h"
#include "TempInspectSystemConfig.h"
#include "TempInspectResult.h"
#include "interface_vision.h"
#include "SharedMemoryBuffer.h"

class AFX_EXT_CLASS CLocatorTool
{
public:
	CLocatorTool();
	~CLocatorTool();

public:
	// getter
	CLocatorToolResult     GetLocRes() { return m_locResult; }
	CParameterLocator      GetParamLoc() { return m_paramLoc; }
	LPBYTE                 GetImageBuffer();
	LPBYTE                 GetResultImageBuffer();
	LPBYTE                 GetTemplateImageBuffer(); // get image template for show UI
	BOOL                   GetDataTrained_TemplateMatching(CLocatorToolResult* pDataTrained); // pass in a structure to get to the data trained

	// setter
	void                   SetLocRes(CLocatorToolResult locaToolRes) { m_locResult = locaToolRes; }
	void                   SetParamLoc(CParameterLocator paramLoc) { m_paramLoc = paramLoc; }
	BOOL                   SetImageBuffer(LPBYTE pBuff);
public:
	BOOL                   Run();
	BOOL                   Initialize(CameraInfo pCamInfo);
	BOOL                   SaveImageTemplate(cv::Mat* pSaveImage, CString strFileTitle);

public:
	BOOL                    NVision_FindLocator_TemplateMatching(); // this func is in order to when the vision camera runtime
	BOOL                    NVision_FindLocator_TemplateMatching_Train(int nCamIdx, CRectForTrainLocTool* paramTrainLoc); // this func is in order to train to get data

private:

	CString                         m_templateImagePath;

	CParameterLocator               m_paramLoc;
	CLocatorToolResult              m_locResult;

	CRectForTrainLocTool            m_paramTrainLoc;

	CSharedMemoryBuffer*            m_pImageBuffer;

	cv::Mat                         m_resultImageBuffer;
	cv::Mat                         m_imageTemplate;
};