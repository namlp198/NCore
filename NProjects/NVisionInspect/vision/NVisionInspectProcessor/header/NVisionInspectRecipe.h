#pragma once
#include "NVisionInspectDefine.h"

struct CNVisionInspectRecipe_CountPixel
{
	int                m_nCountPixel_ROI_X;
	int                m_nCountPixel_ROI_Y;
	int                m_nCountPixel_ROI_Width;
	int                m_nCountPixel_ROI_Height;
	int                m_nCountPixel_ROI_Offset_X;
	int                m_nCountPixel_ROI_Offset_Y;
	double             m_nCountPixel_ROI_AngleRotate;
	int                m_nCountPixel_GrayThreshold_Min;
	int                m_nCountPixel_GrayThreshold_Max;
	int                m_nCountPixel_PixelCount_Min;
	int                m_nCountPixel_PixelCount_Max;
	BOOL               m_bCountPixel_ShowGraphics;
	BOOL               m_bCountPixel_UseOffset;
	BOOL               m_bCountPixel_UseLocator;
};
struct CNVisionInspectRecipe_HSV
{
	int m_nHueMin;
	int m_nHueMax;
	int m_nSaturationMin;
	int m_nSaturationMax;
	int m_nValueMin;
	int m_nValueMax;
};
struct CNVisionInspectRecipe_Decode
{
	int m_nDecode_ROI_X;
	int m_nDecode_ROI_Y;
	int m_nDecode_ROI_Width;
	int m_nDecode_ROI_Height;
	int m_nMaxCodeCount;
};
struct CNVisionInspectRecipe_Locator
{
	// params ROI of Template Matching
	int                m_nTemplateROI_OuterX;
	int                m_nTemplateROI_OuterY;
	int                m_nTemplateROI_Outer_Width;
	int                m_nTemplateROI_Outer_Height;
	int                m_nTemplateROI_InnerX;
	int                m_nTemplateROI_InnerY;
	int                m_nTemplateROI_Inner_Width;
	int                m_nTemplateROI_Inner_Height;
	int                m_nTemplateCoordinatesX;
	int                m_nTemplateCoordinatesY;
	double             m_dTemplateMatchingRate;
	BOOL               m_bTemplateShowGraphics;
};

struct CNVisionInspectRecipe_Cam1
{
public:
	CNVisionInspectRecipe_Locator m_NVisionInspRecipe_Locator;
	CNVisionInspectRecipe_CountPixel m_NVisionInspRecipe_CntPxl[MAX_COUNT_PIXEL_TOOL_COUNT_CAM1];
};
struct CNVisionInspectRecipe_Cam2
{
public:
	BOOL               m_bUseReadCode;
	BOOL               m_bUseInkjetCharactersInspect;
	BOOL               m_bUseRotateROI;
	int                m_nMaxCodeCount;
	int                m_nNumberOfROI;
	// params ROI of Template Matching
	int                m_nTemplateROI_OuterX;
	int                m_nTemplateROI_OuterY;
	int                m_nTemplateROI_Outer_Width;
	int                m_nTemplateROI_Outer_Height;
	int                m_nTemplateROI_InnerX;
	int                m_nTemplateROI_InnerY;
	int                m_nTemplateROI_Inner_Width;
	int                m_nTemplateROI_Inner_Height;
	BOOL               m_bTemplateShowGraphics;
	// params of Template Matching
	int                m_nTemplateCoordinatesX;
	int                m_nTemplateCoordinatesY;
	double             m_dTemplateAngleRotate;
	double             m_dTemplateMatchingRate;
	// ROI1
	int                m_nROI1_X;
	int                m_nROI1_Y;
	int                m_nROI1_Width;
	int                m_nROI1_Height;
	int                m_nROI1_Offset_X;
	int                m_nROI1_Offset_Y;
	double             m_nROI1_AngleRotate;
	BOOL               m_bROI1UseOffset;
	BOOL               m_bROI1UseLocator;
	BOOL               m_bROI1ShowGraphics;
	int                m_nROI1_GrayThreshold_Min;
	int                m_nROI1_GrayThreshold_Max;
	int                m_nROI1_PixelCount_Min;
	int                m_nROI1_PixelCount_Max;
	// ROI2
	int                m_nROI2_X;
	int                m_nROI2_Y;
	int                m_nROI2_Width;
	int                m_nROI2_Height;
	int                m_nROI2_Offset_X;
	int                m_nROI2_Offset_Y;
	double             m_nROI2_AngleRotate;
	BOOL               m_bROI2UseOffset;
	BOOL               m_bROI2UseLocator;
	BOOL               m_bROI2ShowGraphics;
	int                m_nROI2_GrayThreshold_Min;
	int                m_nROI2_GrayThreshold_Max;
	int                m_nROI2_PixelCount_Min;
	int                m_nROI2_PixelCount_Max;
	// ROI3
	int                m_nROI3_X;
	int                m_nROI3_Y;
	int                m_nROI3_Width;
	int                m_nROI3_Height;
	int                m_nROI3_Offset_X;
	int                m_nROI3_Offset_Y;
	double             m_nROI3_AngleRotate;
	BOOL               m_bROI3UseOffset;
	BOOL               m_bROI3UseLocator;
	BOOL               m_bROI3ShowGraphics;
	int                m_nROI3_GrayThreshold_Min;
	int                m_nROI3_GrayThreshold_Max;
	int                m_nROI3_PixelCount_Min;
	int                m_nROI3_PixelCount_Max;
};
struct CNVisionInspectRecipe_Cam3
{
};
struct CNVisionInspectRecipe_Cam4
{
};
struct CNVisionInspectRecipe_Cam5
{
};
struct CNVisionInspectRecipe_Cam6
{
};
struct CNVisionInspectRecipe_Cam7
{
};
struct CNVisionInspectRecipe_Cam8
{
};

class AFX_EXT_CLASS CNVisionInspectRecipe
{
public:
	CNVisionInspectRecipe(void);
	~CNVisionInspectRecipe(void);
public:
	CNVisionInspectRecipe_Cam1 m_NVisionInspRecipe_Cam1;
	CNVisionInspectRecipe_Cam2 m_NVisionInspRecipe_Cam2;
	CNVisionInspectRecipe_Cam3 m_NVisionInspRecipe_Cam3;
	CNVisionInspectRecipe_Cam4 m_NVisionInspRecipe_Cam4;
	CNVisionInspectRecipe_Cam5 m_NVisionInspRecipe_Cam5;
	CNVisionInspectRecipe_Cam6 m_NVisionInspRecipe_Cam6;
	CNVisionInspectRecipe_Cam7 m_NVisionInspRecipe_Cam7;
	CNVisionInspectRecipe_Cam8 m_NVisionInspRecipe_Cam8;
};
class AFX_EXT_CLASS CNVisionInspectRecipe_FakeCam
{
public:
	CNVisionInspectRecipe_FakeCam(void);
	~CNVisionInspectRecipe_FakeCam(void);
public:
	CNVisionInspectRecipe_Locator m_NVisionInspectRecipe_Locator;
	CNVisionInspectRecipe_CountPixel m_NVisionInspectRecipe_CountPixel;
	CNVisionInspectRecipe_Decode m_NVisionInspectRecipe_Decode;
	CNVisionInspectRecipe_HSV m_NVisionInspectRecipe_HSV;
};