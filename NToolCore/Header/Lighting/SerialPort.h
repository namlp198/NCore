#pragma once

interface ISerialPort2Parent
{
	virtual void ISP2P_ReceiveData(LPSTR lpszData, long nLength) = 0;
};

class AFX_EXT_CLASS CSerialPort
{
public:
	CSerialPort(ISerialPort2Parent* pISP2P);
	virtual ~CSerialPort();
	// member function
	BOOL OpenPort(long port_number,long baud_rate_select=6);
	BOOL OpenPort(long port_number,DWORD baud_rate);
	BOOL ClosePort(void);
	BOOL Connected(void);

	BOOL SetupConnection(void);

	long ReadCommBlock(LPSTR lpszBlock, long nMaxLength );
	BOOL WriteCommBlock(LPSTR lpByte , DWORD dwBytesToWrite);
	
	// Event Watch�� ������ �Լ�
	static DWORD CommWatchProc( LPSTR lpData );
	DWORD CommWatchFunc();

private:
	ISerialPort2Parent* m_pISP2P; // �ݹ� �������̽�
	HANDLE m_idComDev;		// ��Ʈ �ڵ�
	
	// ��Ʈ ���� ����
	BOOL    m_fConnected, m_fXonXoff, m_fLocalEcho, m_fAutoWrap, m_fUseCNReceive, m_fDisplayErrors;
	BYTE    m_bByteSize, m_bFlowCtrl, m_bParity, m_bStopBits;
	DWORD   m_dwBaudRate ;

	HANDLE		m_hWatchThread;	// ������ �ڵ�
	DWORD       m_dwThreadID ;	// ������ ID
	OVERLAPPED  m_osWrite, m_osRead ;	// Overlapped I/O�� ���� ����ü
	CCriticalSection		m_csComDev;
};