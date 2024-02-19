// CExcelAutomation.h: interface for the CExcelAutomation class.
//
//////////////////////////////////////////////////////////////////////
// ExcelAutomation 2005. 10. 4. 이준희
// Desc. 데이터를 Excel 파일로 저장
//
// 사용법 : 
// Excel Automation 폴더 프로젝트 안에 복사
// header 파일에 #include ".\Excel Automation\ExcelAutomation.h"
// OLE 초기화 : app->InitInstance->AfxOleInit();
// Excel 파일 생성 : CreateExcel(CString 파일명)
// Excel 파일 열기 : OpenExcel(CString 파일명)
// Excel 파일 저장 : SaveExcel(CString 파일명)
// Excel 파일 닫기 : CloseExcel()
// 작업화면 보기 : ShowWorkSheet(BOOL 0 or FALSE(숨기기) 또는 BOOL 1 or TRUE(보기))
// 시트 이동 : SetSheet(short 시트번호)
// 값 설정 : SetValue(CString 시작셀번호, CString 마지막셀번호, CString 값)
//           SetValue(CString 셀번호, CString 값)
// 값 읽기 : GetValue(CString 시작셀번호, CString 마지막셀번호)
// 값 읽기 : GetValue(CString 셀번호)
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
	BOOL CreateExcel(									// 엑셀 파일 생성
						CString ExcelName,				// 파일 이름
						long SheetNum);					// 시트 수
	BOOL OpenExcel(										// 엑셀 파일 열기
						CString ExcelName);				// 파일 이름
	void SaveExcel(										// 엑셀 파일 저장
						CString ExcelName);				// 파일 이름
	void SetSheet(										// 시트 설정
						short SheetNumber);				// 시트 번호
	void SetValue(										// 값 설정
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd,					// 마지막셀번호
						LPCTSTR CellValue);				// 값
	void SetValue(										// 값 설정
						CString CellStart,				// 셀번호
						LPCTSTR CellValue);				// 값
	void SetName(										// 시트 이름 설정
						short SheetNumber,				// 시트 번호
						LPCTSTR SheetName);				// 시트 이름
	CString GetValue(									// 값 가져오기
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd);				// 마지막셀번호
	CString GetValue(									// 값 가져오기
						LPCTSTR CellStart);				// 셀번호
	void ShowWorkSheet(									// 작업중인 시트 보여주기
						BOOL show);						// True or Not
	void CloseExcel();									// 엑셀 파일 닫기
	void AutoFitColumn(									// Column 맞추기
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd);				// 마지막셀번호
	void AutoFitRow(									// Raw 맞추기
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd);				// 마지막셀번호
	void MergeCells(	CString CellStart,				// 셀 병합
						CString CellEnd);
					//	LPCSTR CellStart,				// 시작셀번호
					//	LPCSTR CellEnd);				// 마지막셀번호
	void DrawTable(		CString CellStart,	
						CString CellEnd,						// 표 그리기
					//	LPCSTR CellStart,				// 시작셀번호	
					//	LPCSTR CellEnd,					// 마지막셀번호
						short LineType);				// 선 모양
	void DrawTableOuter(CString CellStart,	
						CString CellEnd				// 표 그리기
					//	LPCSTR CellStart,				// 시작셀번호	
					//	LPCSTR CellEnd,					// 마지막셀번호
						);				// 선 모양
	void SetPicture(									// 그림 넣기
						LPCTSTR FileName,				// 파일명
						float x,						// x좌표
						float y,						// y좌표
						float width,					// 넓이
						float height,					// 높이
						BOOL ErrMsg = TRUE);			// 에러메세지 처리여부
	void SetHeight(										// 높이 설정
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd,					// 마지막셀번호
						short height);					// 높이
	void SetHeight(		CString CellStart,									// 높이 설정
					//	LPCSTR CellStart,				// 셀번호
						short height);					// 높이
	void SetWidth(										// 넓이 설정
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd,					// 마지막셀번호
						short width);					// 넓이
	void SetWidth(										// 넓이 설정
						LPCTSTR CellStart,				// 셀번호
						short width);					// 넓이
	void SetColor(	CString CellStart,
					CString CellEnd, // 색 설정
					//	LPCSTR CellStart,				// 시작셀번호
					//	LPCSTR CellEnd,					// 마지막셀번호
						COLORREF BgColor);				// 색
	void SetColor(										// 색 설정
						LPCTSTR CellStart,				// 셀번호
						COLORREF BgColor);				// 색
	void SetFontColor(									// 폰트 색 설정
						LPCTSTR CellStart,				// 셀번호
						COLORREF FontColor);			// 색
	void SetFontColor(	CString CellStart,				// 시작셀 번호
						CString CellEnd,				// 마지막 셀 번호
						COLORREF FontColor);			// 색
	void SetFont(										// 폰트 설정
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd,
						int		nSize,
						CString strFont);				// 마지막셀번호
	void SetFontBold(									// 폰트 진하게 설정
						CString CellStart,				// 시작셀번호
						CString CellEnd);				// 마지막셀번호
	void SetFontBold(									// 폰트 진하게 설정
						LPCTSTR CellStart);				// 시작셀번호
	void SetNumberFormat(
						LPCTSTR CellStart,
						LPCTSTR NumberFormat);
	void SetAlignCenter(								// 가운데 정렬
						LPCTSTR CellStart,				// 시작셀번호
						LPCTSTR CellEnd);				// 마지막셀번호
	void SetAlignCenter(								// 가운데 정렬
						LPCTSTR CellStart);				// 셀번호

	int AddSheetAtLast();								// Sheet 개수 리턴
	int GetShttesCount();

	CString GetName(short SheetNumber);

	CString GetCellName(int nRow,int nCol);
	CExcelAutomation();
	virtual ~CExcelAutomation();

};

#endif // !defined(AFX_CExcelAutomation_H__D3DB1116_A911_42E3_A44E_8B7A297189D3__INCLUDED_)
