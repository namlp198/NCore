#include "pch.h"
#include "SharedMemoryView.h"
#include "ControlMemDC.h"

CSharedMemoryView::CSharedMemoryView(CWnd* pParent)
{	
	m_strViewTitle							= _T("No Title");

	m_nHScroll								= 0;
	m_nVScroll								= 0;

	m_nMaxHScroll							= 100;
	m_nMaxVScroll							= 100;

	m_nVScrollStep							= 8;
	m_nHScrollStep							= 8;

	m_rtView								= CRect(0,0,0,0);

	m_dZoomLevel							= 2.;

	m_pEdgeInspectStatus					= NULL;

	m_ptMouseViewPos						= CPoint(-1, -1);

	m_pBmInfo = (BITMAPINFO*)malloc(sizeof(BITMAPINFO)+256*sizeof(RGBQUAD));
	m_pBmInfo->bmiHeader.biBitCount			= 8; 
	m_pBmInfo->bmiHeader.biClrImportant		= 256;
	m_pBmInfo->bmiHeader.biClrUsed			= 256;
	m_pBmInfo->bmiHeader.biCompression		= BI_RGB;
	m_pBmInfo->bmiHeader.biPlanes			= 1;
	m_pBmInfo->bmiHeader.biSize				= sizeof( BITMAPINFOHEADER );
	m_pBmInfo->bmiHeader.biXPelsPerMeter	= 0;
	m_pBmInfo->bmiHeader.biYPelsPerMeter	= 0;

	for(int i=0; i<256; i++) // Palette number is 256
	{
		m_pBmInfo->bmiColors[i].rgbRed= m_pBmInfo->bmiColors[i].rgbGreen = m_pBmInfo->bmiColors[i].rgbBlue = i; 
		m_pBmInfo->bmiColors[i].rgbReserved = 0;
	}
}


CSharedMemoryView::~CSharedMemoryView(void)
{
	if(m_pBmInfo)
	{
		free(m_pBmInfo);
		m_pBmInfo = NULL;
	}
}

BEGIN_MESSAGE_MAP(CSharedMemoryView, CWnd)
	ON_WM_CREATE()
	ON_WM_PAINT()
	ON_WM_HSCROLL()
	ON_WM_VSCROLL()
	ON_WM_MOUSEMOVE()
END_MESSAGE_MAP()

int CSharedMemoryView::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	ShowScrollBar(SB_HORZ, FALSE);
	ShowScrollBar(SB_VERT, FALSE);
	GetClientRect(&m_rtView);

	return 0;

}

void CSharedMemoryView::OnPaint()
{
	CPaintDC dc(this);
	GetClientRect(m_rtView);

	CControlMemDC* pMemDC = new CControlMemDC(&dc, &m_rtView);

	//Draw Scan Image
	DrawImage(*pMemDC);
	DrawStartLine(*pMemDC);
	DrawTopCorner(*pMemDC);
	DrawBottomCorner(*pMemDC);
	DrawSideLine(*pMemDC);
	//DrawInspectLine(*pMemDC);
	DrawChamferOutLine(*pMemDC);
	DrawChamferInLine(*pMemDC);
	DrawEndLine(*pMemDC);

	DrawChippingDefect(*pMemDC);

	DrawViewTitle(*pMemDC);
	DrawMousePositionInfo(*pMemDC);
	DrawZoomInfo(*pMemDC);

	//DrawAllShape(*pMemDC);
	//DrawViewBG(*pMemDC);
	//DrawMakingShape(*pMemDC);

	delete pMemDC;
}

void CSharedMemoryView::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	switch (nSBCode)
	{
	case SB_LEFT:			// Scroll to far left.
	case SB_RIGHT:			// Scroll to far right.
	case SB_ENDSCROLL:		// End scroll.
	case SB_LINELEFT:       // Scroll left.
		m_nHScroll--;
		break;
	case SB_LINERIGHT:		// Scroll right.
		m_nHScroll++;
		break;
	case SB_PAGELEFT:		// Scroll one page left.
	case SB_PAGERIGHT:      // Scroll one page right.
	case SB_THUMBPOSITION:  // Scroll to absolute position. nPos is the position
		break;
	case SB_THUMBTRACK:		// Drag scroll box to specified position. nPos is the
		m_nHScroll = nPos;
		break;
	}

	if(m_nMaxHScroll < m_nHScroll)	// Overflow..
		m_nHScroll = 0;
	else if(m_nHScroll < 0)
		m_nHScroll = 0;

	SetScrollPos(SB_HORZ, m_nHScroll);

	Invalidate(FALSE);

	CWnd::OnHScroll(nSBCode, nPos, pScrollBar);
}

void CSharedMemoryView::OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	switch (nSBCode)
	{
	case SB_TOP:
	case SB_BOTTOM:			// Scroll to bottom. 
	case SB_ENDSCROLL:		// End scroll.
	case SB_LINEDOWN:		// Scroll one line down.
		m_nVScroll++;
		break;
	case SB_LINEUP:			// Scroll one line up. 
		m_nVScroll--;
		break;
	case SB_PAGEDOWN:		// Scroll one page down. 
	case SB_PAGEUP:			// Scroll one page up.
	case SB_THUMBPOSITION:	// Scroll to the absolute position. The current position is provided in nPos. 
		break;
	case SB_THUMBTRACK:		// Drag scroll box to specified position. The current position is provided in nPos. 
		m_nVScroll = nPos;
		break;
	}

	if(m_nMaxVScroll < m_nVScroll)	// overflow..
		m_nVScroll = 0;
	else if(m_nVScroll < 0)
		m_nVScroll = 0;

	// Set the new position of the thumb (scroll box).
	SetScrollPos(SB_VERT, m_nVScroll);

	Invalidate(FALSE);

	CWnd::OnVScroll(nSBCode, nPos, pScrollBar);
}

void CSharedMemoryView::OnMouseMove(UINT nFlags, CPoint point)
{
	m_ptMouseViewPos = point;

	Invalidate();

	CWnd::OnMouseMove(nFlags, point);
}

void CSharedMemoryView::SetScroll()
{
	CRect rtWindow;
	GetWindowRect(&rtWindow);

	int x = int(GetFrameWidth());
	int y = int(GetFrameHeight() * GetFrameCount());

	int nScrollSizeH = (int) ((x - (rtWindow.Width()/m_dZoomLevel)) / m_nHScrollStep);
	int nScrollSizeV = (int) ((y - (rtWindow.Height()/m_dZoomLevel)) / m_nVScrollStep);

	m_nMaxHScroll	 = nScrollSizeH;
	m_nMaxVScroll	 = nScrollSizeV;

	SetScrollRange(SB_HORZ, 0, nScrollSizeH);
	SetScrollRange(SB_VERT, 0, nScrollSizeV);

	Invalidate();
}

void CSharedMemoryView::SetViewPositionFrame(int nFrameIndex)
{
	CRect rtWindow;
	GetWindowRect(&rtWindow);

	int y = int(GetFrameHeight() * GetFrameCount());

	int nScrollSizeV = (int) ((y - (rtWindow.Height()/m_dZoomLevel)) / m_nVScrollStep);

	int nSetScrollPosV = (int) ((nFrameIndex * GetFrameHeight()) / m_nVScrollStep);

	if(nSetScrollPosV < 0)
		nSetScrollPosV = 0;
	else if((int) m_nMaxVScroll < nSetScrollPosV)
		nSetScrollPosV = (int) m_nMaxVScroll;

	m_nVScroll = (UINT) nSetScrollPosV;
	SetScrollPos(SB_VERT, m_nVScroll);

	SendMessage(WM_PAINT);
	Invalidate();
}

void CSharedMemoryView::SetViewPositionX(int nXPos)
{
	CRect rtWindow;
	GetWindowRect(&rtWindow);

	CRect rtViewArea(rtWindow);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (int) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (int) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.right = rtViewImageArea.left + align_4byte(rtViewImageArea.Width());


	int x = int(GetFrameWidth());

	int nScrollSizeH = (int) ((x - (rtWindow.Width()/m_dZoomLevel)) / m_nHScrollStep);

	int nSetScrollPosH = (int) ((nXPos-(rtViewImageArea.Width()/3)) / m_nHScrollStep);

	if(nSetScrollPosH < 0)
		nSetScrollPosH = 0;
	else if((int) m_nMaxHScroll < nSetScrollPosH)
		nSetScrollPosH = (int) m_nMaxHScroll;

	m_nHScroll = (UINT) nSetScrollPosH;
	SetScrollPos(SB_HORZ, m_nHScroll);

	SendMessage(WM_PAINT);
	Invalidate();
}

void CSharedMemoryView::ZoomSet(double dRatio)
{
	m_dZoomLevel = dRatio;

	SetScroll();

	Invalidate();
}

void CSharedMemoryView::ZoomIn()
{
	if(MAX_ZOOM_RATIO < m_dZoomLevel * 2.0)
		return;

	m_dZoomLevel = m_dZoomLevel * 2.0;

	SetScroll();

	Invalidate();
}

void CSharedMemoryView::ZoomOut()
{
	if(m_dZoomLevel / 2.0 < MIN_ZOOM_RATIO)
		return;

	m_dZoomLevel = m_dZoomLevel / 2.0;

	SetScroll();

	Invalidate();
}

void CSharedMemoryView::DrawImage(CDC &dc)
{
	if(m_pSharedMemory == NULL)
		return;

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.right = rtViewImageArea.left + align_4byte(rtViewImageArea.Width());

	BYTE* pViewImage = new BYTE[rtViewImageArea.Width()*rtViewImageArea.Height()];
	ZeroMemory(pViewImage, sizeof(BYTE)*rtViewImageArea.Width()*rtViewImageArea.Height());

	int t1 = 0;
	int t2 = 0;
	int t3 = 0;
	int t4 = 0;

	for(int i=0; i<rtViewImageArea.Height(); i++)
	{
		t1 = rtViewImageArea.Width() * i;
		t2 = (m_nVScroll * m_nVScrollStep) + i;
		t3 = ((m_nVScroll * m_nVScrollStep) + i) * GetFrameWidth();
		t4 = GetFrameWidth();

		memcpy(pViewImage+(rtViewImageArea.Width()*i), m_pSharedMemory+(((m_nVScroll*m_nVScrollStep)+i)*GetFrameWidth()+(m_nHScroll*m_nHScrollStep)), rtViewImageArea.Width());
	}

	m_pBmInfo->bmiHeader.biWidth		= rtViewImageArea.Width(); 
	m_pBmInfo->bmiHeader.biHeight		= - (rtViewImageArea.Height());
	m_pBmInfo->bmiHeader.biSizeImage	= rtViewImageArea.Width() * rtViewImageArea.Height();

	dc.SetStretchBltMode(COLORONCOLOR);
	StretchDIBits(dc, 0, 0, rtViewArea.Width(), rtViewArea.Height(), 0, 0, rtViewImageArea.Width(), rtViewImageArea.Height(), pViewImage, (BITMAPINFO*)m_pBmInfo, DIB_RGB_COLORS, SRCCOPY);

	delete [] pViewImage;
}

void CSharedMemoryView::DrawStartLine(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	if(m_pEdgeInspectStatus->IsFindStartLine() == FALSE)
		return;

	int nFindStartLine = m_pEdgeInspectStatus->GetFindStartLine();

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	if(nFindStartLine < rtViewImageArea.top || rtViewImageArea.bottom < nFindStartLine)
		return;

	int nDrawLine = (int) ((nFindStartLine - rtViewImageArea.top) * m_dZoomLevel);

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	dc.MoveTo(0, nDrawLine);
	dc.LineTo(m_rtView.Width(), nDrawLine);

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawEndLine(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	if(m_pEdgeInspectStatus->IsFindEndLine() == FALSE)
		return;

	int nFindEndLine = m_pEdgeInspectStatus->GetFindEndLine();

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	if(nFindEndLine < rtViewImageArea.top || rtViewImageArea.bottom < nFindEndLine)
		return;

	int nDrawLine = (int) ((nFindEndLine - rtViewImageArea.top) * m_dZoomLevel);

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	dc.MoveTo(0, nDrawLine);
	dc.LineTo(rtViewArea.Width(), nDrawLine);

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawTopCorner(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	if(m_pEdgeInspectStatus->IsFindStartLine() == FALSE)
		return;

	CRect rtTopCornerArea = m_pEdgeInspectStatus->GetTopCornerArea();

	// View to Real
	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	CRect rtTopCornerIntersect;
	rtTopCornerIntersect.IntersectRect(rtTopCornerArea, rtViewImageArea);

	if(rtTopCornerIntersect.IsRectEmpty() || rtTopCornerIntersect.IsRectNull())
		return;

	// Real to View..
	rtTopCornerIntersect.OffsetRect(-rtViewImageArea.left, -rtViewImageArea.top);

	rtTopCornerIntersect.left		= (LONG) (rtTopCornerIntersect.left * m_dZoomLevel);
	rtTopCornerIntersect.top		= (LONG) (rtTopCornerIntersect.top * m_dZoomLevel);
	rtTopCornerIntersect.right		= (LONG) (rtTopCornerIntersect.right * m_dZoomLevel);
	rtTopCornerIntersect.bottom		= (LONG) (rtTopCornerIntersect.bottom * m_dZoomLevel);

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	dc.Rectangle(rtTopCornerIntersect);

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawBottomCorner(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	if(m_pEdgeInspectStatus->IsFindStartLine() == FALSE)
		return;

	CRect rtBottomCornerArea = m_pEdgeInspectStatus->GetBottomCornerArea();

	// View to Real
	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	CRect rtBottomCornerIntersect;
	rtBottomCornerIntersect.IntersectRect(rtBottomCornerArea, rtViewImageArea);

	if(rtBottomCornerIntersect.IsRectEmpty() || rtBottomCornerIntersect.IsRectNull())
		return;

	// Real to View..
	rtBottomCornerIntersect.OffsetRect(-rtViewImageArea.left, -rtViewImageArea.top);

	rtBottomCornerIntersect.left	= (LONG) (rtBottomCornerIntersect.left * m_dZoomLevel);
	rtBottomCornerIntersect.top		= (LONG) (rtBottomCornerIntersect.top * m_dZoomLevel);
	rtBottomCornerIntersect.right	= (LONG) (rtBottomCornerIntersect.right * m_dZoomLevel);
	rtBottomCornerIntersect.bottom	= (LONG) (rtBottomCornerIntersect.bottom * m_dZoomLevel);

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	dc.Rectangle(rtBottomCornerIntersect);

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawSideLine(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	// View To Real
	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	int nStartFrameIndex = (int) (rtViewImageArea.top / GetFrameHeight());
	int nEndFrameIndex = (int) (rtViewImageArea.bottom / GetFrameHeight());

	CPen LinePen(PS_SOLID, 1 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	for(int nFrameIndex = nStartFrameIndex; nFrameIndex <= nEndFrameIndex; nFrameIndex++)
	{
		int nFindFrameSideLine = m_pEdgeInspectStatus->GetFrameSideLine(nFrameIndex);

		if(nFindFrameSideLine < rtViewImageArea.left || rtViewImageArea.right < nFindFrameSideLine)
			continue;

		int nFrameTopPixel = nFrameIndex * GetFrameHeight();
		int nFrameEndPixel = nFrameTopPixel + GetFrameHeight();

		// Real to View
		int nDrawLineX			= (int) ((nFindFrameSideLine - rtViewImageArea.left) * m_dZoomLevel);
		int nDrawLineStartY		= (nFrameTopPixel < m_pEdgeInspectStatus->GetTopCornerArea().bottom) ? m_pEdgeInspectStatus->GetTopCornerArea().bottom : 
									(m_pEdgeInspectStatus->GetBottomCornerArea().top < nFrameTopPixel) ? m_pEdgeInspectStatus->GetBottomCornerArea().top : nFrameTopPixel ;

		int nDrawLineEndY		= (nFrameEndPixel < m_pEdgeInspectStatus->GetTopCornerArea().bottom) ? m_pEdgeInspectStatus->GetTopCornerArea().bottom : 
									(m_pEdgeInspectStatus->GetBottomCornerArea().top < nFrameEndPixel) ? m_pEdgeInspectStatus->GetBottomCornerArea().top : nFrameEndPixel ;

		nDrawLineStartY			= (int) ((nDrawLineStartY - rtViewImageArea.top) * m_dZoomLevel);
		nDrawLineEndY			= (int) ((nDrawLineEndY - rtViewImageArea.top) * m_dZoomLevel);

		dc.MoveTo(nDrawLineX, nDrawLineStartY);
		dc.LineTo(nDrawLineX, nDrawLineEndY);
	}

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawChamferOutLine(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

/*
	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	std::vector<int>* vecOutLine = m_pEdgeInspectStatus->GetFindSideOutLineVector();

	if(vecOutLine == NULL)
		return;

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	for(int i=0; i<rtViewImageArea.Height(); i++)
	{
		// Out Line..
		int nY = i + rtViewImageArea.top;

		if(nY < 0 || (*vecOutLine).size() <= nY)
			continue;

		int nX = (*vecOutLine)[nY];

		if(nX < rtViewImageArea.left || rtViewImageArea.right < nX)
			continue;

		dc.SetPixel((int)((nX-rtViewImageArea.left)*m_dZoomLevel), (int) ((nY-rtViewImageArea.top)*m_dZoomLevel), RGB(0,0,255));
	}

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
*/
}

void CSharedMemoryView::DrawChamferInLine(CDC &dc)
{
/*
	if(m_pEdgeInspectStatus == NULL)
		return;

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	std::vector<int>* vecInLine = m_pEdgeInspectStatus->GetFindSideInLineVector();

	if(vecInLine == NULL)
		return;

	CPen LinePen(PS_SOLID, 2 ,RGB(50,200,50)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	for(int i=0; i<rtViewImageArea.Height(); i++)
	{
		// Out Line..
		int nY = i + rtViewImageArea.top;

		if(nY < 0 || (*vecInLine).size() <= nY)
			continue;

		int nX = (*vecInLine)[nY];

		if(nX < rtViewImageArea.left || rtViewImageArea.right < nX)
			continue;

		dc.SetPixel((int)((nX-rtViewImageArea.left)*m_dZoomLevel), (int) ((nY-rtViewImageArea.top)*m_dZoomLevel), RGB(0,0,255));
	}

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
*/
}

void CSharedMemoryView::DrawInspectLine(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	int nStartFrameIndex = (int) (rtViewImageArea.top / GetFrameHeight());
	int nEndFrameIndex = (int) (rtViewImageArea.bottom / GetFrameHeight());

	CPen*	pOldpen;
	CPen	ChamferLinePen(PS_DOT, 1 ,RGB(200,50,50));
	CPen	ChippingLinePen(PS_DOT, 1 ,RGB(180,50,50));
	CPen	CrackLinePen(PS_DOT, 1 ,RGB(160,50,50));
	CPen	BurrLinePen(PS_DOT, 1 ,RGB(140,50,50));
	CPen	BrokenLinePen(PS_DOT, 1 ,RGB(120,50,50));

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	for(int nFrameIndex = nStartFrameIndex; nFrameIndex <= nEndFrameIndex; nFrameIndex++)
	{
		int nFindFrameSideLine = m_pEdgeInspectStatus->GetFrameSideLine(nFrameIndex);

		if(nFindFrameSideLine < rtViewImageArea.left || rtViewImageArea.right < nFindFrameSideLine)
			continue;

		// if(m_pEdgeInspectStatus->GetUsePanel(nPanelIndex) == FALSE)
		// 	continue;

		int nChippingDrawLine = (int) ((nFindFrameSideLine - rtViewImageArea.left + m_pEdgeInspectStatus->GetFrame_ChipCrack_InsArea(nFrameIndex).left) * m_dZoomLevel);
		int nInChippingDrawLine = (int) ((nFindFrameSideLine - rtViewImageArea.left + m_pEdgeInspectStatus->GetFrame_In_ChipCrack_InsArea(nFrameIndex).left) * m_dZoomLevel);
		int nBurrDrawLine = (int) ((nFindFrameSideLine - rtViewImageArea.left + m_pEdgeInspectStatus->GetFrame_Burr_InsArea(nFrameIndex).right) * m_dZoomLevel);
		int nInBurrDrawLine = (int) ((nFindFrameSideLine - rtViewImageArea.left + m_pEdgeInspectStatus->GetFrame_In_Burr_InsArea(nFrameIndex).right) * m_dZoomLevel);
		//int nBrokenDrawLine = (int) ((nFindFrameSideLine - rtViewImageArea.left + m_pEdgeInspectStatus->GetBrokenRange(nPanelIndex)) * m_dZoomLevel);

		int nFrameTopPixel = nFrameIndex * GetFrameHeight();
		int nFrameEndPixel = nFrameTopPixel + GetFrameHeight();

		// Real to View
		int nDrawLineX			= (int) ((nFindFrameSideLine - rtViewImageArea.left) * m_dZoomLevel);
		int nDrawLineStartY		= (nFrameTopPixel < m_pEdgeInspectStatus->GetTopCornerArea().bottom) ? m_pEdgeInspectStatus->GetTopCornerArea().bottom : 
			(m_pEdgeInspectStatus->GetBottomCornerArea().top < nFrameTopPixel) ? m_pEdgeInspectStatus->GetBottomCornerArea().top : nFrameTopPixel ;

		int nDrawLineEndY		= (nFrameEndPixel < m_pEdgeInspectStatus->GetTopCornerArea().bottom) ? m_pEdgeInspectStatus->GetTopCornerArea().bottom : 
			(m_pEdgeInspectStatus->GetBottomCornerArea().top < nFrameEndPixel) ? m_pEdgeInspectStatus->GetBottomCornerArea().top : nFrameEndPixel ;

		nDrawLineStartY			= (int) ((nDrawLineStartY - rtViewImageArea.top) * m_dZoomLevel);
		nDrawLineEndY			= (int) ((nDrawLineEndY - rtViewImageArea.top) * m_dZoomLevel);

		pOldBrush = dc.SelectObject(&bru);

		pOldpen = dc.SelectObject(&ChamferLinePen);
		dc.MoveTo(nChippingDrawLine, nDrawLineStartY);
		dc.LineTo(nChippingDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);

		pOldpen = dc.SelectObject(&ChippingLinePen);
		dc.MoveTo(nChippingDrawLine, nDrawLineStartY);
		dc.LineTo(nChippingDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);

		pOldpen = dc.SelectObject(&CrackLinePen);
		dc.MoveTo(nInChippingDrawLine, nDrawLineStartY);
		dc.LineTo(nInChippingDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);

		pOldpen = dc.SelectObject(&BurrLinePen);
		dc.MoveTo(nBurrDrawLine, nDrawLineStartY);
		dc.LineTo(nBurrDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);

		pOldpen = dc.SelectObject(&BrokenLinePen);
		dc.MoveTo(nInBurrDrawLine, nDrawLineStartY);
		dc.LineTo(nInBurrDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);

		/*
		pOldpen = dc.SelectObject(&BrokenLinePen);
		dc.MoveTo(nBrokenDrawLine, nDrawLineStartY);
		dc.LineTo(nBrokenDrawLine, nDrawLineEndY);
		dc.SelectObject(pOldpen);
		*/

		dc.SelectObject(pOldBrush);
	}

	bru.DeleteObject();
	ChamferLinePen.DeleteObject();
	ChippingLinePen.DeleteObject();
	CrackLinePen.DeleteObject();
	BurrLinePen.DeleteObject();
	BrokenLinePen.DeleteObject();
}

void CSharedMemoryView::DrawChippingDefect(CDC &dc)
{
	if(m_pEdgeInspectStatus == NULL)
		return;

	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	std::vector<CEdgeInspectDefect>* vecDefectList = m_pEdgeInspectStatus->GetEdgeInspectDefectList();

	if(vecDefectList == NULL)
		return;

	CPen LinePen(PS_SOLID, 2 ,RGB(255,0,0)), *pOldpen;

	CBrush bru, *pOldBrush;

	if(TRUE)	bru.CreateStockObject(NULL_BRUSH);
	else		bru.CreateSolidBrush(RGB(50,200,50));

	pOldpen = dc.SelectObject(&LinePen);
	pOldBrush = dc.SelectObject(&bru);

	for(int i=0; i<(int) (*vecDefectList).size(); i++)
	{
		CEdgeInspectDefect* pDefect = &(*vecDefectList)[i];

		if(pDefect == NULL)
			continue;

		if(pDefect->m_emDefectType != emDefectType_ChipCrack)
			continue;

		CRect rtIntersect;
		CRect rtDefectArea(pDefect->m_nDefectArea_Left, pDefect->m_nDefectArea_Top, pDefect->m_nDefectArea_Right, pDefect->m_nDefectArea_Bottom);

		rtIntersect.IntersectRect(rtDefectArea, rtViewImageArea);

		if(rtIntersect.IsRectEmpty() || rtIntersect.IsRectNull())
			continue;

		rtIntersect.OffsetRect(-rtViewImageArea.left, -rtViewImageArea.top);

		rtIntersect.left	= (LONG) (rtIntersect.left * m_dZoomLevel);
		rtIntersect.top		= (LONG) (rtIntersect.top * m_dZoomLevel);
		rtIntersect.right	= (LONG) (rtIntersect.right * m_dZoomLevel);
		rtIntersect.bottom	= (LONG) (rtIntersect.bottom * m_dZoomLevel);

		dc.Rectangle(rtIntersect);
	}

	dc.SelectObject(pOldpen);
	dc.SelectObject(pOldBrush);
	bru.DeleteObject();
	LinePen.DeleteObject();
}

void CSharedMemoryView::DrawViewTitle(CDC &dc)
{
	dc.SetBkMode(TRANSPARENT);
	dc.SetTextColor(RGB(200, 200, 230));

	CString strViewTitle;
	strViewTitle.Format(_T("[%s]"), m_strViewTitle);

	dc.TextOut(10, 10, strViewTitle);
}

void CSharedMemoryView::DrawMousePositionInfo(CDC &dc)
{
	CRect rtViewArea(m_rtView);
	rtViewArea.right = rtViewArea.left + align_4byte(rtViewArea.Width());

	CRect rtViewImageArea(rtViewArea);
	rtViewImageArea.right = (LONG) (rtViewImageArea.right / m_dZoomLevel);
	rtViewImageArea.bottom = (LONG) (rtViewImageArea.bottom / m_dZoomLevel);
	rtViewImageArea.right = rtViewImageArea.left + align_4byte(rtViewImageArea.Width());
	rtViewImageArea.OffsetRect(m_nHScroll*m_nHScrollStep, m_nVScroll*m_nVScrollStep);

	CPoint ptMouseImagePos(m_ptMouseViewPos);
	ptMouseImagePos.x = (LONG) (ptMouseImagePos.x / m_dZoomLevel);
	ptMouseImagePos.y = (LONG) (ptMouseImagePos.y / m_dZoomLevel);

	ptMouseImagePos.Offset(rtViewImageArea.left, rtViewImageArea.top);

	if(ptMouseImagePos.x < 0 || (LONG) GetFrameWidth() <= ptMouseImagePos.x)
		return;

	if(ptMouseImagePos.y < 0 || (LONG) (GetFrameHeight()*GetFrameCount()) <= ptMouseImagePos.y)
		return;

	int nGrayValue = -1;

	BYTE* pImageBuffer = GetBufferImage(ptMouseImagePos.y);

	nGrayValue = (int) pImageBuffer[ptMouseImagePos.x];

	dc.SetBkMode(TRANSPARENT);
	dc.SetTextColor(RGB(200, 200, 230));

	CString strMousePosInfo;
	strMousePosInfo.Format(_T("[X : %d, Y : %d, Gray : %d]"), ptMouseImagePos.x, ptMouseImagePos.y, nGrayValue);

	dc.TextOut(130, 10, strMousePosInfo);
}

void CSharedMemoryView::DrawZoomInfo(CDC &dc)
{
	dc.SetBkMode(TRANSPARENT);
	dc.SetTextColor(RGB(200, 200, 230));

	CString strZoomInfo;
	strZoomInfo.Format(_T("[Zoom : x %.1f]"), m_dZoomLevel);

	dc.TextOut(m_rtView.Width()-100, 10, strZoomInfo);
}