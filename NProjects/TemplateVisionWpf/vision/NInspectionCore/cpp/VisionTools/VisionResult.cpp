#include "pch.h"
#include "VisionResult.h"

CVisionResults::CVisionResults()
{
	m_bSumResult = FALSE;
	m_vecLocToolRes = NULL;
	m_vecCntPxlRes = NULL;
	m_vecCalAreaRes = NULL;
}

CVisionResults::~CVisionResults()
{
	if (m_vecLocToolRes != NULL)
		delete m_vecLocToolRes, m_vecLocToolRes = NULL;
	if (m_vecCntPxlRes != NULL)
		delete m_vecCntPxlRes, m_vecCntPxlRes = NULL;
	if (m_vecCalAreaRes != NULL)
		delete m_vecCalAreaRes, m_vecCalAreaRes = NULL;
	if(m_pResultImage != NULL)
		m_pResultImage->release();
}

LPBYTE CVisionResults::GetResultImageBuffer()
{
	if (m_pResultImage->empty())
		return nullptr;

	return (LPBYTE)m_pResultImage->data;
}

BOOL CVisionResults::GetSumResult(CSumResult* pSumRes)
{
	pSumRes->m_bSumResult = m_bSumResult;
	pSumRes->m_resultImageBuffer = GetResultImageBuffer();

	return TRUE;
}

BOOL CVisionResults::Judgement_Result()
{
	if (!m_vecLocToolRes->empty())
		for (const auto& locToolRes : *m_vecLocToolRes)
		{
			m_bSumResult = locToolRes.m_bResult;
			if (m_bSumResult == FALSE)
				return FALSE;
		}

	if (!m_vecCntPxlRes->empty())
		for (const auto& cntPxlRes : *m_vecCntPxlRes)
		{
			m_bSumResult = cntPxlRes.m_bResult;
			if (m_bSumResult == FALSE)
				return FALSE;
		}

	if (!m_vecCalAreaRes->empty())
		for (const auto& calAreaRes : *m_vecCalAreaRes)
		{
			m_bSumResult = calAreaRes.m_bResult;
			if (m_bSumResult == FALSE)
				return FALSE;
		}

	return m_bSumResult;
}

BOOL CVisionResults::Reset_Result()
{
	m_vecLocToolRes->clear();
	m_vecCntPxlRes->clear();
	m_vecCalAreaRes->clear();
	m_pResultImage->release();
	m_bSumResult = FALSE;

	return TRUE;
}

BOOL CVisionResults::Draw_Result()
{
	return TRUE;
}
