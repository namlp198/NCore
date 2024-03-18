#include "pch.h"
#include "SelectROITool.h"

CSelectROITool::CSelectROITool()
{
	m_pParamSelROI = new CParameterSelectROI;
}

CSelectROITool::~CSelectROITool()
{
	if (m_pParamSelROI != NULL)
	{
		delete m_pParamSelROI;
		m_pParamSelROI = NULL;
	}

	if (m_pVsAlgorithms != NULL)
	{
		delete m_pVsAlgorithms;
		m_pVsAlgorithms = NULL;
	}

	if (m_pParam != NULL)
	{
		delete m_pParam;
		m_pParam = NULL;
	}
	
	if (m_pResult != NULL)
	{
		delete m_pResult;
		m_pResult = NULL;
	}

}

void CSelectROITool::Initialize(CParameterSelectROI* pParamSelROI, IParameters* pParam, IResults* pResult)
{
	m_pParamSelROI = pParamSelROI;
	m_pParam = pParam;
	m_pResult = pResult;

	m_pVsAlgorithms = new CVisionAlgorithms(m_pParamSelROI->m_emAlgorithm, m_pParam, m_pResult);
}
