#pragma once
#include "SealingInspectDefine.h"
#include "Config.h"

struct CRecipe_TopCam_Frame1
{
	int m_nThresholdBinaryMinEnclosing;
	int m_nThresholdBinaryCannyHoughCircle;
	int m_nDistanceRadiusDiffMin;
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nRadiusInner_Min;
	int m_nRadiusInner_Max;
	int m_nRadiusOuter_Min;
	int m_nRadiusOuter_Max;
	int m_nDeltaRadiusOuterInner;
	int m_nROIWidth;
	int m_nROIHeight;
	int m_nROI12H_OffsetX;
	int m_nROI12H_OffsetY;
	int m_nROI3H_OffsetX;
	int m_nROI3H_OffsetY;
	int m_nROI6H_OffsetX;
	int m_nROI6H_OffsetY;
	int m_nROI9H_OffsetX;
	int m_nROI9H_OffsetY;
	BOOL m_bUseAdvancedAlgorithms;
};
struct CRecipe_TopCam_Frame2
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
};
struct CRecipe_SideCam_Frame1
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
};
struct CRecipe_SideCam_Frame2
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
};
struct CRecipe_SideCam_Frame3
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
};
struct CRecipe_SideCam_Frame4
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
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