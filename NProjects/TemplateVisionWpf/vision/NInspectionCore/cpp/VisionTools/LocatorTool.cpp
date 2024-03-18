#include "pch.h"
#include "LocatorTool.h"

CLocatorTool::CLocatorTool()
{
	m_pLocaResult = new CLocatorToolResult;
	m_pParamLoca = new CParameterLocator;
}

CLocatorTool::~CLocatorTool()
{
	if (m_pLocaResult != NULL)
		delete m_pLocaResult, m_pLocaResult = NULL;
	if (m_pParamLoca != NULL)
		delete m_pParamLoca, m_pParamLoca = NULL;
}

void CLocatorTool::NVision_FindLocator()
{

}
