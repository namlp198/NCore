#pragma once

struct CNVisionInspectRecipe_Cam1
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