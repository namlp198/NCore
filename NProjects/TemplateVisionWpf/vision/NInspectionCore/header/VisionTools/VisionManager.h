#pragma once

#include "VisionDefine.h"
#include "VisionParameter.h"
#include "VisionResult.h"

class AFX_EXT_CLASS CVisionParameterManager
{
public:
	CVisionParameterManager();
	~CVisionParameterManager();

public:
	// Getter

	CParameterCountPixel           GetParamCntPxl() { return m_paramCntPxl; }
	CParameterCalculateArea        GetParamCalArea() { return m_paramCalArea; }

	// Setter
	
	void                           SetParamCntPxl(CParameterCountPixel paramCntPxl) { m_paramCntPxl = paramCntPxl; }
	void                           SetParamCalArea(CParameterCalculateArea paramCalArea) { m_paramCalArea = paramCalArea; }

private:
	
	CParameterCountPixel           m_paramCntPxl;
	CParameterCalculateArea        m_paramCalArea;
};

class AFX_EXT_CLASS CVisionResultManager
{
public:
	CVisionResultManager();
	~CVisionResultManager();

public:
	// Getter
	CAlgorithmsCountPixelResult               GetCntPxlRes() { return m_cntPxlRes; }
	CAlgorithmsCalculateAreaResult            GetCalAreaRes() { return m_calAreaRes; }

	// Setter
	
	void                                      SetCntPxlRes(CAlgorithmsCountPixelResult cntPxlRes) { m_cntPxlRes = cntPxlRes; }
	void                                      SetCalAreaRes(CAlgorithmsCalculateAreaResult calAreaRes) { m_calAreaRes = calAreaRes; }

private:
	
	CAlgorithmsCountPixelResult             m_cntPxlRes;
	CAlgorithmsCalculateAreaResult          m_calAreaRes;
};