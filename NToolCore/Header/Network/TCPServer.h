
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

	// 서버생성
	BOOL CreateServer(unsigned int port);
	// 서버종료
	void CloseServer();
	// 생성된 서버 아이피 값 얻기
	char* GetServerIP();
	// 메세지 송신
	bool Send(char* sendMsg, int length);
	// 소켓 메세지 수신 콜백 설정
	void SetRecvMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// 시스템 메세지 수신 콜백 설정
	void SetSystemMessageCallback(SystemMsgFunc userFunc, void* data = NULL);
	// 서버 생성 상태
	inline bool GetServerState(){return m_isServerCreate;};
	
	bool m_isServerCreate;

	unsigned int m_nRecvBufferSize;

protected:
	void setSystemMsg(const char* format, ...);
private:
	HANDLE m_hMutex; //뮤텍스 변수 선언

	SOCKET m_servSock;
	SOCKADDR_IN m_servAddr;
	
	HANDLE m_hThread;
	unsigned int m_dwThreadID;
	
	void EchoMsg(char* message, int len); //나를 포함한 모두에게 메세지 전송
	void EchoMsgNotMe(SOCKET mySocket, char* message, int len); //나를 제외한 모두에게 메세지 전송

	void RemoveSocketList(SOCKET clntSock, SOCKADDR_IN clntAddr);

	static unsigned int __stdcall ThreadMain(void *arg); //메인 서버 클라이언트 대기용 쓰레드
	static unsigned int __stdcall ClientWorkFunc(void *arg); //클라이언트 메세지 받는 쓰레드
	

	

	//void (*pCallBackRecvMsg)(char*,int,void*);					 // 콜백 함수
	RecvMsgFunc m_recvmsgFunc;
	void* m_recvmsgData;
	SystemMsgFunc m_systemmsgFunc;
	void* m_systemmsgData;

private:
	std::vector<SOCKET> m_socketList; //클라이언트 리스트 관리

	
};

//////////////////////////////////////////////////////////////////////////
/*

1. KimTCPIP.lib, KimTCPIP.dll, KimServer.h 파일 복사

2. 프로젝트에 KimTCPIP.lib 추가

3. 인스턴스 생성
	- CSISTCPServer m_kimserver;

4. 서버 초기화 및 서버 생성
	- InitServer(HWND hwnd);

5. 서버로부터 메세지(ex)
		LRESULT CControlServerAppDlg::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
		{
			// TODO: 여기에 특수화된 코드를 추가 및/또는 기본 클래스를 호출합니다.
			
			switch(message)
			{
			case SERVERMESSAGE:
				{
					switch(wParam)
					{
					case 0: //서버 생성
						{
							m_ctrlMsglist.AddString("서버 생성");
							m_ctrlMsglist.SetTopIndex(m_ctrlMsglist.GetCount()-1);
							break;
						}
					case 1: //클라이언트 접속
						{
							CString msg;
							msg.Format("%s 사용자 접속",(char*)lParam);
							m_ctrlMsglist.AddString(msg);
							m_ctrlMsglist.SetTopIndex(m_ctrlMsglist.GetCount()-1);
							break;
						}
					case 2: //클라이언트 종료
						{
							CString msg;
							msg.Format("%s 사용자 종료",(char*)lParam);
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
6. 서버 종료
	- OnClose(); 
*/
