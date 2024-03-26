#pragma once

#include "VisionDefine.h"
#include <string>
#include <iostream>

struct CameraInfo
{
	int               m_nId;
	CString           m_csName;
	CString           m_csInterfaceType;
	CString           m_csSensorType;
	int               m_nChannels;
	CString           m_csManufacturer;
	int               m_nFrameWidth;
	int               m_nFrameHeight;
	CString           m_csSerialNumber;
};

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

struct CRectForTrainLocTool
{
	int m_nRectIn_X;
	int m_nRectIn_Y;
	int m_nRectIn_Width;
	int m_nRectIn_Height;
	int m_nRectOut_X;
	int m_nRectOut_Y;
	int m_nRectOut_Width;
	int m_nRectOut_Height;
	double m_dMatchingRateLimit;
};

struct CParamCntPxlAlgorithm
{
	int m_nROIX;
	int m_nROIY;
	int m_nROIWidth;
	int m_nROIHeight;
	double m_dROIAngleRotate;
	int m_nThresholdGrayMin;
	int m_nThresholdGrayMax;
	int m_nNumberOfPxlMin;
	int m_nNumberOfPxlMax;
};

struct CParamCalAreaAlgorithm
{
	int m_nROIX;
	int m_nROIY;
	int m_nROIWidth;
	int m_nROIHeight;
	double m_dROIAngleRotate;
	int m_nThreshold;
	int m_nAreaMin;
	int m_nAreaMax;
};