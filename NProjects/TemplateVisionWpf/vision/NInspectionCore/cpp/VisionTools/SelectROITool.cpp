#include "pch.h"
#include "SelectROITool.h"

CSelectROITool::CSelectROITool()
{
	//m_pVsAlgorithms = new CVisionAlgorithms;
}

CSelectROITool::~CSelectROITool()
{
}

void CSelectROITool::Run()
{
	m_pVsAlgorithms.Run();
}
