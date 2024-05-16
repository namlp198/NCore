// CLogView.cpp: 구현 파일
//
#include "pch.h"
#include "LogView.h"


// CLogView 대화 상자

IMPLEMENT_DYNAMIC(CLogView, CDialogEx)

CLogView::CLogView(CWnd* pParent /*=nullptr*/)
	: CDialogEx(IDD_DIALOG_LOG, pParent)
{
}

CLogView::~CLogView()
{
}

void CLogView::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_LOG, m_LogBox);
}

BEGIN_MESSAGE_MAP(CLogView, CDialogEx)
	ON_BN_CLICKED(IDC_CHECK_ALWAYS_TOP_ON, &CLogView::OnBnClickedCheckAlwaysTopOn)
	ON_BN_CLICKED(IDC_BUTTON_OPEN_PATH, &CLogView::OnBnClickedButtonOpenPath)
END_MESSAGE_MAP()


// CLogView 메시지 처리기

void CLogView::ShowMode(BOOL bShow)
{
	CRect rtCurrnt;
	GetWindowRect(&rtCurrnt);

	if (bShow == TRUE)
	{
		SetWindowPos(&wndTopMost, rtCurrnt.left, rtCurrnt.top, rtCurrnt.Width(), rtCurrnt.Height(), SWP_NOSIZE);
		ShowWindow(SW_SHOW);
		SetActiveWindow();			
	}
	else
	{
		SetWindowPos(&wndNoTopMost, rtCurrnt.left, rtCurrnt.top, rtCurrnt.Width(), rtCurrnt.Height(), SWP_NOSIZE);
		ShowWindow(SW_HIDE);
	}
}

void CLogView::SetLogPath(CString strPath, CString strName, int nRemainCount)
{
	m_strLogDirPath = strPath;

	m_LogBox.SetPath(strPath, strName, TRUE, nRemainCount);

#ifndef _DEBUG
	CString strLogPath;
	strLogPath.Format(_T("Log Path : %s"), m_strLogDirPath);
	SetDlgItemText(IDC_STATIC_LOG_PATH, strLogPath);
#endif // !_DEBUG
}

void CLogView::DisplayMessage(CString strLog)
{
#ifndef _DEBUG
	m_LogBox.DisplayMessage(TRUE, (TCHAR*)(LPCTSTR)strLog);
#else
	m_LogBox.DisplayMessage(FALSE, (TCHAR*)(LPCTSTR)strLog);
#endif // !_DEBUG
}

void CLogView::OnBnClickedCheckAlwaysTopOn()
{
	UpdateData(FALSE);

	BOOL bCheck = ((CButton*)(GetDlgItem(IDC_CHECK_ALWAYS_TOP_ON)))->GetCheck();

	CRect rtCurrnt;
	GetWindowRect(&rtCurrnt);

	if (bCheck)
		SetWindowPos(&wndTopMost, rtCurrnt.left, rtCurrnt.top, rtCurrnt.Width(), rtCurrnt.Height(), SWP_NOSIZE);
	else
		SetWindowPos(&wndNoTopMost, rtCurrnt.left, rtCurrnt.top, rtCurrnt.Width(), rtCurrnt.Height(), SWP_NOSIZE);
}

void CLogView::OnBnClickedButtonOpenPath()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	if (m_strLogDirPath.GetLength() == 0)
		return;

	ShellExecute(NULL, _T("open"), _T("explorer.exe"), m_strLogDirPath, NULL, SW_SHOW);
}