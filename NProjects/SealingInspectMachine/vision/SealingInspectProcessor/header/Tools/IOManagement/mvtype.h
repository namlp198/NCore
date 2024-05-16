#pragma once

typedef					bool		MVSOL_BOOL;

#ifdef _UNICODE
typedef					wchar_t		MVSOL_CHAR;
#else
typedef					char		MVSOL_CHAR;
#endif
typedef					short		MVSOL_SHORT;
typedef					int			MVSOL_INT;
typedef					long		MVSOL_LONG;
typedef					long long	MVSOL_LONGLONG;

typedef		unsigned	char		MVSOL_UCHAR;
typedef		unsigned	short		MVSOL_USHORT;
typedef		unsigned	int			MVSOL_UINT;
typedef		unsigned	long		MVSOL_ULONG;
typedef		unsigned	long long	MVSOL_ULONGLONG;

typedef					float		MVSOL_FLOAT;
typedef					double		MVSOL_DOUBLE;

typedef MVSOL_BOOL		MBOOL;
typedef MVSOL_UCHAR		MBYTE;
typedef MVSOL_CHAR		MCHAR;

typedef MVSOL_FLOAT		MFLOAT;
typedef MVSOL_DOUBLE	MDOUBLE;

typedef MVSOL_CHAR		MINT8;
typedef MVSOL_SHORT		MINT16;
typedef MVSOL_LONG		MINT32;
typedef MVSOL_LONGLONG	MINT64;

typedef MVSOL_UCHAR		MUINT8;
typedef MVSOL_USHORT	MUINT16;
typedef MVSOL_ULONG		MUINT32;
typedef MVSOL_ULONGLONG MUINT64;

typedef	MVSOL_INT		MINT;
typedef MVSOL_UINT		MUINT;

typedef MVSOL_LONG		MLONG;
typedef MVSOL_ULONG		MULONG;

typedef CPoint			MPOINT;
typedef CSize			MSIZE;
typedef CRect			MRECT;

#include <vector>
typedef std::vector<MINT>		MINTLIST;
typedef std::vector<MDOUBLE>	MDOUBLELIST;
typedef COLORREF				MCOLOR;

#define MNULL					nullptr

#ifndef MPI
#define MPI 3.14159265358979323846   // pi
#endif
#define MDEG2RAD(x) ((x*MPI)/180.)

namespace mvsol
{

	//юс╫ц
	template<typename T>
	void swap(T& a, T& b)
	{
		T t = a;
		a = b;
		b = t;
	}

	typedef CPoint	MPoint;
	typedef CSize	MSize;
	typedef CRect	MRect;

	typedef struct tagMPOINTD
	{
		MDOUBLE x;
		MDOUBLE y;
	}MPOINTD, *PMPOINTD, far *LPMPOINTD;
	typedef const MPOINTD far *LPCMPOINTD;

	typedef struct tagMSIZED
	{
		MDOUBLE cx;
		MDOUBLE cy;
	}MSIZED, *PMSIZED, far *LPMSIZED;
	typedef const MSIZED far * LPCMSIZED;

	typedef struct tagMRECTD
	{
		MDOUBLE left;
		MDOUBLE top;
		MDOUBLE right;
		MDOUBLE bottom;
	}MRECTD, *PMRECTD, far *LPMRECTD;
	typedef const MRECTD far * LPCMRECTD;

	typedef struct tagMARCD
	{
		MDOUBLE cx;			//CETNER X
		MDOUBLE cy;			//CENTER Y
		MDOUBLE rx;			//XAxis Radius
		MDOUBLE ry;			//YAxis Radius
		MDOUBLE st;			//Start Angle
		MDOUBLE et;			//End Angle
		MDOUBLE xt;			//XAxis Angle
	}MARCD, *PMARCD, far *LPMARCD;
	typedef const MARCD far *LPCMARCD;

	typedef struct tagMRANGED
	{
		MDOUBLE min;
		MDOUBLE max;
	}MRANGED, *PMRANGED, far *LPMRANGED;
	typedef const MRANGED far * LPCMRANGED;

	class MColor
	{
	public:
		MColor() { color = 0; }
		MColor(MCOLOR initColor) { color = initColor; }
		MColor(MBYTE r, MBYTE g, MBYTE b) { color = RGB(r, g, b); }

		MBYTE Red() { return GetRValue(color); }
		MBYTE Green() { return GetGValue(color); }
		MBYTE Blue() { return GetBValue(color); }
		MBYTE Intensity() { return (BYTE)((MDOUBLE)Red() * 0.2126 + (MDOUBLE)Green() * 0.7152 + (MDOUBLE)Blue() * 0.0722); }

		void operator=(MCOLOR value) { color = value; }
		operator MCOLOR();

		static MCOLOR MakeRGB(MBYTE r, MBYTE g, MBYTE b) { return RGB(r, g, b); }

		MCOLOR color;
	};

	class MSizeD;
	class MPointD;
	class MRectD;
	class MArcD;
	class MRangeD;

	class MSizeD
		: public tagMSIZED
	{
	public:
		MSizeD();
		MSizeD(MDOUBLE X, MDOUBLE Y);
		MSizeD(SIZE size);
		MSizeD(MSIZED size);
		MSizeD(MPOINTD point);

		MBOOL operator==(MSIZED size) const;
		MBOOL operator!=(MSIZED size) const;
		void operator+=(MSIZED size);
		void operator-=(MSIZED size);
		void SetSize(MDOUBLE CX, MDOUBLE CY);

		operator CSize();
		operator CSize() const;

		MSizeD operator+(MSIZED size) const;
		MSizeD operator-(MSIZED size) const;
		MSizeD operator-() const;
		MSizeD operator*(MSIZED size) const;
		MSizeD operator/(MSIZED size) const;
		MSizeD operator/(MDOUBLE v) const;

		// Operators returning CPoint values
		MPointD operator+(MPOINTD point) const;
		MPointD operator-(MPOINTD point) const;

		// Operators returning CRect values
		MRectD operator+(const MRECTD* lpRect) const;
		MRectD operator-(const MRECTD* lpRect) const;
	};

	class MPointD
		: public tagMPOINTD
	{
	public:

		MPointD();
		MPointD(MDOUBLE initX, MDOUBLE initY);
		MPointD(POINT point);
		MPointD(MPOINTD point);
		MPointD(MSIZED size);

		void Offset(MDOUBLE X, MDOUBLE Y);
		void Offset(MPOINTD point);
		void Offset(MSIZED size);
		void SetPoint(MDOUBLE X, MDOUBLE Y);

		operator CPoint();
		operator CPoint() const;

		MBOOL	operator==(MPOINTD point) const;
		MBOOL	operator!=(MPOINTD point) const;
		void	operator+=(MSIZED size);
		void	operator-=(MSIZED size);
		void	operator+=(MPOINTD point);
		void	operator-=(MPOINTD point);

		MPointD operator+(MSIZED size) const;
		MPointD operator-(MSIZED size) const;
		MPointD operator-() const;
		MPointD operator+(MPOINTD point) const;
		MPointD operator*(MDOUBLE scale) const;
		MPointD operator*(MPOINTD scale) const;
		MPointD operator*(MSIZED scale) const;
		MPointD operator/(MDOUBLE scale) const;
		MPointD operator/(MPOINTD scale) const;
		MPointD operator/(MSIZED scale) const;

		// Operators returning CSize values
		MSizeD operator-(MPOINTD point) const;

		// Operators returning CRect values
		MRectD operator+(const MRECTD* lpRect) const;
		MRectD operator-(const MRECTD* lpRect) const;
	};


	class MRectD
		: public tagMRECTD
	{
	public:
		MRectD();
		MRectD(
			MDOUBLE l, 
			MDOUBLE t, 
			MDOUBLE r, 
			MDOUBLE b);
		MRectD(const RECT& rect);
		MRectD(const MRECTD& rect);

		MRectD(LPCRECT rect);
		MRectD(LPCMRECTD rect);

		MRectD(MPOINTD point, MSIZED size);

		MRectD(MPOINTD topLeft, MPOINTD bottomRight);

		MDOUBLE Width() const;
		MDOUBLE Height() const;

		MSizeD Size() const;
		MPointD& TopLeft();
		MPointD& BottomRight();
		const MPointD& TopLeft() const;
		const MPointD& BottomRight() const;
		MPointD CenterPoint() const;
		void SwapLeftRight();

		operator LPMRECTD();
		operator LPCMRECTD() const;

		MBOOL IsRectEmpty() const;
		MBOOL IsRectNull() const;
		MBOOL PtInRect(MPOINTD point) const;

		void SetRect(
			MDOUBLE x1,
			MDOUBLE y1,
			MDOUBLE x2,
			MDOUBLE y2);
		void SetRect(MPOINTD topLeft, MPOINTD bottomRight);
		void SetRect(MPOINTD point, MSIZED size);
		void SetRectCenterBase(MPOINTD point, MSIZED size);
		void SetRectCenterBase(MDOUBLE cx, MDOUBLE cy, MDOUBLE width, MDOUBLE height);
		void SetRectEmpty();

		void CopyRect(LPCMRECTD lpRect);
		MBOOL EqualRect(LPCMRECTD lpRect) const;

		
		void InflateRect(MDOUBLE x, MDOUBLE y);
		void InflateRect(MSIZED size);
		void InflateRect(LPCMRECTD lpRect);
		void InflateRect(
			MDOUBLE l,
			MDOUBLE t,
			MDOUBLE r,
			MDOUBLE b);

		void DeflateRect(MDOUBLE x, MDOUBLE y);
		void DeflateRect(MSIZED size);
		void DeflateRect(LPCMRECTD lpRect);
		void DeflateRect(
			MDOUBLE l,
			MDOUBLE t,
			MDOUBLE r,
			MDOUBLE b);

		void OffsetRect(MDOUBLE x, MDOUBLE y);
		void OffsetRect(MSIZED size);
		void OffsetRect(MPOINTD point);
		void NormalizeRect();

		MRectD& MoveToY(MDOUBLE y);
		MRectD& MoveToX(MDOUBLE x);
		MRectD& MoveToXY(MDOUBLE x, MDOUBLE y);
		MRectD& MoveToXY(MPOINTD point);
		MRectD& MoveCenterToXY(MDOUBLE x, MDOUBLE y);
		MRectD& MoveCenterToXY(MPOINTD point);

		MBOOL IntersectRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2);
		MBOOL UnionRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2);
		//MBOOL SubtractRect(LPCMRECTD lpRect1, LPCMRECTD lpRect2);

		operator CRect();
		operator CRect() const;

		void  operator=(const MRECTD& srcRect);
		MBOOL operator==(const MRECTD& rect) const;
		MBOOL operator!=(const MRECTD& rect) const;
		void  operator+=(MPOINTD point);
		void  operator+=(MSIZED size);
		void  operator+=(LPCMRECTD lpRect);
		void  operator-=(MPOINTD point);
		void  operator-=(MSIZED size);
		void  operator-=(LPCMRECTD lpRect);
		void  operator&=(const MRECTD& rect);
		void  operator|=(const MRECTD& rect);

		MRectD operator+(MPOINTD point);
		MRectD operator-(MPOINTD point);
		MRectD operator+(MSIZED size);
		MRectD operator-(MSIZED size);
		MRectD operator+(LPCMRECTD lpRect) const;
		MRectD operator-(LPCMRECTD lpRect) const;
		MRectD operator&(const MRECTD& rect2) const;
		MRectD operator|(const MRECTD& rect2) const;
		MRectD operator*(MPOINTD point);
		MRectD operator/(MPOINTD point);
	};

	class MArcDBase
		: public tagMARCD
	{
	public:
		MArcDBase();
		MArcDBase(
			MDOUBLE CX,
			MDOUBLE CY,
			MDOUBLE RX,
			MDOUBLE RY,
			MDOUBLE ST,
			MDOUBLE ET,
			MDOUBLE XT);

		void SetArc(
			MDOUBLE CX,
			MDOUBLE CY,
			MDOUBLE RX,
			MDOUBLE RY,
			MDOUBLE ST,
			MDOUBLE ET,
			MDOUBLE XT);
	};

	class MArcD
		: public MArcDBase
	{
	public:
		MArcD();
		MArcD(
			MDOUBLE CX,
			MDOUBLE CY,
			MDOUBLE RX,
			MDOUBLE RY,
			MDOUBLE XT);

		MRectD GetRect() const;

	};

	class MRangeD
		: public tagMRANGED
	{
	public:
		MRangeD();
		MRangeD(
			MDOUBLE initMin,
			MDOUBLE initMax);

		void SetRange(MDOUBLE MIN, MDOUBLE MAX);

		operator LPMRANGED();
		operator LPCMRANGED() const;
		
		MBOOL IsEmpty() const;
		void SetEmpty();
	};

	typedef MRangeD MSpecD;
}