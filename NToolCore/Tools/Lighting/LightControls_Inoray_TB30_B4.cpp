#include "stdafx.h"
#include "LightControls_Inoray_TB30_B4.h"

CLightControls_Inoray_TB30_B4::CLightControls_Inoray_TB30_B4(int nIndex, ILightControls2Parent* pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
}

CLightControls_Inoray_TB30_B4::~CLightControls_Inoray_TB30_B4(void)
{
	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_Inoray_TB30_B4::Connect(const CLightControlsParam& param)
{
	CLightControlsParam* pParam = (CLightControlsParam*)&m_ControlStatus;
	*pParam = param;

	int nPort = _ttoi(m_ControlStatus.GetParam_ConnectAddr());

	if (FALSE == m_pSerialPort->OpenPort(nPort, (DWORD)CBR_9600))
	{
		return FALSE;
	}

	SetLightStatusAll(LightControls_Status_On);
	SetLightLevelAll(m_ControlStatus.GetParam_DefaultValue());

	return TRUE;
}

void CLightControls_Inoray_TB30_B4::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}

BOOL CLightControls_Inoray_TB30_B4::SetLightLevel(int nValue, int nChannel /*= 0*/)
{
	// check channel
	if (nChannel < 0 || nChannel>3) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "%c%dw%04d%c", 0x02, nChannel, nValue, 0x03);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	//m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_Inoray_TB30_B4::SetLightLevel(double dValue, int nChannel /*= 0*/)
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_Inoray_TB30_B4::SetLightStatus(int nValue, int nChannel /*= 0*/)
{
	// check channel
	if (nChannel < 0 || nChannel>3) return FALSE;

	// check value
	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;

	switch (nValue)
	{
	case LightControls_Status_Off: // off
		sprintf_s(pSendCmd, "%c%df%c", 0x02, nChannel, 0x03);
		break;

	case LightControls_Status_On: // on
		sprintf_s(pSendCmd, "%c%do%c", 0x02, nChannel, 0x03);
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

BOOL CLightControls_Inoray_TB30_B4::SetLightStatusAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_Inoray_TB30_B4::SetLightLevelAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightLevel(nValue, i);
	}
	return bResult;
}

void CLightControls_Inoray_TB30_B4::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
{

}

BOOL CLightControls_Inoray_TB30_B4::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
