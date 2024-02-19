// CExcelAutomation.h: interface for the CExcelAutomation class.
//
//////////////////////////////////////////////////////////////////////
// ExcelAutomation 2005. 10. 4. ������
// Desc. �����͸� Excel ���Ϸ� ����
//
// ���� : 
// Excel Automation ���� ������Ʈ �ȿ� ����
// header ���Ͽ� #include ".\Excel Automation\ExcelAutomation.h"
// OLE �ʱ�ȭ : app->InitInstance->AfxOleInit();
// Excel ���� ���� : CreateExcel(CString ���ϸ�)
// Excel ���� ���� : OpenExcel(CString ���ϸ�)
// Excel ���� ���� : SaveExcel(CString ���ϸ�)
// Excel ���� �ݱ� : CloseExcel()
// �۾�ȭ�� ���� : ShowWorkSheet(BOOL 0 or FALSE(�����) �Ǵ� BOOL 1 or TRUE(����))
// ��Ʈ �̵� : SetSheet(short ��Ʈ��ȣ)
// �� ���� : SetValue(CString ���ۼ���ȣ, CString ����������ȣ, CString ��)
//           SetValue(CString ����ȣ, CString ��)
// �� �б� : GetValue(CString ���ۼ���ȣ, CString ����������ȣ)
// �� �б� : GetValue(CString ����ȣ)
// Ex)))
//	CExcelAutomation* pExcel;
//	pExcel = new CExcelAutomation;
//	pExcel->CreateExcel("");
//	pExcel->ShowWorkSheet(FALSE);
//	pExcel->SetSheet(1);
//	pExcel->SetValue("B1", "B1", "Data");
//	pExcel->SaveExcel("FileName");
//	pExcel->CloseExcel();
//	delete pExcel;
//	pExcel = NULL;


#include "excel.h"
#if !defined(AFX_CExcelAutomation_H__D3DB1116_A911_42E3_A44E_8B7A297189D3__INCLUDED_)
#define AFX_CExcelAutomation_H__D3DB1116_A911_42E3_A44E_8B7A297189D3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define LINE_SOLID			1
#define LINE_DOT			2
#define LINE_THIN_DOT		3
#define LINE_DOT_SOLID		4
#define LINE_DOT_DOT_SOLID	5
#define LINE_BIG_DOT_SOLID	6
#define LINE_TWIN_SOLID		9

class AFX_EXT_CLASS CExcelAutomation  
{
public:
	// Attributes
	_Application	m_app;
	_Workbook		m_book;
	_Worksheet		m_sheet;
	Range			m_range;
	Workbooks		m_books;
	Worksheets		m_sheets;
	Borders			m_borders;
	Border			m_border;
	Shapes			m_shapes;
	Shape			m_shape;
	Interior		m_interior;
	CFont0			m_font;
	CCellFormat		m_cell;
	BOOL			m_bShow;

// Operations
	//khs-edit void to BOOL
	BOOL CreateExcel(									// ���� ���� ����
						CString ExcelName,				// ���� �̸�
						long SheetNum);					// ��Ʈ ��
	BOOL OpenExcel(										// ���� ���� ����
						CString ExcelName);				// ���� �̸�
	void SaveExcel(										// ���� ���� ����
						CString ExcelName);				// ���� �̸�
	void SetSheet(										// ��Ʈ ����
						short SheetNumber);				// ��Ʈ ��ȣ
	void SetValue(										// �� ����
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd,					// ����������ȣ
						LPCTSTR CellValue);				// ��
	void SetValue(										// �� ����
						CString CellStart,				// ����ȣ
						LPCTSTR CellValue);				// ��
	void SetName(										// ��Ʈ �̸� ����
						short SheetNumber,				// ��Ʈ ��ȣ
						LPCTSTR SheetName);				// ��Ʈ �̸�
	CString GetValue(									// �� ��������
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd);				// ����������ȣ
	CString GetValue(									// �� ��������
						LPCTSTR CellStart);				// ����ȣ
	void ShowWorkSheet(									// �۾����� ��Ʈ �����ֱ�
						BOOL show);						// True or Not
	void CloseExcel();									// ���� ���� �ݱ�
	void AutoFitColumn(									// Column ���߱�
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd);				// ����������ȣ
	void AutoFitRow(									// Raw ���߱�
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd);				// ����������ȣ
	void MergeCells(	CString CellStart,				// �� ����
						CString CellEnd);
					//	LPCSTR CellStart,				// ���ۼ���ȣ
					//	LPCSTR CellEnd);				// ����������ȣ
	void DrawTable(		CString CellStart,	
						CString CellEnd,						// ǥ �׸���
					//	LPCSTR CellStart,				// ���ۼ���ȣ	
					//	LPCSTR CellEnd,					// ����������ȣ
						short LineType);				// �� ���
	void DrawTableOuter(CString CellStart,	
						CString CellEnd				// ǥ �׸���
					//	LPCSTR CellStart,				// ���ۼ���ȣ	
					//	LPCSTR CellEnd,					// ����������ȣ
						);				// �� ���
	void SetPicture(									// �׸� �ֱ�
						LPCTSTR FileName,				// ���ϸ�
						float x,						// x��ǥ
						float y,						// y��ǥ
						float width,					// ����
						float height,					// ����
						BOOL ErrMsg = TRUE);			// �����޼��� ó������
	void SetHeight(										// ���� ����
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd,					// ����������ȣ
						short height);					// ����
	void SetHeight(		CString CellStart,									// ���� ����
					//	LPCSTR CellStart,				// ����ȣ
						short height);					// ����
	void SetWidth(										// ���� ����
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd,					// ����������ȣ
						short width);					// ����
	void SetWidth(										// ���� ����
						LPCTSTR CellStart,				// ����ȣ
						short width);					// ����
	void SetColor(	CString CellStart,
					CString CellEnd, // �� ����
					//	LPCSTR CellStart,				// ���ۼ���ȣ
					//	LPCSTR CellEnd,					// ����������ȣ
						COLORREF BgColor);				// ��
	void SetColor(										// �� ����
						LPCTSTR CellStart,				// ����ȣ
						COLORREF BgColor);				// ��
	void SetFontColor(									// ��Ʈ �� ����
						LPCTSTR CellStart,				// ����ȣ
						COLORREF FontColor);			// ��
	void SetFontColor(	CString CellStart,				// ���ۼ� ��ȣ
						CString CellEnd,				// ������ �� ��ȣ
						COLORREF FontColor);			// ��
	void SetFont(										// ��Ʈ ����
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd,
						int		nSize,
						CString strFont);				// ����������ȣ
	void SetFontBold(									// ��Ʈ ���ϰ� ����
						CString CellStart,				// ���ۼ���ȣ
						CString CellEnd);				// ����������ȣ
	void SetFontBold(									// ��Ʈ ���ϰ� ����
						LPCTSTR CellStart);				// ���ۼ���ȣ
	void SetNumberFormat(
						LPCTSTR CellStart,
						LPCTSTR NumberFormat);
	void SetAlignCenter(								// ��� ����
						LPCTSTR CellStart,				// ���ۼ���ȣ
						LPCTSTR CellEnd);				// ����������ȣ
	void SetAlignCenter(								// ��� ����
						LPCTSTR CellStart);				// ����ȣ

	int AddSheetAtLast();								// Sheet ���� ����
	int GetShttesCount();

	CString GetName(short SheetNumber);

	CString GetCellName(int nRow,int nCol);
	CExcelAutomation();
	virtual ~CExcelAutomation();

};

#endif // !defined(AFX_CExcelAutomation_H__D3DB1116_A911_42E3_A44E_8B7A297189D3__INCLUDED_)
