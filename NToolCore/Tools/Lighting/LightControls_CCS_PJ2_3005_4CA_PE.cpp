#include "stdafx.h"
#include "LightControls_CCS_PJ2_3005_4CA_PE.h"

CLightControls_CCS_PJ2_3005_4CA_PE::CLightControls_CCS_PJ2_3005_4CA_PE(int nIndex, ILightControls2Parent* pILC2P) : CLightControls(nIndex, pILC2P)
{

}

CLightControls_CCS_PJ2_3005_4CA_PE::~CLightControls_CCS_PJ2_3005_4CA_PE(void)
{

}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::Connect(const CLightControlsParam& param)
{
	return FALSE;
}

void CLightControls_CCS_PJ2_3005_4CA_PE::Disconnect()
{

}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::SetLightLevel(int nValue, int nChannel /*= 0*/)
{
	return FALSE;
}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::SetLightLevel(double dValue, int nChannel /*= 0*/)
{
	return FALSE;
}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::SetLightStatus(int nValue, int nChannel /*= 0*/)
{
	return FALSE;
}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::SetLightStatusAll(int nValue)
{
	return FALSE;

}

BOOL CLightControls_CCS_PJ2_3005_4CA_PE::SetLightLevelAll(int nValue)
{
	return FALSE;
}
