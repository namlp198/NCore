#pragma once

#include "VisionAlgorithms.h"

class AFX_EXT_CLASS CSelectROITool
{
public:
	CSelectROITool();
	~CSelectROITool();

private:

	CVisionAlgorithms*                       m_VsAlgorithms;

	// parameter for the algorithms is selected
	CParameterCountPixel*                    m_ParamCountPxl;
	CParameterCalculateArea*                 m_ParamCalArea;
};