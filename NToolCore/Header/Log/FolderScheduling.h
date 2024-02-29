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
	
	// ���� ������.
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

	// ���Կ����� �����ε�.
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
	BOOL SortSubItem(BOOL bIsAscending);	// �������� TRUE, �������� FALSE
	BOOL DeleteFolder(const CString &strFolder);

	// ������ �� �ֻ��� ���.
	CString				m_strMainPath;
	BOOL				m_bTargetFolder;
	BOOL				m_bTargetFile;
	BOOL				m_bIsAscending;

	// �Ϻθ� ����� ����.
	int					m_nRemainItemCount;

	// ���� ��� ����
	SSubPathInfo*		m_pPathInfo;
	int					m_nSubItemCount;
};