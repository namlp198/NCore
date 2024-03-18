#include "pch.h"
#include "VisionParameter.h"

#pragma region CParameterSelectROI
CParameterSelectROI::CParameterSelectROI()
{
	m_csId = "";
	m_csName = "";
	m_csType = "";
	m_emAlgorithm = emNone;
	m_bRotations = FALSE;
	m_nPriority = 2;
}

CParameterSelectROI::~CParameterSelectROI()
{
	
}
#pragma endregion CParameterSelectROI


#pragma region CParameterLocator
CParameterLocator::CParameterLocator()
{
}

CParameterLocator::~CParameterLocator()
{
}
#pragma endregion CParameterLocator


#pragma region CParameterCountPixel
CParameterCountPixel::CParameterCountPixel()
{
}

CParameterCountPixel::~CParameterCountPixel()
{
}
#pragma endregion CParameterCountPixel


#pragma region CParameterCalculateArea
CParameterCalculateArea::CParameterCalculateArea()
{
}

CParameterCalculateArea::~CParameterCalculateArea()
{
}
#pragma endregion CParameterCalculateArea
