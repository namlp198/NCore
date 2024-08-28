#include "pch.h"
#include "TCPSocket.h"
#include <process.h>
#include <stdio.h>

CTCPSocket::CTCPSocket(void)
{
	m_ClinetSocket = NULL;
	m_isConnect = false;
	m_port = 9000;
	
	m_nRecvBufferSize = 2048;


	m_nThreadFlag = 0;

	m_recvmsgFunc = NULL;
	m_recvmsgData = NULL;
	m_systemmsgFunc = NULL;
	m_systemmsgData = NULL;

	m_nInstanceHandle = 0;

	m_bConnectRetry = false;
}

CTCPSocket::~CTCPSocket(void)
{
	m_bConnectRetry = false;
	DisConnect();

	WSACleanup();
}


// 접속 될때까지 접속 시도
void CTCPSocket::ConnectContinue(char* serverip, int port)
{
	m_bConnectRetry = true;
	
	if(m_isConnect == true) return;

	strcpy_s(m_servIP, 15, serverip);
	m_port = port;
	

	if(Connect(serverip, port) != 1)
	{
		unsigned dwThreadID;
		m_hThreadConnect = (HANDLE)_beginthreadex(NULL, 0 , threadConnect , (void*)this ,0,&dwThreadID);
	}
}

unsigned int _stdcall  CTCPSocket::threadConnect(void* arg)
{
	CTCPSocket* pthis = (CTCPSocket*) arg;
	

	int nRet;
	int nTryCount = 0;
	while(nRet = pthis->Connect(pthis->m_servIP, pthis->m_port) != 1 && pthis->m_bConnectRetry == true)
	{
		nTryCount++;

		pthis->setSystemMsg("Try to connect Server(%d) ... ", nTryCount);
		
		Sleep(2000);
	}

	CloseHandle(pthis->m_hThreadConnect);
	return 0;
}

CTCPSocket::CONNECTRESULT CTCPSocket::Connect(char* serverip, int port)
{
	
	WSADATA wsaData;
	if(WSAStartup(MAKEWORD(2, 2), &wsaData)!=0)// 동기화
	{
		setSystemMsg("Connect Sync Fail");
		return CONNECTRESULT_WSASTARTUP_FAIL;
	}

	
	strcpy(m_servIP, serverip);
	m_port = port;



	//소켓 생성
	if(m_ClinetSocket == NULL)
	{
		m_ClinetSocket = socket(PF_INET, SOCK_STREAM, 0);
	}
	else
	{
		WSACleanup();
		setSystemMsg("Already Connected");
		return CONNECTRESULT_ALREADY_CREATESOCKET; //이미 생성된 소켓
	}

	if(m_ClinetSocket == INVALID_SOCKET)
	{
		WSACleanup();
		m_ClinetSocket = NULL;
		setSystemMsg("Create Socket Faile");
		return CONNECTRESULT_INVALID_SOCKET;  // 소켓 생성 실패
	}

	//서버주소 초기화
	SOCKADDR_IN  m_servAddr;
	memset(&m_servAddr, 0, sizeof(m_servAddr));
	m_servAddr.sin_family = AF_INET;
	//m_servAddr.sin_addr.s_addr = inet_addr("192.168.100.18");
	m_servAddr.sin_addr.s_addr = inet_addr(m_servIP);
	m_servAddr.sin_port = htons(m_port);

	if(connect(m_ClinetSocket, (SOCKADDR*)&m_servAddr, sizeof(m_servAddr)) == SOCKET_ERROR)
	{
		WSACleanup();
		m_ClinetSocket = NULL;
		setSystemMsg("Connect Server Fail");
		return CONNECTRESULT_CONNECT_FAIL; //연결 실패
		//AddStringToList(g_hwndList, "connect Error");
	}
	else
	{
		//AddStringToList(g_hwndList, "서버와 연결되었습니다.");
	}

	m_isConnect = true;

	m_hThread = (HANDLE)_beginthreadex(NULL, 0 , CTCPSocket::RecvMSG , (void*)this ,0,&m_dwThreadID);


	setSystemMsg("Complete Connect Server");
	

	return CONNECTRESULT_CONNECT_SUCCESS;
}

// 종료 
void CTCPSocket::DisConnect()                          
{
	if(m_bConnectRetry == true)
	{
		m_bConnectRetry = false;
		WaitForSingleObject(m_hThreadConnect, 2000);
	}

	if(m_isConnect == true)
	{
		m_isConnect = false;

		closesocket(m_ClinetSocket);
		m_ClinetSocket = NULL;
		

		WaitForSingleObject(m_hThread, 1000);
		CloseHandle(m_hThread);
// 		int timeoutcount=300;
// 		while(m_nThreadFlag != 2 && timeoutcount-- > 0)
// 		{
// 			Sleep(1);
// 		}
		m_nThreadFlag = 0;


		setSystemMsg("Exit Connect Client..");
	}

}


void CTCPSocket::SetRecvMessageCallback(RecvMsgFunc userFunc, void* data)
{
	m_recvmsgFunc = userFunc;
	m_recvmsgData = data;
}
// 시스템 메세지 수신 콜백 설정
void CTCPSocket::SetSystemMessageCallback(RecvMsgFunc userFunc, void* data)
{
	m_systemmsgFunc = userFunc;
	m_systemmsgData = data;
}



// 메시지 전송 

bool CTCPSocket::Send(char* sendchar, int length)
{
	if(!m_isConnect) return false;
																		   // TRUE 이면 Send 쓰레드에서 이 문장을 전송한다.

	if(send(m_ClinetSocket, sendchar, length, 0) == -1)
	{
		DisConnect();
		return false;
	}
	
	return true;
}




unsigned int CTCPSocket::RecvMSG(void *arg)
{
	CTCPSocket* pthis = (CTCPSocket*)arg;

	const int bufsize = pthis->m_nRecvBufferSize;
	char* recvmsg = new char[bufsize];
	
	pthis->m_nThreadFlag = 1;
	
	memset(recvmsg, 0, sizeof(char)*bufsize);
	int nHeadSize = 0;
	while(pthis->m_isConnect)
	{
		int nRecv;
		nRecv = recv(pthis->m_ClinetSocket, &recvmsg[nHeadSize], bufsize-nHeadSize, 0); //젤앞에 클라이언트구문자 만들기 위함!!
		//nRecv = recv(pthis->m_ClinetSocket, &recvmsg, bufsize, 0);
		
		
		
		if(nRecv > 0)
		{
			
			if(pthis->m_recvmsgFunc)
			{
				memcpy(recvmsg, &pthis->m_nInstanceHandle, nHeadSize);
				recvmsg[nRecv] = 0;
				pthis->m_recvmsgFunc(recvmsg, nRecv+nHeadSize, pthis->m_recvmsgData);
			}
			
		}
		else if(nRecv == 0 || nRecv == -1 || WSAGetLastError() == WSAENOTCONN)  //서버와 연결이 끊어짐
		{
			break;
		}

	}

	delete [] recvmsg;
	
	
	
	closesocket(pthis->m_ClinetSocket);
	pthis->m_ClinetSocket = NULL;
	WSACleanup();

	if(pthis->m_isConnect == true)
	{
		pthis->setSystemMsg("Close Server");
		pthis->m_isConnect = false;
	}

	if(pthis->m_bConnectRetry)
	{
		pthis->ConnectContinue(pthis->m_servIP, pthis->m_port);
	}

	pthis->m_nThreadFlag = 2;

	//_endthreadex(pthis->m_hThread);
	return 0;
}

void CTCPSocket::setSystemMsg( const char* format, ... )
{
	if(m_systemmsgFunc)
	{
		static char buffer[512];

		va_list vl;

		va_start(vl, format);

		int nLen = vsnprintf(buffer, 512, format, vl);

		va_end(vl);

		m_systemmsgFunc(buffer, nLen, m_systemmsgData);
	}
	
}
