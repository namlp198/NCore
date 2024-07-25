#pragma once

#include "BasedConfig.h"

#include "FileList.h"
#include "FileMap.h"
#include "Registery.h"
#include "IniManager.h"

enum Grouping	//!> 파일 관리 모듈 그룹
{
	FileMap_Mode = 1,		///> STL Map 이용 관리
	FileManager_Mode = 2,	///> MFC COblist 이용 관리
	Registery_mode = 4,		///> 레지스트리 관리
	Ini_Mode = 5			///> INI 파일 이용 관리
};

/*!
* \brief
* 데이터 관리 클래스 
* 
* 데이터 관리 클래스로서 (파일 관리, 레지스트리 관리를 지원한다.) \n\n
* 파일 모드\n
* SetRegiConfig 을 이용하여 파일 오픈시 메모리로 파일의 내용을 적재 한다. \n
* 데이터 입/출력 후 WriteToFile 함수를 통해서 동기화 하거나 ReWriteMode를 이용하여 실시간 동기화 할 수 있다.
*/
class AFX_EXT_CLASS CConfig  
{
public:
	CConfig();
	virtual ~CConfig();

	BOOL	SetLogWriteMode(BOOL bUse,CString strFilePath);													//!> 로그 모드 설정
	
	BOOL	SetRegiConfig(HKEY hKey, CString strKey, TCHAR* szFilename, Grouping Select = FileMap_Mode);	//!> 파일/레지스트리 로드 

	BOOL	DeleteAllItem();																				//!> 전체 데이터 삭제
	BOOL	DeleteValue(CString strvalue);																	//!> 선택 데이터 삭제
	BOOL	DeleteValue(CString strvalue, int nIdx);														//!> 선택 데이터 삭제

	// File_Mode 만 적용.
	BOOL	WriteToFile();																					//!> FIle Mode 일 경우 메모리의 내용을 파일에 저장한다.
	void	SetRewriteMode(BOOL bRewrite);																	//!> FIle Mode 일 경우 실시간 동기화 모드를 설정한다.
	BOOL	GetRewriteMode();																				//!> File Mode 일 경우 실시간 동기화 모드 여부를 반환한다.

	CString	GetFilePath();																					//!> File Mode 일 경우 관리하는 파일의 경로를 반환한다.

	//////////////////////////////////////////////////////////////////////////
	BOOL	SetItemValue(CString strName,  CString strvalue);												//!> Set strName : 항목구분자 타입 : CString형  
	BOOL	SetItemValue(CString strName, int nValue);														//!> Set strName : 항목구분자 타입 : int형  
	BOOL	SetItemValue(CString strName, unsigned short sValue);											//!> Set strName : 항목구분자 타입 : unsigned short형  
	BOOL	SetItemValue(CString strName, double dValue);													//!> Set strName : 항목구분자 타입 : double형  
	
	BOOL	SetItemValue(int nIdx, CString strName, CString strvalue);										//!> Set nIdx : 배열 인덱스, strName : 항목구분자 타입 : CString형  
	BOOL	SetItemValue(int nIdx, CString strName, int nValue);											//!> Set nIdx : 배열 인덱스, strName : 항목구분자 타입 : int형  
	BOOL	SetItemValue(int nIdx, CString strName, unsigned short sValue);									//!> Set nIdx : 배열 인덱스, strName : 항목구분자 타입 : unsigned short형
	BOOL	SetItemValue(int nIdx, CString strName, double dValue);											//!> Set nIdx : 배열 인덱스, strName : 항목구분자 타입 : double형  
	
	//////////////////////////////////////////////////////////////////////////
	int		GetItemValue(CString strName, CString& strValue, CString strDefault = _T(""));					//!> Get strName : 항목구분자 타입 : CString형  
	int		GetItemValue(CString strName, int& nValue, int nDefault = 0);									//!> Get strName : 항목구분자 타입 : int형  
	int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault = 0);				//!> Get strName : 항목구분자 타입 : unsigned short형  
	int		GetItemValue(CString strName, double& dValue, double dDefault = 0.0);							//!> Get strName : 항목구분자 타입 : double형  
	
	int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault = _T(""));		//!> Get nIdx : 배열 인덱스, strName : 항목구분자 타입 : CString형  
	int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault = 0);							//!> Get nIdx : 배열 인덱스, strName : 항목구분자 타입 : int형  
	int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault = 0);	//!> Get nIdx : 배열 인덱스, strName : 항목구분자 타입 : unsigned short형
	int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault = 0.0);					//!> Get nIdx : 배열 인덱스, strName : 항목구분자 타입 : double형  

public:
	
	Grouping		m_Classification;																		///> 파일/레지스트리 관리 타입

protected:
	
	CBasedConfig	*m_pConfig;																				///> 기본 객체 포인터

	CString			m_strFilePath;																			///> 파일 경로
};