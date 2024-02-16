#pragma once
class AFX_EXT_CLASS CSharedMemoryBuffer
{
public:
	CSharedMemoryBuffer(void);
	virtual ~CSharedMemoryBuffer(void);

	BOOL	CreateSharedMemory(CString strMapFileName, DWORD64 dw64MemSize);
	void	DeleteSharedMemory();

	BYTE* GetSharedBuffer() { return m_pSharedMemory; }

	BOOL	SetFrameImage(DWORD dwFrame, LPBYTE pBuf);
	LPBYTE  GetFrameImage(DWORD dwFrame);
	LPBYTE	GetBufferImage(DWORD dwY);
	BOOL	ClearBufferImage();

	void	SetFrameWidth(DWORD dwFrameWidth) { m_dwFrameWidth = dwFrameWidth; };
	void	SetFrameHeight(DWORD dwFrameHeight) { m_dwFrameHeight = dwFrameHeight; };
	void	SetFrameCount(DWORD dwFrameCount) { m_dwFrameCount = dwFrameCount; };
	void	SetFrameSize(DWORD dwFrameSize) { m_dwFrameSize = dwFrameSize; };

	DWORD	GetFrameWidth() { return m_dwFrameWidth; };
	DWORD	GetFrameHeight() { return m_dwFrameHeight; };
	DWORD	GetFrameCount() { return m_dwFrameCount; };
	DWORD	GetFrameSize() { return m_dwFrameSize; };

private:
	DWORD				m_dwFrameWidth;
	DWORD				m_dwFrameHeight;
	DWORD				m_dwFrameCount;
	DWORD				m_dwFrameSize;

protected:
	HANDLE				m_hMap;

	CCriticalSection	m_csSharedMemory;
	BYTE* m_pSharedMemory;
};