#pragma once
#include "JigInspectDefine.h"

class AFX_EXT_CLASS CJigInspectRecipe
{
public:
	CJigInspectRecipe(void);
	~CJigInspectRecipe(void);

public:
	TCHAR m_sRecipeName[MAX_STRING_SIZE];
	TCHAR m_sAlgorithm[MAX_STRING_SIZE];
	int m_nRectX;
	int m_nRectY;
	int m_nRectWidth;
	int m_nRectHeight;
	double m_dMatchingRate;
	int m_nCenterX;
	int m_nCenterY;
	TCHAR m_sImageTemplate[MAX_STRING_SIZE];
	int m_nOffsetROI0_X;
	int m_nOffsetROI0_Y;
	int m_nOffsetROI1_X;
	int m_nOffsetROI1_Y;
	int m_nROIWidth;
	int m_nROIHeight;
	int m_nNumberOfArray;
	int m_nThresholdHeightMin;
	int m_nThresholdHeightMax;
	int m_nThresholdWidthMin;
	int m_nThresholdWidthMax;
	int m_nKSizeX;
	int m_nKSizeY;
	int m_nContourSizeMin;
	int m_nContourSizeMax;
};