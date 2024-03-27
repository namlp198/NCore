#pragma once

#include "VisionParameter.h"
#include <vector>

struct CSumResult
{
	BOOL m_bSumResult;
	BYTE* m_resultImageBuffer;
};

struct CLocatorToolResult
{
	int          m_nX;
	int          m_nY;
	double       m_dMatchingRate;
	int          m_nDelta_x;
	int          m_nDelta_y;
	double       m_dDif_Angle;
	BOOL         m_bResult;
};

struct CSelectROIToolResult
{
	BOOL         m_bResult;
};

struct CAlgorithmsCountPixelResult
{
	int          m_nNumberOfPixel;
	BOOL         m_bResult;
};

struct CAlgorithmsCalculateAreaResult
{
	int          m_dArea;
	BOOL         m_bResult;
};

class AFX_EXT_CLASS CVisionResults
{
public:
	CVisionResults(void);
	~CVisionResults(void);
private:
	std::vector<CLocatorToolResult>                 m_vecLocToolRes;
	std::vector<CAlgorithmsCountPixelResult>        m_vecCntPxlRes;
	std::vector<CAlgorithmsCalculateAreaResult>     m_vecCalAreaRes;
};