#include "stdafx.h"
#include "LightControls_Simulation.h"


CLightControls_Simulation::CLightControls_Simulation(int nIndex, ILightControls2Parent* pILC2P) : CLightControls(nIndex, pILC2P)
{

}

CLightControls_Simulation::~CLightControls_Simulation()
{
}

BOOL CLightControls_Simulation::Connect(const CLightControlsParam& param)
{
	CLightControlsParam* pParam = (CLightControlsParam*)&m_ControlStatus;
	*pParam = param;

	SetLightStatusAll(LightControls_Status_On);
	SetLightLevelAll(m_ControlStatus.GetParam_DefaultValue());
	m_ControlStatus.SetStatus_Connected(1);

	return TRUE;
}

void CLightControls_Simulation::Disconnect()
{

}

BOOL CLightControls_Simulation::SetLightLevel(int nValue, int nChannel /*= 0*/)
{
	m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_Simulation::SetLightLevel(double dValue, int nChannel /*= 0*/)
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_Simulation::SetLightStatus(int nValue, int nChannel /*= 0*/)
{
	return TRUE;
}

BOOL CLightControls_Simulation::SetLightStatusAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		m_ControlStatus.SetStatus_CurrentStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_Simulation::SetLightLevelAll(int nValue)
{
	return TRUE;
}

BOOL CLightControls_Simulation::GetConnected()
{
	return TRUE;
}
