#include "stdafx.h"
#include "LightControls.h"


CLightControls::CLightControls(int nIndex, ILightControls2Parent* pILC2P) : m_pILC2P(pILC2P), m_nIndex(nIndex)
{
}

CLightControls::~CLightControls()
{
}

BOOL CLightControls::GetLightLevel(int &nValue, int nChannel /*= 0*/)
{
	nValue = m_ControlStatus.GetStatus_CurrentValue(nChannel);
	return TRUE;
}

BOOL CLightControls::GetLightLevel(double &dValue, int nChannel /*= 0*/)
{
	int nTemp = 0;
	GetLightLevel(nTemp, nChannel);
	dValue = nTemp;
	return TRUE;
}

BOOL CLightControls::GetLightStatus(int &nValue, int nChannel /*= 0*/)
{
	nValue = m_ControlStatus.GetStatus_CurrentStatus(nChannel);
	return TRUE;
}

BOOL CLightControls::GetConnected()
{
	return m_ControlStatus.GetStatus_Connected();
}

const CLightControlsStatus* CLightControls::GetStatus() const
{
	return &m_ControlStatus;
}