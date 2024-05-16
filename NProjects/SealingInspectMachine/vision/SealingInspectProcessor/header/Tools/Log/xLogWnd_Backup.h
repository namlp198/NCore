// xLogWnd.h: interface for the xLogWnd class.
//
//////////////////////////////////////////////////////////////////////

#pragma once

// [xLogWnd Handling Document at 20040528 by xnizx]
//
// 로그아웃을 사용하려면 다음을 참조하십시오
// 로그아웃의 유형은 세가지입니다.
//
// 1) 일반로그 : 일반적인 목적으로 사용됩니다.
// 2) 경고로그 : 경고성 목적으로 사용됩니다.
// 3) 에러로그 : 에러발생시 사용됩니다.
//
// 다음과 같은 방법으로 사용하십시오.
// A) XLOGOUT:일반로그
//      Ex1: XLOGOUT(_T("printf의 형식으로 포맷팅하십시오."));
//      Ex2: XLOGOUT(_T("%04d : 일반로그입니다."), 100);
//
// B) XLOGWAR:경고로그
//      Ex1: XLOGWAR(_T("%04d : 경고로그입니다."), 100);
//
// C) XLOGERR:에러로그
//      Ex1: XLOGERR(_T("%04d : 에러로그입니다."), 100);
//
// 자세한 로그아웃을 위한 덤프를 지원합니다.
// Prepare Ex)
//      TCHAR pDump[2048];
//      ::lstrcpy(pDump, _T("이것은 덤프데이터입니다."), 10);
//
// A) XDUMPOUT:일반로그
//      Ex1: XDUMPOUT(pDump)(_T("%04d : 일반로그입니다."), 100);
//
// B) XDUMPWAR:경고로그
//      Ex1: XDUMPWAR(pDump)(_T("%04d : 경고로그입니다."), 100);
//
// C) XDUMPERR:일반로그
//      Ex1: XDUMPERR(pDump)(_T("%04d : 에러로그입니다."), 100);
//
// 더욱 자세한 로그아웃을 지원합니다.
//      GetLastError()의 메시지는 덤프됩니다.
//
// A) XLOGOUT_LASTERROR : GetLastError()를 포맷팅합니다.
//      Ex1:
//      try {
//          ... some error
//      } catch (...) {
//          XLOGOUT_LASTERROR;
//      }



static  LPCTSTR    g_logOut                = _T("xLogWnd");
static  LPCTSTR    g_logOutwindowClassName = _T("xLogWnd Receiver");

// iType
#define XLIT_ADVANCE	0
#define XLIT_WARNING	1
#define XLIT_ERROR		2

// Buffer Limitation
#define _MAX_FROM		1024
#define _MAX_WHERE		1024
#define _MAX_MESSAGE	2048
#define _MAX_DUMP		2048

#define MAX_STRING		2048

typedef struct _XLOGITEM {
	int		iType;
	TCHAR	pszFrom[_MAX_FROM];
	TCHAR	pszWhere[_MAX_WHERE];
	TCHAR	pszMessage[_MAX_MESSAGE];
	TCHAR	pszDump[_MAX_DUMP];
} XLOGITEM, FAR* LPXLOGITEM;

inline void _LogOut (int nType, LPCTSTR pFrom, LPCTSTR pWhere, LPCTSTR pMessage, LPCTSTR pDump)
{
    COPYDATASTRUCT cd; 
    HWND hWnd = ::FindWindow (g_logOutwindowClassName, g_logOut); 
    if (hWnd)
    {  
		XLOGITEM li;
		::memset(&li, NULL, sizeof(XLOGITEM));

		li.iType = nType;
		if (pFrom)
		{
#ifdef _UNICODE
			::lstrcpynW(li.pszFrom, pFrom, _MAX_FROM);
#else
			::lstrcpyn(li.pszFrom, pFrom, _MAX_FROM);
#endif
		}

		if (pWhere)
		{
#ifdef _UNICODE
			::lstrcpynW(li.pszWhere, pWhere, _MAX_WHERE);
#else
			::lstrcpyn(li.pszWhere, pWhere, _MAX_WHERE);
#endif
		}

		if (pMessage)
		{
#ifdef _UNICODE
			::lstrcpynW(li.pszMessage, pMessage, _MAX_MESSAGE);
#else
			::lstrcpyn(li.pszMessage, pMessage, _MAX_MESSAGE);
#endif
		}

		if (pDump)
		{
#ifdef _UNICODE
			::lstrcpynW(li.pszDump, pDump, _MAX_DUMP);
#else
			::lstrcpyn(li.pszDump, pDump, _MAX_DUMP);
#endif
		}

#ifdef _UNICODE
        cd.dwData = 0xFEFF;
#else
		cd.dwData = 0;
#endif
        cd.cbData = sizeof(XLOGITEM);
        cd.lpData = (void *)&li;
        ::SendMessage (hWnd, WM_COPYDATA, 0, (LPARAM)&cd);
    } 
}

inline LPCTSTR LogFormat (LPCTSTR pFormat, ...)
{
	va_list args;
	va_start(args, pFormat);

	static _TCHAR buffer[MAX_STRING] = {0,};
#if (_MSC_VER >= 1400)
	_vsntprintf_s(buffer, MAX_STRING, MAX_STRING - 1, pFormat, args);
#else  
	_vsntprintf(buffer, MAX_STRING - 1, pFormat, args);
#endif

    va_end(args);

	return buffer;
}

inline void LogOut (int iType, void* pFilename, unsigned int lineno, LPCTSTR pszMessage, LPCTSTR pDump = NULL)
{
    TCHAR where[_MAX_WHERE] = {0,};
	TCHAR Filename[_MAX_WHERE] = {0,};
	#ifdef _UNICODE
		MultiByteToWideChar(CP_ACP, 0, (LPCSTR)pFilename, (int)strlen((LPCSTR)pFilename)+1, Filename, int(sizeof(Filename)/sizeof(Filename[0])) );
	#else
		::lstrcpyn(Filename, (LPTSTR)pFilename, _MAX_WHERE);
	#endif
	wsprintf(where, _T("%d line in %s"), lineno, (LPTSTR)Filename);
    _LogOut (iType, ::AfxGetAppName(), where, pszMessage, pDump);
}

inline DWORD LogOutLastError (void* pFilename, unsigned int lineno, LPCTSTR pszMessage)
{
   if (::GetLastError() == 0) 
        return 0;
   
    LPVOID pDump;
    DWORD  result;
    result = ::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
                             NULL,
                             GetLastError(),
                             MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US),
                             (LPTSTR)&pDump,
                             0,
                             NULL);
  
    ::LogOut(XLIT_ERROR, pFilename, lineno, pszMessage, (LPCTSTR)pDump);
    
    if(result)
        ::LocalFree(pDump);
   
    return result;
}

class CxLogSystem {
public:
	CxLogSystem(int nType, void* pFrom, int nLineno, LPCTSTR pDump = NULL)
		: m_pFrom(pFrom), m_nLineno(nLineno), m_nType(nType), m_pDump(pDump), m_nLastError(0)
	{
	}
	CxLogSystem(int nType, void* pFrom, int nLineno, int nLastError)
		: m_pFrom(pFrom), m_nLineno(nLineno), m_nType(nType), 
		m_pDump(NULL), m_nLastError(nLastError)
	{
	}

	inline void Trace(LPCTSTR pFormat, ...)
	{
		va_list args;
		va_start(args, pFormat);

		static _TCHAR buffer[MAX_STRING] = {0,};
#if (_MSC_VER >= 1400)
	_vsntprintf_s(buffer, MAX_STRING, MAX_STRING - 1, pFormat, args);
#else  
	_vsntprintf(buffer, MAX_STRING - 1, pFormat, args);
#endif

		va_end(args);
		LogOut(m_nType, m_pFrom, m_nLineno, buffer, m_pDump);
	}
	inline void TraceLastError()
	{
		if (m_nLastError == 0)
			return;

		static _TCHAR buffer[MAX_STRING] = {0,};
		wsprintf(buffer, _T(" %d : the calling thread's last-error code value"), m_nLastError);

		LPVOID pDump;
		DWORD  result;
		result = ::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
								 NULL,
								 m_nLastError,
								 MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US),
								 (LPTSTR)&pDump,
								 0,
								 NULL);

		::LogOut(XLIT_ERROR, m_pFrom, m_nLineno, buffer, (LPCTSTR)pDump);

		if(result)
			::LocalFree(pDump);
	}

protected:
	int m_nLastError;
	int m_nType;
	int m_nLineno;
	void* m_pFrom;
	LPCTSTR m_pDump;
};

// XLOGOUT
#define XLOGFMT	::LogFormat
#define XLOGOUT	CxLogSystem(XLIT_ADVANCE, __FILE__, __LINE__).Trace
#define XLOGWAR	CxLogSystem(XLIT_WARNING, __FILE__, __LINE__).Trace
#define XLOGERR	CxLogSystem(XLIT_ERROR, __FILE__, __LINE__).Trace
#define XDUMPOUT(dump)	CxLogSystem(XLIT_ADVANCE, __FILE__, __LINE__, dump).Trace
#define XDUMPWAR(dump)	CxLogSystem(XLIT_WARNING, __FILE__, __LINE__, dump).Trace
#define XDUMPERR(dump)	CxLogSystem(XLIT_ERROR, __FILE__, __LINE__, dump).Trace
// XLOGOUT_LASTERROR
#define XLOGOUT_LASTERROR	CxLogSystem(XLIT_ERROR, __FILE__, __LINE__, GetLastError()).TraceLastError()
