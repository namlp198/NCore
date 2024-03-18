#pragma once

#include "VisionAlgorithms.h"
#include "interface_vision.h"
#include <string>

class AFX_EXT_CLASS CSelectROITool : public ITools
{
public:
	CSelectROITool();
	~CSelectROITool();

public:
	CVisionAlgorithms*                       GetVsAlgorithms() { return m_pVsAlgorithms; }
	CParameterSelectROI*                     GetParamSelectROI() { return m_pParamSelROI; }

public:
	void Initialize(CParameterSelectROI* pParamSelROI, IParameters* pParam, IResults* pResult);

private:
	CParameterSelectROI*                     m_pParamSelROI;
	IParameters*                             m_pParam;
	IResults*                                m_pResult;

	CVisionAlgorithms*                       m_pVsAlgorithms;
};