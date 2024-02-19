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
		CONNECTRESULT_CONNECT_FAIL = 0, //�������
		CONNECTRESULT_CONNECT_SUCCESS = 1, //���Ἲ��
		CONNECTRESULT_ALREADY_CREATESOCKET = 2, //�̹̻����� ����
		CONNECTRESULT_INVALID_SOCKET = 3, //���ϻ��� ����
		CONNECTRESULT_WSASTARTUP_FAIL = 5 //StartUP����
		
	};
	// ����
	CONNECTRESULT Connect(char* serverip, int port);
	// ���� �ɶ����� ���� �õ�
	void ConnectContinue(char* serverip, int port);
	// ����
	void DisConnect();  
	// ���� �޼��� ���� �ݹ� ����
	void SetRecvMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// �ý��� �޼��� ���� �ݹ� ����
	void SetSystemMessageCallback(RecvMsgFunc userFunc, void* data = NULL);
	// �޼��� ����
	bool Send(char* sendchar, int length);      
	// ���� ����
	inline BOOL GetConnectState(){return (m_isConnect == true) ? TRUE : FALSE;};	
	
	SOCKET getSocket(){return m_ClinetSocket;};

	static unsigned int _stdcall RecvMSG(void *arg);      // �޴� ������

	bool m_bConnectRetry; //���� �ɶ����� ���� �õ�
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
	
	SOCKET m_ClinetSocket;		//Ŭ���̾�Ʈ ����

	bool m_isConnect;			//���� ����
	
	int m_port;					//���� ��Ʈ
	char m_servIP[15];			//���� ����
	

	HANDLE m_hThread;       //������ �ڵ�    
	unsigned int m_dwThreadID;  //������ ���̵�


	RecvMsgFunc m_recvmsgFunc;
	void* m_recvmsgData;
	SystemMsgFunc m_systemmsgFunc;
	void* m_systemmsgData;

	int m_nInstanceHandle;
};