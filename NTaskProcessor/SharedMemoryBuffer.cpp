#include "pch.h"
#include "SharedMemoryBuffer.h"

CSharedMemoryBuffer::CSharedMemoryBuffer(void)
{
	m_hMap = NULL;
	m_pSharedMemory = NULL;
	m_dwFrameWidth = 0;
	m_dwFrameHeight = 0;
	m_dwFrameCount = 0;
	m_dwFrameSize = 0;
}

CSharedMemoryBuffer::~CSharedMemoryBuffer(void)
{
	DeleteSharedMemory();
}

BOOL CSharedMemoryBuffer::CreateSharedMemory(CString strMapFileName, DWORD64 dw64MemSize)
{
	DWORD dwHigh = dw64MemSize >> 32;
	DWORD dwLow = dw64MemSize & 0xFFFFFFFF;

	m_hMap = ::CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE
		, dwHigh, dwLow, strMapFileName);

	BOOL bExist = FALSE;
	if (::GetLastError() == ERROR_ALREADY_EXISTS)
	{
		m_hMap = ::OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, strMapFileName);
		bExist = TRUE;
	}

	if (m_hMap == NULL)
	{
		return FALSE;
	}

	m_pSharedMemory = (BYTE*)::MapViewOfFile(m_hMap, FILE_MAP_ALL_ACCESS, 0, 0, (SIZE_T)dw64MemSize);

	if (m_pSharedMemory == NULL)
	{
		TRACE("MapViewOfFile Fail Last Error[%d]\n", GetLastError());
		return FALSE;
	}

	//새로 생성한 경우만 초기화하자
	if (bExist == FALSE)
		ZeroMemory(m_pSharedMemory, (SIZE_T)dw64MemSize);

	return TRUE;
}

void CSharedMemoryBuffer::DeleteSharedMemory()
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory != NULL)
	{
		::UnmapViewOfFile(m_pSharedMemory);
		m_pSharedMemory = NULL;
	}

	if (m_hMap != NULL)
	{
		::CloseHandle(m_hMap);
		m_hMap = NULL;
	}

	localLock.Unlock();
}

BOOL CSharedMemoryBuffer::SetFrameImage(DWORD dwFrame, LPBYTE pBuf)
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	if (dwFrame >= m_dwFrameCount)
	{
		localLock.Unlock();
		return FALSE;
	}

	DWORD64 dw64Size = (DWORD64)dwFrame * m_dwFrameSize;

	memcpy(m_pSharedMemory + dw64Size, pBuf, m_dwFrameSize);

	localLock.Unlock();

	return TRUE;
}

LPBYTE CSharedMemoryBuffer::GetFrameImage(DWORD dwFrame)
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory == NULL)
	{
		localLock.Unlock();
		return NULL;
	}

	if (dwFrame >= m_dwFrameCount)
	{
		localLock.Unlock();
		return NULL;
	}

	DWORD64 dw64Size = (DWORD64)dwFrame * m_dwFrameSize;

	return (LPBYTE)(m_pSharedMemory + dw64Size);
}

LPBYTE CSharedMemoryBuffer::GetBufferImage(DWORD dwY)
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory == NULL)
	{
		localLock.Unlock();
		return NULL;
	}

	if (dwY < 0 || (m_dwFrameSize * m_dwFrameCount) - 1 < dwY)
	{
		localLock.Unlock();
		return NULL;
	}

	DWORD64 dw64Size = (DWORD64)dwY * m_dwFrameWidth;

	return (LPBYTE)(m_pSharedMemory + dw64Size);
}

BOOL CSharedMemoryBuffer::ClearBufferImage()
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	DWORD64 dw64Size = (DWORD64)(m_dwFrameCount * m_dwFrameSize);

	if (dw64Size == 0)
	{
		localLock.Unlock();
		return FALSE;
	}


	ZeroMemory(m_pSharedMemory, (SIZE_T)dw64Size);

	localLock.Unlock();

	return TRUE;
}
