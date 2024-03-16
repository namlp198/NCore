#pragma once

#define		MAP_FILE_NAME_INS_BUFFER				_T("MAP_FILE_INS_BUFFER")
#define		MAP_FILE_NAME_ALIGN_BUFFER				_T("MAP_FILE_ALIGN_BUFFER")
#define		MAP_FILE_NAME_ALIGN_RESULT_BUFFER		_T("MAP_FILE_ALIGN_RESULT_BUFFER")

class AFX_EXT_CLASS CSharedMemoryBuffer
{
public:
	CSharedMemoryBuffer(void);
	virtual ~CSharedMemoryBuffer(void);

	BOOL	CreateSharedMemory(CString strMapFileName, DWORD64 dw64MemSize);
	void	DeleteSharedMemory();

	BOOL	LoadHughImage(CString strPath);
	BOOL	SaveHughImage(CString strPath);

	BYTE*	GetSharedBuffer() { return m_pSharedMemory; }

	BOOL	SetFrameImage(DWORD dwFrame, LPBYTE pBuf);
	LPBYTE  GetFrameImage(DWORD dwFrame);
	LPBYTE	GetBufferImage(DWORD dwY);
	BOOL	ClearBufferImage();

	void	SetFrameWidth(DWORD dwFrameWidth)	{m_dwFrameWidth = dwFrameWidth;};
	void	SetFrameHeight(DWORD dwFrameHeight)	{m_dwFrameHeight = dwFrameHeight;};
	void	SetFrameCount(DWORD dwFrameCount)	{m_dwFrameCount = dwFrameCount;};
	void	SetFrameSize(DWORD dwFrameSize)		{m_dwFrameSize = dwFrameSize;};

	DWORD	GetFrameWidth()						{return m_dwFrameWidth;};
	DWORD	GetFrameHeight()					{return m_dwFrameHeight;};
	DWORD	GetFrameCount()						{return m_dwFrameCount;};
	DWORD	GetFrameSize()						{return m_dwFrameSize;};

private:
	DWORD				m_dwFrameWidth;
	DWORD				m_dwFrameHeight;
	DWORD				m_dwFrameCount;
	DWORD				m_dwFrameSize;

protected:
	HANDLE				m_hMap;

	CCriticalSection	m_csSharedMemory;
	BYTE*				m_pSharedMemory;
};
