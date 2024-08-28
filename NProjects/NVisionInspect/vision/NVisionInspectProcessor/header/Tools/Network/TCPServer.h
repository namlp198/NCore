
#pragma once

#include <vector>
#define SERVERCLOSEMSG "SERVERCLOSE"
#define CLIENTDISCONNECT -10001
#define SERVERDISCONNECT -10002
#define SERVERMESSAGE (WM_USER + 10)

struct _WorkThreadArg 
{
	_WorkThreadArg()
	{
		pthis = NULL;
		clntSock;
		clntAddr;
		clntAddrSize = 0;
		hThread = NULL;
		dwThreadID = NULL;

		clntAddrSize=sizeof(clntAddr);
	}

	void* pthis;
	SOCKET clntSock;
	SOCKADDR_IN clntAddr;
	int clntAddrSize;
	HANDLE hThread;
	unsigned int dwThreadID;
};

class AFX_EXT_CLASS CTCPServer
{
public:
	typedef void (*RecvMsgFunc) (char* pMsg, int nMsglen, void*);
	typedef void (*SystemMsgFunc) (char* pMsg, int nMsglen, void*);
public:
	CTCPServer(void);
	~CTCPServer(void);

	// ��������
	BOOL CreateServer(unsigned int port);
	// ��������
	void CloseServer();
	// ������ ���� ������ �� ���
	char* GetServerIP();
	// �޼��� �۽�
	bool Send(char* sendMsg, int length);
	// ���� �޼��� ���� �ݹ� ����
	void SetRecvMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// �ý��� �޼��� ���� �ݹ� ����
	void SetSystemMessageCallback(SystemMsgFunc userFunc, void* data = NULL);
	// ���� ���� ����
	inline bool GetServerState(){return m_isServerCreate;};
	
	bool m_isServerCreate;

	unsigned int m_nRecvBufferSize;

protected:
	void setSystemMsg(const char* format, ...);
private:
	HANDLE m_hMutex; //���ؽ� ���� ����

	SOCKET m_servSock;
	SOCKADDR_IN m_servAddr;
	
	HANDLE m_hThread;
	unsigned int m_dwThreadID;
	
	void EchoMsg(char* message, int len); //���� ������ ��ο��� �޼��� ����
	void EchoMsgNotMe(SOCKET mySocket, char* message, int len); //���� ������ ��ο��� �޼��� ����

	void RemoveSocketList(SOCKET clntSock, SOCKADDR_IN clntAddr);

	static unsigned int __stdcall ThreadMain(void *arg); //���� ���� Ŭ���̾�Ʈ ���� ������
	static unsigned int __stdcall ClientWorkFunc(void *arg); //Ŭ���̾�Ʈ �޼��� �޴� ������
	

	

	//void (*pCallBackRecvMsg)(char*,int,void*);					 // �ݹ� �Լ�
	RecvMsgFunc m_recvmsgFunc;
	void* m_recvmsgData;
	SystemMsgFunc m_systemmsgFunc;
	void* m_systemmsgData;

private:
	std::vector<SOCKET> m_socketList; //Ŭ���̾�Ʈ ����Ʈ ����

	
};

//////////////////////////////////////////////////////////////////////////
/*

1. KimTCPIP.lib, KimTCPIP.dll, KimServer.h ���� ����

2. ������Ʈ�� KimTCPIP.lib �߰�

3. �ν��Ͻ� ����
	- CSISTCPServer m_kimserver;

4. ���� �ʱ�ȭ �� ���� ����
	- InitServer(HWND hwnd);

5. �����κ��� �޼���(ex)
		LRESULT CControlServerAppDlg::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
		{
			// TODO: ���⿡ Ư��ȭ�� �ڵ带 �߰� ��/�Ǵ� �⺻ Ŭ������ ȣ���մϴ�.
			
			switch(message)
			{
			case SERVERMESSAGE:
				{
					switch(wParam)
					{
					case 0: //���� ����
						{
							m_ctrlMsglist.AddString("���� ����");
							m_ctrlMsglist.SetTopIndex(m_ctrlMsglist.GetCount()-1);
							break;
						}
					case 1: //Ŭ���̾�Ʈ ����
						{
							CString msg;
							msg.Format("%s ����� ����",(char*)lParam);
							m_ctrlMsglist.AddString(msg);
							m_ctrlMsglist.SetTopIndex(m_ctrlMsglist.GetCount()-1);
							break;
						}
					case 2: //Ŭ���̾�Ʈ ����
						{
							CString msg;
							msg.Format("%s ����� ����",(char*)lParam);
							m_ctrlMsglist.AddString(msg);
							m_ctrlMsglist.SetTopIndex(m_ctrlMsglist.GetCount()-1);
							break;
						}
						
					}
					

					break;
				}
			}
			return CDialog::WindowProc(message, wParam, lParam);
		}
6. ���� ����
	- OnClose(); 
*/
