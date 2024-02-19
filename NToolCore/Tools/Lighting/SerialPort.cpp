/*******************************************************************************
*                         �� �� �� �� �� �� �� Ʈ                              *
********************************************************************************
* �� �� �� : FlowerMJ                                                          *
*                                                                              *
* ���� �Ϸ��� : 060517                                                         *
*                                                                              *
*	��Ʈ�� Input �Ǵ� Character�� ȭ��� ���                                *
*	���� �ð����� Ư�� ������ ���ڿ��� ��Ʈ�� Output                           *
*	�� ��Ʈ�� �����Ͽ� �׽�Ʈ �Ҷ��� Baud_Rate�� ��ġ�ؾ߸� ��������           *
*	����� ���� �� �ִ�                                                        *
* --------------------------- �ֿ� �ڵ� ---------------------------------------*
* �ڵ��      �ǹ�                                                             *
* ----------- -----------------------------------------------------------------*
*                                                                              *
*                                                                              *
* --------------------------- ���� �̷� ---------------------------------------*
* �Ͻ�        ��������                                                         *
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
#define FC_DTRDSR       0x01   //������ �ܸ���(DTR) ���,������ ��Ʈ(DSR) ��⸦ ���� ��ȣ
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
	// ��Ʈ ���� ���� �������� �ʱ�ȭ �Ѵ�
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
	// ��Ʈ ���� ���� �������� �ʱ�ȭ �Ѵ�
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
	
	
	// Overlapped I/O�� ���̴� Event ��ü���� ����

	// Read�� ���� Event ��ü ����
	m_osRead.hEvent = CreateEvent(
		NULL,    
		TRUE,    
		FALSE,   
		NULL ) ;
	if (m_osRead.hEvent==NULL)
	{
		return FALSE ;
	}
	
	// Write�� ���� Event ��ü ����
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

	
	// ��Ʈ�� �����Ѵ�
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
	SetupComm( m_idComDev, 4096, 4096 ) ;	// ���� ����
	PurgeComm( m_idComDev, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR ) ;	// ������ ��� ����Ÿ�� �����
	

	// Overlapped I/O�� ���� Time Out ����
	COMMTIMEOUTS  CommTimeOuts ;
	CommTimeOuts.ReadIntervalTimeout = 0xFFFFFFFF ;
	CommTimeOuts.ReadTotalTimeoutMultiplier = 0 ;
	CommTimeOuts.ReadTotalTimeoutConstant = 1000 ;

	CommTimeOuts.WriteTotalTimeoutMultiplier = 2*CBR_9600/m_dwBaudRate ; // CBR_9600 ���� ms�� ����Ʈ�� �ι���� ����
	CommTimeOuts.WriteTotalTimeoutConstant = 0 ;
	SetCommTimeouts( m_idComDev, &CommTimeOuts ) ;
	
	// ��Ʈ�� ��밡�� ���·� ����� Event�� ������ �����带 �����Ѵ�
	if(SetupConnection()==TRUE)
	{
		m_fConnected = TRUE ;	
		
		// ������ ����
		m_hWatchThread = CreateThread(
			(LPSECURITY_ATTRIBUTES) NULL,
			0,
			(LPTHREAD_START_ROUTINE) CommWatchProc,
			(LPVOID)this,
			0,
			&m_dwThreadID );

		if(m_hWatchThread==NULL)	// ������ ���� ����
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
			// ��ġ�� DTR(Data-Terminal-Ready)�� �˸���
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
	// DCB ����ü�� �̿��Ͽ� ��Ʈ�� �����Ѵ�
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
	
	// ť���� �о�� �� ����Ÿ ũ�⸦ �����´�
	ClearCommError( m_idComDev, &dwErrorFlags, &ComStat ) ;
	dwLength = min( (DWORD) nMaxLength, ComStat.cbInQue ) ;
	
	if (dwLength > 0)	// �о�� �� ����Ÿ�� �ִ� ���
	{
		// ����Ÿ�� �д´�. Overlapped I/O���� ����.
		fReadStat = ReadFile( m_idComDev, lpszBlock,
			dwLength, &dwLength, &m_osRead) ;

		if (!fReadStat)	// �о�� �� ����Ʈ�� �� ���� ���ߴ�
		{
			if (GetLastError() == ERROR_IO_PENDING)	// I/O Pending�� ���� �� ���� ���� ���
			{
				// I/O�� �Ϸ�Ǳ⸦ ��ٸ���.
				while(!GetOverlappedResult( m_idComDev,
					&m_osRead, &dwLength, TRUE ))
				{
					dwError = GetLastError();
					if(dwError == ERROR_IO_INCOMPLETE)	// I/O�� ���� ������ �ʾҴ�
						continue;
					else	// ���� �߻�
					{
						wsprintf( szError, L"<CE-%u>\n\r", dwError ) ;
	//					printf(szError);
						// ������ Ŭ���� �ϰ� �ٸ� I/O�� �����ϵ��� �Ѵ�
						ClearCommError(m_idComDev, &dwErrorFlags, &ComStat ) ;
						break;
					}
					
				}
				
			}
			else // I/O Pending�� �ƴ� �ٸ� ������ �߻��� ���
			{
				dwLength = 0 ;
				// ������ Ŭ���� �ϰ� �ٸ� I/O�� �����ϵ��� �Ѵ�
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
	
	if (!fWriteStat)	// ����� ����Ʈ�� �� ���� ���ߴ�
	{
		if(GetLastError() == ERROR_IO_PENDING)	// I/O Pending�� ���� ���
		{
			// I/O�� �Ϸ�Ǳ⸦ ��ٸ���
			while(!GetOverlappedResult( m_idComDev,
				&m_osWrite, &dwBytesWritten, TRUE ))
			{
				dwError = GetLastError();
				if(dwError == ERROR_IO_INCOMPLETE)
				{
					// ���� ��ü ����Ʈ ���� üũ
					dwBytesSent += dwBytesWritten;
					continue;
				}
				else
				{
					// ���� �߻�
					wsprintf( szError, L"<CE-%u>", dwError ) ;
					//printf("%s\r\n",szError);
					//WriteTTYBlock( hWnd, szError, lstrlen( szError ) ) ;
					ClearCommError( m_idComDev, &dwErrorFlags, &ComStat ) ;
					break;
				}
			}
			
			dwBytesSent += dwBytesWritten;
			
			if( dwBytesSent != dwBytesToWrite )	// ������ �� ����Ʈ�� ���� ����Ʈ�� ��ġ���� �ʴ� ���
				wsprintf(szError,L"\nProbable Write Timeout: Total of %ld bytes sent", dwBytesSent);
			else	// ���������� ��� ���� ���
				wsprintf(szError,L"\n%ld bytes written", dwBytesSent);
			
			OutputDebugString(szError);
		}
		else // I/O Pending ���� �ٸ� ����
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

	// �̺�Ʈ �߻��� �����Ѵ�
	SetCommMask(m_idComDev, 0 ) ;
	
	// Event Watch �����尡 �����Ǳ⸦ ��ٸ���
	DWORD64 dwStart = GetTickCount64();
	while (m_dwThreadID != 0)
	{
		::Sleep(10);
		if ((GetTickCount64() - dwStart) > 1000)
		{
			break;
		}
	};
	
	// DTR(Data-Terminal-Ready) �ñ׳��� Clear �Ѵ�
	EscapeCommFunction( m_idComDev, CLRDTR ) ;
	
	// ������� ��� ����Ÿ�� �����	
	PurgeComm( m_idComDev, PURGE_TXABORT | PURGE_RXABORT |
		PURGE_TXCLEAR | PURGE_RXCLEAR ) ;

	// �ڵ��� ��ȯ�Ѵ�
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


// Event Watch ������
DWORD CSerialPort::CommWatchFunc()
{
	DWORD       dwEvtMask ;
	OVERLAPPED  os ;
	long        nLength ;
	BYTE       abIn[ MAXBLOCK + 1] ;
	
	memset( &os, 0, sizeof( OVERLAPPED ) ) ;
	
	// Event ��ü ����
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

		// Event�� �߻��� �� ���� ��
		WaitCommEvent( m_idComDev, &dwEvtMask, NULL );
		// MSDN ���� ������� �޸� WaitCommEvent�� ������ �Ű� ������ NULL�̴�
		// ���� ���ϸ� FILE_FLAG_OVERLAPPED�� ������ ��� �ݵ�� NULL�� �ƴ� Overlapped
		// ����ü�� ����ؾ� �Ѵٰ� �Ǿ������� ��� ������ ������ ��� ������ Code 87 ������
		// �߻��Ѵ�
		// �̷� ������ os�� Event ��ü�� �� �ǹ̰� ����
		// ���� �̷� ������ ��Ƽ �����尡 �ƴ� ���� �����忡���� Overlapped I/O�� ����ϴ�
		// �ǹ̰� ���� �� ���� ( Polling �� ���Ǳ� ����)

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
