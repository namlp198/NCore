#pragma once

#include "VisionResult.h"
#include "VisionParameter.h"
#include "TempInspectRecipe.h"
#include <vector>
#include <queue>
#include <string>


interface ITempInspectResultToParent
{
	virtual CTempInspectRecipe* GetRecipe(int nIdx) = 0;
};

class AFX_EXT_CLASS CTempInspectResult
{
public:
	CTempInspectResult(ITempInspectResultToParent* pInterface);
	~CTempInspectResult();

public:
	// Getter
	std::vector<CLocatorToolResult>                 GetVecLocToolResult() { return m_vecLocRes; }
	std::vector<CAlgorithmsCountPixelResult>        GetVecCntPxlResult() { return m_vecCntPxlRes; }
	std::vector<CAlgorithmsCalculateAreaResult>     GetVecCalAreaResult() { return m_vecCalAreaRes; }
	LPBYTE                                          GetImageResult_DrawnResult();
	

	// Setter
	void SetVecLocToolResult(std::vector<CLocatorToolResult> vecLocToolRes) { m_vecLocRes = vecLocToolRes; }
	void SetVecCntPxlResult(std::vector<CAlgorithmsCountPixelResult> vecCntPxlRes) { m_vecCntPxlRes = vecCntPxlRes; }
	void SetVecCalAreaResult(std::vector<CAlgorithmsCalculateAreaResult> vecCalAreaRes) { m_vecCalAreaRes = vecCalAreaRes; }
	

public:
	BOOL Judge_Result();
	BOOL Draw_Result(int nIdx);
	void Reset();
private:
	// interface
	ITempInspectResultToParent*                    m_pInterface;

	std::vector<CLocatorToolResult>                m_vecLocRes;
	std::vector<CAlgorithmsCountPixelResult>       m_vecCntPxlRes;
	std::vector<CAlgorithmsCalculateAreaResult>    m_vecCalAreaRes;

	cv::Mat                                        m_imageResult;
	CCriticalSection					           m_crsJudgement;

	// variable result
	BOOL  m_bSumResults = TRUE;
};