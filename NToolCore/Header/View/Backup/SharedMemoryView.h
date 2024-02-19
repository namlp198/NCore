#pragma once
#include "afxwin.h"
#include "SharedMemoryBuffer.h"
#include "EdgeInspectStatus.h"

#define align_4byte(in) ((in + 3)/4)*4

#define MAX_ZOOM_RATIO		16.0
#define MIN_ZOOM_RATIO		0.125

class AFX_EXT_CLASS CSharedMemoryView : public CWnd,
						  public CSharedMemoryBuffer
{
public:
	CSharedMemoryView(CWnd* pParent);
	~CSharedMemoryView(void);

protected:
	DECLARE_MESSAGE_MAP()

public:
	void					SetScroll();
	void					SetEdgeInspectStatus(CEdgeInspectStatus* pInspectStatus)	{m_pEdgeInspectStatus = pInspectStatus;};
	void					SetViewTitle(CString strTitle)								{m_strViewTitle = strTitle;};

	void					SetViewPositionFrame(int nFrameIndex);
	void					SetViewPositionX(int nXPos);
	
	void					ZoomIn();
	void					ZoomOut();

private:
	void					DrawImage(CDC &dc);

	void					DrawStartLine(CDC &dc);
	void					DrawEndLine(CDC &dc);

	void					DrawTopCorner(CDC &dc);
	void					DrawBottomCorner(CDC &dc);
	
	void					DrawSideLine(CDC &dc);
	void					DrawInspectLine(CDC &dc);

	void					DrawChamferOutLine(CDC &dc);
	void					DrawChamferInLine(CDC &dc);

	void					DrawDefect(CDC &dc);

	void					DrawViewTitle(CDC &dc);
	void					DrawMousePositionInfo(CDC &dc);
	void					DrawZoomInfo(CDC &dc);

protected:
	CString					m_strViewTitle;

	//View & Image Size
	CRect					m_rtView;

	//Scroll View
	UINT					m_nVScroll;
	UINT					m_nHScroll;

	UINT					m_nMaxVScroll;
	UINT					m_nMaxHScroll;

	UINT					m_nVScrollStep;
	UINT					m_nHScrollStep;

	// Zoom
	double					m_dZoomLevel;

	BITMAPINFO*				m_pBmInfo;

	CEdgeInspectStatus*		m_pEdgeInspectStatus;

	// Mouse..
	CPoint					m_ptMouseViewPos;

public:
	afx_msg virtual int	OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg virtual void OnPaint();
	afx_msg virtual void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg virtual void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg virtual void OnMouseMove(UINT nFlags, CPoint point);
};

