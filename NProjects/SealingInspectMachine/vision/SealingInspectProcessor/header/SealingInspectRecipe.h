#pragma once
#include "SealingInspectDefine.h"
#include "Config.h"

struct CRecipe_TopCam_Frame1
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nRadius_Min;
	int m_nRadius_Max;
};
struct CRecipe_TopCam_Frame2
{
	int m_nDistanceMeasurementTolerance_Min;
	int m_nDistanceMeasurementTolerance_Max;
	int m_nRadius_Min;
	int m_nRadius_Max;
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