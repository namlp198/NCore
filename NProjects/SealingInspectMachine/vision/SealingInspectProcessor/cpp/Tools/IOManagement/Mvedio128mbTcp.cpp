#include "pch.h"
#include "Mvedio128mbTcp.h"
#include "mvtype.h"
#include "mvcp.h"
using namespace mvsol::protocol;

Mvedio128mbTcp::Mvedio128mbTcp()
{
	m_bConnected = FALSE;
	m_DO = 0;
	m_DI = 0;

	AfxSocketInit(NULL);
	Create();
}


Mvedio128mbTcp::~Mvedio128mbTcp()
{
	Close();
}

BOOL Mvedio128mbTcp::Connect(const CString& strIP, INT port)
{
	m_bConnected = CAsyncSocket::Connect(strIP, port);
	if (m_bConnected)
	{
		SetEnableAll();
	}
	return m_bConnected;
}

void Mvedio128mbTcp::Close()
{
	//CAsyncSocket::ShutDown();
	//CAsyncSocket::Close();
	m_bConnected = FALSE;
}

BOOL Mvedio128mbTcp::IsConnected()
{
	return m_bConnected;
}

int Mvedio128mbTcp::Send(BYTE command, USHORT address, ULONGLONG data)
{
	BYTE buffer[256];
	BYTE buffer2[256];
	memset(buffer, 0, 256);
	memset(buffer2, 0, 256);
	int bufferSize = 0;
	int bufferSize2 = 0;

	buffer[0] = 0x02;
	buffer[1] = command;
	memcpy(&buffer[2], &address, sizeof(address));
	memcpy(&buffer[4], &data, sizeof(data));
	buffer[12] = 0x00;

	UINT sum = 0;
	for (int i = 1; i < 13; i++)
	{
		sum += buffer[i];
	}
	buffer[13] = (sum & 0xFF);
	buffer[14] = 0x03;
	bufferSize = 15;

	Security::Encrypt(buffer, bufferSize, buffer2, &bufferSize2);

	//std::lock_guard<std::mutex> guard(mutex);
	return CAsyncSocket::Send(buffer2, bufferSize2);
}

int Mvedio128mbTcp::Write(USHORT address, ULONGLONG data)
{
	return Send('W', address, data);
}

int Mvedio128mbTcp::Read(USHORT address)
{
	return Send('R', address, 0);
}

void Mvedio128mbTcp::OnConnect(int nErrorCode)
{
	TRACE("Mvlp600a04Tcp - 소켓연결됨\n");
}

void Mvedio128mbTcp::OnClose(int nErrorCode)
{
	CAsyncSocket::ShutDown();
	CAsyncSocket::Close();
	TRACE("Mvlp600a04Tcp - 소켓종료됨\n");
}

void Mvedio128mbTcp::OnReceive(int nErrorCode)
{
	static int i = 0;
	i++;

	BYTE buff[4096];
	int nRead;
	nRead = Receive(buff, 4096);

	switch (nRead)
	{
	case 0:
		Close();
		break;
	case SOCKET_ERROR:
		if (GetLastError() != WSAEWOULDBLOCK)
		{
			AfxMessageBox(_T("Error occurred"));
			Close();
		}
		break;
	default:

		if (buff[0] == 0x02 && buff[nRead - 1] == 0x03)
		{
			BYTE buffer[4096];
			INT bufferSize = 0;

			Security::Decrypt(buff, nRead, buffer, &bufferSize);

			BYTE command;
			USHORT address;
			ULONGLONG data;
			if (buffer[12] == 0x00)
			{
				command = buffer[1];
				memcpy(&address, &buffer[2], sizeof(address));
				memcpy(&data, &buffer[4], sizeof(data));

				if (address == 0x5800 || address == 0x2000)
				{
					m_DO = data;
				}
				else if (address == 0x0050)
				{
					m_DI = data;
				}
			}

		}
	}
	CAsyncSocket::OnReceive(nErrorCode);
}



//////////////////////////////////////////////////////////////////
// 사용자 영역
//
bool Mvedio128mbTcp::Trigger()
{
	return (m_DI >> 1) & 0x1;
}

int Mvedio128mbTcp::ReadReceiveValue(ULONGLONG& nValue)
{
	int nReturn = Read(0x0050);

	if (nReturn==SOCKET_ERROR)
	{
		return 0;
	}

	nValue = m_DI;

	return 1;
}

int Mvedio128mbTcp::WriteSendValue(const ULONGLONG& nValue)
{
	//Read(0x5800);
	m_DO = nValue;
	int nReturn = Write(0x0020, nValue);

	if (nReturn==SOCKET_ERROR)
	{
		return 0;
	}

	return 1;
}

void Mvedio128mbTcp::ClearDo()
{
	Write(0x2000, 0);
}

void Mvedio128mbTcp::SetBusy(bool busy)
{
	if (busy)
	{
		m_DO = 0x2;
	}
	else
	{
		m_DO &= ~0x2;
	}
	Write(0x2000, m_DO);
}

void Mvedio128mbTcp::SetOK()
{
	m_DO &= ~(0x7 << 2);
	m_DO |= 0x4;
	Write(0x2000, m_DO);
}

void Mvedio128mbTcp::SetNG1()
{
	m_DO &= ~(0x7 << 2);
	m_DO |= 0x8;
	Write(0x2000, m_DO);
}

void Mvedio128mbTcp::SetNG2()
{
	m_DO &= ~(0x7 << 2);
	m_DO |= 0x10;
	Write(0x2000, m_DO);
}

void Mvedio128mbTcp::SetEnableAll()
{
	Write(0x0018, 0xFFFFFFFFFFFFFFFF);
}
