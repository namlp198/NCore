#if !defined(AFX_MEMDC_H__18BA4DAB_63FC_4FFA_83B0_2AFF3EDF0420__INCLUDED_)
#define AFX_MEMDC_H__18BA4DAB_63FC_4FFA_83B0_2AFF3EDF0420__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// MemDC.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CMemDC window

class AFX_EXT_CLASS CControlMemDC : public CDC {
private:	
	CBitmap		m_bitmap;		// Offscreen bitmap
	CBitmap*	m_oldBitmap;	// bitmap originally found in CControlMemDC
	CDC*		m_pDC;			// Saves CDC passed in constructor
	CRect		m_rect;			// Rectangle of drawing area.
	BOOL		m_bMemDC;		// TRUE if CDC really is a Memory DC.
	COLORREF	m_MaskColor;
	BOOL		m_bTransparent;
public:

	CControlMemDC(CDC* pDC, const CRect* pRect = NULL, BOOL bTransparent = FALSE, COLORREF Mask = RGB(255,254,253)) : CDC()
	{
		ASSERT(pDC != NULL); 

		// Some initialization
		m_pDC = pDC;
		m_oldBitmap = NULL;
		m_bTransparent = bTransparent;
		m_MaskColor = Mask;
		m_bMemDC = !pDC->IsPrinting();

		// Get the rectangle to draw
		if (pRect == NULL) {
			pDC->GetClipBox(&m_rect);
		} else {
			m_rect = *pRect;
		}

		if (m_bMemDC) {
			// Create a Memory DC
			CreateCompatibleDC(pDC);
			pDC->LPtoDP(&m_rect);

			m_bitmap.CreateCompatibleBitmap(pDC, m_rect.Width(), m_rect.Height());
			m_oldBitmap = SelectObject(&m_bitmap);

			SetMapMode(pDC->GetMapMode());

			SetWindowExt(pDC->GetWindowExt());
			SetViewportExt(pDC->GetViewportExt());

			pDC->DPtoLP(&m_rect);
			SetWindowOrg(m_rect.left, m_rect.top);
		} else {
			// Make a copy of the relevent parts of the current DC for printing
			m_bPrinting = pDC->m_bPrinting;
			m_hDC       = pDC->m_hDC;
			m_hAttribDC = pDC->m_hAttribDC;
		}

		// Fill background 
		if(m_bTransparent == FALSE)
		FillSolidRect(m_rect, pDC->GetBkColor());
		else
			DrawMaskRect(m_MaskColor);
	}

	~CControlMemDC()	
	{		
		if (m_bMemDC) {
			// Copy the offscreen bitmap onto the screen.
			if(m_bTransparent == FALSE)
			{
			m_pDC->BitBlt(m_rect.left, m_rect.top, m_rect.Width(), m_rect.Height(),
				this, m_rect.left, m_rect.top, SRCCOPY);			
			}			

			//Swap back the original bitmap.
			SelectObject(m_oldBitmap);
		} else {
			// All we need to do is replace the DC with an illegal value,
			// this keeps us from accidently deleting the handles associated with
			// the CDC that was passed to the constructor.			
			m_hDC = m_hAttribDC = NULL;
		}	
	}

	// Allow usage as a pointer	
	CControlMemDC* operator->() 
	{
		return this;
	}	

	// Allow usage as a pointer	
	operator CControlMemDC*() 
	{
		return this;
	}

	CDC *GetOriginalDC()
	{
		return m_pDC;
	}

	void SetTransparent(int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh)
	{	
		if(m_bTransparent)
			::TransparentBlt(m_pDC->GetSafeHdc(), dx, dy, dw, dh, GetSafeHdc(), sx, sy, sw, sh, m_MaskColor);
	}

	void DrawMaskRect(COLORREF Mask)
	{
		CPen LinePen(PS_SOLID,1,Mask), *pOldpen;
		CBrush bru, *pOldBrush;
		bru.CreateSolidBrush(Mask);
		pOldpen = SelectObject(&LinePen);
		pOldBrush = SelectObject(&bru);	
		Rectangle(m_rect);
		SelectObject(pOldpen);
		SelectObject(pOldBrush);
		bru.DeleteObject();
		LinePen.DeleteObject();
	}
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MEMDC_H__18BA4DAB_63FC_4FFA_83B0_2AFF3EDF0420__INCLUDED_)
