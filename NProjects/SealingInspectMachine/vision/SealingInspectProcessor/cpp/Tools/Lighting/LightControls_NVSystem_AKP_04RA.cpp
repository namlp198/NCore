#include "pch.h"
#include "LightControls_NVSystem_AKP_04RA.h"

CLightControls_NVSystem_AKP_04RA::CLightControls_NVSystem_AKP_04RA(int nIndex, ILightControls2Parent * pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
}

CLightControls_NVSystem_AKP_04RA::~CLightControls_NVSystem_AKP_04RA(void)
{
	Disconnect();

	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_NVSystem_AKP_04RA::Connect(const CLightControlsParam & param)
{
	CLightControlsParam* pParam = (CLightControlsParam*)&m_ControlStatus;
	*pParam = param;

	int nPort = _ttoi(m_ControlStatus.GetParam_ConnectAddr());

	if (FALSE == m_pSerialPort->OpenPort(nPort, (DWORD)CBR_9600))
	{
		return FALSE;
	}

	m_ControlStatus.SetStatus_Connected(1);
	SetLightStatusAll(LightControls_Status_On);
	SetLightLevelAll(m_ControlStatus.GetParam_DefaultValue());

	return TRUE;
}

void CLightControls_NVSystem_AKP_04RA::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}

BOOL CLightControls_NVSystem_AKP_04RA::SetLightLevel( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>15) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "[%02d%03d", nChannel + 1, nValue);

	nSendSize = (DWORD)strlen(pSendCmd);
	
	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_NVSystem_AKP_04RA::SetLightLevel( double dValue, int nChannel /*= 0*/ )
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_NVSystem_AKP_04RA::SetLightStatus( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>15) return FALSE;
	
	// check value
	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	
	switch (nValue)
	{
	case LightControls_Status_Off: // off
		sprintf_s(pSendCmd, "]%02d0", nChannel+1);
		break;

	case LightControls_Status_On: // on
		sprintf_s(pSendCmd, "]%02d1", nChannel+1);
		break;

	default:
		return FALSE;
		break;
	}
	   	 
	nSendSize = (DWORD)strlen(pSendCmd);
	
	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	m_ControlStatus.SetStatus_CurrentStatus(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_NVSystem_AKP_04RA::SetLightStatusAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_NVSystem_AKP_04RA::SetLightLevelAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightLevel(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_NVSystem_AKP_04RA::GetConnected()
{
	return m_pSerialPort->Connected();
}

void CLightControls_NVSystem_AKP_04RA::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
{
	if (nLength < 1) return;	
	
	return;
}

BOOL CLightControls_NVSystem_AKP_04RA::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
