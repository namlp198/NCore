#pragma once
/*!
* \brief
* 인터페이스 클래스 
* 
* 파일 입출력 라이브러리의 인터페이스 클래스
* 
*/
class CBasedConfig  
{
public:
	CBasedConfig();
	virtual ~CBasedConfig();

	virtual BOOL	Initialize(HKEY hKey, CString strKey, CString strFilename) = 0;									//!> 초기화 함수 인터페이스
	
	//////////////////////////////////////////////////////////////////////////
	virtual BOOL	SetItemValue(CString strName,  CString& strvalue) = 0;											//!> Set 인터페이스 strName : 항목구분자 타입 : CString형  
	virtual BOOL	SetItemValue(CString strName, int& nValue) = 0;													//!> Set 인터페이스 strName : 항목구분자 타입 : int형  
	virtual BOOL	SetItemValue(CString strName, unsigned short &sValue) = 0;										//!> Set 인터페이스 strName : 항목구분자 타입 : unsigned short형  
	virtual BOOL	SetItemValue(CString strName, double& dValue) = 0;												//!> Set 인터페이스 strName : 항목구분자 타입 : double형  
	
	virtual BOOL	SetItemValue(int nIdx, CString strName, CString& strvalue) = 0;									//!> Set 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : CString형  
	virtual BOOL	SetItemValue(int nIdx, CString strName, int& nValue) = 0;										//!> Set 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : int형  
	virtual BOOL	SetItemValue(int nIdx, CString strName, unsigned short &sValue) = 0;							//!> Set 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : unsigned short형  
	virtual BOOL	SetItemValue(int nIdx, CString strName, double& dValue) = 0;									//!> Set 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : double형  
	
	//////////////////////////////////////////////////////////////////////////
	virtual int		GetItemValue(CString strName, CString& strValue, CString strDefault) = 0;						//!> Get 인터페이스 strName : 항목구분자 타입 : CString형  
	virtual int		GetItemValue(CString strName, int& nValue, int nDefault) = 0;									//!> Get 인터페이스 strName : 항목구분자 타입 : int형  
	virtual int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault) = 0;				//!> Get 인터페이스 strName : 항목구분자 타입 : unsigned short형  
	virtual int		GetItemValue(CString strName, double& dValue, double dDefault) = 0;								//!> Get 인터페이스 strName : 항목구분자 타입 : double형  
	
	virtual int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault) = 0;				//!> Get 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : CString형  
	virtual int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault) = 0;							//!> Get 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : int형  
	virtual int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault) = 0;	//!> Get 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : unsigned short형 
	virtual int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault) = 0;					//!> Get 인터페이스 nIdx : 배열 인덱스, strName : 항목구분자 타입 : double형  
	
	virtual BOOL	RemoveAllItem() = 0;																			//!> 로드된 메모리의 모든 항목을 삭제
	virtual BOOL	RemoveItem(CString strName) = 0;																//!> strName 이름을 가진 항목을 삭제
	virtual BOOL	RemoveItem(int nIdx, CString strName) = 0;														//!> 배열(nIdx), strName 이름을 가진 항목을 삭제

	
	virtual BOOL	WriteToFile() = 0;																				//!> 파일에 항목 쓰기 ( 메모리 -> 파일 )

	virtual void	SetRewriteMode(BOOL bRewrite) = 0;																//!> ReWrite 모드의 설정 ( TRUE일 경우 메모리와 파일을 실시간 동기화 한다)
	virtual BOOL	GetRewriteMode() = 0;																			//!> ReWrite 모드 상태 받기
	

	// OhByungGil  Modify -> LogFile Write
	virtual void	SetLogMode(BOOL bMode) = 0;																		//!> 로그 모드의 설정 ( TRUE일 경우 항목이 변경될 시에 로그를 출력한다. )
	virtual BOOL	GetLogMode() = 0;																				//!> 로그 모드 상태 받기
	virtual void	SetLogFilePath(CString strLogFilePath) = 0;														//!> 로그 파일의 경로 설정
	virtual CString GetLogFilePath() = 0;																			//!> 설정된 로그 파일의 경로 받기
};