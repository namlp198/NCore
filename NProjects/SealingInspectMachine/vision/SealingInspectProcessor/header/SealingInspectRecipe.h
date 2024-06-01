#pragma once
#include "SealingInspectDefine.h"
#include "Config.h"

struct CRecipe_TopCam_Frame1
{
	int m_nThresholdBinaryMinEnclosing;
	int m_nThresholdBinaryCannyHoughCircle;
	int m_nDistanceRadiusDiffMin;
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nRadiusInner_Min;
	int m_nRadiusInner_Max;
	int m_nRadiusOuter_Min;
	int m_nRadiusOuter_Max;
	int m_nDeltaRadiusOuterInner;
	int m_nROIWidth_Hor;
	int m_nROIHeight_Hor;
	int m_nROIWidth_Ver;
	int m_nROIHeight_Ver;
	int m_nROI12H_OffsetX;
	int m_nROI12H_OffsetY;
	int m_nROI3H_OffsetX;
	int m_nROI3H_OffsetY;
	int m_nROI6H_OffsetX;
	int m_nROI6H_OffsetY;
	int m_nROI9H_OffsetX;
	int m_nROI9H_OffsetY;
	BOOL m_bUseAdvancedAlgorithms;
	int m_nContourSizeMinEnclosingCircle_Min;
	int m_nContourSizeMinEnclosingCircle_Max;
	double m_dIncrementAngle;
	int m_nThresholdCanny1_MakeROI;
	int m_nThresholdCanny2_MakeROI;
	int m_nDelayTimeGrab;
};
struct CRecipe_TopCam_Frame2
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
};
struct CRecipe_SideCam_Frame1
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
};
struct CRecipe_SideCam_Frame2
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
};
struct CRecipe_SideCam_Frame3
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
};
struct CRecipe_SideCam_Frame4
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
};

class AFX_EXT_CLASS CSealingInspectRecipe_TopCam
{
public:
	CSealingInspectRecipe_TopCam();
	~CSealingInspectRecipe_TopCam();
public:
	CRecipe_TopCam_Frame1 m_recipeFrame1;
	CRecipe_TopCam_Frame2 m_recipeFrame2;
};

class AFX_EXT_CLASS CSealingInspectRecipe_SideCam
{
public:
	CSealingInspectRecipe_SideCam();
	~CSealingInspectRecipe_SideCam();
public:
	CRecipe_SideCam_Frame1 m_recipeFrame1;
	CRecipe_SideCam_Frame2 m_recipeFrame2;
	CRecipe_SideCam_Frame3 m_recipeFrame3;
	CRecipe_SideCam_Frame4 m_recipeFrame4;
};


class AFX_EXT_CLASS CSealingInspectRecipe
{
public:
	CSealingInspectRecipe(void);
	~CSealingInspectRecipe(void);

public:
	CSealingInspectRecipe_TopCam    m_sealingInspRecipe_TopCam[MAX_TOPCAM_COUNT];
	CSealingInspectRecipe_SideCam   m_sealingInspRecipe_SideCam[MAX_SIDECAM_COUNT];
};