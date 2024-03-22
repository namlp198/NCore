#pragma once

#include "VisionDefine.h"

struct CLocatorToolResult
{
	int          m_nX;
	int          m_nY;
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