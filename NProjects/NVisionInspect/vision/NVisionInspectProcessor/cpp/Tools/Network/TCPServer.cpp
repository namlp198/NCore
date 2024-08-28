#include "pch.h"
#include "TCPServer.h"
#include <process.h>

CTCPServer::CTCPServer(void)
{
	m_isServerCreate = false;
	m_recvmsgFunc = NULL;
	m_recvmsgData = NULL;
	m_systemmsgFunc = NULL;
	m_systemmsgData = NULL;

	m_nRecvBufferSize = 2048;

	m_servSock = NULL;
}

CTCPServer::~CTCPServer(void)
{
	CloseServer();
	
}

char* CTCPServer::GetServerIP()
{
	return inet_ntoa(m_servAddr.sin_addr);
}

BOOL CTCPServer::CreateServer(unsigned int port)
{
	if(m_isServerCreate)
	{
		setSystemMsg("생성된서버존재 (port:%d)",ntohs(m_servAddr.sin_port));
		return FALSE;
	}
	WSADATA wsaData;
	//윈속사용
	if(WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		setSystemMsg("WSAStartup error!");
		return 0;
	}

	//뮤텍스 생성 및 초기화
	m_hMutex=CreateMutex(NULL, FALSE, NULL);

	if(m_hMutex == NULL)
	{
		setSystemMsg("CreateMutex error!");
		return 0;
	}

	//서버소켓 생성
	m_servSock=socket(PF_INET, SOCK_STREAM, 0);   
	if(m_servSock == INVALID_SOCKET)
	{
		setSystemMsg("socket error");
		return 0;
	}

	memset(&m_servAddr, 0, sizeof(m_servAddr));
	m_servAddr.sin_family=AF_INET;
	m_servAddr.sin_addr.s_addr=htonl(INADDR_ANY);
	m_servAddr.sin_port=htons(port); //서버 포트는 9999

	//서버소켓 바인드
	if(bind(m_servSock, (SOCKADDR*) &m_servAddr, sizeof(m_servAddr))==SOCKET_ERROR)
	{
		setSystemMsg("bind error");
		return 0;
	}

	//리스닝
	if(listen(m_servSock, 5)==SOCKET_ERROR)
	{
		setSystemMsg("listen error");
		return 0;
	}


	//SendMessage(m_pMainhwnd,SERVERMESSAGE,0,0);

	m_isServerCreate = TRUE;
	m_hThread = (HANDLE)_beginthreadex( NULL, 0, CTCPServer::ThreadMain, this, 0, &m_dwThreadID);


	setSystemMsg("서버생성 (port:%d)", port);
	

	return TRUE;
}


unsigned int __stdcall CTCPServer::ThreadMain(void *arg)
{
	CTCPServer* pthis = (CTCPServer*)arg;

	
	while(pthis->m_isServerCreate)
	{
		_WorkThreadArg arg;
		arg.pthis = pthis;
			
		//클라이언트의 연결이 오기를 기다린다.
		arg.clntSock=accept(pthis->m_servSock, (SOCKADDR*)&arg.clntAddr, &arg.clntAddrSize);

		if(pthis->m_isServerCreate == FALSE)
		{
			break;
		}
		if(arg.clntSock==INVALID_SOCKET)
		{
			printf("accept() error");
			continue;
		}

		
		//클라이언트가 정상적으로 붙었다면 워킹스레드 생성
		arg.hThread = (HANDLE)_beginthreadex(NULL, 0, CTCPServer::ClientWorkFunc, &arg, 0, &arg.dwThreadID);
		if(arg.hThread == 0)
		{
			pthis->setSystemMsg("쓰레드 생성 오류");
			closesocket(arg.clntSock);
			continue;
		}

		//WaitForSingleObject(pthis->m_hMutex, INFINITE); //뮤텍스로 공유영역을 잠그고
		pthis->m_socketList.push_back(arg.clntSock); //클라이언트 소켓을 리스트에 넣고...
		//ReleaseMutex(pthis->m_hMutex); //다사용 했으면 뮤텍스를 푼다.
 
		
		//setSystemMsg("CLIENTCONNECT : %d", arg.clntSock);
		
		pthis->setSystemMsg("새로운연결, 클라이언트 IP : %s(%d)", inet_ntoa(arg.clntAddr.sin_addr), arg.clntSock);
		
		//다시 위로 올라가서 클라이언트의 접속을 기다린다.
	}

	//프로그램 종료시 깔끔하게 리소스 반환
	WSACleanup();
	return 0;
}


//워킹쓰레드에서 사용할 함수
unsigned int __stdcall CTCPServer::ClientWorkFunc(void *arg)
{
	_WorkThreadArg* WorkThreadArg = (_WorkThreadArg*)arg;

	CTCPServer* pthis = (CTCPServer*)WorkThreadArg->pthis;

	SOCKET clntSock = WorkThreadArg->clntSock;
	SOCKADDR_IN clntAddr = WorkThreadArg->clntAddr;

	int strLen=0;
	//char message[BUFSIZE] = {0};
	char* message = new char[pthis->m_nRecvBufferSize];
	
	memset(message, 0, sizeof(char)*pthis->m_nRecvBufferSize);
	
	//메세지가 오기를 기다린다.
	while( (strLen=recv(clntSock, message, pthis->m_nRecvBufferSize, 0)) != 0)
	{
		if(strLen == -1)
		{
			break;
		}
		//메세지를 받았다면 다시 전달
		pthis->EchoMsgNotMe(WorkThreadArg->clntSock, message, strLen);

		memset(message, 0 , sizeof(char)*pthis->m_nRecvBufferSize);
	}

	if(pthis->m_isServerCreate == true)
	{
		pthis->setSystemMsg("클라이언트연결종료, IP:%s(%d)", inet_ntoa(WorkThreadArg->clntAddr.sin_addr), WorkThreadArg->clntSock);
	}

	


	//현재 쓰레드와 연결되어 작업하던 클라이언트가 종료하면 종료한 놈을 찾아서 삭제
	pthis->RemoveSocketList(clntSock, clntAddr);

	//클라이언트와 연결된 소켓을 닫는다.
	closesocket(clntSock);

	//pthis->pCallBackRecvMsg(packet, 1, pthis->m_pParent);

	delete [] message;
	
	return 0;
}

//전송되는 모든 메세지를 연결된 클라이언트에게 전송
void CTCPServer::EchoMsg(char* message, int len)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //뮤텍스로 공유영역을 잠그고
	if(m_recvmsgFunc)
	{
		m_recvmsgFunc(message, 1, m_recvmsgData);
	}
	for(itor = m_socketList.begin(); itor != m_socketList.end(); itor++)
	{
		SOCKET client = *itor;
		send(client, message, len, 0);
	}
	ReleaseMutex(m_hMutex); //다사용 했으면 뮤텍스를 푼다.
}

void CTCPServer::EchoMsgNotMe(SOCKET mySocket, char* message, int len)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //뮤텍스로 공유영역을 잠그고
	if(m_recvmsgFunc && mySocket != m_servSock)
	{		
		m_recvmsgFunc(message, len, m_recvmsgData);
	}
	for(itor = m_socketList.begin(); itor != m_socketList.end(); itor++)
	{
		SOCKET client = *itor;
		if(client == mySocket) continue;
		send(client, message, len, 0);
	}
	ReleaseMutex(m_hMutex); //다사용 했으면 뮤텍스를 푼다.
}

//끊어진 클라이언트는 깔끔하게 제거
void CTCPServer::RemoveSocketList(SOCKET clntSock, SOCKADDR_IN clntAddr)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //뮤텍스로 공유영역을 잠그고
	for(itor = m_socketList.begin(); itor != m_socketList.end(); itor++)
	{
		SOCKET client = *itor;

		if(client == clntSock)
		{
			m_socketList.erase(itor);
			
			break;
		}
	}
	ReleaseMutex(m_hMutex); //다사용 했으면 뮤텍스를 푼다.

	//SendMessage(m_pMainhwnd,SERVERMESSAGE,2,(LPARAM)inet_ntoa(clntAddr.sin_addr));
}

void CTCPServer::CloseServer()
{
	if(m_isServerCreate == TRUE)
	{
		m_isServerCreate = FALSE;

			
		closesocket(m_servSock);
		
		WaitForSingleObject(m_hThread, 500);
		
		CloseHandle(m_hThread);
		
		setSystemMsg("서버종료");
		WSACleanup(); 
	}
}

void CTCPServer::SetRecvMessageCallback(RecvMsgFunc userFunc, void* data)
{
	m_recvmsgFunc = userFunc;
	m_recvmsgData = data;
}
// 시스템 메세지 수신 콜백 설정
void CTCPServer::SetSystemMessageCallback(SystemMsgFunc userFunc, void* data)
{
	m_systemmsgFunc = userFunc;
	m_systemmsgData = data;
}

bool CTCPServer::Send(char* sendMsg, int length)
{
	if(m_isServerCreate == false) return false;

	EchoMsgNotMe(m_servSock, sendMsg, length);

	return true;
}

void CTCPServer::setSystemMsg( const char* format, ... )
{
	if(m_systemmsgFunc)
	{
		static char buffer[256];

		va_list vl;

		va_start(vl, format);

		int nLen = vsnprintf_s(buffer, 256, 256, format, vl);

		va_end(vl);

		m_systemmsgFunc(buffer, nLen, m_systemmsgData);
	}

}
