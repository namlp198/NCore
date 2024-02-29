#pragma once
#include <afxstr.h>

#define TARGETTYPE_FOLDER		1
#define TARGETTYPE_FILE			2

struct SSubPathInfo
{
	CString		s_strItemPath;
	CString		s_strItemName;
	long		s_nCreateDate;
	int			s_nItemType;

	SSubPathInfo()
	{
		Reset();
	}
	void Reset()
	{
		s_strItemPath = _T("");
		s_strItemName = _T("");
		s_nCreateDate = 0;
		s_nItemType = 0;
	}
	
	// 복사 생성자.
	SSubPathInfo(const SSubPathInfo& rhs)
	{
		if(this != &rhs)
		{
			s_strItemPath = rhs.s_strItemPath;
			s_strItemName = rhs.s_strItemName;
			s_nCreateDate = rhs.s_nCreateDate;
			s_nItemType = rhs.s_nItemType;
		}
	}

	// 대입연산자 오버로딩.
    SSubPathInfo& operator=(const SSubPathInfo& rhs)
	{
		if(this != &rhs)
		{
			s_strItemPath = rhs.s_strItemPath;
			s_strItemName = rhs.s_strItemName;
			s_nCreateDate = rhs.s_nCreateDate;
			s_nItemType = rhs.s_nItemType;
		}
		return *this;
	}
};

class CFolderScheduling  
{
public:
	CFolderScheduling();
	CFolderScheduling(CString strPath);
	CFolderScheduling(CString strPath, int nTargetType);
	virtual ~CFolderScheduling();

	BOOL SetRemainDelete(CString strPath, int nTargetType, int nRemainItemCount);
	BOOL CommitSchedule();
	void ResetSchedule();

protected:
	
	BOOL SearchSubItem(const CString &strMainPath, BOOL bIsAscending);
	BOOL SortSubItem(BOOL bIsAscending);	// 오름차순 TRUE, 내림차순 FALSE
	BOOL DeleteFolder(const CString &strFolder);

	// 관리를 할 최상위 경로.
	CString				m_strMainPath;
	BOOL				m_bTargetFolder;
	BOOL				m_bTargetFile;
	BOOL				m_bIsAscending;

	// 일부만 남기고 삭제.
	int					m_nRemainItemCount;

	// 관리 대상 정보
	SSubPathInfo*		m_pPathInfo;
	int					m_nSubItemCount;
};