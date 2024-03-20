#pragma once

#include "VisionDefine.h"
#include <tuple>
#include <string>
#include <iostream>

struct CParameterLocator
{
	CString                    m_csId;
	CString                    m_csName;
	int                        m_nPriority;
	BOOL                       m_bHasChildren;
	CString                    m_csChildren;
	int                        m_RectangleInSide[4];
	int                        m_RectangleOutSide[4];
	int                        m_DataTrain[2];
};

struct CParameterSelectROI
{
	CString                    m_csId;
	CString                    m_csName;
	CString                    m_csType;
	emAlgorithms               m_emAlgorithm;
	BOOL                       m_bRotations;
	int                        m_nPriority;
};

struct CParameterCountPixel
{
	std::tuple<int, int, int, int, double>       m_tupROI;
	int                                          m_arrThresholdGray[2];
	int                                          m_arrNumberOfPixel[2];
};

struct CParameterCalculateArea
{
	std::tuple<int, int, int, int, double>       m_tupROI;
	int                                          m_nThreshold;
	int                                          m_arrArea[2];
};