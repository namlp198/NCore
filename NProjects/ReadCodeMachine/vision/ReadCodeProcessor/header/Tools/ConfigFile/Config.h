#pragma once

#include "BasedConfig.h"

#include "FileList.h"
#include "FileMap.h"
#include "Registery.h"
#include "IniManager.h"

enum Grouping	//!> ���� ���� ��� �׷�
{
	FileMap_Mode = 1,		///> STL Map �̿� ����
	FileManager_Mode = 2,	///> MFC COblist �̿� ����
	Registery_mode = 4,		///> ������Ʈ�� ����
	Ini_Mode = 5			///> INI ���� �̿� ����
};

/*!
* \brief
* ������ ���� Ŭ���� 
* 
* ������ ���� Ŭ�����μ� (���� ����, ������Ʈ�� ������ �����Ѵ�.) \n\n
* ���� ���\n
* SetRegiConfig �� �̿��Ͽ� ���� ���½� �޸𸮷� ������ ������ ���� �Ѵ�. \n
* ������ ��/��� �� WriteToFile �Լ��� ���ؼ� ����ȭ �ϰų� ReWriteMode�� �̿��Ͽ� �ǽð� ����ȭ �� �� �ִ�.
*/
class AFX_EXT_CLASS CConfig  
{
public:
	CConfig();
	virtual ~CConfig();

	BOOL	SetLogWriteMode(BOOL bUse,CString strFilePath);													//!> �α� ��� ����
	
	BOOL	SetRegiConfig(HKEY hKey, CString strKey, TCHAR* szFilename, Grouping Select = FileMap_Mode);	//!> ����/������Ʈ�� �ε� 

	BOOL	DeleteAllItem();																				//!> ��ü ������ ����
	BOOL	DeleteValue(CString strvalue);																	//!> ���� ������ ����
	BOOL	DeleteValue(CString strvalue, int nIdx);														//!> ���� ������ ����

	// File_Mode �� ����.
	BOOL	WriteToFile();																					//!> FIle Mode �� ��� �޸��� ������ ���Ͽ� �����Ѵ�.
	void	SetRewriteMode(BOOL bRewrite);																	//!> FIle Mode �� ��� �ǽð� ����ȭ ��带 �����Ѵ�.
	BOOL	GetRewriteMode();																				//!> File Mode �� ��� �ǽð� ����ȭ ��� ���θ� ��ȯ�Ѵ�.

	CString	GetFilePath();																					//!> File Mode �� ��� �����ϴ� ������ ��θ� ��ȯ�Ѵ�.

	//////////////////////////////////////////////////////////////////////////
	BOOL	SetItemValue(CString strName,  CString strvalue);												//!> Set strName : �׸񱸺��� Ÿ�� : CString��  
	BOOL	SetItemValue(CString strName, int nValue);														//!> Set strName : �׸񱸺��� Ÿ�� : int��  
	BOOL	SetItemValue(CString strName, unsigned short sValue);											//!> Set strName : �׸񱸺��� Ÿ�� : unsigned short��  
	BOOL	SetItemValue(CString strName, double dValue);													//!> Set strName : �׸񱸺��� Ÿ�� : double��  
	
	BOOL	SetItemValue(int nIdx, CString strName, CString strvalue);										//!> Set nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : CString��  
	BOOL	SetItemValue(int nIdx, CString strName, int nValue);											//!> Set nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : int��  
	BOOL	SetItemValue(int nIdx, CString strName, unsigned short sValue);									//!> Set nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : unsigned short��
	BOOL	SetItemValue(int nIdx, CString strName, double dValue);											//!> Set nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : double��  
	
	//////////////////////////////////////////////////////////////////////////
	int		GetItemValue(CString strName, CString& strValue, CString strDefault = _T(""));					//!> Get strName : �׸񱸺��� Ÿ�� : CString��  
	int		GetItemValue(CString strName, int& nValue, int nDefault = 0);									//!> Get strName : �׸񱸺��� Ÿ�� : int��  
	int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault = 0);				//!> Get strName : �׸񱸺��� Ÿ�� : unsigned short��  
	int		GetItemValue(CString strName, double& dValue, double dDefault = 0.0);							//!> Get strName : �׸񱸺��� Ÿ�� : double��  
	
	int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault = _T(""));		//!> Get nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : CString��  
	int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault = 0);							//!> Get nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : int��  
	int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault = 0);	//!> Get nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : unsigned short��
	int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault = 0.0);					//!> Get nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : double��  

public:
	
	Grouping		m_Classification;																		///> ����/������Ʈ�� ���� Ÿ��

protected:
	
	CBasedConfig	*m_pConfig;																				///> �⺻ ��ü ������

	CString			m_strFilePath;																			///> ���� ���
};