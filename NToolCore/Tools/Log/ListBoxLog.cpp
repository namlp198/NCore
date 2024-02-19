#include "pch.h"
#include "ListBoxLog.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define UM_DISPLAYMESSAGE		WM_USER + 8378

CListBoxLog::CListBoxLog()
{
	m_pFileLog = NULL;
	m_strLogPath = _T("");
	m_strLogFile = _T("");
	m_strLogDir = _T("");
	m_strFileName = _T("");

	for (int i = 0; i < STRINGBUFFER_COUNT; i++)
	{
		m_strArray[i] = _T("");
		m_bMustDisplay[i] = FALSE;
	}

	m_nAddIndex = 0;
	m_nReadIndex = 0;
	SetTextColor();
	InitializeCriticalSection(&m_csLog);
}

CListBoxLog::~CListBoxLog()
{
	for (int i = 0; i < STRINGBUFFER_COUNT; i++)
	{
		m_strArray[i] = _T("");
		m_bMustDisplay[i] = FALSE;
	}
	if (m_pFileLog)
	{
		delete m_pFileLog;
		m_pFileLog = NULL;
	}

	DeleteCriticalSection(&m_csLog);
}

BEGIN_MESSAGE_MAP(CListBoxLog, CListBox)
	ON_MESSAGE(UM_DISPLAYMESSAGE, OnDisplayMessage)
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()

// Logging
void CListBoxLog::SetPath(CString& strPath, CString& strName, BOOL bDelete, int nRemainCount)
{
	strPath.Replace(_T("/"), _T("\\"));
	CString str = _T("");
	str = strPath.Right(1);
	if (str == "\\")
		m_strLogPath = strPath;
	else
		m_strLogPath = strPath + _T("\\");

	m_strLogDir = m_strLogPath;
	m_strFileName = strName;

 	if (bDelete && nRemainCount > 0)
 		m_FolderSchedule.SetRemainDelete(m_strLogPath, (int)TARGETTYPE_FOLDER, nRemainCount);

	m_strLogPath += strName;

	MakeLogFile();

	m_dwThreadID = GetCurrentThreadId();
}

void CListBoxLog::DisplayMessages(BOOL bAddMsgBox, int nLevel, TCHAR *str, ...)
{
	try
	{
		EnterCriticalSection(&m_csLog);

		// TRACE에 쓰기.
		va_list list;
		ZeroMemory(m_strBuffer, 512);
		va_start(list, str);
		vswprintf_s(m_strBuffer, 512, str, list);
//		OutputDebugString(m_strBuffer);
		va_end(list);

		// 입력할 문자열 만들기.
		SYSTEMTIME time;
		// CTime	time = CTime::GetCurrentTime();
		GetLocalTime(&time);

		TCHAR strTemp[512] = { 0, };
		swprintf_s(strTemp, 512, _T("[%02d:%02d:%02d.%03d] : %s"), time.wHour, time.wMinute, time.wSecond, time.wMilliseconds, m_strBuffer);

		WriteToFile(time, strTemp);

		// 화면에 출력하는 메시지라면...
		if (bAddMsgBox)
		{
			m_strArray[m_nAddIndex] = _T("");
			m_strArray[m_nAddIndex] = strTemp;
			m_bMustDisplay[m_nAddIndex] = TRUE;
			m_nAddIndex = (m_nAddIndex + 1) % STRINGBUFFER_COUNT;

			if (GetSafeHwnd())
			{
				if (m_dwThreadID == GetCurrentThreadId())
					OnDisplayMessage(1, 0);
				else
				{
					PostMessage(UM_DISPLAYMESSAGE, 0, 0);
					Sleep(0);
				}
			}
		}

		LeaveCriticalSection(&m_csLog);
	}
	catch (...)
	{
		LeaveCriticalSection(&m_csLog);
		return;
	}
}

void CListBoxLog::DisplayMessages(BOOL bAddMsgBox, TCHAR *str, ...)
{
	try
	{
		EnterCriticalSection(&m_csLog);

		// TRACE에 쓰기.
		va_list list;
		ZeroMemory(m_strBuffer, 512);
		va_start(list, str);
		vswprintf_s(m_strBuffer, 512, str, list);
//		OutputDebugString(m_strBuffer);
		va_end(list);

		// 입력할 문자열 만들기.
		SYSTEMTIME time;
		// CTime	time = CTime::GetCurrentTime();
		GetLocalTime(&time);

		TCHAR strTemp[512] = { 0, };
		swprintf_s(strTemp, 512, _T("[%02d:%02d:%02d.%03d] : %s"), time.wHour, time.wMinute, time.wSecond, time.wMilliseconds, m_strBuffer);

		WriteToFile(time, strTemp);

		// 화면에 출력하는 메시지라면...
		if (bAddMsgBox)
		{
			m_strArray[m_nAddIndex] = _T("");
			m_strArray[m_nAddIndex] = strTemp;
			m_bMustDisplay[m_nAddIndex] = TRUE;
			m_nAddIndex = (m_nAddIndex + 1) % STRINGBUFFER_COUNT;

			if (GetSafeHwnd())
			{
				if (m_dwThreadID == GetCurrentThreadId())
					OnDisplayMessage(1, 0);
				else
				{
					PostMessage(UM_DISPLAYMESSAGE, 0, 0);
					Sleep(0);
				}
			}
		}

		LeaveCriticalSection(&m_csLog);
	}
	catch (...)
	{
		LeaveCriticalSection(&m_csLog);
		return;
	}
}

void CListBoxLog::DisplayMessage(BOOL bAddMsgBox, int nLevel, TCHAR *str)
{
	try
	{
		EnterCriticalSection(&m_csLog);

		// 입력할 문자열 만들기.
		SYSTEMTIME time;
		//CTime	time = CTime::GetCurrentTime();
		GetLocalTime(&time);

		TCHAR strTemp[512] = { 0, };
		swprintf_s(strTemp, 512, _T("[%02d:%02d:%02d.%03d] : %s"), time.wHour, time.wMinute, time.wSecond, time.wMilliseconds, str);

		WriteToFile(time, strTemp);

		// 화면에 출력하는 메시지라면...
		if (bAddMsgBox)
		{
			m_strArray[m_nAddIndex] = _T("");
			m_strArray[m_nAddIndex] = strTemp;
			m_bMustDisplay[m_nAddIndex] = TRUE;
			m_nAddIndex = (m_nAddIndex + 1) % STRINGBUFFER_COUNT;

			if (GetSafeHwnd())
			{
				if (m_dwThreadID == GetCurrentThreadId())
					OnDisplayMessage(1, 0);
				else
				{
					PostMessage(UM_DISPLAYMESSAGE, 0, 0);
					Sleep(0);
				}
			}
		}

		LeaveCriticalSection(&m_csLog);
	}
	catch (...)
	{
		LeaveCriticalSection(&m_csLog);
		return;
	}
}

void CListBoxLog::DisplayMessage(BOOL bAddMsgBox, TCHAR *str)
{
	try
	{
		EnterCriticalSection(&m_csLog);

		// 입력할 문자열 만들기.
		SYSTEMTIME time;
		// CTime	time = CTime::GetCurrentTime();
		GetLocalTime(&time);

		TCHAR strTemp[512] = {0, };
		swprintf_s(strTemp, 512, _T("[%02d:%02d:%02d.%03d] : %s"), time.wHour, time.wMinute, time.wSecond, time.wMilliseconds, str);

		WriteToFile(time, strTemp);

		// 화면에 출력하는 메시지라면...
		if (bAddMsgBox)
		{
			m_strArray[m_nAddIndex] = _T("");
			m_strArray[m_nAddIndex] = strTemp;
			m_bMustDisplay[m_nAddIndex] = TRUE;
			m_nAddIndex = (m_nAddIndex + 1) % STRINGBUFFER_COUNT;

			if (GetSafeHwnd())
			{
				if (m_dwThreadID == GetCurrentThreadId())
					OnDisplayMessage(1, 0);
				else
				{
					PostMessage(UM_DISPLAYMESSAGE, 0, 0);
					Sleep(0);
				}
			}
		}

		LeaveCriticalSection(&m_csLog);
	}
	catch (...)
	{
		LeaveCriticalSection(&m_csLog);
		return;
	}
}

// Log File에 쓰기.
BOOL CListBoxLog::WriteToFile(CTime& time, CString& strContents)
{
	// 날짜가 바뀌면 파일명을 갱신해서 쓴다.
	if ((time.GetMonth() != m_TimeLogFile.GetMonth()) || (time.GetDay() != m_TimeLogFile.GetDay()) || !m_pFileLog)
		MakeLogFile();

	if (!m_pFileLog->Open(m_strLogFile, CFile::modeWrite))
	{
		if (!m_pFileLog->Open(m_strLogFile, CFile::modeCreate | CFile::modeWrite))
		{
			delete m_pFileLog;
			m_pFileLog = NULL;
		}
	}

	if (m_pFileLog)
	{
		m_pFileLog->SeekToEnd();

USES_CONVERSION;
		char strTemp[1024] = {};
		sprintf_s(strTemp, "%s", W2A(strContents));

		m_pFileLog->Write(strTemp, (UINT) strlen(strTemp));
		m_pFileLog->Write("\r\n", (UINT) strlen("\r\n"));
		//m_pFileLog->Write(_T("\r\n"), static_cast<UINT>(wcslen(_T("\r\n"))));
		m_pFileLog->Close();
	}

	return TRUE;
}

BOOL CListBoxLog::WriteToFile(CTime& time, TCHAR* strContents)
{
	// 날짜가 바뀌면 파일명을 갱신해서 쓴다.
	if ((time.GetMonth() != m_TimeLogFile.GetMonth()) || (time.GetDay() != m_TimeLogFile.GetDay()) || !m_pFileLog)
		MakeLogFile();

	if (!m_pFileLog->Open(m_strLogFile, CFile::modeWrite))
	{
		if (!m_pFileLog->Open(m_strLogFile, CFile::modeCreate | CFile::modeWrite))
		{
			delete m_pFileLog;
			m_pFileLog = NULL;
		}
	}

	if (m_pFileLog)
	{
		m_pFileLog->SeekToEnd();

USES_CONVERSION;
		char strTemp[1024] = {};
		sprintf_s(strTemp, "%s", W2A(strContents));

		m_pFileLog->Write(strTemp, (UINT) strlen(strTemp));
		m_pFileLog->Write("\r\n", (UINT) strlen("\r\n"));
		m_pFileLog->Close();
	}

	return TRUE;
}

BOOL CListBoxLog::WriteToFile(SYSTEMTIME& time, CString& strContents)
{
	// 날짜가 바뀌면 파일명을 갱신해서 쓴다.
	if ((time.wMonth != m_TimeLogFile.GetMonth()) || (time.wDay != m_TimeLogFile.GetDay()) || !m_pFileLog)
		MakeLogFile();

	if (!m_pFileLog->Open(m_strLogFile, CFile::modeWrite))
	{
		if (!m_pFileLog->Open(m_strLogFile, CFile::modeCreate | CFile::modeWrite))
		{
			delete m_pFileLog;
			m_pFileLog = NULL;
		}
	}

	if (m_pFileLog)
	{
		m_pFileLog->SeekToEnd();

		USES_CONVERSION;
		char strTemp[1024] = {};
		sprintf_s(strTemp, "%s", W2A(strContents));

		m_pFileLog->Write(strTemp, (UINT)strlen(strTemp));
		m_pFileLog->Write("\r\n", (UINT)strlen("\r\n"));
		//m_pFileLog->Write(_T("\r\n"), static_cast<UINT>(wcslen(_T("\r\n"))));
		m_pFileLog->Close();
	}

	return TRUE;
}

BOOL CListBoxLog::WriteToFile(SYSTEMTIME& time, TCHAR* strContents)
{
	// 날짜가 바뀌면 파일명을 갱신해서 쓴다.
	if ((time.wMonth != m_TimeLogFile.GetMonth()) || (time.wDay != m_TimeLogFile.GetDay()) || !m_pFileLog)
		MakeLogFile();

	if (m_pFileLog == NULL)
		return FALSE;

	if (!m_pFileLog->Open(m_strLogFile, CFile::modeWrite))
	{
		if (!m_pFileLog->Open(m_strLogFile, CFile::modeCreate | CFile::modeWrite))
		{
			delete m_pFileLog;
			m_pFileLog = NULL;
		}
	}

	if (m_pFileLog)
	{
		m_pFileLog->SeekToEnd();

		USES_CONVERSION;
		char strTemp[1024] = {};
		sprintf_s(strTemp, "%s", W2A(strContents));

		m_pFileLog->Write(strTemp, (UINT)strlen(strTemp));
		m_pFileLog->Write("\r\n", (UINT)strlen("\r\n"));
		m_pFileLog->Close();
	}

	return TRUE;
}

LRESULT CListBoxLog::OnDisplayMessage(WPARAM wParam, LPARAM lParam)
{
	if (wParam == 0)
		EnterCriticalSection(&m_csLog);

	int count=0, nInsertCount = 0;
	while (m_bMustDisplay[m_nReadIndex] && nInsertCount < STRINGBUFFER_COUNT)
	{
		// 1000개일때 리스트를 클리어.
		count = GetCount();
		if(count > 1000)
		{
			ResetContent();
			count = GetCount();
		}

		InsertString(count, (LPCTSTR)m_strArray[m_nReadIndex]);

		m_strArray[m_nReadIndex] = _T("");
		m_bMustDisplay[m_nReadIndex] = FALSE;

		m_nReadIndex = (m_nReadIndex + 1) % STRINGBUFFER_COUNT;

		nInsertCount++;
	}
	SetCaretIndex(count + 1);
//	SetFocus();
	SendMessage(WM_KEYDOWN, VK_END);

	if (wParam == 0)
		LeaveCriticalSection(&m_csLog);

	return 0;
}

BOOL CListBoxLog::MakeLogFile()
{
	// Log 처리
	if (m_strLogPath.IsEmpty())
		return FALSE;

	// 일정 수 이상 되면 오래된 항목을 지운다.
	m_FolderSchedule.CommitSchedule();

	m_TimeLogFile = CTime::GetCurrentTime();

	CString	strLogPathWitSISM_DD;
	strLogPathWitSISM_DD = _T("");
	strLogPathWitSISM_DD.Format(_T("%s%02d"),m_strLogDir, m_TimeLogFile.GetMonth());
	CreateDirectory(strLogPathWitSISM_DD, NULL);

	//strLogPathWitSISM_DD_HH.AppendFormat("\\%02d",m_TimeLogFile.GetDay());
	//CreateDirectory(strLogPathWitSISM_DD_HH, NULL);


	m_strLogFile.Format(_T("%s\\%s_%02d_%02d.log"), strLogPathWitSISM_DD, m_strFileName
		,m_TimeLogFile.GetMonth(), m_TimeLogFile.GetDay());

//	m_strLogFile.Format("%s_%02d_%02d.log", m_strLogPath, m_TimeLogFile.GetMonth(), m_TimeLogFile.GetDay());

	if (m_pFileLog)
		delete m_pFileLog;
	m_pFileLog = NULL;

	m_pFileLog = new CFile();

	return TRUE;
}

void CListBoxLog::SetTextColor(long clbg1, long clbg2,long clText)
{
	m_clBG1 = clbg1;
	m_clBG2 = clbg2;
	m_clText = clText;
}

void CListBoxLog::DrawItem(LPDRAWITEMSTRUCT lpDIS)
{
	COLORREF oldtextcolor, oldbackgroundcolor;

	CDC* pDC = CDC::FromHandle(lpDIS->hDC);
	
	int m_nTabStopPositions[100];
	for (int i = 0; i < 100; i++)
		m_nTabStopPositions[i] = (i+1) * 8 * 7;

	int m_cxExtent = 0;

	if (lpDIS->itemID == (UINT)-1)
	{
		//if (lpDIS->itemAction & ODA_FOCUS)
		//	pDC->DrawFocusRect(&lpDIS->rcItem);
		return;
	}
	else
	{
		int selChange   = lpDIS->itemAction & ODA_SELECT;
		int focusChange = lpDIS->itemAction & ODA_FOCUS;
		int drawEntire  = lpDIS->itemAction & ODA_DRAWENTIRE;

		if (selChange || drawEntire)
		{
			BOOL sel = lpDIS->itemState & ODS_SELECTED;

			int nLen = CListBox::GetTextLen(lpDIS->itemID);

			if (nLen != LB_ERR)
			{
				TCHAR *buf = new TCHAR [nLen + 16];
				ASSERT(buf);
				buf[0] = 0;
				if (buf && (CListBox::GetText(lpDIS->itemID, buf) != LB_ERR))
				{
					CRect rectItem(lpDIS->rcItem);
					CSize size;					
									
					COLORREF textcolor = sel ? RGB(255, 255, 255) : m_clText;
					oldtextcolor = pDC->SetTextColor(textcolor);
					COLORREF backgroundcolor = sel ? RGB(0, 0, 255):lpDIS->itemID%2==0?m_clBG1:m_clBG2;
					oldbackgroundcolor = pDC->SetBkColor(backgroundcolor);					

					// fill the rectangle with the background color the fast way
					pDC->ExtTextOut(0, 0, ETO_OPAQUE, &rectItem, NULL, 0, NULL);

					pDC->TabbedTextOut(rectItem.left+2, rectItem.top, &buf[0],
						(int)_tcslen(&buf[0]), 100, (LPINT)m_nTabStopPositions, 0);

					size = pDC->GetOutputTextExtent(&buf[0]);
					int nScrollBarWidth = ::GetSystemMetrics(SM_CXVSCROLL);
					size.cx += nScrollBarWidth;	// in case of vertical scrollbar

					int cxExtent = (size.cx > m_cxExtent) ? size.cx : m_cxExtent;

					if (cxExtent > m_cxExtent)
					{
						m_cxExtent = cxExtent;
						SetHorizontalExtent(m_cxExtent+(m_cxExtent/32));
					}
				}
				if (buf)
					delete [] buf;
			}
		}

		//if (focusChange || (drawEntire && (lpDIS->itemState & ODS_FOCUS)))
		//	pDC->DrawFocusRect(&lpDIS->rcItem);
	}
}


BOOL CListBoxLog::OnEraseBkgnd(CDC* pDC)
{	
	CRect rect;
	GetClientRect(&rect);	
	
	int nHeight = GetItemHeight(0);
	int nCount = rect.Height() / nHeight;

	CRect rtItem(rect);
	for(int i = 0; i < nCount; i++)
	{
		rtItem.bottom = rtItem.top + nHeight;
		if(i % 2 == 0)
		{
			pDC->FillSolidRect(rtItem, m_clBG1);
		}
		else
		{
			pDC->FillSolidRect(rtItem, m_clBG2);
		}
		rtItem.top = rtItem.bottom;
	}

	return TRUE;
}