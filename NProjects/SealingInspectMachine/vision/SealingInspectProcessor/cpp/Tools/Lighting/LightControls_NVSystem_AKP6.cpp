#include "pch.h"
#include "LightControls_NVSystem_AKP6.h"

#define ASCII_LF        0x0A
#define ASCII_CR        0x0D

CLightControls_NVSystem_AKP6::CLightControls_NVSystem_AKP6(int nIndex, ILightControls2Parent * pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
}

CLightControls_NVSystem_AKP6::~CLightControls_NVSystem_AKP6(void)
{
	Disconnect();

	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_NVSystem_AKP6::Connect(const CLightControlsParam & param)
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

void CLightControls_NVSystem_AKP6::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}

BOOL CLightControls_NVSystem_AKP6::SetLightLevel( int nValue, int nChannel /*= 0*/ )
{
	// check channel
	if (nChannel < 0 || nChannel>15) return FALSE;

	// check value
	if (nValue < m_ControlStatus.GetParam_MinValue() || nValue > m_ControlStatus.GetParam_MaxValue()) return FALSE;

	CHAR pSendCmd[20] = { 0 };
	DWORD nSendSize = 0;
	sprintf_s(pSendCmd, "C%d%02x%c%c", nChannel + 1, nValue, ASCII_CR, ASCII_LF);

	nSendSize = (DWORD)strlen(pSendCmd);
	
	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}

	//m_ControlStatus.SetStatus_CurrentValue(nValue, nChannel);
	return TRUE;
}

BOOL CLightControls_NVSystem_AKP6::SetLightLevel( double dValue, int nChannel /*= 0*/ )
{
	return SetLightLevel((int)dValue, nChannel);
}

BOOL CLightControls_NVSystem_AKP6::SetLightStatus( int nValue, int nChannel /*= 0*/ )
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

BOOL CLightControls_NVSystem_AKP6::SetLightStatusAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightStatus(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_NVSystem_AKP6::SetLightLevelAll(int nValue)
{
	BOOL bResult = TRUE;
	for (int i = 0; i < m_ControlStatus.GetParam_ChannelCount(); i++)
	{
		bResult = bResult && SetLightLevel(nValue, i);
	}
	return bResult;
}

BOOL CLightControls_NVSystem_AKP6::GetConnected()
{
	BOOL bConnect = m_pSerialPort->Connected();
	m_ControlStatus.SetStatus_Connected(bConnect);
	return m_ControlStatus.GetStatus_Connected();
}

void CLightControls_NVSystem_AKP6::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
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


BOOL CLightControls_NVSystem_AKP6::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;

	CSingleLock localLock(&m_csLightControl);
	localLock.Lock();
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
