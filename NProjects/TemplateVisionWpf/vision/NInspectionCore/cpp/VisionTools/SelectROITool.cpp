#include "pch.h"
#include "SelectROITool.h"

CSelectROITool::CSelectROITool()
{
	m_pVsAlgorithms = new CVisionAlgorithms;
}

CSelectROITool::~CSelectROITool()
{
}

void CSelectROITool::Run()
{
	if (m_pVsAlgorithms == NULL)
		return;

	m_pVsAlgorithms->Run();
}
