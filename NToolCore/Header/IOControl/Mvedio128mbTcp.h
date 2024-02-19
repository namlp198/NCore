#pragma once

#include <afxsock.h>
//#include <mutex>

/*
사용법:
	Mvedio128mbTcp tcp;
	
	//연결
	//포트번호는 34700입니다.
	if (!tcp.Connect("127.0.0.1", 34700))
	{
		TRACE("접속실패.\n");
		return;
	}
	
	//만든 함수 호출
	if (tcp.Trigger())
	{
		TRACE("트리거 입력.\n");
	}
	
	tcp.SetBusy(true);
	tcp.SetOK();
	
	
	//연결해제
	tcp.Close();
*/

class Mvedio128mbTcp : public CSocket
{
public:
	Mvedio128mbTcp();
	virtual ~Mvedio128mbTcp();

	BOOL Connect(const CString& strIP, INT port);
	void Close();

	BOOL IsConnected();

	void ClearDo();
	int ReadReceiveValue(ULONGLONG& nValue);
	int WriteSendValue(const ULONGLONG& nValue);

	////////////////////////////////////////////////////////////////////
	// 사용자에 맞게 함수를 만들어 주세요.
	// 멤버변수 DI와 DO를 사용하세요.
	//
	//DI
	bool Trigger();
	//
	//DO
	void SetBusy(bool busy);
	void SetOK();
	void SetNG1();
	void SetNG2();
	void SetEnableAll();
	//
	////////////////////////////////////////////////////////////////////

protected:
	int Send(BYTE command, USHORT address, ULONGLONG data);
	int Write(USHORT address, ULONGLONG data);
	int Read(USHORT address);

	virtual void OnConnect(int nErrorCode) override;
	virtual void OnClose(int nErrorCode) override;
	virtual void OnReceive(int nErrorCode) override;


	ULONGLONG	m_DI;
	ULONGLONG	m_DO;
	BOOL		m_bConnected;
};

