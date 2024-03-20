#include "pch.h"
#include "LocatorTool.h"

CLocatorTool::CLocatorTool()
{
}

CLocatorTool::~CLocatorTool()
{
}

void CLocatorTool::Run()
{
	NVision_FindLocator();
}

void CLocatorTool::NVision_FindLocator()
{

	//char cText[1024] = {};
	//sprintf_s(cText, "%s: %s", "tool name: ", m_ParamLoca.m_csName);

	AfxMessageBox(_T("tool name: " + m_ParamLoca.m_csName));
}
