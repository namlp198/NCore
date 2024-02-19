#include "pch.h"
#include "mvtype.h"

mvsol::MSizeD::MSizeD()
{
	cx = 0.0;
	cy = 0.0;
}

mvsol::MSizeD::MSizeD(MDOUBLE X, MDOUBLE Y)
{
	cx = X;
	cy = Y;
}

mvsol::MSizeD::MSizeD(SIZE size)
{
	cx = static_cast<MDOUBLE>(size.cx);
	cy = static_cast<MDOUBLE>(size.cy);
}

mvsol::MSizeD::MSizeD(MSIZED size)
{
	cx = size.cx;
	cy = size.cy;
}

mvsol::MSizeD::MSizeD(MPOINTD point)
{
	cx = point.x;
	cy = point.y;
}

MBOOL mvsol::MSizeD::operator==(MSIZED size) const
{
	return (cx == size.cx && cy == size.cy);
}

MBOOL mvsol::MSizeD::operator!=(MSIZED size) const
{
	return (cx != size.cx && cy != size.cy);
}

void mvsol::MSizeD::operator+=(MSIZED size)
{
	cx += size.cx;
	cy += size.cy;
}

void mvsol::MSizeD::operator-=(MSIZED size)
{
	cx -= size.cx;
	cy -= size.cy;
}

void mvsol::MSizeD::SetSize(MDOUBLE CX, MDOUBLE CY)
{
	cx = CX;
	cy = CY;
}

mvsol::MSizeD::operator CSize()
{
	return CSize(static_cast<int>(cx), static_cast<int>(cy));
}

mvsol::MSizeD::operator CSize() const
{
	return CSize(static_cast<int>(cx), static_cast<int>(cy));
}

mvsol::MSizeD mvsol::MSizeD::operator+(MSIZED size) const
{
	return MSizeD(cx + size.cx, cy + size.cy);
}

mvsol::MSizeD mvsol::MSizeD::operator-(MSIZED size) const
{
	return MSizeD(cx - size.cx, cy - size.cy);
}

mvsol::MSizeD mvsol::MSizeD::operator-() const
{
	return MSizeD(-cx, -cy);
}

mvsol::MSizeD mvsol::MSizeD::operator*(MSIZED size) const
{
	return MSizeD(cx * size.cx, cy * size.cy);
}

mvsol::MSizeD mvsol::MSizeD::operator/(MDOUBLE v) const
{
	return MSizeD(cx / v, cy / v);
}

mvsol::MSizeD mvsol::MSizeD::operator/(MSIZED size) const
{
	return MSizeD(cx / size.cx, cy / size.cy);
}

mvsol::MPointD mvsol::MSizeD::operator+(MPOINTD point) const
{
	return MPointD(cx + point.x, cy + point.y);
}

mvsol::MPointD mvsol::MSizeD::operator-(MPOINTD point) const
{
	return MPointD(cx - point.x, cy - point.y);
}

mvsol::MRectD mvsol::MSizeD::operator+(const MRECTD* lpRect) const
{
	return MRectD(lpRect) + *this;
}

mvsol::MRectD mvsol::MSizeD::operator-(const MRECTD* lpRect) const
{
	return MRectD(lpRect) - *this;
}

mvsol::MPointD::MPointD()
{
	x = 0.0;
	y = 0.0;
}

mvsol::MPointD::MPointD(MDOUBLE initX, MDOUBLE initY)
{
	x = initX;
	y = initY;
}

mvsol::MPointD::MPointD(POINT point)
{
	x = static_cast<MDOUBLE>(point.x);
	y = static_cast<MDOUBLE>(point.y);
}

mvsol::MPointD::MPointD(MPOINTD point)
{
	x = point.x;
	y = point.y;
}

mvsol::MPointD::MPointD(MSIZED size)
{
	x = size.cx;
	y = size.cy;
}

void mvsol::MPointD::Offset(MDOUBLE X, MDOUBLE Y)
{
	x += X;
	y += Y;
}

void mvsol::MPointD::Offset(MPOINTD point)
{
	x += point.x;
	y += point.y;
}

void mvsol::MPointD::Offset(MSIZED size)
{
	x += size.cx;
	y += size.cy;
}

void mvsol::MPointD::SetPoint(MDOUBLE X, MDOUBLE Y)
{
	x = X;
	y = Y;
}

mvsol::MPointD::operator CPoint()
{
	return CPoint(static_cast<int>(x), static_cast<int>(y));
}

mvsol::MPointD::operator CPoint() const
{
	return CPoint(static_cast<int>(x), static_cast<int>(y));
}

MBOOL mvsol::MPointD::operator==(MPOINTD point) const
{
	return (x == point.x && y == point.y);
}

MBOOL mvsol::MPointD::operator!=(MPOINTD point) const
{
	return (x != point.x && y != point.y);
}

void mvsol::MPointD::operator+=(MSIZED size)
{
	x += size.cx;
	y += size.cy;
}

void mvsol::MPointD::operator-=(MSIZED size)
{
	x -= size.cx;
	y -= size.cy;
}

void mvsol::MPointD::operator+=(MPOINTD point)
{
	x += point.x;
	y += point.y;
}

void mvsol::MPointD::operator-=(MPOINTD point)
{
	x -= point.x;
	y -= point.y;
}

mvsol::MPointD mvsol::MPointD::operator+(MSIZED size) const
{
	return MPointD(x + size.cx, y + size.cy);
}

mvsol::MPointD mvsol::MPointD::operator-(MSIZED size) const
{
	return MPointD(x - size.cx, y - size.cy);
}

mvsol::MPointD mvsol::MPointD::operator-() const
{
	return MPointD(-x, -y);
}

mvsol::MPointD mvsol::MPointD::operator+(MPOINTD point) const
{
	return MPointD(x + point.x, y + point.y);
}

mvsol::MSizeD mvsol::MPointD::operator-(MPOINTD point) const
{
	return MSizeD(x - point.x, y - point.y);
}

mvsol::MPointD mvsol::MPointD::operator*(MDOUBLE scale) const
{
	return MPointD(x * scale, y * scale);
}

mvsol::MPointD mvsol::MPointD::operator*(MSIZED scale) const
{
	return MPointD(x * scale.cx, y * scale.cy);
}

mvsol::MPointD mvsol::MPointD::operator*(MPOINTD scale) const
{
	return MPointD(x * scale.x, y * scale.y);
}

mvsol::MPointD mvsol::MPointD::operator/(MDOUBLE scale) const
{
	return MPointD(x / scale, y / scale);
}

mvsol::MPointD mvsol::MPointD::operator/(MPOINTD scale) const
{
	return MPointD(x / scale.x, y / scale.y);
}

mvsol::MPointD mvsol::MPointD::operator/(MSIZED scale) const
{
	return MPointD(x / scale.cx, y / scale.cy);
}

mvsol::MRectD mvsol::MPointD::operator+(const MRECTD* lpRect) const
{
	return MRectD(lpRect) + *this;
}

mvsol::MRectD mvsol::MPointD::operator-(const MRECTD* lpRect) const
{
	return MRectD(lpRect) - *this;
}

mvsol::MRectD::MRectD()
{
	left = 0.0;
	top = 0.0;
	right = 0.0;
	bottom = 0.0;
}

mvsol::MRectD::MRectD(MDOUBLE l, MDOUBLE t, MDOUBLE r, MDOUBLE b)
{
	left = l;
	top = t;
	right = r;
	bottom = b;
}

mvsol::MRectD::MRectD(const RECT& rect)
{
	left	= static_cast<MDOUBLE>(rect.left);
	top		= static_cast<MDOUBLE>(rect.top);
	right	= static_cast<MDOUBLE>(rect.right);
	bottom	= static_cast<MDOUBLE>(rect.bottom);
}

mvsol::MRectD::MRectD(const MRECTD& rect)
{
	left = rect.left;
	top = rect.top;
	right = rect.right;
	bottom = rect.bottom;
}

mvsol::MRectD::MRectD(MPOINTD topLeft, MPOINTD bottomRight)
{
	left = topLeft.x;
	top = topLeft.y;
	right = bottomRight.x;
	bottom = bottomRight.y;
}

mvsol::MRectD::MRectD(LPCRECT rect)
{
	left	= static_cast<MDOUBLE>(rect->left);
	top		= static_cast<MDOUBLE>(rect->top);
	right	= static_cast<MDOUBLE>(rect->right);
	bottom	= static_cast<MDOUBLE>(rect->bottom);
}

mvsol::MRectD::MRectD(LPCMRECTD rect)
{
	left = rect->left;
	top = rect->top;
	right = rect->right;
	bottom = rect->bottom;
}

mvsol::MRectD::MRectD(MPOINTD point, MSIZED size)
{
	left = point.x;
	top = point.y;
	right = point.x + size.cx;
	bottom = point.y + size.cy;
}

MDOUBLE mvsol::MRectD::Width() const
{
	return (right - left);
}

MDOUBLE mvsol::MRectD::Height() const
{
	return (bottom - top);
}

mvsol::MSizeD mvsol::MRectD::Size() const
{
	return MSizeD(right - left, bottom - top);
}

mvsol::MPointD& mvsol::MRectD::TopLeft()
{
	return *((MPointD*)this);
}

mvsol::MPointD& mvsol::MRectD::BottomRight()
{
	return *((MPointD*)this + 1);
}

mvsol::MPointD mvsol::MRectD::CenterPoint() const
{
	return MPointD((left + right) / 2, (top + bottom) / 2);
}

const mvsol::MPointD& mvsol::MRectD::TopLeft() const
{
	return *((MPointD*)this);
}

const mvsol::MPointD& mvsol::MRectD::BottomRight() const
{
	return *((MPointD*)this + 1);
}

void mvsol::MRectD::SwapLeftRight()
{
	MDOUBLE temp = left;
	left = right;
	right = temp;
}

mvsol::MRectD::operator mvsol::LPMRECTD()
{
	return this;
}

mvsol::MRectD::operator mvsol::LPCMRECTD() const
{
	return this;
}

MBOOL mvsol::MRectD::IsRectEmpty() const
{
	return (right - left == 0 || bottom - top == 0);
}

MBOOL mvsol::MRectD::IsRectNull() const
{
	return (left == 0.0 && top == 0.0 && right == 0.0 && bottom == 0.0);
}

MBOOL mvsol::MRectD::PtInRect(MPOINTD point) const
{
	return (left <= point.x && point.x <= right) &&
		(top <= point.y && point.y <= bottom);
}

void mvsol::MRectD::SetRect(MDOUBLE x1, MDOUBLE y1, MDOUBLE x2, MDOUBLE y2)
{
	left = x1;
	top = y1;
	right = x2;
	bottom = y2;
}

void mvsol::MRectD::SetRect(MPOINTD topLeft, MPOINTD bottomRight)
{
	left = topLeft.x;
	top = topLeft.y;
	right = bottomRight.x;
	bottom = bottomRight.y;
}

void mvsol::MRectD::SetRect(MPOINTD point, MSIZED size)
{
	left = point.x;
	top = point.y;
	right = point.x + size.cx;
	bottom = point.y + size.cy;
}

void mvsol::MRectD::SetRectCenterBase(MDOUBLE cx, MDOUBLE cy, MDOUBLE width, MDOUBLE height)
{
	left = cx - width / 2;
	top = cy - height / 2;
	right = left + width;
	bottom = top + height;
}

void mvsol::MRectD::SetRectCenterBase(MPOINTD point, MSIZED size)
{
	SetRectCenterBase(point.x, point.y, size.cx, size.cy);
}

void mvsol::MRectD::SetRectEmpty()
{
	left = 0.0;
	top = 0.0;
	right = 0.0;
	bottom = 0.0;
}

void mvsol::MRectD::CopyRect(LPCMRECTD lpRect)
{
	left = lpRect->left;
	top = lpRect->top;
	right = lpRect->right;
	bottom = lpRect->bottom;
}

MBOOL mvsol::MRectD::EqualRect(LPCMRECTD lpRect) const
{
	return (left == lpRect->left && top == lpRect->top && right == lpRect->right && bottom == lpRect->bottom);
}

void mvsol::MRectD::InflateRect(MDOUBLE x, MDOUBLE y)
{
	InflateRect(x, y, x, y);
}

void mvsol::MRectD::InflateRect(MSIZED size)
{
	InflateRect(size.cx, size.cy);
}

void mvsol::MRectD::InflateRect(LPCMRECTD lpRect)
{
	InflateRect(lpRect->left, lpRect->top, lpRect->right, lpRect->bottom);
}

void mvsol::MRectD::InflateRect(MDOUBLE l, MDOUBLE t, MDOUBLE r, MDOUBLE b)
{
	left -= l;
	top -= t;
	right += r;
	bottom += b;
}

void mvsol::MRectD::DeflateRect(MDOUBLE x, MDOUBLE y)
{
	DeflateRect(x, y, x, y);
}

void mvsol::MRectD::DeflateRect(MSIZED size)
{
	DeflateRect(size.cx, size.cy);
}

void mvsol::MRectD::DeflateRect(LPCMRECTD lpRect)
{
	DeflateRect(lpRect->left, lpRect->top, lpRect->right, lpRect->bottom);
}

void mvsol::MRectD::DeflateRect(MDOUBLE l, MDOUBLE t, MDOUBLE r, MDOUBLE b)
{
	left += l;
	top += t;
	right -= r;
	bottom -= b;
}

void mvsol::MRectD::OffsetRect(MDOUBLE x, MDOUBLE y)
{
	left += x;
	top += y;
	right += x;
	bottom += y;
}

void mvsol::MRectD::OffsetRect(MSIZED size)
{
	OffsetRect(size.cx, size.cy);
}

void mvsol::MRectD::OffsetRect(MPOINTD point)
{
	OffsetRect(point.x, point.y);
}

void mvsol::MRectD::NormalizeRect()
{
	if (left > right)
	{
		swap<MDOUBLE>(left, right);
	}
	if (top > bottom)
	{
		swap<MDOUBLE>(top, bottom);
	}
}

mvsol::MRectD& mvsol::MRectD::MoveToY(MDOUBLE y)
{
	return MoveToXY(left, y);
}

mvsol::MRectD& mvsol::MRectD::MoveToX(MDOUBLE x)
{
	return MoveToXY(x, top);
}

mvsol::MRectD& mvsol::MRectD::MoveToXY(MDOUBLE x, MDOUBLE y)
{
	MSizeD size = Size();
	left = x;
	top = y;
	right = x + size.cx;
	bottom = y + size.cy;
	return *this;
}

mvsol::MRectD& mvsol::MRectD::MoveCenterToXY(MPOINTD point)
{
	return MoveToXY(MPointD(point) - MSizeD(Width(), Height()) / 2);
}

mvsol::MRectD& mvsol::MRectD::MoveCenterToXY(MDOUBLE x, MDOUBLE y)
{
	return MoveToXY(x - Width() / 2, y - Height() / 2);
}

mvsol::MRectD& mvsol::MRectD::MoveToXY(MPOINTD point)
{
	return MoveToXY(point.x, point.y);
}

MBOOL mvsol::MRectD::IntersectRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2)
{
	MRectD rect1(lpRect1), rect2(lpRect2);
	rect1.NormalizeRect();
	rect2.NormalizeRect();

	left = max(rect1.left, rect2.left);
	top = max(rect1.top, rect2.top);
	right = min(rect1.right, rect2.right);
	bottom = min(rect1.bottom, rect2.bottom);

	return (rect1.left < rect2.right &&
		rect1.top < rect2.bottom &&
		rect1.right > rect2.left &&
		rect1.bottom > rect2.bottom);
}

MBOOL mvsol::MRectD::UnionRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2)
{
	MRectD rect1(lpRect1), rect2(lpRect2);
	rect1.NormalizeRect();
	rect2.NormalizeRect();

	left = min(rect1.left, rect2.left);
	top = min(rect1.top, rect2.top);
	right = max(rect1.right, rect2.right);
	bottom = max(rect1.bottom, rect2.bottom);

	return IsRectEmpty();
}

mvsol::MRectD::operator CRect()
{
	return CRect(
		static_cast<int>(left),
		static_cast<int>(top),
		static_cast<int>(right),
		static_cast<int>(bottom));
}

mvsol::MRectD::operator CRect() const
{
	return CRect(
		static_cast<int>(left),
		static_cast<int>(top),
		static_cast<int>(right),
		static_cast<int>(bottom));
}

//MBOOL mvsol::MRectD::SubtractRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2)
//{
//
//}

void mvsol::MRectD::operator=(const MRECTD& srcRect)
{
	SetRect(srcRect.left, srcRect.top, srcRect.right, srcRect.bottom);
}

MBOOL mvsol::MRectD::operator==(const MRECTD& rect) const
{
	return (
		left == rect.left &&
		top == rect.top &&
		right == rect.right &&
		bottom == rect.bottom);
}

MBOOL mvsol::MRectD::operator!=(const MRECTD& rect) const
{
	return (
		left != rect.left &&
		top != rect.top &&
		right != rect.right &&
		bottom != rect.bottom);
}

void mvsol::MRectD::operator+=(MPOINTD point)
{
	OffsetRect(point);
}

void mvsol::MRectD::operator+=(MSIZED size)
{
	OffsetRect(size);
}

void mvsol::MRectD::operator+=(LPCMRECTD lpRect)
{
	InflateRect(lpRect);
}

void mvsol::MRectD::operator-=(MPOINTD point)
{
	OffsetRect(-point.x, -point.y);
}

void mvsol::MRectD::operator-=(MSIZED size)
{
	OffsetRect(-size.cx, -size.cy);
}

void mvsol::MRectD::operator-=(LPCMRECTD lpRect)
{
	DeflateRect(lpRect);
}

void mvsol::MRectD::operator&=(const MRECTD& rect)
{
	IntersectRect(this, &rect);
}

void mvsol::MRectD::operator|=(const MRECTD& rect)
{
	UnionRect(this, &rect);
}

mvsol::MRectD mvsol::MRectD::operator+(LPCMRECTD lpRect) const
{
	MRectD rect(this);
	rect.InflateRect(lpRect);
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator-(LPCMRECTD lpRect) const
{
	MRectD rect(this);
	rect.DeflateRect(lpRect);
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator&(const MRECTD& rect2) const
{
	MRectD rect;
	rect.IntersectRect(this, &rect2);
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator|(const MRECTD& rect2) const
{
	MRectD rect;
	rect.UnionRect(this, &rect2);
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator+(MPOINTD point)
{
	MRectD rect(*this);
	rect.left += point.x;
	rect.top += point.y;
	rect.right += point.x;
	rect.bottom += point.y;

	return rect;
}

mvsol::MRectD mvsol::MRectD::operator-(MPOINTD point)
{
	MRectD rect(*this);
	rect.left -= point.x;
	rect.top -= point.y;
	rect.right -= point.x;
	rect.bottom -= point.y;
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator+(MSIZED size)
{
	MRectD rect(*this);
	rect.left += size.cx;
	rect.top += size.cy;
	rect.right += size.cx;
	rect.bottom += size.cy;
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator-(MSIZED size)
{
	MRectD rect(*this);
	rect.left -= size.cx;
	rect.top -= size.cy;
	rect.right -= size.cx;
	rect.bottom -= size.cy;
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator*(MPOINTD point)
{
	MRectD rect(*this);
	rect.left *= point.x;
	rect.top *= point.y;
	rect.right *= point.x;
	rect.bottom *= point.y;
	return rect;
}

mvsol::MRectD mvsol::MRectD::operator/(MPOINTD point)
{
	MRectD rect(*this);
	rect.left /= point.x;
	rect.top /= point.y;
	rect.right /= point.x;
	rect.bottom /= point.y;
	return rect;
}

mvsol::MArcDBase::MArcDBase()
{
	cx = 0.0;
	cy = 0.0;
	rx = 0.0;
	ry = 0.0;
	st = 0.0;
	et = 360.0;
	xt = 0.0;
}

mvsol::MArcDBase::MArcDBase(MDOUBLE CX, MDOUBLE CY, MDOUBLE RX, MDOUBLE RY, MDOUBLE ST, MDOUBLE ET, MDOUBLE XT)
{
	cx = CX;
	cy = CY;
	rx = RX;
	ry = RY;
	st = ST;
	et = ET;
	xt = XT;
}

void mvsol::MArcDBase::SetArc(MDOUBLE CX, MDOUBLE CY, MDOUBLE RX, MDOUBLE RY, MDOUBLE ST, MDOUBLE ET, MDOUBLE XT)
{
	cx = CX;
	cy = CY;
	rx = RX;
	ry = RY;
	st = ST;
	et = ET;
	xt = XT;
}

mvsol::MArcD::MArcD()
{

}

mvsol::MArcD::MArcD(MDOUBLE CX, MDOUBLE CY, MDOUBLE RX, MDOUBLE RY, MDOUBLE XT)
	: MArcDBase(CX, CY, RX, RY, 0.0, 360.0, XT)
{

}

mvsol::MRectD mvsol::MArcD::GetRect() const
{
	MDOUBLE diff = rx - ry;
	MDOUBLE width = (rx - diff * sin(MDEG2RAD(xt))) * 2;
	MDOUBLE height = (ry + diff * sin(MDEG2RAD(xt))) * 2;
	
	MRectD rect;
	rect.SetRectCenterBase(cx, cy, width, height);
	return rect;
}

mvsol::MRangeD::MRangeD()
{
	min = 0.0;
	max = 0.0;
}

mvsol::MRangeD::MRangeD(MDOUBLE initMin, MDOUBLE initMax)
{
	min = initMin;
	max = initMax;
}

void mvsol::MRangeD::SetRange(MDOUBLE MIN, MDOUBLE MAX)
{
	min = MIN;
	max = MAX;
}

mvsol::MRangeD::operator mvsol::LPMRANGED()
{
	return this;
}

mvsol::MRangeD::operator mvsol::LPCMRANGED() const
{
	return this;
}

MBOOL mvsol::MRangeD::IsEmpty() const
{
	return (min == 0.0 && max == 0.0);
}

void mvsol::MRangeD::SetEmpty()
{
	min = 0.0;
	max = 0.0;
}

mvsol::MColor::operator MCOLOR()
{
	return color;
}
