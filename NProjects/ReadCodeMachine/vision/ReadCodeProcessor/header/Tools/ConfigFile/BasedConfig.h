#pragma once
/*!
* \brief
* �������̽� Ŭ���� 
* 
* ���� ����� ���̺귯���� �������̽� Ŭ����
* 
*/
class CBasedConfig  
{
public:
	CBasedConfig();
	virtual ~CBasedConfig();

	virtual BOOL	Initialize(HKEY hKey, CString strKey, CString strFilename) = 0;									//!> �ʱ�ȭ �Լ� �������̽�
	
	//////////////////////////////////////////////////////////////////////////
	virtual BOOL	SetItemValue(CString strName,  CString& strvalue) = 0;											//!> Set �������̽� strName : �׸񱸺��� Ÿ�� : CString��  
	virtual BOOL	SetItemValue(CString strName, int& nValue) = 0;													//!> Set �������̽� strName : �׸񱸺��� Ÿ�� : int��  
	virtual BOOL	SetItemValue(CString strName, unsigned short &sValue) = 0;										//!> Set �������̽� strName : �׸񱸺��� Ÿ�� : unsigned short��  
	virtual BOOL	SetItemValue(CString strName, double& dValue) = 0;												//!> Set �������̽� strName : �׸񱸺��� Ÿ�� : double��  
	
	virtual BOOL	SetItemValue(int nIdx, CString strName, CString& strvalue) = 0;									//!> Set �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : CString��  
	virtual BOOL	SetItemValue(int nIdx, CString strName, int& nValue) = 0;										//!> Set �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : int��  
	virtual BOOL	SetItemValue(int nIdx, CString strName, unsigned short &sValue) = 0;							//!> Set �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : unsigned short��  
	virtual BOOL	SetItemValue(int nIdx, CString strName, double& dValue) = 0;									//!> Set �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : double��  
	
	//////////////////////////////////////////////////////////////////////////
	virtual int		GetItemValue(CString strName, CString& strValue, CString strDefault) = 0;						//!> Get �������̽� strName : �׸񱸺��� Ÿ�� : CString��  
	virtual int		GetItemValue(CString strName, int& nValue, int nDefault) = 0;									//!> Get �������̽� strName : �׸񱸺��� Ÿ�� : int��  
	virtual int		GetItemValue(CString strName, unsigned short &nValue, unsigned short sDefault) = 0;				//!> Get �������̽� strName : �׸񱸺��� Ÿ�� : unsigned short��  
	virtual int		GetItemValue(CString strName, double& dValue, double dDefault) = 0;								//!> Get �������̽� strName : �׸񱸺��� Ÿ�� : double��  
	
	virtual int		GetItemValue(int nIdx, CString strName, CString& strValue, CString strDefault) = 0;				//!> Get �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : CString��  
	virtual int		GetItemValue(int nIdx, CString strName, int& nValue, int nDefault) = 0;							//!> Get �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : int��  
	virtual int		GetItemValue(int nIdx, CString strName, unsigned short &nValue, unsigned short sDefault) = 0;	//!> Get �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : unsigned short�� 
	virtual int		GetItemValue(int nIdx, CString strName, double& dValue, double dDefault) = 0;					//!> Get �������̽� nIdx : �迭 �ε���, strName : �׸񱸺��� Ÿ�� : double��  
	
	virtual BOOL	RemoveAllItem() = 0;																			//!> �ε�� �޸��� ��� �׸��� ����
	virtual BOOL	RemoveItem(CString strName) = 0;																//!> strName �̸��� ���� �׸��� ����
	virtual BOOL	RemoveItem(int nIdx, CString strName) = 0;														//!> �迭(nIdx), strName �̸��� ���� �׸��� ����

	
	virtual BOOL	WriteToFile() = 0;																				//!> ���Ͽ� �׸� ���� ( �޸� -> ���� )

	virtual void	SetRewriteMode(BOOL bRewrite) = 0;																//!> ReWrite ����� ���� ( TRUE�� ��� �޸𸮿� ������ �ǽð� ����ȭ �Ѵ�)
	virtual BOOL	GetRewriteMode() = 0;																			//!> ReWrite ��� ���� �ޱ�
	

	// OhByungGil  Modify -> LogFile Write
	virtual void	SetLogMode(BOOL bMode) = 0;																		//!> �α� ����� ���� ( TRUE�� ��� �׸��� ����� �ÿ� �α׸� ����Ѵ�. )
	virtual BOOL	GetLogMode() = 0;																				//!> �α� ��� ���� �ޱ�
	virtual void	SetLogFilePath(CString strLogFilePath) = 0;														//!> �α� ������ ��� ����
	virtual CString GetLogFilePath() = 0;																			//!> ������ �α� ������ ��� �ޱ�
};