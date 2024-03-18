#include "pch.h"
#include "VisionResult.h"

#pragma region CLocatorToolResult
CLocatorToolResult::CLocatorToolResult()
{
	m_nDelta_x = 0;
	m_nDelta_y = 0;
	m_dDif_Angle = 0;
	m_bResult = FALSE;
}

CLocatorToolResult::~CLocatorToolResult()
{
}
#pragma endregion CLocatorToolResult
