/*******************************************************************************
*                         태 양 추 적 프 로 젝 트                              *
********************************************************************************
* 개 발 자 : FlowerMJ                                                          *
*                                                                              *
* 최초 완료일 : 060517                                                         *
*                                                                              *
*	포트로 Input 되는 Character를 화면상에 출력                                *
*	일정 시간마다 특정 길이의 문자열을 포트로 Output                           *
*	두 포트를 연결하여 테스트 할때는 Baud_Rate가 일치해야만 정상적인           *
*	결과를 얻을 수 있다                                                        *
* --------------------------- 주요 코드 ---------------------------------------*
* 코드명      의미                                                             *
* ----------- -----------------------------------------------------------------*
*                                                                              *
*                                                                              *
* --------------------------- 수정 이력 ---------------------------------------*
* 일시        수정내용                                                         *
* ----------- -----------------------------------------------------------------*
*                                                                              *
*                                                                              *
*******************************************************************************/

#include <stdafx.h>

#include "SerialPort.h"

#define MAXBLOCK        80

DWORD BAUD_RATE_TABLE[] = {	CBR_110,CBR_300,CBR_600,CBR_1200,CBR_2400,CBR_4800,CBR_9600,CBR_14400,
							CBR_19200,CBR_38400,CBR_56000,CBR_57600,CBR_115200,230400,460800,921600,0};

// Flow control flags
#define FC_DTRDSR       0x01   //데이터 단말기(DTR) 대기,데이터 세트(DSR) 대기를 위한 신호
#define FC_RTSCTS       0x02
#define FC_XONXOFF      0x04

// ascii definitions
#define ASCII_BEL       0x07
#define ASCII_BS        0x08
#define ASCII_LF        0x0A
#define ASCII_CR        0x0D
#define ASCII_XON       0x11
#define ASCII_XOFF      0x13

CSerialPort::CSerialPort(ISerialPort2Parent* pISP2P) : m_pISP2P(pISP2P)
{
	// 포트 상태 관련 변수들을 초기화 한다
	m_idComDev = NULL;
	m_fConnected = FALSE;
	m_fLocalEcho = FALSE;
	m_fAutoWrap	= TRUE;
	m_dwBaudRate = CBR_19200;
	m_bByteSize = 8;
	m_bFlowCtrl = FC_XONXOFF;
	m_bParity = NOPARITY;
	m_bStopBits	= ONESTOPBIT;
	m_fXonXoff = FALSE;
	m_fUseCNReceive	= TRUE;
	m_fDisplayErrors = TRUE;
	m_osWrite.Offset = 0;
	m_osWrite.OffsetHigh = 0;
	m_osRead.Offset = 0;
	m_osRead.OffsetHigh = 0;

	m_hWatchThread = NULL;
	m_dwThreadID = 0;
	

}

CSerialPort::~CSerialPort()
{
	ClosePort();
	m_pISP2P = NULL;
}

BOOL CSerialPort::OpenPort(long port_number,DWORD baud_rate)
{
	// 포트 상태 관련 변수들을 초기화 한다
	m_idComDev = NULL;
	m_fConnected = FALSE;
	m_fLocalEcho = FALSE;
	m_fAutoWrap	= TRUE;
	m_dwBaudRate = baud_rate;
	m_bByteSize = 8;
	m_bFlowCtrl = FC_XONXOFF;
	m_bParity = NOPARITY;
	m_bStopBits	= ONESTOPBIT;
	m_fXonXoff = FALSE;
	m_fUseCNReceive	= TRUE;
	m_fDisplayErrors = TRUE;
	m_osWrite.Offset = 0;
	m_osWrite.OffsetHigh = 0;
	m_osRead.Offset = 0;
	m_osRead.OffsetHigh = 0;
	
	
	// Overlapped I/O에 쓰이는 Event 객체들을 생성

	// Read를 위한 Event 객체 생성
	m_osRead.hEvent = CreateEvent(
		NULL,    
		TRUE,    
		FALSE,   
		NULL ) ;
	if (m_osRead.hEvent==NULL)
	{
		return FALSE ;
	}
	
	// Write를 위한 Event 객체 생성
	m_osWrite.hEvent = CreateEvent(
		NULL,   
		TRUE,   
		FALSE,  
		NULL ) ;
	
	if (m_osWrite.hEvent==NULL)
	{
		CloseHandle( m_osRead.hEvent );

		m_osRead.hEvent = NULL;
		return FALSE;
	}

	
	// 포트를 생성한다
	CString strPort = L"";
	strPort.Format(L"\\\\.\\COM%d", port_number);
	
	m_idComDev = CreateFile(strPort,
		GENERIC_READ | GENERIC_WRITE,
		0,							
		NULL,						
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED,	// Overlapped I/O
		NULL);

	if (m_idComDev == INVALID_HANDLE_VALUE)
	{
		CloseHandle(m_osRead.hEvent);
		CloseHandle(m_osWrite.hEvent);

		m_osRead.hEvent = NULL;
		m_osWrite.hEvent = NULL;
		return FALSE;
	}
	
	SetCommMask( m_idComDev, EV_RXCHAR ) ;
	SetupComm( m_idComDev, 4096, 4096 ) ;	// 버퍼 설정
	PurgeComm( m_idComDev, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR ) ;	// 버퍼의 모든 데이타를 지운다
	

	// Overlapped I/O를 위한 Time Out 설정
	COMMTIMEOUTS  CommTimeOuts ;
	CommTimeOuts.ReadIntervalTimeout = 0xFFFFFFFF ;
	CommTimeOuts.ReadTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.ReadTotalTimeoutConstant = 1000 ;

	CommTimeOuts.WriteTotalTimeoutMultiplier = 2*CBR_9600/m_dwBaudRate ; // CBR_9600 기준 ms당 바이트를 두배까지 설정
	CommTimeOuts.WriteTotalTimeoutConstant = 0 ;
	SetCommTimeouts( m_idComDev, &CommTimeOuts ) ;
	
	// 포트를 사용가능 상태로 만들고 Event를 감시할 쓰레드를 생성한다
	if(SetupConnection()==TRUE)
	{
		m_fConnected = TRUE ;	
		
		// 쓰레드 생성
		m_hWatchThread = CreateThread(
			(LPSECURITY_ATTRIBUTES) NULL,
			0,
			(LPTHREAD_START_ROUTINE) CommWatchProc,
			(LPVOID)this,
			0,
			&m_dwThreadID );

		if(m_hWatchThread==NULL)	// 쓰레드 생성 실패
		{
			m_fConnected = FALSE ;

			CloseHandle(m_osRead.hEvent);
			CloseHandle(m_osWrite.hEvent);
			CloseHandle( m_idComDev );

			m_osRead.hEvent = NULL;
			m_osWrite.hEvent = NULL;
			m_idComDev = NULL;
			return FALSE;
		}
		else
		{
			// 장치에 DTR(Data-Terminal-Ready)을 알린다
			EscapeCommFunction( m_idComDev, SETDTR ) ;
		}
	}
	else
	{
		m_fConnected = FALSE ;

		CloseHandle(m_osRead.hEvent);
		CloseHandle(m_osWrite.hEvent);
		CloseHandle( m_idComDev ) ;

		m_osRead.hEvent = NULL;
		m_osWrite.hEvent = NULL;
		m_idComDev = NULL;
		return FALSE;
	}
	return TRUE;
}

BOOL CSerialPort::OpenPort(long port_number,long baud_rate_select)
{
	return OpenPort(port_number, BAUD_RATE_TABLE[baud_rate_select]);
}

BOOL CSerialPort::SetupConnection(void)
{
	// DCB 구조체를 이용하여 포트를 셋팅한다
	BYTE       bSet ;
	DCB        dcb ;
	
	dcb.DCBlength = sizeof( DCB ) ;
	
	GetCommState( m_idComDev, &dcb ) ;
	
	dcb.BaudRate = m_dwBaudRate ;
	dcb.ByteSize = m_bByteSize;
	dcb.Parity = m_bParity;
	dcb.StopBits = m_bStopBits;
	
	// setup hardware flow control
	
	bSet = (BYTE) ((m_bFlowCtrl & FC_DTRDSR) != 0) ;
	dcb.fOutxDsrFlow = bSet ;
	if (bSet)
		dcb.fDtrControl = DTR_CONTROL_HANDSHAKE ;
	else
		dcb.fDtrControl = DTR_CONTROL_ENABLE ;
	
	bSet = (BYTE) ((m_bFlowCtrl & FC_RTSCTS) != 0) ;
	dcb.fOutxCtsFlow = bSet ;
	if (bSet)
		dcb.fRtsControl = RTS_CONTROL_HANDSHAKE ;
	else
		dcb.fRtsControl = RTS_CONTROL_ENABLE ;
	
	// setup software flow control
	
	bSet = (BYTE) ((m_bFlowCtrl & FC_XONXOFF) != 0) ;
	
	dcb.fInX = dcb.fOutX = bSet ;
	dcb.XonChar = ASCII_XON ;
	dcb.XoffChar = ASCII_XOFF ;
	dcb.XonLim = 100 ;
	dcb.XoffLim = 100 ;
	
	// other various settings
	
	dcb.fBinary = TRUE ;
	dcb.fParity = TRUE ;
	
	return SetCommState( m_idComDev, &dcb ) ;
}

long CSerialPort::ReadCommBlock(LPSTR lpszBlock, long nMaxLength )
{
//	CSingleLock localLock(&m_csComDev);
//	localLock.Lock();

	BOOL       fReadStat ;
	COMSTAT    ComStat ;
	DWORD      dwErrorFlags;
	DWORD      dwLength;
	DWORD      dwError;
	TCHAR       szError[ 10 ] ;
	
	// 큐에서 읽어야 할 데이타 크기를 가져온다
	ClearCommError( m_idComDev, &dwErrorFlags, &ComStat ) ;
	dwLength = min( (DWORD) nMaxLength, ComStat.cbInQue ) ;
	
	if (dwLength > 0)	// 읽어야 할 데이타가 있는 경우
	{
		// 데이타를 읽는다. Overlapped I/O임을 주의.
		fReadStat = ReadFile( m_idComDev, lpszBlock,
			dwLength, &dwLength, &m_osRead) ;

		if (!fReadStat)	// 읽어야 할 바이트를 다 읽지 못했다
		{
			if (GetLastError() == ERROR_IO_PENDING)	// I/O Pending에 의해 다 읽지 못한 경우
			{
				// I/O가 완료되기를 기다린다.
				while(!GetOverlappedResult( m_idComDev,
					&m_osRead, &dwLength, TRUE ))
				{
					dwError = GetLastError();
					if(dwError == ERROR_IO_INCOMPLETE)	// I/O가 아직 끝나지 않았다
						continue;
					else	// 에러 발생
					{
						wsprintf( szError, L"<CE-%u>\n\r", dwError ) ;
	//					printf(szError);
						// 에러를 클리어 하고 다른 I/O가 가능하도록 한다
						ClearCommError(m_idComDev, &dwErrorFlags, &ComStat ) ;
						break;
					}
					
				}
				
			}
			else // I/O Pending이 아닌 다른 에러가 발생한 경우
			{
				dwLength = 0 ;
				// 에러를 클리어 하고 다른 I/O가 가능하도록 한다
				ClearCommError( m_idComDev, &dwErrorFlags, &ComStat ) ;
			}
		}
		
	}
	
	return ( dwLength ) ;
	
}

BOOL CSerialPort::WriteCommBlock(LPSTR lpByte , DWORD dwBytesToWrite)
{
//	CSingleLock localLock(&m_csComDev);
//	localLock.Lock();

	BOOL        fWriteStat ;
	DWORD       dwBytesWritten ;
	DWORD       dwErrorFlags;
	DWORD   	dwError;
	DWORD       dwBytesSent=0;
	COMSTAT     ComStat;
	TCHAR        szError[ 128 ] ;
	
	fWriteStat = WriteFile( m_idComDev, lpByte, dwBytesToWrite,
		&dwBytesWritten, &m_osWrite) ;
	
	if (!fWriteStat)	// 써야할 바이트를 다 쓰지 못했다
	{
		if(GetLastError() == ERROR_IO_PENDING)	// I/O Pending에 의한 경우
		{
			// I/O가 완료되기를 기다린다
			while(!GetOverlappedResult( m_idComDev,
				&m_osWrite, &dwBytesWritten, TRUE ))
			{
				dwError = GetLastError();
				if(dwError == ERROR_IO_INCOMPLETE)
				{
					// 보낸 전체 바이트 수를 체크
					dwBytesSent += dwBytesWritten;
					continue;
				}
				else
				{
					// 에러 발생
					wsprintf( szError, L"<CE-%u>", dwError ) ;
					//printf("%s\r\n",szError);
					//WriteTTYBlock( hWnd, szError, lstrlen( szError ) ) ;
					ClearCommError( m_idComDev, &dwErrorFlags, &ComStat ) ;
					break;
				}
			}
			
			dwBytesSent += dwBytesWritten;
			
			if( dwBytesSent != dwBytesToWrite )	// 보내야 할 바이트와 보낸 바이트가 일치하지 않는 경우
				wsprintf(szError,L"\nProbable Write Timeout: Total of %ld bytes sent", dwBytesSent);
			else	// 성공적으로 모두 보낸 경우
				wsprintf(szError,L"\n%ld bytes written", dwBytesSent);
			
			OutputDebugString(szError);
		}
		else // I/O Pending 외의 다른 에러
		{
			ClearCommError(m_idComDev, &dwErrorFlags, &ComStat ) ;
			return FALSE;
		}
	}

	return TRUE;
}

BOOL CSerialPort::ClosePort(void)
{
	if (m_fConnected==FALSE) return TRUE;

	m_fConnected = FALSE ;

	// 이벤트 발생을 중지한다
	SetCommMask(m_idComDev, 0 ) ;
	
	// Event Watch 쓰레드가 중지되기를 기다린다
	DWORD64 dwStart = GetTickCount64();
	while (m_dwThreadID != 0)
	{
		::Sleep(10);
		if ((GetTickCount64() - dwStart) > 1000)
		{
			break;
		}
	};
	
	// DTR(Data-Terminal-Ready) 시그널을 Clear 한다
	EscapeCommFunction( m_idComDev, CLRDTR ) ;
	
	// 대기중인 모든 데이타를 지운다	
	PurgeComm( m_idComDev, PURGE_TXABORT | PURGE_RXABORT |
		PURGE_TXCLEAR | PURGE_RXCLEAR ) ;

	// 핸들을 반환한다
	CloseHandle(m_osRead.hEvent);
	CloseHandle(m_osWrite.hEvent);
	CloseHandle( m_idComDev) ;

	m_osRead.hEvent = NULL;
	m_osWrite.hEvent = NULL;
	m_idComDev = NULL;
	
	return TRUE;

}

BOOL CSerialPort::Connected(void)
{
	return m_fConnected;
}


// Event Watch 쓰레드
DWORD CSerialPort::CommWatchFunc()
{
	DWORD       dwEvtMask ;
	OVERLAPPED  os ;
	long        nLength ;
	BYTE       abIn[ MAXBLOCK + 1] ;
	
	memset( &os, 0, sizeof( OVERLAPPED ) ) ;
	
	// Event 객체 생성
	os.hEvent = CreateEvent(
		NULL,    
		TRUE,    
		FALSE,   
		NULL ) ; 
	
	if (os.hEvent == NULL)
		return FALSE;
	
	if (!SetCommMask( m_idComDev, EV_RXCHAR ))
	{
		CloseHandle(os.hEvent);
		return FALSE;
	}
	
	while ( m_fConnected )
	{
		dwEvtMask = 0 ;

		// Event가 발생할 때 까지 블럭
		WaitCommEvent( m_idComDev, &dwEvtMask, NULL );
		// MSDN 상의 설명과는 달리 WaitCommEvent의 마지막 매개 변수가 NULL이다
		// 설명에 의하면 FILE_FLAG_OVERLAPPED로 생성된 경우 반드시 NULL이 아닌 Overlapped
		// 구조체를 사용해야 한다고 되어있으나 몇몇 샘플을 검토한 결과 오히려 Code 87 에러가
		// 발생한다
		// 이런 이유로 os의 Event 객체는 별 의미가 없다
		// 또한 이런 이유로 멀티 쓰레드가 아닌 단일 쓰레드에서는 Overlapped I/O를 사용하는
		// 의미가 없을 것 같다 ( Polling 시 블럭되기 때문)

		if ((dwEvtMask & EV_RXCHAR) == EV_RXCHAR)
		{
			do
			{
				if (nLength = ReadCommBlock((LPSTR) abIn, MAXBLOCK ))
				{
					if (m_pISP2P) m_pISP2P->ISP2P_ReceiveData((LPSTR)abIn, nLength);
				}
			}
			while ( nLength > 0 ) ;
		}
		else
		{
			//printf("<Other Event>\r\n");
		}
	}
	
	CloseHandle( os.hEvent ) ;
	
	m_dwThreadID = 0 ;
	m_hWatchThread = NULL ;
	
	return TRUE;
}

DWORD CSerialPort::CommWatchProc( LPSTR lpData )
{
	CSerialPort *pp=(CSerialPort *)lpData;
	return pp->CommWatchFunc();
}
