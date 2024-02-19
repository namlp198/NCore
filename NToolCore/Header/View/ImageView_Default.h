#pragma once

// CCameraView
#include "ImageBuffer.h"

#define IDR_USER_MENU						9000
#define IDR_LOAD_IMAGE						(IDR_USER_MENU+1)
#define IDR_SAVE_IMAGE						(IDR_USER_MENU+2)

#define IDR_VIEW_ORIGIN						(IDR_USER_MENU+3)
#define IDR_VIEW_FIT						(IDR_USER_MENU+4)
#define IDR_VIEW_NAME						(IDR_USER_MENU+5)
#define IDR_CENTER_LINE						(IDR_USER_MENU+6)
#define IDR_VIEW_PIXEL_INFO					(IDR_USER_MENU+7)

#define IDR_USER_SELECT						(IDR_USER_MENU+8)

#define IDR_SUB_CLASS_MENU					(IDR_USER_MENU+20)

#define ZOOM_INIT_RATIO						0.5
#define ZOOM_STEP_RATIO						1.2
#define ZOOM_MAX_RATIO						16.0
#define ZOOM_MIN_RATIO						0.5

#define USER_SELECT_HIT_AREA				4

class AFX_EXT_CLASS CImageView_Default : public CWnd, public CImageBuffer
{
	DECLARE_DYNAMIC(CImageView_Default)

public:
	CImageView_Default(int nViewIndex = 0, CWnd* pParentWnd = NULL);
	virtual ~CImageView_Default();

protected:
	DECLARE_MESSAGE_MAP()

public:
	void							SetViewIdx(int nIdx)	{m_nViewIndex = nIdx;};
	int								GetViewIdx()			{return m_nViewIndex;};

public:		// Image Data Process..
	BOOL							LoadImage(const CString& strFilename);
	BOOL							SetViewImage(CImageBuffer* pImageData);
	BOOL							SetViewImage(int nWidth, int nHeight, int nChannels, int nWidthStep, const BYTE* pBuffer);
	BOOL							GetViewImage(CImageBuffer* pImageData);
	BOOL							CopyImageFrom(const CImageBuffer* pFromImage);
	BOOL							GetBandImage(int nColorBand, CImageBuffer* pImageData);
	BOOL							GetSubROIRect(CRect& rtRect);

public:		// View Info..
	double							GetWidthScale() { return m_dWidthScale; };
	double							GetHeightScale() { return m_dHeightScale; };
	int								GetScaleWidth();
	int								GetScaleHeight();
	int								GetHScrollPos() { return m_nHScroll; };
	int								GetVScrollPos() { return m_nVScroll; };

	void							SetViewMode(int nViewMode);
	void							ChangeViewScale(int nViewMode, double dNewZoomRatio, CPoint ptMouseFocus);

	void							SetViewName(const CString& strViewName) { m_strViewName = strViewName; Invalidate(); };
	void							SetParentWnd(CWnd* pParentWnd) { m_pParentWnd = pParentWnd; };
	void							SetResolution(double dResX, double dResY) { m_dResolutionX = dResX, m_dResolutionY = dResY; Invalidate(); };

protected:
	CCriticalSection				m_csImageData;
	CWnd* m_pParentWnd;
	CRect							m_rtViewRect;
	int								m_nViewIndex;
	int								m_nViewMode;
	int								m_nScaleWidth;
	int								m_nScaleHeight;
	double							m_dZoomRatio;
	double							m_dWidthScale;
	double							m_dHeightScale;
	int								m_nVScroll;
	int								m_nHScroll;
	int								m_nMaxVScroll;
	int								m_nMaxHScroll;
	double							m_dResolutionX;
	double							m_dResolutionY;

	int								m_nViewMode_Result;

protected:
	void							UpdateView(CDC* pDC);
	virtual void					PopUpCommandMenu(const CPoint& point);
	void							ControlZoom(BOOL bZoomIn, CPoint ptMouse);

public:		// Draw View Name..
	void							SetDrawViewName(BOOL bDraw) { m_bDrawViewName = bDraw; Invalidate(); };

protected:
	BOOL							m_bDrawViewName;
	CString							m_strViewName;

public:		// Draw Center Line..
	void							DrawCenterLine(CDC* pDC);
	void							SetDrawCenterLine(BOOL bDraw) { m_bDrawCenterLine = bDraw; Invalidate(); };

protected:
	BOOL							m_bDrawCenterLine;

protected:	// Draw ETC..
	void							DrawZoomInfo(CDC* pDC);
	void							DrawUserSelectMeasure(CDC* pDC);
	void							DrawPixelInfo(CDC* pDC);

protected:
	BOOL							m_bDrawPixelInfo;

public:	// Mouse Position..
	CPoint							GetMouseOnImagePos() { return m_ptMousePos; };

protected:
	CPoint							m_ptMousePos;

public:		// User View Control..
	void							CalcuateImagePosToViewWndPos();
	CRect							CalcuateImagePosToViewWndPos(CRect rtImage);
	CPoint							CalcuateImagePosToViewWndPos(CPoint ptImagePos);
	CRect							CalcuateWndPosToImagePos(CRect rtWnd);
	CPoint							CalcuateWndPosToImagePos(CPoint ptWndPos);
	void							CheckModifyUserSelect(CPoint ptMousePos_wnd);

protected:
	BOOL							m_bDrawUserSelect;
	CRect							m_rtUserSelect_pxl;
	int								m_nUserSelect_Modify_type;

public:
	void							DrawDC(CDC* pDC);
	void							SaveDC(CString strFilePath);

public:
	afx_msg void OnPaint();
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDblClk(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnPopUpCommand_SaveImage();
	afx_msg void OnPopUpCommand_ViewZoomMode();
	afx_msg void OnPopUpCommand_ViewFitMode();
	afx_msg void OnPopUpCommand_DrawViewName();
	afx_msg void OnPopUpCommand_DrawCenterLine();
	afx_msg void OnPopUpCommand_DrawPixelInfo();
	afx_msg void OnPopUpCommand_DrawUserSelectMeasure();
};