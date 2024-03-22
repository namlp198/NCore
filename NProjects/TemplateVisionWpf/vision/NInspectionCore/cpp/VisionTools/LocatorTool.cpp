#include "pch.h"
#include "LocatorTool.h"

CLocatorTool::CLocatorTool()
{
}

CLocatorTool::~CLocatorTool()
{
}

LPBYTE CLocatorTool::GetTemplateImageBuffer()
{
	if (m_pImageTemplate->empty())
		return nullptr;

	return (LPBYTE)m_pImageTemplate->data;
}

BOOL CLocatorTool::GetDataTrained(DataTrained* pDataTrained)
{
	return 0;
}

BOOL CLocatorTool::Run()
{
	NVision_FindLocator();
}

BOOL CLocatorTool::SaveImageTemplate(CString saveImagePath)
{
}

BOOL CLocatorTool::NVision_FindLocator()
{
	//AfxMessageBox(_T("This is tool: " + m_ParamLoca.m_csName));

	// implement algorithm at here

}

BOOL CLocatorTool::NVision_TrainLocator(RectForTrainLocTool* paramTrainLoc)
{
}
