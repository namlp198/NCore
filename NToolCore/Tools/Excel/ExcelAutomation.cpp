// CExcelAutomation.cpp: implementation of the CExcelAutomation class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "ExcelAutomation.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

COleVariant vTrue((short)TRUE),  vFalse((short)FALSE), vOptional((long)DISP_E_PARAMNOTFOUND, VT_ERROR);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CExcelAutomation::CExcelAutomation()
{
	m_bShow = FALSE;
	CoInitialize(NULL);
}

CExcelAutomation::~CExcelAutomation()
{
}

////////////////////////////////////////////////////////////////
// Excel 파일 생성
// CreateExcel(파일명, 시트 수)

//khs-edit void to BOOL
BOOL CExcelAutomation::CreateExcel(CString ExcelName, long SheetNum)
{
//	
	//khs-edit
	if(!m_app.CreateDispatch( _T("Excel.Application") ) )
	{
//		AfxMessageBox("Failed Excel Start");
		return FALSE;
	}
	m_app.SetSheetsInNewWorkbook(SheetNum);
	m_books = m_app.GetWorkbooks();
	m_book = m_books.Add(vOptional);
	m_sheets = m_book.GetWorksheets();
	m_sheet = m_sheets.GetItem(COleVariant((short)1));
	m_sheet.Activate();
	m_app.SetVisible(m_bShow);
	//khs-edit

	return TRUE;
}

////////////////////////////////////////////////////////////////
// Excel 파일 열기 
// OpenExcel(파일명)

BOOL CExcelAutomation::OpenExcel(CString ExcelName)
{
	if(!m_app.CreateDispatch( _T("Excel.Application") ) )
	{
//		AfxMessageBox("Failed Excel Start");
		return FALSE;
	}
	m_books = m_app.GetWorkbooks();
	m_book = m_books.Open(ExcelName, 
							vOptional, vOptional, vOptional, vOptional,
							vOptional, vOptional, vOptional, vOptional,
							vOptional, vOptional, vOptional, vOptional,
							vOptional, vOptional);

	m_sheets = m_book.GetWorksheets();
	m_sheet = m_sheets.GetItem(COleVariant((short)1));
	m_sheet.Activate();
	m_app.SetVisible(m_bShow);
	return TRUE;
}

////////////////////////////////////////////////////////////////
// Excel 파일 저장
// SaveExcel(파일명)

void CExcelAutomation::SaveExcel(CString ExcelName)
{
	m_app.SetDisplayAlerts(FALSE); // 저장 경고 메세지 출력 안함.
	m_book.SaveAs(COleVariant(ExcelName), COleVariant((long)-4143),  
					vOptional, vOptional, vFalse, vFalse, (long)1,
					COleVariant((long)1), vFalse, vFalse,vFalse, vFalse);
}

////////////////////////////////////////////////////////////////
// Excel 파일 닫기
// CloseExcel()

void CExcelAutomation::CloseExcel()
{
	m_app.SetVisible(FALSE);
	m_app.Quit();
	m_app.m_bAutoRelease = TRUE;
	m_app.ReleaseDispatch();
	m_app.DetachDispatch();
}

////////////////////////////////////////////////////////////////
// 시트 이동
// SetSheet(이동할 시트 번호)

void CExcelAutomation::SetSheet(short SheetNumber)
{
	m_sheet = m_sheets.GetItem(COleVariant(SheetNumber));
	m_sheet.Activate();
}

////////////////////////////////////////////////////////////////
// 현재 시트 이름 설정
// SetName(시트 번호, 시트 이름)

void CExcelAutomation::SetName(short SheetNumber, LPCTSTR SheetName)
{
	m_sheet = m_sheets.GetItem(COleVariant(SheetNumber));
	m_sheet.SetName(SheetName);
}

////////////////////////////////////////////////////////////////
// 값 설정
// SetValue(시작셀번호, 마지막셀번호, 값)

void CExcelAutomation::SetValue(LPCTSTR CellStart, 
								LPCTSTR CellEnd, 
								LPCTSTR CellValue)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_range.SetValue2(COleVariant(CellValue));
}

////////////////////////////////////////////////////////////////
// 값 설정
// SetValue(셀번호, 값)

void CExcelAutomation::SetValue(CString CellStart, //LPCTSTR CellStart, 
								LPCTSTR CellValue)
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if( chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(char)*10, _T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));

		chTemp = CellStart.GetAt(1);
		if( chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(char)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_range.SetValue2(COleVariant(CellValue));
}

//void SetValue(CString CellStart, int nValue)
//{
//	CString strValue;
//
//	strValue.Format(_T("%d"), nValue);
//
//	SetValue(CellStart, (LPTSTR)(LPCTSTR)strValue);
//}
//
//void SetValue(CString CellStart, double	dValue)
//{
//
//	CString strValue;
//
//	strValue.Format(_T("%f"), dValue);
//
//	SetValue(CellStart, (LPTSTR)(LPCTSTR)strValue);
//}

////////////////////////////////////////////////////////////////
// 값 읽기
// GetValue(시작셀번호, 마지막셀번호)

CString CExcelAutomation::GetValue(	LPCTSTR CellStart, 
									LPCTSTR CellEnd)
{
	CString returnValue=_T("");
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	VARIANT CellValue = m_range.GetValue2();
	switch(CellValue.vt)
	{
		case VT_R8:
			returnValue.Format(_T("%.4f"), CellValue.dblVal);
			break;
		case VT_BSTR:
			returnValue = (CString)CellValue.bstrVal;
			break;
		case VT_EMPTY:
			returnValue = _T("");
			break;
	}
	return returnValue;
}

////////////////////////////////////////////////////////////////
// 값 읽기
// GetValue(셀번호)

CString CExcelAutomation::GetValue(LPCTSTR CellStart)
{
	CString returnValue=_T("");
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	VARIANT CellValue = m_range.GetValue2();
	switch(CellValue.vt)
	{
	case VT_R8:
		returnValue.Format(_T("%.4f"), CellValue.dblVal);
		break;
	case VT_BSTR:
		returnValue = (CString)CellValue.bstrVal;
		break;
	case VT_EMPTY:
		returnValue = _T("");
		break;
	}
	return returnValue;
}

////////////////////////////////////////////////////////////////
// Work Sheet 보여주기
// ShowWorkSheet(TRUE or FALSE)

void CExcelAutomation::ShowWorkSheet(BOOL show)
{
	m_bShow = show;
	m_app.SetVisible(m_bShow);
}

////////////////////////////////////////////////////////////////
// Column 맞추기
// AutoFitColumn(시작셀번호, 마지막셀번호)

void CExcelAutomation::AutoFitColumn(LPCTSTR CellStart, LPCTSTR CellEnd)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart),COleVariant(CellEnd));
	m_range.Select();
	m_range = m_range.GetEntireColumn();		
	m_range.AutoFit();
}

////////////////////////////////////////////////////////////////
// Row 맞추기
// AutoFitRow(시작셀번호, 마지막셀번호)

void CExcelAutomation::AutoFitRow(LPCTSTR CellStart, LPCTSTR CellEnd)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart),COleVariant(CellEnd));
	m_range.Select();
	m_range = m_range.GetEntireRow();		
	m_range.AutoFit();
}

////////////////////////////////////////////////////////////////
// 높이 맞추기
// SetHeight(시작셀번호, 마지막셀번호, 높이)

void CExcelAutomation::SetHeight(LPCTSTR CellStart, LPCTSTR CellEnd, short height)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_range.SetRowHeight(COleVariant(height));
}

////////////////////////////////////////////////////////////////
// 높이 맞추기
// SetHeight(셀번호, 높이)

void CExcelAutomation::SetHeight(CString CellStart, short height)
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);
	if( chTemp > 'Z')
	{
		_stprintf_s(temp,sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp,sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_range.SetRowHeight(COleVariant(height));
}

////////////////////////////////////////////////////////////////
// 넓이 맞추기
// SetWidth(시작셀번호, 마지막셀번호, 넓이)

void CExcelAutomation::SetWidth(LPCTSTR CellStart, LPCTSTR CellEnd, short width)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_range.SetColumnWidth(COleVariant(width));
}

////////////////////////////////////////////////////////////////
// 넓이 맞추기
// SetWidth(셀번호, 넓이)

void CExcelAutomation::SetWidth(LPCTSTR CellStart, short width)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_range.SetColumnWidth(COleVariant(width));
}

////////////////////////////////////////////////////////////////
// 셀 합치기
// MergeCells(시작셀번호, 마지막셀번호)

void CExcelAutomation::MergeCells(
								  CString CellStart, //LPCSTR CellStart, 
								  CString CellEnd) //LPCSTR CellEnd)
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);
	
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}
	
	m_range = m_sheet.GetRange(COleVariant(CellStart),COleVariant(CellEnd));
	
	m_range.SetMergeCells(COleVariant((short)1)); 
	
}

////////////////////////////////////////////////////////////////
// 테이블 그리기
// DrawTable(시작셀번호, 마지막셀번호, 라인타입)

void CExcelAutomation::DrawTable(CString CellStart, //LPCSTR CellStart, 
								 CString CellEnd, //LPCSTR CellEnd, 
								 short LineType)
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}
	
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	
	m_borders = m_range.GetBorders();
	
	m_borders.SetLineStyle(COleVariant(LineType));
}

////////////////////////////////////////////////////////////////
// 그림 삽입
// SetPicture(파일명, x좌표, y좌표, 넓이, 높이, True or False)

void CExcelAutomation::SetPicture(LPCTSTR FileName, float x, float y, float width, float height, BOOL ErrMsg)
{
	m_shapes = m_sheet.GetShapes();
	// 영상 파일이 없는 경우 Error Message와 함께 종료되므로
	// 종료되는 것을 막기 위해 파일이 없는 경우 Error Message를 출력 후 계속 진행한다.
	// ErrMsg가 FALSE인 경우에는 이 함수를 호출해주는 부분에서 예외처리를 해야 한다.
	if(ErrMsg == TRUE)
	{
		try
		{
			m_shape = m_shapes.AddPicture(FileName, 0, 1, x, y, width, height);
		}
		catch(...)
		{
			CString str=_T("");
			str.Format(_T("%s 파일이 존재하지 않습니다."), FileName);
//			AfxMessageBox(str);
		}
	}
	else
		m_shape = m_shapes.AddPicture(FileName, 0, 1, x, y, width, height);
}

////////////////////////////////////////////////////////////////
// 셀 색 설정
// SetColor(시작셀번호, 마지막셀번호, 색)

void CExcelAutomation::SetColor(CString CellStart, //LPCSTR CellStart, 
								CString CellEnd, //LPCSTR CellEnd, 
								COLORREF BgColor)
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10,_T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}
	
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_interior = m_range.GetInterior();
	m_interior.SetColor(COleVariant((double)BgColor));
}

////////////////////////////////////////////////////////////////
// 셀 색 설정
// SetColor(셀번호, 색)

void CExcelAutomation::SetColor(LPCTSTR CellStart, COLORREF BgColor)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_interior = m_range.GetInterior();
	m_interior.SetColor(COleVariant((double)BgColor));
}

////////////////////////////////////////////////////////////////
// 가운데 정렬
// SetAlignCenter(시작셀번호, 마지막셀번호)

void CExcelAutomation::SetAlignCenter(LPCTSTR CellStart, LPCTSTR CellEnd)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_range.SetVerticalAlignment(COleVariant((short)-4108));   //xlVAlignCenter = -4108
	m_range.SetHorizontalAlignment(COleVariant((short)-4108));
}

////////////////////////////////////////////////////////////////
// 가운데 정렬
// SetAlignCenter(셀번호)

void CExcelAutomation::SetAlignCenter(LPCTSTR CellStart)
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_range.SetVerticalAlignment(COleVariant((short)-4108));   //xlVAlignCenter = -4108
	m_range.SetHorizontalAlignment(COleVariant((short)-4108));
}

////////////////////////////////////////////////////////////////
// 폰트 설정
// SetFont(시작셀번호, 마지막셀번호)

void CExcelAutomation::SetFont(LPCTSTR CellStart, LPCTSTR CellEnd, int nSize, CString strFont)
{																   
	m_font = m_range.GetFont();
	m_font.put_Size(COleVariant((short)nSize));
	m_font.put_Name(COleVariant(strFont));
}

int CExcelAutomation::AddSheetAtLast()
{
	COleVariant oOleVarOptional(DISP_E_PARAMNOTFOUND,VT_ERROR);	
	m_sheet = m_sheets.GetItem(COleVariant((short)m_sheets.GetCount()));

	VARIANT varAfter = {0};
	varAfter.vt = VT_DISPATCH;
	varAfter.pdispVal = m_sheet.m_lpDispatch;
	m_sheet.m_lpDispatch->AddRef();

	m_sheet = m_sheets.Add(oOleVarOptional,varAfter,oOleVarOptional,oOleVarOptional);
	m_sheet.Activate();
	VariantClear(&varAfter);

	return m_sheet.GetIndex();
}

int CExcelAutomation::GetShttesCount()
{
	return m_sheets.GetCount();
}

CString CExcelAutomation::GetName(short SheetNumber)
{
	m_sheet = m_sheets.GetItem(COleVariant(SheetNumber));
	return m_sheet.GetName();
}

CString CExcelAutomation::GetCellName( int nRow,int nCol )
{
	CString strCol = _T("");
	TCHAR chCol = 0;
	while(1)
	{
		int nVal = (nCol-1) / 26;
		int nMod = nCol % 26;

		if(nMod == 0)
		{
			chCol = 'Z';
		}
		else
		{
			chCol = nMod + 64;
		}

		CString strTmp = strCol;
		strCol.Format(_T("%c%s"),chCol,strTmp);

		nCol = nVal;
		if(nCol == 0)
			break;

		if(nCol <= 26)
		{
			chCol = nCol + 64;
			CString strTmp = strCol;
			strCol.Format(_T("%c%s"),chCol,strTmp);
			break;
		}
	}

	CString strCell;
	strCell.Format(_T("%s%d"),strCol,nRow);
	return strCell;
}

void CExcelAutomation::SetFontColor( /* 폰트 ?설정 */ LPCTSTR CellStart, /* 셀번호 */ COLORREF FontColor )
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_font = m_range.GetFont();
	m_font.put_Color(COleVariant((double)FontColor));
}

void CExcelAutomation::SetFontColor( CString CellStart, /* 시작셀 번호 */ CString CellEnd, /* 마지?셀 번호 */ COLORREF FontColor )
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10,_T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}

	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_font = m_range.GetFont();
	m_font.put_Color(COleVariant((double)FontColor));
}

void CExcelAutomation::SetNumberFormat( LPCTSTR CellStart, LPCTSTR NumberFormat )
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_cell = m_range.GetCells();
	m_cell.put_NumberFormat(COleVariant(NumberFormat));
}

void CExcelAutomation::SetFontBold( /* 폰트 진하게 설정 */ LPCTSTR CellStart )
{
	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellStart));
	m_font = m_range.GetFont();
	m_font.put_Bold(COleVariant((short)1));
}

void CExcelAutomation::SetFontBold( /* 폰트 진하게 설정 */ CString CellStart, /* 시작셀번호 */ CString CellEnd )
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10,_T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}

	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));
	m_font = m_range.GetFont();
	m_font.put_Bold(COleVariant((short)1));

}

void CExcelAutomation::DrawTableOuter( CString CellStart, CString CellEnd /* 표 그리기 */ /* LPCSTR CellStart, // 시작셀번호 */ /* LPCSTR CellEnd, // 마지?셀번호 */ )
{
	TCHAR temp[10];
	ZeroMemory(temp,sizeof(TCHAR)*10);

	TCHAR chTemp = 0;
	chTemp = CellStart.GetAt(0);

	if(chTemp > 'Z')
	{
		_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Left(1));
		CellStart.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(1, 10));
		chTemp = CellStart.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellStart.Mid(1, 1));
			CellStart.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellStart.Mid(2, 10));
		}
	}
	chTemp = CellEnd.GetAt(0);
	if(chTemp > 'Z')
	{
		_stprintf_s(temp, _T("%s"), (LPCTSTR)CellEnd.Left(1));
		CellEnd.Format(_T("A%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(1, 10));
		chTemp = CellEnd.GetAt(1);
		if(chTemp > 'Z')
		{
			_stprintf_s(temp, sizeof(TCHAR)*10, _T("%s"), (LPCTSTR)CellEnd.Mid(1, 1));
			CellEnd.Format(_T("B%c%s"), temp[0] - 'Z' + 'A' - 1, (LPCTSTR)CellEnd.Mid(2, 10));
		}
	}

	m_range = m_sheet.GetRange(COleVariant(CellStart), COleVariant(CellEnd));

	m_borders = m_range.GetBorders();
	
	// 8 top
	m_border = m_borders.GetItem((long)8);
	m_border.SetLineStyle(COleVariant((short)-4119));   //xlDouble = -4119
	m_border.SetWeight(COleVariant((short)4));       //xlThick = 4

	// 9 bottom
	m_border = m_borders.GetItem((long)9);
	m_border.SetLineStyle(COleVariant((short)-4119));   //xlDouble = -4119
	m_border.SetWeight(COleVariant((short)4));       //xlThick = 4

	// 7 left
	m_border = m_borders.GetItem((long)7);
	m_border.SetLineStyle(COleVariant((short)-4119));   //xlDouble = -4119
	m_border.SetWeight(COleVariant((short)4));       //xlThick = 4

	// 10 right
	m_border = m_borders.GetItem((long)10);
	m_border.SetLineStyle(COleVariant((short)-4119));   //xlDouble = -4119
	m_border.SetWeight(COleVariant((short)4));       //xlThick = 4


}
