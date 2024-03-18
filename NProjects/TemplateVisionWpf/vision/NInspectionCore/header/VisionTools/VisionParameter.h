#pragma once

#include "interface_vision.h"
#include "VisionDefine.h"
#include <tuple>

class AFX_EXT_CLASS CParameterLocator : public IParameters
{
public:
	CParameterLocator();
	~CParameterLocator();
public:

	CString                    m_csId;
	CString                    m_csName;
	int                        m_nPriority;
	BOOL                       m_bHasChildren;
	CString                    m_csChildren;
	int                        m_RectangleInSide[4];
	int                        m_RectangleOutSide[4];
	int                        m_DataTrain[2];
};

class AFX_EXT_CLASS CParameterSelectROI : IParameters
{
public:
	CParameterSelectROI();
	~CParameterSelectROI();
public:

	CString                    m_csId;
	CString                    m_csName;
	CString                    m_csType;
	emAlgorithms               m_emAlgorithm;
	BOOL                       m_bRotations;
	int                        m_nPriority;
};

class AFX_EXT_CLASS CParameterCountPixel : IParameters
{
public:
	CParameterCountPixel();
	~CParameterCountPixel();
public:
	std::tuple<int, int, int, int, double>       m_tupROI;
	int                                          m_arrThresholdGray[2];
	int                                          m_arrNumberOfPixel[2];
};

class AFX_EXT_CLASS CParameterCalculateArea : IParameters
{
public:
	CParameterCalculateArea();
	~CParameterCalculateArea();

public:
	std::tuple<int, int, int, int, double>       m_tupROI;
	int                                          m_nThreshold;
	int                                          m_arrArea[2];
};