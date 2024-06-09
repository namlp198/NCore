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
	double m_dThresholdCanny1_MakeROI;
	double m_dThresholdCanny2_MakeROI;
	int m_nDelayTimeGrab;
	int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	int m_nHoughCircleParam1;
	int m_nHoughCircleParam2;
};
struct CRecipe_TopCam_Frame2
{
	int m_nROIWidth;
	int m_nROIHeight;
	int m_nROI1H_OffsetX;
	int m_nROI1H_OffsetY;
	int m_nROI5H_OffsetX;
	int m_nROI5H_OffsetY;
	int m_nROI7H_OffsetX;
	int m_nROI7H_OffsetY;
	int m_nROI11H_OffsetX;
	int m_nROI11H_OffsetY;
	double m_dDistanceMeasurementTolerance_Refer;
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nThresholdBinary;
	int m_nContourSizeFindBlob_Min;
	int m_nContourSizeFindBlob_Max;
	int m_nThreshBinary_FindBlobWhite;
	int m_nThreshBinary_FindBlobWhite_Max;
	int m_nThreshBinary_FindBlobBlack;
	int m_nThreshBinary_FindBlobBlack_Max;
	int m_nBlobCount_Max;
	double m_dBlobArea_Min;
	double m_dBlobArea_Max;
};
struct CRecipe_SideCam_Frame1
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
	double m_dThresholdCanny1_MakeROI;
	double m_dThresholdCanny2_MakeROI;
	BOOL m_bUseAdvancedAlgorithms;
	int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	int b_bUseFindROIAdvancedAlgorithms;
	int m_nOffetY_Top;
	int m_nOffetY_Bottom;
	int m_nThresholdBinaryFindROI;
};
struct CRecipe_SideCam_Frame2
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
	double m_dThresholdCanny1_MakeROI;
	double m_dThresholdCanny2_MakeROI;
	BOOL m_bUseAdvancedAlgorithms;
	int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	int b_bUseFindROIAdvancedAlgorithms;
	int m_nOffetY_Top;
	int m_nOffetY_Bottom;
	int m_nThresholdBinaryFindROI;
};
struct CRecipe_SideCam_Frame3
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
	double m_dThresholdCanny1_MakeROI;
	double m_dThresholdCanny2_MakeROI;
	BOOL m_bUseAdvancedAlgorithms;
	int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	int b_bUseFindROIAdvancedAlgorithms;
	int m_nOffetY_Top;
	int m_nOffetY_Bottom;
	int m_nThresholdBinaryFindROI;
};
struct CRecipe_SideCam_Frame4
{
	int m_nROI_Top[ROI_PARAMETER_COUNT];
	int m_nROI_Bottom[ROI_PARAMETER_COUNT];
	double m_dDistanceMeasurementTolerance_Min;
	double m_dDistanceMeasurementTolerance_Max;
	int m_nDelayTimeGrab;
	int m_nFindStartEndX;
	int m_nFindStartEndSearchRangeX;
	int m_nFindStartEndXThresholdGray;
	double m_dThresholdCanny1_MakeROI;
	double m_dThresholdCanny2_MakeROI;
	BOOL m_bUseAdvancedAlgorithms;
	int m_nNumberOfDistanceMaxCount_AdvancedAlgorithms;
	int b_bUseFindROIAdvancedAlgorithms;
	int m_nOffetY_Top;
	int m_nOffetY_Bottom;
	int m_nThresholdBinaryFindROI;
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