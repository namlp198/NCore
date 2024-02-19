#include "StdAfx.h"
#include "LightControls_PLS_Ch8.h"

CLightControls_PLS_Ch8::CLightControls_PLS_Ch8(int nIndex, ILightControls2Parent * pILC2P) : CLightControls(nIndex, pILC2P)
{
	m_pSerialPort = new CSerialPort(this);
}

CLightControls_PLS_Ch8::~CLightControls_PLS_Ch8(void)
{
	Disconnect();

	if (m_pSerialPort)
	{
		delete m_pSerialPort;
	}
	m_pSerialPort = nullptr;
}

BOOL CLightControls_PLS_Ch8::Connect(const CLightControlsParam & param)
{
	CLightControlsParam* pParam = (CLightControlsParam*)&m_ControlStatus;
	*pParam = param;

	int nPort = _ttoi(m_ControlStatus.GetParam_ConnectAddr());

	if (FALSE == m_pSerialPort->OpenPort(nPort, (DWORD)CBR_115200))
	{
		return FALSE;
	}

	m_ControlStatus.SetStatus_Connected(1);
	SetLightStatusAll(LightControls_Status_On);
	SetLightLevelAll(m_ControlStatus.GetParam_DefaultValue());

	return TRUE;
}

void CLightControls_PLS_Ch8::Disconnect()
{
	if (FALSE == m_pSerialPort->ClosePort())
	{
		return;
	}
	return;
}


BOOL CLightControls_PLS_Ch8::SetLightLevel(int nValue, int nChannel /*= 0*/)
{
	CHAR pSendCmd[256] = { 0 };
	DWORD nSendSize = 0;

	sprintf_s(pSendCmd, "*W%02d%04d$", nChannel+1, nValue);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}
	return TRUE;
}

BOOL CLightControls_PLS_Ch8::SetLightLevel(std::vector<int> nValue)
{
	CHAR pSendCmd[256] = { 0 };
	DWORD nSendSize = 0;

	pSendCmd[nSendSize++] = '*';
	pSendCmd[nSendSize++] = 'M';
	pSendCmd[nSendSize++] = 'C';

	for(int i=0; i< (int) nValue.size(); i++)
	{
		char strValue[5] = {};
		sprintf_s(strValue, "%04d", nValue[i]);

		pSendCmd[nSendSize++] = ',';
		pSendCmd[nSendSize++] = strValue[0];
		pSendCmd[nSendSize++] = strValue[1];
		pSendCmd[nSendSize++] = strValue[2];
		pSendCmd[nSendSize++] = strValue[3];
	}

	pSendCmd[nSendSize++] = '$';

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}
	return TRUE;
}

BOOL CLightControls_PLS_Ch8::SetLightLevel(double dValue, int nChannel /*= 0*/)
{
	CHAR pSendCmd[256] = { 0 };
	DWORD nSendSize = 0;

	sprintf_s(pSendCmd, "*W%02d%04d$", nChannel + 1, (int) dValue);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}
	return TRUE;
}

BOOL CLightControls_PLS_Ch8::SetLightLevelAll(int nValue)
{
	CHAR pSendCmd[256] = { 0 };
	DWORD nSendSize = 0;

	sprintf_s(pSendCmd, "*W00%04d$", nValue);

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}
	return TRUE;
}

BOOL CLightControls_PLS_Ch8::GetConnected()
{
	BOOL bConnect = m_pSerialPort->Connected();
	m_ControlStatus.SetStatus_Connected(bConnect);
	return m_ControlStatus.GetStatus_Connected();
}

BOOL CLightControls_PLS_Ch8::SetLightAllOnOFF(bool bOn)
{
	CHAR pSendCmd[64] = { 0 };
	DWORD nSendSize = 0;

	if(bOn == true)
		sprintf_s(pSendCmd, "*O001$");
	else
		sprintf_s(pSendCmd, "*O000$");

	nSendSize = (DWORD)strlen(pSendCmd);

	if (FALSE == SendData(pSendCmd, nSendSize))
	{
		return FALSE;
	}
	return TRUE;
}

void CLightControls_PLS_Ch8::ISP2P_ReceiveData(LPSTR lpszData, long nLength)
{
	return;

/*
	if (nLength < 1) return;

	// parsing

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
*/
	
	return;
}


BOOL CLightControls_PLS_Ch8::SendData(CHAR* pData, DWORD nSize)
{
	if (m_pSerialPort == NULL) return FALSE;
	if (m_pSerialPort->Connected() == FALSE) return FALSE;

	CSingleLock localLock(&m_csLightControl);
	localLock.Lock();
	return m_pSerialPort->WriteCommBlock((LPSTR)pData, nSize);
}
