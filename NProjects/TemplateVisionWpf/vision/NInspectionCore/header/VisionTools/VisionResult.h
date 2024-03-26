#pragma once

#include "VisionParameter.h"
#include <vector>

struct CSumResult
{
	BOOL m_bSumResult;
	BYTE* m_resultImageBuffer;
};

struct CLocatorToolResult
{
	int          m_nX;
	int          m_nY;
	double       m_dMatchingRate;
	int          m_nDelta_x;
	int          m_nDelta_y;
	double       m_dDif_Angle;
	BOOL         m_bResult;
};

struct CSelectROIToolResult
{
	BOOL         m_bResult;
};

struct CAlgorithmsCountPixelResult
{
	int          m_nNumberOfPixel;
	BOOL         m_bResult;
};

struct CAlgorithmsCalculateAreaResult
{
	int          m_dArea;
	BOOL         m_bResult;
};

typedef std::vector<CLocatorToolResult> VecLocToolRes;
typedef std::vector<CAlgorithmsCountPixelResult> VecCntPxlRes;
typedef std::vector<CAlgorithmsCalculateAreaResult> VecCalAreaRes;

class AFX_EXT_CLASS CVisionResults
{
public:
	CVisionResults();
	~CVisionResults();

public:
	// Getter
	VecLocToolRes*      GetVecLocToolRes() { return m_vecLocToolRes; }
	VecCntPxlRes*       GetVecCntPxlRes() { return m_vecCntPxlRes; }
	VecCalAreaRes*      GetVecCalAreaRes() { return m_vecCalAreaRes; }
	cv::Mat*            GetResultImage() { return m_pResultImage; }

	LPBYTE              GetResultImageBuffer();
	BOOL                GetSumResult(CSumResult* pSumRes);

	// Setter
	void                SetVecLocToolRes(VecLocToolRes* vecLocToolRes) { m_vecLocToolRes = vecLocToolRes; }
	void                SetVecCntPxlRes(VecCntPxlRes* vecCntPxlRes) { m_vecCntPxlRes = vecCntPxlRes; }
	void                SetVecCalAreaRes(VecCalAreaRes* vecCalAreaRes) { m_vecCalAreaRes = vecCalAreaRes; }


public:
	BOOL Judgement_Result();
	BOOL Reset_Result();
	BOOL Draw_Result();

private:
	VecLocToolRes*      m_vecLocToolRes;
	VecCntPxlRes*       m_vecCntPxlRes;
	VecCalAreaRes*      m_vecCalAreaRes;

	cv::Mat*            m_pResultImage;

	// this variable summary all results
	BOOL                m_bSumResult;
};