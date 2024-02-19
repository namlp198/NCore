#include "pch.h"
#include "ImageView_Default.h"
#include "BufferDC.h"

IMPLEMENT_DYNAMIC(CImageView_Default, CWnd)

CImageView_Default::CImageView_Default(int nViewIndex, CWnd* pParentWnd) : m_nViewIndex(nViewIndex), m_pParentWnd(pParentWnd)
{
	m_strViewName = _T("");

	m_nHScroll = 0;
	m_nVScroll = 0;

	m_nMaxHScroll = 0;
	m_nMaxVScroll = 0;
	m_nViewMode = 1;

	m_rtViewRect = CRect(0, 0, 0, 0);

	m_bDrawViewName = FALSE;
	m_bDrawCenterLine = FALSE;
	m_bDrawPixelInfo = FALSE;

	m_dZoomRatio = ZOOM_INIT_RATIO;
	m_dWidthScale = 1.0;
	m_dHeightScale = 1.0;
	m_nScaleWidth = 0;
	m_nScaleHeight = 0;
	m_dResolutionX = 1.0;
	m_dResolutionY = 1.0;

	m_bDrawUserSelect = FALSE;
	m_rtUserSelect_pxl = CRect(0, 0, 0, 0);
	m_nUserSelect_Modify_type = -1;
}

CImageView_Default::~CImageView_Default()
{
}

BEGIN_MESSAGE_MAP(CImageView_Default, CWnd)
	ON_COMMAND(IDR_SAVE_IMAGE, &CImageView_Default::OnPopUpCommand_SaveImage)
	ON_COMMAND(IDR_VIEW_ORIGIN, &CImageView_Default::OnPopUpCommand_ViewZoomMode)
	ON_COMMAND(IDR_VIEW_FIT, &CImageView_Default::OnPopUpCommand_ViewFitMode)
	ON_COMMAND(IDR_VIEW_NAME, &CImageView_Default::OnPopUpCommand_DrawViewName)
	ON_COMMAND(IDR_CENTER_LINE, &CImageView_Default::OnPopUpCommand_DrawCenterLine)
	ON_COMMAND(IDR_VIEW_PIXEL_INFO, &CImageView_Default::OnPopUpCommand_DrawPixelInfo)
	ON_COMMAND(IDR_USER_SELECT, &CImageView_Default::OnPopUpCommand_DrawUserSelectMeasure)
	ON_WM_PAINT()
	ON_WM_RBUTTONDOWN()
	ON_WM_RBUTTONUP()
	ON_WM_RBUTTONDBLCLK()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_LBUTTONDBLCLK()
	ON_WM_MOUSEMOVE()
	ON_WM_VSCROLL()
	ON_WM_HSCROLL()
	ON_WM_MOUSEWHEEL()
END_MESSAGE_MAP()

void CImageView_Default::PopUpCommandMenu(const CPoint& point)
{
	CMenu menu;
	CMenu popMenu;
	menu.CreateMenu();			// 최상위 뼈대 메뉴
	popMenu.CreatePopupMenu();  // 팝업메뉴
	BOOL bAddMenu = FALSE;

	// popMenu.AppendMenu(MF_STRING, IDR_LOAD_IMAGE, _T("Load Image"));
	popMenu.AppendMenu(MF_STRING, IDR_SAVE_IMAGE, _T("Save Image"));
	popMenu.AppendMenu(MF_SEPARATOR);
	popMenu.AppendMenu(MF_STRING, IDR_VIEW_ORIGIN, _T("Zoom View"));
	popMenu.AppendMenu(MF_STRING, IDR_VIEW_FIT, _T("Fit View"));
	popMenu.AppendMenu(MF_SEPARATOR);
	popMenu.AppendMenu(MF_STRING, IDR_CENTER_LINE, _T("Center Line"));
	popMenu.AppendMenu(MF_STRING, IDR_VIEW_PIXEL_INFO, _T("Pixel Info"));
	popMenu.AppendMenu(MF_SEPARATOR);
	popMenu.AppendMenu(MF_STRING, IDR_USER_SELECT, _T("Select/Measure Region"));

	if (m_nViewMode == 0)						popMenu.CheckMenuItem(IDR_VIEW_ORIGIN, MF_CHECKED);
	else if (m_nViewMode == 1)					popMenu.CheckMenuItem(IDR_VIEW_FIT, MF_CHECKED);

	if (m_bDrawCenterLine == TRUE)				popMenu.CheckMenuItem(IDR_CENTER_LINE, MF_CHECKED);
	if (m_bDrawPixelInfo == TRUE)				popMenu.CheckMenuItem(IDR_VIEW_PIXEL_INFO, MF_CHECKED);
	if (m_bDrawUserSelect == TRUE)				popMenu.CheckMenuItem(IDR_USER_SELECT, MF_CHECKED);

	// 컨텍스트 메뉴 호출
	CRect rect;
	GetWindowRect(rect);
	int nX = rect.left + point.x;
	int nY = rect.top + point.y;

	popMenu.TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON, nX, nY, this);
}

void CImageView_Default::DrawCenterLine(CDC* pDC)
{
	if (m_bDrawCenterLine == FALSE)
		return;

	if (GetAllocatedData() == FALSE)
		return;

	CRect rtCamArea(0, 0, GetWidth(), GetHeight());

	CRect rtDraw = CalcuateImagePosToViewWndPos(rtCamArea);

	CPen pen, * pOldPen = NULL;
	pen.CreatePen(PS_DOT, 1, RGB(255, 0, 0));
	pOldPen = (CPen*)pDC->SelectObject(&pen);
	CBrush* pOldBrush = (CBrush*)pDC->SelectStockObject(NULL_BRUSH);
	int nOldROP = pDC->SetROP2(R2_COPYPEN);

	int nOldBk = pDC->SetBkMode(TRANSPARENT);

	if(m_nViewMode != 1)
	{
		// center line
		pDC->MoveTo((rtDraw.Width() / 2) - m_nHScroll, 0);
		pDC->LineTo((rtDraw.Width() / 2) - m_nHScroll, rtDraw.Height());
		pDC->MoveTo(0, (rtDraw.Height() / 2) - m_nVScroll);
		pDC->LineTo(rtDraw.Width(), (rtDraw.Height() / 2) - m_nVScroll);
	}
	else
	{
		// center line
		pDC->MoveTo((rtDraw.Width() / 2), 0);
		pDC->LineTo((rtDraw.Width() / 2), rtDraw.Height());
		pDC->MoveTo(0, (rtDraw.Height() / 2));
		pDC->LineTo(rtDraw.Width(), (rtDraw.Height() / 2));
	}

	pDC->SetROP2(nOldROP);
	if (pOldPen != NULL) pDC->SelectObject(pOldPen);
	if (pOldBrush != NULL) pDC->SelectObject(pOldBrush);

	pDC->SetBkMode(nOldBk);
}

void CImageView_Default::ControlZoom(BOOL bZoomIn, CPoint ptMouse)
{
	CPoint ptCurPos(ptMouse);
	ScreenToClient(&ptCurPos);

	double dNewZoomRatio;

	if (bZoomIn == FALSE)
	{
		dNewZoomRatio = m_dZoomRatio * ZOOM_STEP_RATIO;

		if (dNewZoomRatio > ZOOM_MAX_RATIO)
		{
			dNewZoomRatio = ZOOM_MAX_RATIO;
			return;
		}
	}
	else
	{
		dNewZoomRatio = m_dZoomRatio / ZOOM_STEP_RATIO;

		if (dNewZoomRatio < ZOOM_MIN_RATIO)
		{
			dNewZoomRatio = ZOOM_MIN_RATIO;
			return;
		}
	}

	ChangeViewScale(0, dNewZoomRatio, ptCurPos);

	Invalidate(FALSE);
}

BOOL CImageView_Default::GetSubROIRect(CRect& rtRect)
{
	rtRect = m_rtUserSelect_pxl;

	return TRUE;
}

void CImageView_Default::DrawDC(CDC* pDC)
{
	CRect rtViewRect;
	GetClientRect(rtViewRect);

	UpdateView(pDC);

	DrawPixelInfo(pDC);
	DrawUserSelectMeasure(pDC);
	DrawCenterLine(pDC);
}

void CImageView_Default::SaveDC(CString strFilePath)
{
	CBitmap Bitmap, * pOldBmp;

	CRect BitmapSize;

	GetClientRect(BitmapSize);

	CLSID                encoderClsid;

	USES_CONVERSION;
	// 인코더 설정
	const WCHAR* wcMime = _T("image/jpeg");

	unsigned int  num = 0;          // number of image encoders
	unsigned int  size = 0;         // size of the image encoder array in bytes

	ImageCodecInfo* pImageCodecInfo = NULL;
	GetImageEncodersSize(&num, &size);

	if (size == 0)
		return;  // Failure

	pImageCodecInfo = (ImageCodecInfo*)(malloc(size));

	if (pImageCodecInfo == NULL)
		return;  // Failure

	GetImageEncoders(num, size, pImageCodecInfo);

	BOOL bSuccess = FALSE;

	for (UINT j = 0; j < num; ++j)
	{
		if (wcscmp(pImageCodecInfo[j].MimeType, wcMime) == 0)
		{
			encoderClsid = pImageCodecInfo[j].Clsid;
			free(pImageCodecInfo);
			bSuccess = TRUE;
			break;  // Success
		}
	}

	if (bSuccess == FALSE)
		return;

	EncoderParameters encoderParameters;
	ULONG             Quality = 100;
	encoderParameters.Count = 1;
	encoderParameters.Parameter[0].Guid = EncoderQuality;
	encoderParameters.Parameter[0].Type = EncoderParameterValueTypeLong;
	encoderParameters.Parameter[0].NumberOfValues = 1;
	encoderParameters.Parameter[0].Value = &Quality;

	//----- CDC의 내용을 Bitmap으로 전송 ----//
	CBufferDC* pDC = new CBufferDC(this); // device context for painting

	Bitmap.CreateCompatibleBitmap(pDC, BitmapSize.Width(), BitmapSize.Height());
	pOldBmp = (CBitmap*)pDC->SelectObject(&Bitmap);

	DrawDC(pDC);

	HPALETTE hPalette = (HPALETTE)GetCurrentObject(pDC->GetSafeHdc(), OBJ_PAL);

	Gdiplus::Bitmap* pBitmap;
	pBitmap = Gdiplus::Bitmap::FromHBITMAP((HBITMAP)Bitmap.GetSafeHandle(), hPalette);

	if (NULL == pBitmap)
		return;

	Gdiplus::Status  status = pBitmap->Save(strFilePath, &encoderClsid, &encoderParameters);

	DeleteObject(hPalette);
	DeleteObject(Bitmap);

	delete pBitmap; pBitmap = NULL;

	delete pDC;
}

// CZoomView 메시지 처리기입니다.
void CImageView_Default::OnPaint()
{
	CBufferDC* pDC = new CBufferDC(this); // device context for painting

	DrawDC(pDC);

	delete pDC;
}

BOOL CImageView_Default::SetViewImage(int nWidth, int nHeight, int nChannels, int nWidthStep, const BYTE* pBuffer)
{
	if (nWidth < 0 || nHeight < 0 || nChannels < 0 || pBuffer == NULL) return FALSE;

	CSingleLock localLock(&m_csImageData);
	localLock.Lock();


	if (nWidth != GetWidth() || nHeight != GetHeight() || nChannels != GetChannels())
	{
		if (CreateImage(nWidth, nHeight, 8, nChannels) == FALSE)
		{
			localLock.Unlock();
			return FALSE;
		}
	}

	memcpy(GetImageBuffer(), pBuffer, nWidthStep * nHeight);

	localLock.Unlock();

	return TRUE;
}

BOOL CImageView_Default::CopyImageFrom(const CImageBuffer* pFromImage)
{
	CSingleLock localLock(&m_csImageData);

	return CImageBuffer::CopyImageFrom(pFromImage);
}

BOOL CImageView_Default::GetBandImage(int nColorBand, CImageBuffer* pImageData)
{
	if (pImageData == NULL) return FALSE;

	CSingleLock localLock(&m_csImageData);
	localLock.Lock();

	BOOL bReturn = CImageBuffer::GetBandImage(nColorBand, pImageData);

	localLock.Unlock();

	return bReturn;
}

BOOL CImageView_Default::SetViewImage(CImageBuffer* pImageData)
{
	if (pImageData == NULL) return FALSE;

	if (!pImageData->GetImageExist()) return FALSE;

	CSingleLock localLock(&m_csImageData);
	localLock.Lock();

	this->CopyImageFrom(pImageData);

	// Image Size에 맞춰 View Mode 다시 계산
	SetViewMode(m_nViewMode);

	this->Invalidate(TRUE);

	return TRUE;
}

BOOL CImageView_Default::GetViewImage(CImageBuffer* pImageData)
{
	if (pImageData == NULL) return FALSE;

	CSingleLock localLock(&m_csImageData);
	localLock.Lock();

	BOOL bReturn = CImageBuffer::CopyImageTo(pImageData);

	localLock.Unlock();

	return bReturn;
}

BOOL CImageView_Default::LoadImage(const CString& strFilename)
{
	CSingleLock localLock(&m_csImageData);
	localLock.Lock();

	SetFocus();

	if (CImageBuffer::LoadImage(strFilename) == FALSE)
	{
		localLock.Unlock();

		m_nHScroll = m_nVScroll = 0;
		m_nMaxHScroll = m_nMaxVScroll = 0;
		SetScrollRange(SB_HORZ, 0, m_nMaxHScroll);
		SetScrollRange(SB_VERT, 0, m_nMaxVScroll);
		SetScrollPos(SB_HORZ, m_nHScroll);
		SetScrollPos(SB_VERT, m_nVScroll);
		return FALSE;
	}

	localLock.Unlock();

	// Image Size에 맞춰 View Mode 다시 계산
	SetViewMode(m_nViewMode);

	//ChangeViewScale(m_nViewMode, ZOOM_INIT_RATIO, CPoint(0,0));

	Invalidate(TRUE);

	return TRUE;
}

void CImageView_Default::UpdateView(CDC* pDC)
{
	CSingleLock localLock(&m_csImageData);
	localLock.Lock();

	CRect rtCurrentRect, rtSourceRect;
	GetClientRect(rtCurrentRect);

	if (GetImageExist())
	{
		if (m_nViewMode == 0)
		{
			CRect rtImageRegion(m_nHScroll / m_dZoomRatio,
				GetHeight() - (m_nVScroll / m_dZoomRatio) - (rtCurrentRect.Height() / m_dZoomRatio),
				(m_nHScroll / m_dZoomRatio) + (rtCurrentRect.Width() / m_dZoomRatio),
				GetHeight() - (m_nVScroll / m_dZoomRatio));

			pDC->SelectStockObject(GRAY_BRUSH);
			pDC->Rectangle(rtCurrentRect);

			ShowImage(pDC->m_hDC, rtImageRegion, rtCurrentRect);
		}
		else
		{
			ShowImage(pDC->m_hDC, rtCurrentRect);
		}

		pDC->SelectStockObject(NULL_BRUSH);
		pDC->Rectangle(rtCurrentRect);
	}
	else
	{
		pDC->SelectStockObject(GRAY_BRUSH);
		pDC->Rectangle(rtCurrentRect);
	}

	localLock.Unlock();
}

int	CImageView_Default::GetScaleWidth()
{
	if (GetImageExist() == FALSE) return 0;

	CRect rect;
	int nWidth = GetWidth();

	GetClientRect(rect);

	switch (m_nViewMode)
	{
	case 0:
		if (nWidth != 0)
			m_dWidthScale = (double(rect.Width()) / double(nWidth)) * m_dZoomRatio;
		return int(double(nWidth) * m_dWidthScale + 0.5);

	case 1:
		if (nWidth != 0)
			m_dWidthScale = double(rect.Width()) / double(nWidth);
		return int(double(nWidth) * m_dWidthScale + 0.5);
	}

	return 0;
}

int	CImageView_Default::GetScaleHeight()
{
	if (GetImageExist() == FALSE) return 0;

	CRect rect;
	int nHeight = GetHeight();

	GetClientRect(rect);

	switch (m_nViewMode)
	{
	case 0:
		if (nHeight != 0)
			m_dHeightScale = (double(rect.Height()) / double(nHeight)) * m_dZoomRatio;
		return int(double(nHeight) * m_dHeightScale + 0.5);

	case 1:
		if (nHeight != 0)
			m_dHeightScale = double(rect.Height()) / double(nHeight);
		return int(double(nHeight) * m_dHeightScale + 0.5);
	}

	return 0;
}

void CImageView_Default::SetViewMode(int nViewMode)
{
	m_nViewMode = nViewMode;

	GetClientRect(m_rtViewRect);

	GetScaleWidth();
	GetScaleHeight();

	return;

	if (m_nViewMode == 1)
	{
		m_nMaxHScroll = 0;
		m_nMaxVScroll = 0;
		m_nHScroll = 0;
		m_nVScroll = 0;
	}
	else
	{
		if (m_rtViewRect.Width() >= m_nScaleWidth)
		{
			m_nMaxHScroll = 0;
		}
		else
		{
			//m_nMaxHScroll = m_nScaleWidth - (m_rtViewRect.Width()*m_nScaleWidth+17);
			//m_nMaxHScroll = GetWidth()*m_dZoomRatio - (m_nScaleWidth+17);
			m_nMaxHScroll = (GetWidth() * m_dWidthScale) - ((m_rtViewRect.Width() * m_dWidthScale) + 17);

			if (m_nMaxHScroll < 0)
			{
				m_nMaxHScroll = 0;
			}
			else
			{
				m_nMaxHScroll += 17;
			}
		}

		if (m_rtViewRect.Height() >= m_nScaleHeight)
		{
			m_nMaxVScroll = 0;
		}
		else
		{
			//m_nMaxVScroll = m_nScaleHeight - (m_rtViewRect.Height()*m_nScaleHeight+17);
			//m_nMaxVScroll = GetHeight()*m_dZoomRatio - (m_nScaleHeight+17);
			m_nMaxVScroll = (GetHeight() * m_dHeightScale) - ((m_rtViewRect.Height() * m_dHeightScale) + 17);

			if (m_nMaxVScroll < 0)
			{
				m_nMaxVScroll = 0;
			}
			else
			{
				m_nMaxVScroll += 17;
			}
		}
	}

	SetScrollRange(SB_HORZ, 0, m_nMaxHScroll);
	SetScrollRange(SB_VERT, 0, m_nMaxVScroll);
	SetScrollPos(SB_HORZ, m_nHScroll);
	SetScrollPos(SB_VERT, m_nVScroll);
}


void CImageView_Default::ChangeViewScale(int nViewMode, double dNewZoomRatio, CPoint ptMouseFocus)
{
	m_nViewMode = nViewMode;

	CRect rtWindow;

	GetClientRect(m_rtViewRect);

	if (m_nViewMode == 0)	// 1:1 Origin View..
	{
		double dPreZoomRatio = m_dZoomRatio;

		m_dZoomRatio = dNewZoomRatio;

		// Scale
		m_dWidthScale = 1 / m_dZoomRatio;
		m_dHeightScale = 1 / m_dZoomRatio;

		// View Scale
		m_nScaleWidth = int(m_dWidthScale * double(m_rtViewRect.Width()) + 0.5);
		m_nScaleHeight = int(m_dHeightScale * double(m_rtViewRect.Height()) + 0.5);


		if (m_rtViewRect.Width() * m_dWidthScale >= GetWidth())
		{
			m_nMaxHScroll = 0;
		}
		else
		{
			m_nMaxHScroll = GetWidth() * m_dZoomRatio - m_rtViewRect.Width() - 17;

			if (m_nMaxHScroll < 0) 			m_nMaxHScroll = 0;
			else							m_nMaxHScroll += 17;
		}

		if (m_rtViewRect.Height() * m_dHeightScale >= GetHeight())
		{
			m_nMaxVScroll = 0;
		}
		else
		{
			m_nMaxVScroll = GetHeight() * m_dZoomRatio - m_rtViewRect.Height() - 17;

			if (m_nMaxVScroll < 0) 			m_nMaxVScroll = 0;
			else							m_nMaxVScroll += 17;
		}

		SetScrollRange(SB_HORZ, 0, m_nMaxHScroll);
		SetScrollRange(SB_VERT, 0, m_nMaxVScroll);

		CPoint ptMouseFocusReal = ptMouseFocus;
		ptMouseFocusReal.x = (m_nHScroll + ptMouseFocus.x) / dPreZoomRatio;
		ptMouseFocusReal.y = (m_nVScroll + ptMouseFocus.y) / dPreZoomRatio;

		m_nHScroll = (ptMouseFocusReal.x * dNewZoomRatio) - ptMouseFocus.x; //(m_rtViewRect.Width()/2);
		m_nVScroll = (ptMouseFocusReal.y * dNewZoomRatio) - ptMouseFocus.y; //(m_rtViewRect.Height()/2);

		m_nHScroll = (m_nHScroll <= 0) ? 0 : m_nHScroll;
		m_nVScroll = (m_nVScroll <= 0) ? 0 : m_nVScroll;

		m_nHScroll = (m_nMaxHScroll <= m_nHScroll) ? m_nMaxHScroll : m_nHScroll;
		m_nVScroll = (m_nMaxVScroll <= m_nVScroll) ? m_nMaxVScroll : m_nVScroll;

		SetScrollPos(SB_HORZ, m_nHScroll);
		SetScrollPos(SB_VERT, m_nVScroll);
	}
	else	// Fit View..
	{
		// Scale
		m_dWidthScale = double(GetWidth()) / double(m_rtViewRect.Width());
		m_dHeightScale = double(GetHeight()) / double(m_rtViewRect.Height());

		// View Scale
		m_nScaleWidth = int(m_dWidthScale * double(m_rtViewRect.Width()) + 0.5);
		m_nScaleHeight = int(m_dHeightScale * double(m_rtViewRect.Height()) + 0.5);

		SetScrollRange(SB_HORZ, 0, 0);
		SetScrollRange(SB_VERT, 0, 0);
		SetScrollPos(SB_HORZ, 0);
		SetScrollPos(SB_VERT, 0);
	}
}

void CImageView_Default::OnPopUpCommand_SaveImage()
{
	CString szFilter = _T("BMP(*.bmp)|*.bmp| JPG(*.jpg)|*.jpg| All Files(*.*)|*.*||");

	CString strPath;
	CFileDialog dlg(FALSE, szFilter, NULL, OFN_HIDEREADONLY, szFilter);
	dlg.m_ofn.lpstrTitle = _T("Save Image");

	if (dlg.DoModal() == IDOK)
	{
		CSingleLock localLock(&m_csImageData);
		localLock.Lock();

		if (SaveImage(dlg.GetPathName()))
		{

		}

		localLock.Unlock();
	}
}

void CImageView_Default::OnPopUpCommand_DrawViewName()
{
	m_bDrawViewName = !m_bDrawViewName;
	Invalidate(TRUE);
}

void CImageView_Default::OnPopUpCommand_DrawCenterLine()
{
	m_bDrawCenterLine = !m_bDrawCenterLine;
	Invalidate(TRUE);
}

void CImageView_Default::OnPopUpCommand_DrawPixelInfo()
{
	m_bDrawPixelInfo = !m_bDrawPixelInfo;
	Invalidate(TRUE);
}

void CImageView_Default::OnPopUpCommand_DrawUserSelectMeasure()
{
	m_bDrawUserSelect = !m_bDrawUserSelect;

	m_rtUserSelect_pxl = CRect(-1, -1, -1, -1);

	Invalidate(TRUE);
}

void CImageView_Default::OnPopUpCommand_ViewZoomMode()
{
	m_nViewMode = 0;

	SetViewMode(m_nViewMode);
	ChangeViewScale(0, ZOOM_INIT_RATIO, CPoint(0, 0));
	Invalidate(TRUE);
}

void CImageView_Default::OnPopUpCommand_ViewFitMode()
{
	m_nViewMode = 1;

	SetViewMode(m_nViewMode);
	ChangeViewScale(1, ZOOM_INIT_RATIO, CPoint(0, 0));
	Invalidate(TRUE);
}

void CImageView_Default::OnRButtonDown(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL) return;

	CRect rect;
	this->GetClientRect(rect);

	point.x += m_nHScroll;
	point.y += m_nVScroll;
	if (point.x > 0 && point.x < this->GetScaleWidth() - 1 &&
		point.y > 0 && point.y < this->GetScaleHeight() - 1)
	{
		m_pParentWnd->SendMessage(WM_RBUTTONDOWN, static_cast<WPARAM>(nFlags), MAKELPARAM(point.x, point.y));
	}

	__super::OnRButtonDown(nFlags, point);
}

void CImageView_Default::OnRButtonUp(UINT nFlags, CPoint point)
{
	PopUpCommandMenu(point);

	if (m_pParentWnd == NULL) return;

	CRect rect;
	this->GetClientRect(rect);

	point.x += m_nHScroll;
	point.y += m_nVScroll;

	if (point.x > 0 && point.x < this->GetScaleWidth() - 1 &&
		point.y > 0 && point.y < this->GetScaleHeight() - 1)
	{
		m_pParentWnd->SendMessage(WM_RBUTTONUP, static_cast<WPARAM>(nFlags), MAKELPARAM(point.x, point.y));
	}

	__super::OnRButtonUp(nFlags, point);
}


void CImageView_Default::OnRButtonDblClk(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL) return;

	CRect rect;
	this->GetClientRect(rect);

	point.x += m_nHScroll;
	point.y += m_nVScroll;
	if (point.x > 0 && point.x < this->GetScaleWidth() - 1 &&
		point.y > 0 && point.y < this->GetScaleHeight() - 1)
	{
		m_pParentWnd->SendMessage(WM_RBUTTONDBLCLK, static_cast<WPARAM>(nFlags), MAKELPARAM(point.x, point.y));
	}

	__super::OnRButtonDblClk(nFlags, point);
}


void CImageView_Default::OnLButtonDown(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL) return;

	CheckModifyUserSelect(point);

	__super::OnLButtonDown(nFlags, point);
}

void CImageView_Default::CheckModifyUserSelect(CPoint ptMousePos_wnd)
{
	CPoint ptMousePos_pxl = CalcuateWndPosToImagePos(ptMousePos_wnd);

	CRect rtModifyPoint_pxl[9];

	int nPointIdx = 0;

	CPoint ptUserSelect_Center_pxl = m_rtUserSelect_pxl.CenterPoint();

	int nHitRange = (m_nViewMode == 0) ? 10 : 30;

	// Top
	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.left, m_rtUserSelect_pxl.top, m_rtUserSelect_pxl.left, m_rtUserSelect_pxl.top);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(ptUserSelect_Center_pxl.x, m_rtUserSelect_pxl.top, ptUserSelect_Center_pxl.x, m_rtUserSelect_pxl.top);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.right, m_rtUserSelect_pxl.top, m_rtUserSelect_pxl.right, m_rtUserSelect_pxl.top);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	// Center
	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.left, ptUserSelect_Center_pxl.y, m_rtUserSelect_pxl.left, ptUserSelect_Center_pxl.y);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(ptUserSelect_Center_pxl.x, ptUserSelect_Center_pxl.y, ptUserSelect_Center_pxl.x, ptUserSelect_Center_pxl.y);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.right, ptUserSelect_Center_pxl.y, m_rtUserSelect_pxl.right, ptUserSelect_Center_pxl.y);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	// Bottom
	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.left, m_rtUserSelect_pxl.bottom, m_rtUserSelect_pxl.left, m_rtUserSelect_pxl.bottom);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(ptUserSelect_Center_pxl.x, m_rtUserSelect_pxl.bottom, ptUserSelect_Center_pxl.x, m_rtUserSelect_pxl.bottom);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	rtModifyPoint_pxl[nPointIdx].SetRect(m_rtUserSelect_pxl.right, m_rtUserSelect_pxl.bottom, m_rtUserSelect_pxl.right, m_rtUserSelect_pxl.bottom);
	rtModifyPoint_pxl[nPointIdx++].InflateRect(USER_SELECT_HIT_AREA + nHitRange, USER_SELECT_HIT_AREA + nHitRange);

	m_nUserSelect_Modify_type = -1;

	for(int i=0; i<9; i++)
	{
		if(rtModifyPoint_pxl[i].PtInRect(ptMousePos_pxl))
		{
			m_nUserSelect_Modify_type = i;
			break;
		}
	}

	if(m_nUserSelect_Modify_type == -1)
	{
		m_rtUserSelect_pxl.left = ptMousePos_pxl.x;
		m_rtUserSelect_pxl.top = ptMousePos_pxl.y;
		m_rtUserSelect_pxl.right = ptMousePos_pxl.x;
		m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y;
	}
}

void CImageView_Default::OnLButtonUp(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL) return;

	m_rtUserSelect_pxl.NormalizeRect();

	CRect rtTemp = m_rtUserSelect_pxl;
	IntersectRect(m_rtUserSelect_pxl, rtTemp, CRect(0, 0, GetWidth() - 1, GetHeight() - 1));

	m_pParentWnd->SendMessage(WM_LBUTTONUP, static_cast<WPARAM>(nFlags), MAKELPARAM(point.x, point.y));

	__super::OnLButtonUp(nFlags, point);
}


void CImageView_Default::OnLButtonDblClk(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL) return;

	CRect rect;
	this->GetClientRect(rect);

	point.x += m_nHScroll;
	point.y += m_nVScroll;
	if (point.x > 0 && point.x < this->GetScaleWidth() - 1 &&
		point.y > 0 && point.y < this->GetScaleHeight() - 1)
	{
		m_pParentWnd->SendMessage(WM_LBUTTONDBLCLK, static_cast<WPARAM>(nFlags), MAKELPARAM(point.x, point.y));
	}

	__super::OnLButtonDblClk(nFlags, point);
}


void CImageView_Default::OnMouseMove(UINT nFlags, CPoint point)
{
	if (m_pParentWnd == NULL)
		return;

	CRect rtWnd;

	this->GetClientRect(rtWnd);

	if (nFlags & MK_LBUTTON)
	{
		CPoint ptMousePos_pxl = CalcuateWndPosToImagePos(point);

		if (m_nUserSelect_Modify_type == 0)
		{
			m_rtUserSelect_pxl.left = ptMousePos_pxl.x;
			m_rtUserSelect_pxl.top = ptMousePos_pxl.y;
		}
		else if(m_nUserSelect_Modify_type == 1)
		{
			m_rtUserSelect_pxl.top = ptMousePos_pxl.y;
		}
		else if (m_nUserSelect_Modify_type == 2)
		{
			m_rtUserSelect_pxl.right = ptMousePos_pxl.x;
			m_rtUserSelect_pxl.top = ptMousePos_pxl.y;
		}
		else if (m_nUserSelect_Modify_type == 3)
		{
			m_rtUserSelect_pxl.left = ptMousePos_pxl.x;
		}
		else if (m_nUserSelect_Modify_type == 4)
		{
			int nHalf_Width_pxl = m_rtUserSelect_pxl.Width() / 2;
			int nHalf_Height_pxl = m_rtUserSelect_pxl.Height() / 2;

			m_rtUserSelect_pxl.left = ptMousePos_pxl.x - nHalf_Width_pxl;
			m_rtUserSelect_pxl.top = ptMousePos_pxl.y - nHalf_Height_pxl;
			m_rtUserSelect_pxl.right = ptMousePos_pxl.x + nHalf_Width_pxl;
			m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y + nHalf_Height_pxl;
		}
		else if (m_nUserSelect_Modify_type == 5)
		{
			m_rtUserSelect_pxl.right = ptMousePos_pxl.x;
		}
		else if (m_nUserSelect_Modify_type == 6)
		{
			m_rtUserSelect_pxl.left = ptMousePos_pxl.x;
			m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y;
		}
		else if (m_nUserSelect_Modify_type == 7)
		{
			m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y;
		}
		else if (m_nUserSelect_Modify_type == 8)
		{
			m_rtUserSelect_pxl.right = ptMousePos_pxl.x;
			m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y;
		}
		else
		{
			m_rtUserSelect_pxl.right = ptMousePos_pxl.x;
			m_rtUserSelect_pxl.bottom = ptMousePos_pxl.y;
		}
	}

	if (m_nViewMode == 0)
	{
		// Get image point for making zoom window
		m_ptMousePos.x = int(double(point.x) / m_dZoomRatio + 0.5);
		m_ptMousePos.y = int(double(point.y) / m_dZoomRatio + 0.5);

		m_ptMousePos.x += m_nHScroll / m_dZoomRatio;
		m_ptMousePos.y += m_nVScroll / m_dZoomRatio;
		//
	}
	else
	{
		// Get image point for making zoom window
		m_ptMousePos.x = int(double(point.x) / m_dWidthScale);
		m_ptMousePos.y = int(double(point.y) / m_dHeightScale);
	}

	Invalidate(TRUE);

	__super::OnMouseMove(nFlags, point);
}


BOOL CImageView_Default::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
	if (m_nViewMode == 0)	// Only Origin View Mode Zoom..
	{
		if (zDelta < 0)
		{
			ControlZoom(FALSE, pt);
		}
		else if (zDelta > 0)
		{
			ControlZoom(TRUE, pt);
		}
	}

	return CWnd::OnMouseWheel(nFlags, zDelta, pt);// __super::OnMouseWheel(nFlags, zDelta, pt);
}

void CImageView_Default::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	switch (nSBCode)
	{
	case SB_BOTTOM:			// Scroll to bottom. 
		break;
	case SB_ENDSCROLL:		// End scroll.
		break;
	case SB_LINEDOWN:		// Scroll one line down.
		if (m_nVScroll < m_nMaxVScroll)
			m_nVScroll++;
		break;
	case SB_LINEUP:			// Scroll one line up. 
		if (m_nVScroll > 0)
			m_nVScroll--;
		break;
	case SB_PAGEDOWN:		// Scroll one page down. 
		if (5 < m_nMaxVScroll / 256)
			m_nVScroll += 20;//m_nVScroll += 5;
		else
			m_nVScroll += 10;//m_nVScroll += (m_nMaxVScroll / 256);

		if (m_nVScroll > m_nMaxVScroll)
			m_nVScroll = m_nMaxVScroll;
		break;
	case SB_PAGEUP:			// Scroll one page up.
		if (5 < m_nMaxVScroll / 256)
			m_nVScroll -= 20;//m_nVScroll -= 5;
		else
			m_nVScroll -= 10;//m_nVScroll -= (m_nMaxVScroll / 256);

		if (m_nVScroll < 0)
			m_nVScroll = 0;
		break;
	case SB_THUMBPOSITION:	// Scroll to the absolute position. The current position is provided in nPos. 
		break;
	case SB_THUMBTRACK:		// Drag scroll box to specified position. The current position is provided in nPos. 
		m_nVScroll = nPos;
		break;
	case SB_TOP:			// Scroll to top. 
		break;
	}

	// Set the new position of the thumb (scroll box).
	SetScrollPos(SB_VERT, m_nVScroll);

	Invalidate(FALSE);

	__super::OnVScroll(nSBCode, nPos, pScrollBar);
}


void CImageView_Default::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	switch (nSBCode)
	{
	case SB_LEFT:      // Scroll to far left.
		break;
	case SB_RIGHT:      // Scroll to far right.
		break;
	case SB_ENDSCROLL:   // End scroll.
		break;
	case SB_LINELEFT:      // Scroll left.
		if (m_nHScroll > 0)
			m_nHScroll--;
		break;
	case SB_LINERIGHT:   // Scroll right.
		if (m_nHScroll < m_nMaxHScroll)
			m_nHScroll++;
		break;
	case SB_PAGELEFT:    // Scroll one page left.
		if (5 < m_nMaxHScroll / 256)
			m_nHScroll -= 20;//m_nHScroll -= 5;
		else
			m_nHScroll -= 10;//m_nHScroll -= m_nMaxHScroll / 256;

		if (m_nHScroll < 0)
			m_nHScroll = 0;
		break;
	case SB_PAGERIGHT:      // Scroll one page right.
		if (5 < m_nMaxHScroll / 256)
			m_nHScroll += 20;//m_nHScroll += 5;
		else
			m_nHScroll += 10;//m_nHScroll += m_nMaxHScroll / 256;

		if (m_nHScroll > m_nMaxHScroll)
			m_nHScroll = m_nMaxHScroll;
		break;
	case SB_THUMBPOSITION: // Scroll to absolute position. nPos is the position
		break;
	case SB_THUMBTRACK:   // Drag scroll box to specified position. nPos is the
		m_nHScroll = nPos;
		break;
	}

	// Set the new position of the thumb (scroll box).
	SetScrollPos(SB_HORZ, m_nHScroll);

	Invalidate(FALSE);

	__super::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CImageView_Default::DrawZoomInfo(CDC* pDC)
{
	COLORREF textColor = RGB(255, 127, 39);

	int nOldBk = pDC->SetBkMode(TRANSPARENT);
	COLORREF oldTextColor = pDC->SetTextColor(textColor);

	CString strMousePos;
	strMousePos.Format(_T("X : %d, Y : %d, Gray : %d"), m_ptMousePos.x, m_ptMousePos.y, GetPixelValue(m_ptMousePos.x, m_ptMousePos.y));
	pDC->TextOut(10, 10, strMousePos);

	pDC->SetBkMode(nOldBk);
	pDC->SetTextColor(oldTextColor);


	if (m_dZoomRatio == ZOOM_MAX_RATIO)
	{
		CRect rtImageAreaWnd;
		CRect rtImageAreaPixel;

		GetClientRect(rtImageAreaWnd);
	}
}

void CImageView_Default::DrawUserSelectMeasure(CDC* pDC)
{
	if (m_bDrawUserSelect == FALSE)
		return;

	CRect rtUserSelect_wnd = CalcuateImagePosToViewWndPos(m_rtUserSelect_pxl);

	HPEN hOutLine = CreatePen(PS_SOLID, 1, RGB(0, 255, 255));
	HGDIOBJ hOld = pDC->SelectObject(hOutLine);
	pDC->SelectStockObject(NULL_BRUSH);

	CRect rtViewRect;
	GetClientRect(rtViewRect);

	pDC->MoveTo(rtUserSelect_wnd.left, 0);
	pDC->LineTo(rtUserSelect_wnd.left, rtViewRect.Height());

	pDC->MoveTo(rtUserSelect_wnd.right, 0);
	pDC->LineTo(rtUserSelect_wnd.right, rtViewRect.Height());

	pDC->MoveTo(0, rtUserSelect_wnd.top);
	pDC->LineTo(rtViewRect.Width(), rtUserSelect_wnd.top);

	pDC->MoveTo(0, rtUserSelect_wnd.bottom);
	pDC->LineTo(rtViewRect.Width(), rtUserSelect_wnd.bottom);

	pDC->MoveTo(rtUserSelect_wnd.CenterPoint().x, rtUserSelect_wnd.top);
	pDC->LineTo(rtUserSelect_wnd.CenterPoint().x, rtUserSelect_wnd.bottom);
	pDC->MoveTo(rtUserSelect_wnd.left, rtUserSelect_wnd.CenterPoint().y);
	pDC->LineTo(rtUserSelect_wnd.right, rtUserSelect_wnd.CenterPoint().y);

	if (0 < rtUserSelect_wnd.Width() && 0 < rtUserSelect_wnd.Height())
	{
		CPoint ptCenter_wnd = rtUserSelect_wnd.CenterPoint();

		// Top
		CRect rtTemp;
		rtTemp.SetRect(rtUserSelect_wnd.left, rtUserSelect_wnd.top, rtUserSelect_wnd.left, rtUserSelect_wnd.top);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(ptCenter_wnd.x, rtUserSelect_wnd.top, ptCenter_wnd.x, rtUserSelect_wnd.top);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(rtUserSelect_wnd.right, rtUserSelect_wnd.top, rtUserSelect_wnd.right, rtUserSelect_wnd.top);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		// Center
		rtTemp.SetRect(rtUserSelect_wnd.left, ptCenter_wnd.y, rtUserSelect_wnd.left, ptCenter_wnd.y);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(ptCenter_wnd.x, ptCenter_wnd.y, ptCenter_wnd.x, ptCenter_wnd.y);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(rtUserSelect_wnd.right, ptCenter_wnd.y, rtUserSelect_wnd.right, ptCenter_wnd.y);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		// Bottom
		rtTemp.SetRect(rtUserSelect_wnd.left, rtUserSelect_wnd.bottom, rtUserSelect_wnd.left, rtUserSelect_wnd.bottom);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(ptCenter_wnd.x, rtUserSelect_wnd.bottom, ptCenter_wnd.x, rtUserSelect_wnd.bottom);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);

		rtTemp.SetRect(rtUserSelect_wnd.right, rtUserSelect_wnd.bottom, rtUserSelect_wnd.right, rtUserSelect_wnd.bottom);
		rtTemp.InflateRect(USER_SELECT_HIT_AREA, USER_SELECT_HIT_AREA);
		pDC->Rectangle(rtTemp);
	}

	pDC->SelectObject(hOld);
	DeleteObject(hOutLine);

	// Size Info
	COLORREF textColor = RGB(255, 127, 39);

	int nOldBk = pDC->SetBkMode(TRANSPARENT);
	COLORREF oldTextColor = pDC->SetTextColor(textColor);

	CPoint ptMouse = CalcuateImagePosToViewWndPos(GetMouseOnImagePos());

	int nWidth = abs(m_rtUserSelect_pxl.Width());
	int nHeight = abs(m_rtUserSelect_pxl.Height());
	double dWidth = nWidth * m_dResolutionX;
	double dHeight = nHeight * m_dResolutionY;

	CString strTemp;
	strTemp.Format(_T("Width : %d pxl, Height : %d pxl"), nWidth, nHeight);
	pDC->TextOut(ptMouse.x + 10, ptMouse.y + 15, strTemp);

	strTemp.Format(_T("Width : %.3f um, Height : %.3f um"), dWidth, dHeight);
	pDC->TextOut(ptMouse.x + 10, ptMouse.y + 35, strTemp);

	strTemp.Format(_T("Diagonal : %.3f um"), abs(sqrt((dWidth * dWidth) + (dHeight * dHeight))));
	pDC->TextOut(ptMouse.x + 10, ptMouse.y + 55, strTemp);

	pDC->SetBkMode(nOldBk);
	pDC->SetTextColor(oldTextColor);
}

void CImageView_Default::DrawPixelInfo(CDC* pDC)
{
	if(m_bDrawPixelInfo == FALSE)
		return;

	COLORREF textColor = RGB(255, 127, 39);

	int nOldBk = pDC->SetBkMode(TRANSPARENT);
	COLORREF oldTextColor = pDC->SetTextColor(textColor);

	CString strMousePos;
	if(GetChannels() == 1)	
		strMousePos.Format(_T("X : %d, Y : %d, Gray : %d"), m_ptMousePos.x, m_ptMousePos.y, GetPixelValue(m_ptMousePos.x, m_ptMousePos.y));
	else
	{
		int nR = 0;
		int nG = 0;
		int nB = 0;
		int nGray = 0;
		nGray = GetPixelValue(m_ptMousePos.x, m_ptMousePos.y, nB, nG, nR);
		strMousePos.Format(_T("X : %d, Y : %d, Gray : %d, R : %d, G : %d, B : %d"), m_ptMousePos.x, m_ptMousePos.y, nGray, nR, nG, nB);
	}
	pDC->TextOut(10, 10, strMousePos);

	HPEN hOutLine = CreatePen(PS_SOLID, 1, RGB(255, 0, 0));
	HGDIOBJ hOld = pDC->SelectObject(hOutLine);
	pDC->SelectStockObject(NULL_BRUSH);

	CRect rtMouseImagePos = CRect(m_ptMousePos.x, m_ptMousePos.y, m_ptMousePos.x, m_ptMousePos.y);
	rtMouseImagePos.InflateRect(1, 1);
	CRect rtMouseViewPos = CalcuateImagePosToViewWndPos(rtMouseImagePos);
	pDC->Rectangle(rtMouseViewPos);

	pDC->MoveTo(rtMouseViewPos.CenterPoint().x, rtMouseViewPos.top - 1);
	pDC->LineTo(rtMouseViewPos.CenterPoint().x, rtMouseViewPos.bottom + 1);

	pDC->MoveTo(rtMouseViewPos.left - 1, rtMouseViewPos.CenterPoint().y);
	pDC->LineTo(rtMouseViewPos.right + 1, rtMouseViewPos.CenterPoint().y);

	pDC->SelectObject(hOld);
	DeleteObject(hOutLine);

	pDC->SetBkMode(nOldBk);
	pDC->SetTextColor(oldTextColor);
}

CPoint CImageView_Default::CalcuateWndPosToImagePos(CPoint ptWndPos)
{
	CRect rtTemp = CRect(ptWndPos.x, ptWndPos.y, ptWndPos.x, ptWndPos.y);

	rtTemp = CalcuateWndPosToImagePos(rtTemp);

	return CPoint(rtTemp.left, rtTemp.top);
}

CRect CImageView_Default::CalcuateWndPosToImagePos(CRect rtWnd)
{
	CRect rtReturn;

	if (m_nViewMode == 0)
	{
		rtReturn.left = int(double(rtWnd.left) / m_dZoomRatio + 0.5);
		rtReturn.top = int(double(rtWnd.top) / m_dZoomRatio + 0.5);
		rtReturn.right = int(double(rtWnd.right) / m_dZoomRatio + 0.5);
		rtReturn.bottom = int(double(rtWnd.bottom) / m_dZoomRatio + 0.5);

		rtReturn.OffsetRect(m_nHScroll / m_dZoomRatio, m_nVScroll / m_dZoomRatio);
	}
	else
	{
		rtReturn.left = int(double(rtWnd.left) / m_dWidthScale);
		rtReturn.top = int(double(rtWnd.top) / m_dHeightScale);
		rtReturn.right = int(double(rtWnd.right) / m_dWidthScale);
		rtReturn.bottom = int(double(rtWnd.bottom) / m_dHeightScale);
	}

	return rtReturn;
}

CPoint CImageView_Default::CalcuateImagePosToViewWndPos(CPoint ptImagePos)
{
	CRect rtTemp(ptImagePos.x, ptImagePos.y, ptImagePos.x, ptImagePos.y);

	rtTemp = CalcuateImagePosToViewWndPos(rtTemp);

	return CPoint(rtTemp.left, rtTemp.top);
}

CRect CImageView_Default::CalcuateImagePosToViewWndPos(CRect rtImage)
{
	CRect rtReturn = CRect(0, 0, 0, 0);

	GetScaleWidth();
	GetScaleHeight();

	if (m_nViewMode == 0)
	{
		// Change View Coordinate..
		rtReturn.left = int((rtImage.left - (((double)m_nHScroll) / m_dZoomRatio)) * m_dZoomRatio);		//int(double(m_rtRoiRect.left) / m_dZoomRatio + 0.5) - m_nHScroll;
		rtReturn.top = int((rtImage.top - (((double)m_nVScroll) / m_dZoomRatio)) * m_dZoomRatio);			//int(double(m_rtRoiRect.top) / m_dZoomRatio + 0.5) - m_nVScroll;
		rtReturn.right = int((rtImage.right - (((double)m_nHScroll) / m_dZoomRatio)) * m_dZoomRatio);		//int(double(m_rtRoiRect.right) / m_dZoomRatio + 0.5) - m_nHScroll;
		rtReturn.bottom = int((rtImage.bottom - (((double)m_nVScroll) / m_dZoomRatio)) * m_dZoomRatio);		//int(double(m_rtRoiRect.bottom) / m_dZoomRatio + 0.5) - m_nVScroll;
	}
	else
	{
		// Change View Coordinate..
		rtReturn.left = double(rtImage.left) * m_dWidthScale;			//int(double(m_rtRoiRect.left) / m_dZoomRatio + 0.5) - m_nHScroll;
		rtReturn.top = double(rtImage.top) * m_dHeightScale;			//int(double(m_rtRoiRect.top) / m_dZoomRatio + 0.5) - m_nVScroll;
		rtReturn.right = double(rtImage.right) * m_dWidthScale;			//int(double(m_rtRoiRect.right) / m_dZoomRatio + 0.5) - m_nHScroll;
		rtReturn.bottom = double(rtImage.bottom) * m_dHeightScale;		//int(double(m_rtRoiRect.bottom) / m_dZoomRatio + 0.5) - m_nVScroll;
	}

	return rtReturn;
}
