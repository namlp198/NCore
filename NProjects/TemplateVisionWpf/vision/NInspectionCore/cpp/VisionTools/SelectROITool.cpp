#include "pch.h"
#include "SelectROITool.h"

CSelectROITool::CSelectROITool()
{
	
}

CSelectROITool::~CSelectROITool()
{
}

BOOL CSelectROITool::Run(emAlgorithms algorithm)
{
	return m_pVsAlgorithms.Run(algorithm);
}
