#pragma once

#include "interface_vision.h"
#include "VisionDefine.h"

class AFX_EXT_CLASS CLocatorToolResult : public IResults
{
public:
	CLocatorToolResult();
	~CLocatorToolResult();

public:
	int          m_nDelta_x;
	int          m_nDelta_y;
	double       m_dDif_Angle;
	BOOL         m_bResult;
};

class AFX_EXT_CLASS CSelectROIToolResult : IResults
{
public:
	BOOL         m_bResult;
};

class AFX_EXT_CLASS CAlgorithmsCountPixelResult : IResults
{
public:
	int          m_nNumberOfPixel;
	BOOL         m_bResult;
};
 
class AFX_EXT_CLASS CAlgorithmsCalculateAreaResult : IResults
{
public:
	int          m_dArea;
	BOOL         m_bResult;
};