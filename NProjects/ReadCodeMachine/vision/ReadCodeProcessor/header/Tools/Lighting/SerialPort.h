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
	
	// Event Watch용 쓰레드 함수
	static DWORD CommWatchProc( LPSTR lpData );
	DWORD CommWatchFunc();

private:
	ISerialPort2Parent* m_pISP2P; // 콜백 인터페이스
	HANDLE m_idComDev;		// 포트 핸들
	
	// 포트 상태 관련
	BOOL    m_fConnected, m_fXonXoff, m_fLocalEcho, m_fAutoWrap, m_fUseCNReceive, m_fDisplayErrors;
	BYTE    m_bByteSize, m_bFlowCtrl, m_bParity, m_bStopBits;
	DWORD   m_dwBaudRate ;

	HANDLE		m_hWatchThread;	// 쓰레드 핸들
	DWORD       m_dwThreadID ;	// 쓰레드 ID
	OVERLAPPED  m_osWrite, m_osRead ;	// Overlapped I/O를 위한 구조체
	CCriticalSection		m_csComDev;
};