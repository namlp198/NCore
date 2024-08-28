#include "pch.h"
#include "LightControls_L_LIGHT_50W4CH_LP.h"

#define ASCII_STX        0x02
#define ASCII_ETX        0x03

CLightControls_L_LIGHT_50W4CH_LP::CLightControls_L_LIGHT_50W4CH_LP(int nIndex, ILightControls2Parent * pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
	ZeroMemory(m_nSetLightValue, sizeof(m_nSetLightValue));
}

CLightControls_L_LIGHT_50W4CH_LP::~CLightControls_L_LIGHT_50W4CH_LP(void)
{
	Disconnect();

	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::Connect(const CLightControlsParam & param)
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

void CLightControls_L_LIGHT_50W4CH_LP::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightLevel( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>=m_ControlStatus.GetParam_ChannelCount()) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "%cCH%dS%03d%c", ASCII_STX, nChannel+1, nValue, ASCII_ETX);

	nSendSize = (DWORD)strlen(pSendCmd);
	
	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	m_nSetLightValue[nChannel] = nValue;
	//m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightLevel( double dValue, int nChannel /*= 0*/ )
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightStatus( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel >= m_ControlStatus.GetParam_ChannelCount()) return FALSE;
	
	// check value
	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	
	switch (nValue)
	{
	case LightControls_Status_Off: // off
		sprintf_s(pSendCmd, "%c%02dON0%c", ASCII_STX, nChannel + 1, ASCII_ETX);
		break;

	case LightControls_Status_On: // on
		sprintf_s(pSendCmd, "%c%02dON1%c", ASCII_STX, nChannel + 1, ASCII_ETX);
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

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightStatusAll(int nValue)
{
	// check value
	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;

	switch (nValue)
	{
	case LightControls_Status_Off: // off
		sprintf_s(pSendCmd, "%c99ON0%c", ASCII_STX, ASCII_ETX);
		break;

	case LightControls_Status_On: // on
		sprintf_s(pSendCmd, "%c99ON1%c", ASCII_STX, ASCII_ETX);
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

	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		m_ControlStatus.SetStatus_CurrentStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightLevelAll(int nValue)
{
	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "%cCHAS%03d%c", ASCII_STX, nValue, ASCII_ETX);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	return TRUE;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::SetLightLevelAll(int nValueCh0, int nValueCh1, int nValueCh2, int nValueCh3)
{
	// check value
	if (nValueCh0 < m_ControlStatus.GetParam_MinValue() || nValueCh0 > m_ControlStatus.GetParam_MaxValue()) return FALSE;
	if (nValueCh1 < m_ControlStatus.GetParam_MinValue() || nValueCh1 > m_ControlStatus.GetParam_MaxValue()) return FALSE;
	if (nValueCh2 < m_ControlStatus.GetParam_MinValue() || nValueCh2 > m_ControlStatus.GetParam_MaxValue()) return FALSE;
	if (nValueCh3 < m_ControlStatus.GetParam_MinValue() || nValueCh3 > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[30] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "%cCHAS%03d%03d%03d%03d%c", ASCII_STX, nValueCh0, nValueCh1, nValueCh2, nValueCh3, ASCII_ETX);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	m_nSetLightValue[0] = nValueCh0;
	m_nSetLightValue[1] = nValueCh1;
	m_nSetLightValue[2] = nValueCh2;
	m_nSetLightValue[3] = nValueCh3;

	return TRUE;
}

BOOL CLightControls_L_LIGHT_50W4CH_LP::GetConnected()
{
	BOOL bConnect = m_pSerialPort->Connected();
	m_ControlStatus.SetStatus_Connected(bConnect);
	return m_ControlStatus.GetStatus_Connected();
}

void CLightControls_L_LIGHT_50W4CH_LP::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
{
	if (nLength < 1) return;

	CHAR pRecvData[20] = { 0 };
	DWORD nRecvData = 0;

	memcpy(pRecvData, lpszData, sizeof(CHAR)*nLength);
	nRecvData = nLength;

	if (lpszData[0] == ASCII_STX && lpszData[nLength - 1] == ASCII_ETX)
	{
		if (lpszData[1] == 'C' && lpszData[2] == 'H' && lpszData[3] != 'A' && lpszData[4] == 'S')
		{
			CString strTmp = L"";
			strTmp.Format(L"%c", lpszData[3]);

			int nChannel = _ttoi(strTmp);
			nChannel -= 1;

			strTmp.Format(L"%c%c", lpszData[5], lpszData[6]);
			if (strTmp == L"OK")
				m_ControlStatus.SetStatus_CurrentValue(m_nSetLightValue[nChannel], nChannel);
		}
		else if (lpszData[1] == 'C' && lpszData[2] == 'H' && lpszData[3] == 'A' && lpszData[4] == 'S')
		{
			CString strTmp = L"";
			
			strTmp.Format(L"%c%c", lpszData[5], lpszData[6]);
			if (strTmp == L"OK")
			{
				m_ControlStatus.SetStatus_CurrentValue(m_nSetLightValue[0], 0);
				m_ControlStatus.SetStatus_CurrentValue(m_nSetLightValue[1], 1);
				m_ControlStatus.SetStatus_CurrentValue(m_nSetLightValue[2], 2);
				m_ControlStatus.SetStatus_CurrentValue(m_nSetLightValue[3], 3);			
			}
		}
	}

	return;
}


BOOL CLightControls_L_LIGHT_50W4CH_LP::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
