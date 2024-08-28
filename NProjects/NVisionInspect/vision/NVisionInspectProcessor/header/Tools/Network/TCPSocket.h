#pragma once

class AFX_EXT_CLASS CTCPSocket
{
public:
	typedef void (*RecvMsgFunc) (char* pMsg, int nMsglen, void*);
	typedef void (*SystemMsgFunc) (char* pMsg, int nMsglen, void*);

public:
	 CTCPSocket(void);
	 ~CTCPSocket(void);

	enum CONNECTRESULT
	{
		CONNECTRESULT_CONNECT_FAIL = 0, //연결실패
		CONNECTRESULT_CONNECT_SUCCESS = 1, //연결성공
		CONNECTRESULT_ALREADY_CREATESOCKET = 2, //이미생성된 소켓
		CONNECTRESULT_INVALID_SOCKET = 3, //소켓생성 실패
		CONNECTRESULT_WSASTARTUP_FAIL = 5 //StartUP실패
		
	};
	// 접속
	CONNECTRESULT Connect(char* serverip, int port);
	// 접속 될때까지 접속 시도
	void ConnectContinue(char* serverip, int port);
	// 종료
	void DisConnect();  
	// 소켓 메세지 수신 콜백 설정
	void SetRecvMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// 시스템 메세지 수신 콜백 설정
	void SetSystemMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// 메세지 전송
	bool Send(char* sendchar, int length);      
	// 접속 상태
	inline BOOL GetConnectState(){return (m_isConnect == true) ? TRUE : FALSE;};	
	
	SOCKET getSocket(){return m_ClinetSocket;};

	static unsigned int _stdcall RecvMSG(void *arg);      // 받는 쓰레드

	bool m_bConnectRetry; //접속 될때까지 접속 시도
	HANDLE m_hThreadConnect;
	unsigned int m_nRecvBufferSize;


	int getInstanceHandle() const { return m_nInstanceHandle; }
	void setInstanceHandle(int val) { m_nInstanceHandle = val; }

protected:
	void setSystemMsg(const char* pMsg, ...);
private:
	static unsigned int _stdcall threadConnect(void* arg);
private:
	int m_nThreadFlag;
	
	SOCKET m_ClinetSocket;		//클라이언트 소켓

	bool m_isConnect;			//접속 상태
	
	int m_port;					//접속 포트
	char m_servIP[15];			//접속 서버
	

	HANDLE m_hThread;       //쓰레드 핸들    
	unsigned int m_dwThreadID;  //쓰레드 아이디


	RecvMsgFunc m_recvmsgFunc;
	void* m_recvmsgData;
	SystemMsgFunc m_systemmsgFunc;
	void* m_systemmsgData;

	int m_nInstanceHandle;
};