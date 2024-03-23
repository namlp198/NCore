#include "pch.h"
#include "TempInspectResult.h"

CTempInspectResult::CTempInspectResult(ITempInspectResultToParent* pInterface)
{
	m_pInterface = pInterface;
}

CTempInspectResult::~CTempInspectResult()
{
}

LPBYTE CTempInspectResult::GetImageResult_DrawnResult()
{
	if (!m_imageResult.empty())
		return nullptr;

	return (LPBYTE)m_imageResult.data;
}

BOOL CTempInspectResult::Judge_Result()
{
	CSingleLock localLock(&m_crsJudgement);
	localLock.Lock();
	if (!m_vecLocRes.empty())
		for (CLocatorToolResult locRes : m_vecLocRes)
			m_bSumResults = locRes.m_bResult;

	if (!m_vecCntPxlRes.empty())
		for (CAlgorithmsCountPixelResult cntPxlRes : m_vecCntPxlRes)
			m_bSumResults = cntPxlRes.m_bResult;

	if (!m_vecCalAreaRes.empty())
		for (CAlgorithmsCalculateAreaResult calAreaRes : m_vecCalAreaRes)
			m_bSumResults = calAreaRes.m_bResult;
	localLock.Unlock();

	return m_bSumResults;
}

BOOL CTempInspectResult::Draw_Result(int nIdx)
{
	CTempInspectRecipe* recipe = m_pInterface->GetRecipe(nIdx);
	if (recipe == NULL)
		return FALSE;

	// draw result into m_imageResult object


	return TRUE;
}

void CTempInspectResult::Reset()
{
	m_vecLocRes.clear();
	m_vecCntPxlRes.clear();
	m_vecCalAreaRes.clear();
}
