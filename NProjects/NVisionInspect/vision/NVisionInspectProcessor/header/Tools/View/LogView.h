#pragma once
#include "afxdialogex.h"
#include "ListBoxLog.h"
// CLogView 대화 상자

#ifndef IDD_DIALOG_LOG
#define IDD_DIALOG_LOG                  4000
#endif
#ifndef IDC_CHECK_ALWAYS_TOP_ON
#define IDC_CHECK_ALWAYS_TOP_ON         4001
#endif
#ifndef IDC_STATIC_LOG_PATH
#define IDC_STATIC_LOG_PATH             4002
#endif
#ifndef IDC_BUTTON_OPEN_PATH
#define IDC_BUTTON_OPEN_PATH            4003
#endif

class AFX_EXT_CLASS CLogView : public CDialogEx
{
	DECLARE_DYNAMIC(CLogView)

public:
	CLogView(CWnd* pParent = nullptr);   // 표준 생성자입니다.
	virtual ~CLogView();

// 대화 상자 데이터입니다.
	enum { IDD = IDD_DIALOG_LOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 지원입니다.

	DECLARE_MESSAGE_MAP()

public:
	void ShowMode(BOOL bShow);

public:
	void SetLogPath(CString strPath, CString strName, int nRemainCount);
	void DisplayMessage(CString strLog);

private:
	CString			m_strLogDirPath;
	CListBoxLog		m_LogBox;
public:
	afx_msg void OnBnClickedCheckAlwaysTopOn();
	afx_msg void OnBnClickedButtonOpenPath();
};
