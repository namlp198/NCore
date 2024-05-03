#pragma once
#include "JigInspectDefine.h"

class AFX_EXT_CLASS CLocatorTool_TemplateMatching_Result
{
public:
	CLocatorTool_TemplateMatching_Result(void);
	~CLocatorTool_TemplateMatching_Result(void);

public:
	int m_nLeft;
	int m_nTop;
	int m_nWidth;
	int m_nHeight;
	int m_nCenterX;
	int m_nCenterY;
	double m_dMatchingRate;
	int m_nDelta_X;
	int m_nDelta_Y;
	double m_dDif_Angle;
	bool m_bResult;
};