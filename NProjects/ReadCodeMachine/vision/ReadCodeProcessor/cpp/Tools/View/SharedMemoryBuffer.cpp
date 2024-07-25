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
	DWORD dwHigh = dw64MemSize>>32;
	DWORD dwLow = dw64MemSize & 0xFFFFFFFF;

	m_hMap = ::CreateFileMapping(INVALID_HANDLE_VALUE,NULL,PAGE_READWRITE
		, dwHigh, dwLow, strMapFileName);

	BOOL bExist = FALSE;
	if (::GetLastError() == ERROR_ALREADY_EXISTS)
	{
		m_hMap = ::OpenFileMapping(FILE_MAP_ALL_ACCESS,FALSE, strMapFileName);
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
	if(bExist == FALSE)
		ZeroMemory(m_pSharedMemory, (SIZE_T) dw64MemSize);

	return TRUE;
}

void CSharedMemoryBuffer::DeleteSharedMemory()
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	if (m_pSharedMemory!=NULL)
	{
		::UnmapViewOfFile(m_pSharedMemory);
		m_pSharedMemory = NULL;
	}

	if (m_hMap != NULL )
	{
		::CloseHandle(m_hMap);
		m_hMap = NULL;
	}

	localLock.Unlock();
}

BOOL CSharedMemoryBuffer::LoadHughImage(CString strPath)
{
	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	HANDLE hFile = CreateFile(strPath, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	if (hFile == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	DWORD dwFileSizeHigh;
	DWORD64 qwFileSize = GetFileSize(hFile, &dwFileSizeHigh);

	HANDLE hFileMapping = CreateFileMapping(hFile, NULL, PAGE_READONLY, dwFileSizeHigh, (DWORD)qwFileSize, 0);

	qwFileSize += ( ((__int64)dwFileSizeHigh) << 32 );

	if(hFileMapping == NULL)
	{
		if (hFile != NULL )
		{
			::CloseHandle(hFile);
			hFile = NULL;
		}

		localLock.Unlock();
		return FALSE;
	}

	SYSTEM_INFO si;
	GetSystemInfo(&si);

	__int64 qwFileOffset = 0;

	BITMAPFILEHEADER	fileHeader;
	BITMAPINFOHEADER	infoHeader;

	DWORD dwBytesInBlock = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD) * 256;

	PBYTE pbFile = (PBYTE)MapViewOfFile(hFileMapping, FILE_MAP_READ,
		(DWORD)(qwFileOffset >> 32),  // 상위 오프셋
		(DWORD)(qwFileOffset & 0xFFFFFFFF), // 하위 오프셋
		qwFileSize);

	memcpy(&fileHeader, pbFile, sizeof(BITMAPFILEHEADER));
	memcpy(&infoHeader, pbFile+sizeof(BITMAPFILEHEADER), sizeof(BITMAPINFOHEADER));

	DWORD64 dw64Idx = 0;

	int nWidth = infoHeader.biWidth;
	int nHeight = infoHeader.biHeight;

	BYTE *pBuffer = m_pSharedMemory;

	DWORD64 dw64LineSize = (DWORD64)(nHeight - 1) * nWidth + fileHeader.bfOffBits;
	for(int i= nHeight - 1; i >= 0; i--)
	{
		memcpy(pBuffer + dw64Idx, pbFile + dw64LineSize, nWidth);
		dw64Idx += nWidth;
		dw64LineSize -= nWidth;
	}

	UnmapViewOfFile(pbFile);

	if (pbFile != NULL )
	{
		::UnmapViewOfFile(pbFile);
		pbFile = NULL;
	}

	if (hFileMapping != NULL )
	{
		::CloseHandle(hFileMapping);
		hFileMapping = NULL;
	}

	if (hFile != NULL )
	{
		::CloseHandle(hFile);
		hFile = NULL;
	}

	localLock.Unlock();

	return TRUE;
}

struct stBMPHeader
{
#define DIB_HEADER_MARKER   ((WORD) ('M' << 8) | 'B')
	BITMAPFILEHEADER	fileHeader;
	BITMAPINFOHEADER	infoHeader;
	RGBQUAD				quad[256];

	stBMPHeader()
	{
		fileHeader.bfType = DIB_HEADER_MARKER;  // "BM"
		fileHeader.bfReserved1 = 0;
		fileHeader.bfReserved2 = 0;
		fileHeader.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD)*256;

		infoHeader.biSize = 40;// sizeof(BITMAPINFOHEADER)
		infoHeader.biWidth = 0;
		infoHeader.biHeight = 0;
		infoHeader.biPlanes = 1;
		infoHeader.biBitCount = 8;
		infoHeader.biCompression = 0;
		infoHeader.biSizeImage = 0;
		infoHeader.biXPelsPerMeter = 2923;
		infoHeader.biYPelsPerMeter = 2923;
		infoHeader.biClrUsed = 0;
		infoHeader.biClrImportant = 0;

		int i;
		for(i= 0; i< 256; i++)
		{
			quad[i].rgbRed= i;
			quad[i].rgbGreen= i;
			quad[i].rgbBlue= i;
			quad[i].rgbReserved= 0;
		}
	}
};

BOOL CSharedMemoryBuffer::SaveHughImage(CString strPath)
{
	int nWidth = m_dwFrameWidth;
	int nHeight = m_dwFrameHeight * m_dwFrameCount;

	DWORD64 dw64Size = (DWORD64)nWidth * nHeight;
	if(dw64Size < 10)
		return FALSE;

	stBMPHeader bmpHeader;
	bmpHeader.fileHeader.bfSize = bmpHeader.fileHeader.bfOffBits + (DWORD)dw64Size;

	bmpHeader.infoHeader.biWidth = nWidth;
	bmpHeader.infoHeader.biHeight = nHeight;
	bmpHeader.infoHeader.biSizeImage = (DWORD)dw64Size;

	DWORD dwHeaderSize = bmpHeader.fileHeader.bfOffBits;
	dw64Size += (DWORD64)dwHeaderSize;
	DWORD dwHigh = dw64Size>>32;
	DWORD dwLow = dw64Size & 0xFFFFFFFF;

	CSingleLock localLock(&m_csSharedMemory);
	localLock.Lock();

	HANDLE hFile = CreateFile(strPath, GENERIC_READ | GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);

	if (hFile == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	HANDLE hFileMapping = CreateFileMapping(hFile, NULL, PAGE_READWRITE, dwHigh, dwLow, NULL);

	if (hFileMapping == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	LPBYTE pFileMemory = (LPBYTE)::MapViewOfFile(hFileMapping, FILE_MAP_ALL_ACCESS, 0, 0, dw64Size);

	if (pFileMemory == NULL)
	{
		localLock.Unlock();
		return FALSE;
	}

	DWORD64 dw64Idx = 0, dw64LineSize = (DWORD64)(m_dwFrameHeight * (m_dwFrameCount + 1) - 1) * nWidth;
	memcpy(pFileMemory, &bmpHeader.fileHeader, sizeof(BITMAPFILEHEADER));
	memcpy(pFileMemory + sizeof(BITMAPFILEHEADER), &bmpHeader.infoHeader, sizeof(BITMAPINFOHEADER));
	memcpy(pFileMemory + sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER), &bmpHeader.quad, sizeof(RGBQUAD)*256);

	dw64Idx += dwHeaderSize;

	for(int i= nHeight - 1; i >= 0; i--)
	{
		memcpy(pFileMemory + dw64Idx, m_pSharedMemory + dw64LineSize, nWidth);
		dw64Idx += nWidth;
		dw64LineSize -= nWidth;
	}

	if (pFileMemory!=NULL)
	{
		::UnmapViewOfFile(pFileMemory);
		pFileMemory = NULL;
	}

	if (hFileMapping != NULL )
	{
		::CloseHandle(hFileMapping);
		hFileMapping = NULL;
	}

	if (hFile != NULL )
	{
		::CloseHandle(hFile);
		hFile = NULL;
	}

	localLock.Unlock();

	return TRUE;
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

	DWORD64 dw64Size = (DWORD64) (m_dwFrameCount * m_dwFrameSize);

	if (dw64Size == 0)
	{
		localLock.Unlock();
		return FALSE;
	}


	ZeroMemory(m_pSharedMemory, (SIZE_T) dw64Size);

	localLock.Unlock();

	return TRUE;
}
