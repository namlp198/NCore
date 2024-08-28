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
		setSystemMsg("�����ȼ������� (port:%d)",ntohs(m_servAddr.sin_port));
		return FALSE;
	}
	WSADATA wsaData;
	//���ӻ��
	if(WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
	{
		setSystemMsg("WSAStartup error!");
		return 0;
	}

	//���ؽ� ���� �� �ʱ�ȭ
	m_hMutex=CreateMutex(NULL, FALSE, NULL);

	if(m_hMutex == NULL)
	{
		setSystemMsg("CreateMutex error!");
		return 0;
	}

	//�������� ����
	m_servSock=socket(PF_INET, SOCK_STREAM, 0);   
	if(m_servSock == INVALID_SOCKET)
	{
		setSystemMsg("socket error");
		return 0;
	}

	memset(&m_servAddr, 0, sizeof(m_servAddr));
	m_servAddr.sin_family=AF_INET;
	m_servAddr.sin_addr.s_addr=htonl(INADDR_ANY);
	m_servAddr.sin_port=htons(port); //���� ��Ʈ�� 9999

	//�������� ���ε�
	if(bind(m_servSock, (SOCKADDR*) &m_servAddr, sizeof(m_servAddr))==SOCKET_ERROR)
	{
		setSystemMsg("bind error");
		return 0;
	}

	//������
	if(listen(m_servSock, 5)==SOCKET_ERROR)
	{
		setSystemMsg("listen error");
		return 0;
	}


	//SendMessage(m_pMainhwnd,SERVERMESSAGE,0,0);

	m_isServerCreate = TRUE;
	m_hThread = (HANDLE)_beginthreadex( NULL, 0, CTCPServer::ThreadMain, this, 0, &m_dwThreadID);


	setSystemMsg("�������� (port:%d)", port);
	

	return TRUE;
}


unsigned int __stdcall CTCPServer::ThreadMain(void *arg)
{
	CTCPServer* pthis = (CTCPServer*)arg;

	
	while(pthis->m_isServerCreate)
	{
		_WorkThreadArg arg;
		arg.pthis = pthis;
			
		//Ŭ���̾�Ʈ�� ������ ���⸦ ��ٸ���.
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

		
		//Ŭ���̾�Ʈ�� ���������� �پ��ٸ� ��ŷ������ ����
		arg.hThread = (HANDLE)_beginthreadex(NULL, 0, CTCPServer::ClientWorkFunc, &arg, 0, &arg.dwThreadID);
		if(arg.hThread == 0)
		{
			pthis->setSystemMsg("������ ���� ����");
			closesocket(arg.clntSock);
			continue;
		}

		//WaitForSingleObject(pthis->m_hMutex, INFINITE); //���ؽ��� ���������� ��װ�
		pthis->m_socketList.push_back(arg.clntSock); //Ŭ���̾�Ʈ ������ ����Ʈ�� �ְ�...
		//ReleaseMutex(pthis->m_hMutex); //�ٻ�� ������ ���ؽ��� Ǭ��.
 
		
		//setSystemMsg("CLIENTCONNECT : %d", arg.clntSock);
		
		pthis->setSystemMsg("���ο��, Ŭ���̾�Ʈ IP : %s(%d)", inet_ntoa(arg.clntAddr.sin_addr), arg.clntSock);
		
		//�ٽ� ���� �ö󰡼� Ŭ���̾�Ʈ�� ������ ��ٸ���.
	}

	//���α׷� ����� ����ϰ� ���ҽ� ��ȯ
	WSACleanup();
	return 0;
}


//��ŷ�����忡�� ����� �Լ�
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
	
	//�޼����� ���⸦ ��ٸ���.
	while( (strLen=recv(clntSock, message, pthis->m_nRecvBufferSize, 0)) != 0)
	{
		if(strLen == -1)
		{
			break;
		}
		//�޼����� �޾Ҵٸ� �ٽ� ����
		pthis->EchoMsgNotMe(WorkThreadArg->clntSock, message, strLen);

		memset(message, 0 , sizeof(char)*pthis->m_nRecvBufferSize);
	}

	if(pthis->m_isServerCreate == true)
	{
		pthis->setSystemMsg("Ŭ���̾�Ʈ��������, IP:%s(%d)", inet_ntoa(WorkThreadArg->clntAddr.sin_addr), WorkThreadArg->clntSock);
	}

	


	//���� ������� ����Ǿ� �۾��ϴ� Ŭ���̾�Ʈ�� �����ϸ� ������ ���� ã�Ƽ� ����
	pthis->RemoveSocketList(clntSock, clntAddr);

	//Ŭ���̾�Ʈ�� ����� ������ �ݴ´�.
	closesocket(clntSock);

	//pthis->pCallBackRecvMsg(packet, 1, pthis->m_pParent);

	delete [] message;
	
	return 0;
}

//���۵Ǵ� ��� �޼����� ����� Ŭ���̾�Ʈ���� ����
void CTCPServer::EchoMsg(char* message, int len)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //���ؽ��� ���������� ��װ�
	if(m_recvmsgFunc)
	{
		m_recvmsgFunc(message, 1, m_recvmsgData);
	}
	for(itor = m_socketList.begin(); itor != m_socketList.end(); itor++)
	{
		SOCKET client = *itor;
		send(client, message, len, 0);
	}
	ReleaseMutex(m_hMutex); //�ٻ�� ������ ���ؽ��� Ǭ��.
}

void CTCPServer::EchoMsgNotMe(SOCKET mySocket, char* message, int len)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //���ؽ��� ���������� ��װ�
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
	ReleaseMutex(m_hMutex); //�ٻ�� ������ ���ؽ��� Ǭ��.
}

//������ Ŭ���̾�Ʈ�� ����ϰ� ����
void CTCPServer::RemoveSocketList(SOCKET clntSock, SOCKADDR_IN clntAddr)
{
	std::vector<SOCKET>::iterator itor;

	WaitForSingleObject(m_hMutex, INFINITE); //���ؽ��� ���������� ��װ�
	for(itor = m_socketList.begin(); itor != m_socketList.end(); itor++)
	{
		SOCKET client = *itor;

		if(client == clntSock)
		{
			m_socketList.erase(itor);
			
			break;
		}
	}
	ReleaseMutex(m_hMutex); //�ٻ�� ������ ���ؽ��� Ǭ��.

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
		
		setSystemMsg("��������");
		WSACleanup(); 
	}
}

void CTCPServer::SetRecvMessageCallback(RecvMsgFunc userFunc, void* data)
{
	m_recvmsgFunc = userFunc;
	m_recvmsgData = data;
}
// �ý��� �޼��� ���� �ݹ� ����
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
