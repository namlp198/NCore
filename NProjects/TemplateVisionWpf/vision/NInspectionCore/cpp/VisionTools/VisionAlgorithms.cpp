#include "pch.h"
#include "VisionAlgorithms.h"

CVisionAlgorithms::CVisionAlgorithms(emAlgorithms algorithm, IParameters* pParam, IResults* pResult)
{
	m_emAlgorithm = algorithm;
	m_pParam = pParam;
	m_pResult = pResult;
}

CVisionAlgorithms::~CVisionAlgorithms()
{
	if (m_pParam != NULL)
		delete m_pParam, m_pParam = NULL;

	if (m_pResult != NULL)
		delete m_pResult, m_pResult = NULL;
}

void CVisionAlgorithms::Run()
{
	switch (m_emAlgorithm)
	{
	case emCountPixel:
		NVision_CountPixelAlgorithm();
		break;
	case emCalculateArea:
		break;
	case emCalculateCoordinate:
		break;
	case emCountBlob:
		break;
	case emFindLine:
		break;
	case emFindCircle:
		break;
	case emOCR:
		break;
	default:
		break;
	}
}

void CVisionAlgorithms::NVision_CountPixelAlgorithm()
{
	CParameterCountPixel* param = (CParameterCountPixel*)m_pParam;
}
