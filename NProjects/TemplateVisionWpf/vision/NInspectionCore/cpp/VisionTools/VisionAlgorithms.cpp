#include "pch.h"
#include "VisionAlgorithms.h"

CVisionAlgorithms::CVisionAlgorithms()
{
	m_vsParamManeger = new CVisionParameterManager;
	m_vsResultManager = new CVisionResultManager;
}

CVisionAlgorithms::~CVisionAlgorithms()
{
}

void CVisionAlgorithms::Run()
{
	switch (m_emAlgorithm)
	{
	case emCountPixel:
		NVision_CountPixelAlgorithm();
		break;
	case emCalculateArea:
		NVision_CalculateAreaAlgorithm();
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
	CParameterCountPixel param = m_vsParamManeger->GetParamCntPxl();

	//char cText[1024] = {};
	//sprintf_s(cText, "This is Count Pixel tool, %s: %s", "angle rotation: ", std::to_string(std::get<4>(param.m_tupROI)));
	//// code here
	//AfxMessageBox((CString)cText);
}

void CVisionAlgorithms::NVision_CalculateAreaAlgorithm()
{
	CParameterCalculateArea param = m_vsParamManeger->GetParamCalArea();

	//char cText[1024] = {};
	//sprintf_s(cText, "This is Cal Area tool, %s: %s", "area: ", std::to_string(param.m_arrArea[1]));
	//// code here
	//AfxMessageBox((CString)cText);
}
