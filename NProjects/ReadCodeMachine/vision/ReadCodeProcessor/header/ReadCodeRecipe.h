#pragma once

class AFX_EXT_CLASS CReadCodeRecipe
{
public:
	CReadCodeRecipe(void);
	~CReadCodeRecipe(void);

public:
	BOOL               m_bUseReadCode;
	BOOL               m_bUseInkjetCharactersInspect;
	BOOL               m_bUseRotateROI;
	int                m_nMaxCodeCount;
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
	// ROI1
	int                m_nROI1_OffsetX;
	int                m_nROI1_OffsetY;
	int                m_nROI1_Width;
	int                m_nROI1_Height;
	double             m_nROI1_AngleRotate;
	int                m_nROI1_GrayThreshold_Min;
	int                m_nROI1_GrayThreshold_Max;
	int                m_nROI1_PixelCount_Min;
	int                m_nROI1_PixelCount_Max;
	BOOL               m_bROI1ShowGraphics;
	// ROI2
	int                m_nROI2_OffsetX;
	int                m_nROI2_OffsetY;
	int                m_nROI2_Width;
	int                m_nROI2_Height;
	double             m_nROI2_AngleRotate;
	int                m_nROI2_GrayThreshold_Min;
	int                m_nROI2_GrayThreshold_Max;
	int                m_nROI2_PixelCount_Min;
	int                m_nROI2_PixelCount_Max;
	BOOL               m_bROI2ShowGraphics;
	// ROI3
	int                m_nROI3_OffsetX;
	int                m_nROI3_OffsetY;
	int                m_nROI3_Width;
	int                m_nROI3_Height;
	double             m_nROI3_AngleRotate;
	int                m_nROI3_GrayThreshold_Min;
	int                m_nROI3_GrayThreshold_Max;
	int                m_nROI3_PixelCount_Min;
	int                m_nROI3_PixelCount_Max;
	BOOL               m_bROI3ShowGraphics;
};