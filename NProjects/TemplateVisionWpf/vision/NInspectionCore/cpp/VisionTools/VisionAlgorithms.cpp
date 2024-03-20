#include "pch.h"
#include "VisionAlgorithms.h"

CVisionAlgorithms::CVisionAlgorithms()
{
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
	CParameterCountPixel param = m_vsParamManeger.GetParamCntPxl();

	char cText[1024] = {};
	sprintf_s(cText, "%s: %s", "angle rotation: ", std::to_string(std::get<4>(param.m_tupROI)));

	// code here
	AfxMessageBox((CString)cText);
}
