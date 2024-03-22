#pragma once

#include "VisionParameter.h"
#include "VisionResult.h"
#include "VisionManager.h"
#include "interface_vision.h"

struct DataTrained
{
	int m_nCntX;
	int m_nCntY;
};

struct RectForTrainLocTool
{
	int m_nRectIn_X;
	int m_nRectIn_Y;
	int m_nRectIn_Width;
	int m_nRectIn_Height;
	int m_nRectOut_X;
	int m_nRectOut_Y;
	int m_nRectOut_Width;
	int m_nRectOut_Height;
};

class AFX_EXT_CLASS CLocatorTool
{
public:
	CLocatorTool();
	~CLocatorTool();

public:
	// getter
	CLocatorToolResult     GetLocaToolRes() { return m_LocaResult; }
	CParameterLocator      GetParamLoca() { return m_ParamLoca; }
	LPBYTE                 GetImageBuffer() { return m_pImageBuffer; }
	LPBYTE                 GetTemplateImageBuffer();
	BOOL                   GetDataTrained(DataTrained* pDataTrained);

	// setter
	void                   SetLocaToolRes(CLocatorToolResult locaToolRes) { m_LocaResult = locaToolRes; }
	void                   SetParamLoca(CParameterLocator paramLoca) { m_ParamLoca = paramLoca; }
	void                   SetImageBuffer(LPBYTE pImgBuff) { m_pImageBuffer = pImgBuff; }
public:
	BOOL                   Run();
	BOOL                   SaveImageTemplate(CString saveImagePath);

protected:
	BOOL                    NVision_FindLocator();
	BOOL                    NVision_TrainLocator(RectForTrainLocTool* paramTrainLoc);

private:
	CString                         m_templateImagePath;

	CParameterLocator               m_ParamLoca;
	CLocatorToolResult              m_LocaResult;

	RectForTrainLocTool             m_paramTrainLoc;
	DataTrained                     m_dataTrained;

	LPBYTE                          m_pImageBuffer;
	cv::Mat*                        m_pResultImageBuffer;
	cv::Mat*                        m_pImageTemplate;
};