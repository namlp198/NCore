#include "pch.h"
#include "LightControls_Dasol_Ch16.h"
#include "Serial.h"

CLightControls_Dasol_Ch16::CLightControls_Dasol_Ch16(int nIndex, ILightControls2Parent * pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
}

CLightControls_Dasol_Ch16::~CLightControls_Dasol_Ch16(void)
{
	Disconnect();

	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_Dasol_Ch16::Connect(const CLightControlsParam & param)
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

void CLightControls_Dasol_Ch16::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}

BOOL CLightControls_Dasol_Ch16::SetLightLevel( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>15) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "[%02d%03d", nChannel, nValue);

	nSendSize = (DWORD)strlen(pSendCmd);
	
	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	//m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_Dasol_Ch16::SetLightLevel( double dValue, int nChannel /*= 0*/ )
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_Dasol_Ch16::SetLightStatus( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>15) return FALSE;
	
	// check value
	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	
	switch (nValue)
	{
	case LightControls_Status_Off: // off
		sprintf_s(pSendCmd, "H%dOF%c%c", nChannel+1, ASCII_CR, ASCII_LF);
		break;

	case LightControls_Status_On: // on
		sprintf_s(pSendCmd, "H%dON%c%c", nChannel+1, ASCII_CR, ASCII_LF);
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

BOOL CLightControls_Dasol_Ch16::SetLightStatusAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_Dasol_Ch16::SetLightLevelAll(int nValue)
{
	// check channel
	// if (nChannel < 0 || nChannel>15) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;

	pSendCmd[nSendSize++] = 0x02;
	pSendCmd[nSendSize++] = 0x04;
	pSendCmd[nSendSize++] = 0x85;
	pSendCmd[nSendSize++] = (CHAR) nValue;

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	//m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;

	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightLevel(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_Dasol_Ch16::SetLightAllOnOFF(bool bOn)
{

	return TRUE;
}

BOOL CLightControls_Dasol_Ch16::SetLightLevel(std::vector<int> nValue)
{
	return TRUE;
}


BOOL CLightControls_Dasol_Ch16::GetConnected()
{
	BOOL bConnect = m_pSerialPort->Connected();
	m_ControlStatus.SetStatus_Connected(bConnect);
	return m_ControlStatus.GetStatus_Connected();
}

void CLightControls_Dasol_Ch16::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
{
	if (nLength < 1) return;

	// parsing
// 	for (int i = 0; i < nLength-4; i++)
// 	{		
// 		if (lpszData[i] == 'R')
// 		{
// 			CString strTmp = L"";
// 			strTmp.Format(L"%c", lpszData[i + 1]);
// 
// 			int nChannel = _ttoi(strTmp);
// 
// 			break;
// 		}
// 	}

	if (lpszData[0] == 'R' && lpszData[2] != 'O')
	{
		CString strTmp = L"";
		strTmp.Format(L"%c", lpszData[1]);
		
		int nChannel = _ttoi(strTmp);

		CString str;
		str.Format(L"%c%c", lpszData[2], lpszData[3]);

		wchar_t *end = NULL;
		long nValue = wcstol(str, &end, 16);

		m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel - 1);
	}
	
	return;
}


BOOL CLightControls_Dasol_Ch16::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;

	CSingleLock localLock(&m_csLightControl);
	localLock.Lock();
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
