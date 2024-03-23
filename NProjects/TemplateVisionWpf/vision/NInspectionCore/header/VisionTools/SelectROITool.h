#pragma once

#include "VisionAlgorithms.h"
#include "interface_vision.h"
#include <string>

class AFX_EXT_CLASS CSelectROITool
{
public:
	CSelectROITool();
	~CSelectROITool();

public:
	// Getter
	CVisionAlgorithms                GetVsAlgorithms() { return m_pVsAlgorithms; }
	CParameterSelectROI              GetParamSelROI() { return m_paramSelROI; }

	// Setter
	void                  SetVsAlgorithms(CVisionAlgorithms pVsAlgorithm) { m_pVsAlgorithms = pVsAlgorithm; }
	void                  SetParamSelROI(CParameterSelectROI paramSelROI) { m_paramSelROI = paramSelROI; }

public:
	BOOL Run(emAlgorithms algorithm);

private:

	CParameterSelectROI                     m_paramSelROI;

	CVisionAlgorithms                       m_pVsAlgorithms;
};