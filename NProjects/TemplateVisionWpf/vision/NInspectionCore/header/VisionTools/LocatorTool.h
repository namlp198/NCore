#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "VisionDefine.h"
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
	BOOL                   Initialize(CameraInfo* pCamInfo);
	BOOL                   SaveImageTemplate(cv::Mat* pSaveImage, CString strFileTitle);
	void                   DrawAxis(cv::Mat& img, cv::Point p, cv::Point q, cv::Scalar colour,const float scale = 0.2);
	void                   DrawCenterPt(cv::Mat& img, cv::Point p, cv::Scalar colour);

public:
	BOOL                    NVision_FindLocator_TemplateMatching(); // this func is in order to when the vision camera runtime
	BOOL                    NVision_FindLocator_TemplateMatching_TRAIN(int nCamIdx, BYTE* pBuff, CameraInfo* camInfo, CString templatePath, CRectForTrainLocTool* paramTrainLoc); // this func is in order to train to get data

private:

	CString                         m_templateImagePath;

	CParameterLocator               m_paramLoc;
	CLocatorToolResult              m_locResult;

	CRectForTrainLocTool            m_paramTrainLoc;

	//CCriticalSection                m_crsGetTemplateImage;
	CSharedMemoryBuffer*            m_pImageBuffer;
	BYTE*                           m_pTemplateImageBuffer;

	cv::Mat                         m_matResultImage;
	cv::Mat                         m_matImageTemplate;
};