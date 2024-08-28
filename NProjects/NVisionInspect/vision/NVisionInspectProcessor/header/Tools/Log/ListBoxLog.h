#pragma once

#include "FolderScheduling.h"

/////////////////////////////////////////////////////////////////////////////
// CListBoxLog window

#define LOG_NORMAL		1
#define LOG_HISTORY		2
#define LOG_WARNING		3
#define LOG_ERROR		4
#define LOG_DEBUG		5

#define STRINGBUFFER_COUNT		200
class AFX_EXT_CLASS CListBoxLog : public CListBox
{
// Construction
public:
	CListBoxLog();
	virtual ~CListBoxLog();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CListBoxLog)
	//}}AFX_VIRTUAL

	//////////////////////////////////////////////////////////////////////////
	// Logging
	void	SetPath(CString& strPath, CString& strName, BOOL bDelete, int nRemainCount);
	void	DisplayMessages(BOOL bAddMsgBox, int nLevel, TCHAR *str, ...);
	void	DisplayMessages(BOOL bAddMsgBox, TCHAR *str, ...);
	void	DisplayMessage(BOOL bAddMsgBox, int nLevel, TCHAR *str);
	void	DisplayMessage(BOOL bAddMsgBox, TCHAR *str);
	void	SetTextColor(long clbg1 = RGB(255,255,255), long clbg2 = RGB(255,255,255), long clText = RGB(0,0,0));
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDIS);
	// Generated message map functions
protected:
	//{{AFX_MSG(CListBoxLog)
	afx_msg LRESULT OnDisplayMessage(WPARAM wParam, LPARAM lParam);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()

	//////////////////////////////////////////////////////////////////////////
	// Folder Management
	CFolderScheduling	m_FolderSchedule;
		
	//////////////////////////////////////////////////////////////////////////
	// Logging
	CString				m_strLogDir;
	CString				m_strFileName;

	CString				m_strLogPath;
	CString				m_strLogFile;
	CTime				m_TimeLogFile;
	CFile*				m_pFileLog;

	TCHAR				m_strBuffer[512];

	BOOL				MakeLogFile();
	BOOL				WriteToFile(CTime& time, CString& strContents);
	BOOL				WriteToFile(CTime& time, TCHAR* strContents);
	BOOL				WriteToFile(SYSTEMTIME& time, CString& strContents);
	BOOL				WriteToFile(SYSTEMTIME& time, TCHAR* strContents);

	DWORD				m_dwThreadID;

	CString				m_strArray[STRINGBUFFER_COUNT];
	BOOL				m_bMustDisplay[STRINGBUFFER_COUNT];
	int					m_nAddIndex;
	int					m_nReadIndex;
	CRITICAL_SECTION	m_csLog;

	long				m_clBG1;
	long				m_clBG2;
	long				m_clText;
};