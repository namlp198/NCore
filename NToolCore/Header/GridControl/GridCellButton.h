#ifndef __GRID_CELL_BUTTON__
#define __GRID_CELL_BUTTON__

#include "GridCell.h"

class CGridCellButton : public CGridCell
{
	friend class CGridCtrl;
	DECLARE_DYNCREATE(CGridCellButton)

public:
	CGridCellButton(void);
	virtual ~CGridCellButton(void);

	void SetPushing(BOOL bPush) {m_bPushing = bPush;}

public:
	virtual BOOL Draw(CDC* pDC, int nRow, int nCol, CRect rect, BOOL bEraseBkgnd = TRUE);

protected:
	virtual void OnClick(CPoint PointCellRelative);
	virtual void OnMouseLeave();

protected:
	CRect m_rect;
	BOOL m_bPushing;
};
#endif
