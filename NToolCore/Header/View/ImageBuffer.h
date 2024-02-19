#pragma once

#include "cxcore.h"
#include "cv.h"
#include "highgui.h"

enum ImageBandType { ImageBand_Red = 0, ImageBand_Green, ImageBand_Blue, ImageBand_Gray, ImageBand_Color, ImageBand_Count };

class AFX_EXT_CLASS CImageBuffer
{
public:
	CImageBuffer(void);
	CImageBuffer(int nWidth, int nHeight, int nDepth = 8, int nChannels = 1);
	virtual ~CImageBuffer(void);

public: // virtual functions
	virtual BOOL	CreateImage(int nWidth, int nHeight, int nDepth = 8, int nChannels = 1);
	virtual BOOL	CreateImageHeader(int nWidth, int nHeight, int nDepth = 8, int nChannels = 1);
	virtual BOOL	GetImageExist(void) const;
	virtual BOOL	GetAllocatedData(void) const;

	virtual void	ReleaseImage(void);
	virtual int		GetHeight(void) const;
	virtual int		GetWidth(void) const;
	virtual int		GetWidthStep(void) const;
	virtual int		GetDepth(void) const;
	virtual int		GetChannels(void) const;
	virtual int		GetBpp(void) const;
	virtual int		GetImageSize(void) const;

	virtual char* GetImageBuffer(void);
	virtual char* GetImageBufferOrigin(void);
	virtual const char* GetImageBuffer(void) const;
	virtual const char* GetImageBufferOrigin(void) const;
	virtual int		GetPixelValue(int x, int y) const;
	virtual int		GetPixelValue(int x, int y, int& nR, int& nG, int& nB) const;
	virtual int		SetPixelValue(int x, int y, int nR, int nG, int nB);
	virtual int		SetPixelValue(int x, int y, int nR, int nG, int nB, int nA);

	virtual BOOL	LoadImage(const CString& strFilename);
	virtual BOOL	SaveImage(const CString& strFilename, int nQiality = 95) const;

public: // common functions
	// clear
	BOOL	ClearImage(int nValue);
	BOOL	ClearImage(int nValue, int nA);

	// copy func
	BOOL	CopyImageTo(CImageBuffer* pToImage) const;
	BOOL	CopyImageTo(IplImage* pToImage) const;
	BOOL	CopyImageFrom(const CImageBuffer* pFromImage);
	BOOL	CopyImageFrom(const IplImage* pFromImage);

	// show func
	void	ShowImage(HDC hDC, const CRect& dstRect);
	void	ShowImage(HDC hDC, const CRect& srcRect, const CRect& dstRect);
	void	ShowImage(HDC hDC, int nSx, int nSy, int nWidth, int nHeight, int nFromX, int nFromY);

	// get image data
	BOOL	GetSubImage(int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pGetImage);
	BOOL	GetSubImage(int nSx, int nSy, int nWidth, int nHeight, int nChannels, BYTE* pGetBuffer);
	BOOL	GetSubImage(const CRect& rtRect, CImageBuffer* pGetImage);
	BOOL	GetSubImage(const CRect& rtRect, int nChannels, BYTE* pGetBuffer);

	BOOL	GetBandImage(int nColorBand, CImageBuffer* pGetImage) const;
	BOOL	GetBandImage(int nColorBand, int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pGetImage);
	BOOL	GetBandImage(int nColorBand, int nSx, int nSy, int nWidth, int nHeight, BYTE* pGetBuffer);
	BOOL	GetBandImage(int nColorBand, const CRect& rtRect, CImageBuffer* pGetImage);
	BOOL	GetBandImage(int nColorBand, const CRect& rtRect, BYTE* pGetBuffer);

	// set image data
	BOOL	SetSubImage(int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pSetImage);
	BOOL	SetSubImage(int nSx, int nSy, int nWidth, int nHeight, int nBpp, BYTE* pSetBuffer);
	BOOL	SetSubImage(const CRect& rtRect, CImageBuffer* pSetImage);
	BOOL	SetSubImage(const CRect& rtRect, int nChannels, BYTE* pSetBuffer);

	// draw function
	BOOL	DrawLine(CPoint ptPos1, CPoint ptPos2, DWORD nColor, int nThickness = 1, int nLineType = 8, int nShift = 0);
	BOOL	DrawRectangle(CPoint ptPos1, CPoint ptPos2, DWORD nColor, int nThickness = 1, int nLineType = 8, int nShift = 0);
	BOOL	DrawCircle(CPoint ptCenter, int nRadius, DWORD nColor, int nThickness = 1, int nLineType = 8, int nShift = 0);
	BOOL	DrawEllipse(CPoint ptCenter, CPoint ptSize, double dAngle, DWORD nColor, int nThickness = 1, int nLineType = 8, int nShift = 0);
	BOOL	DrawText(CPoint ptPoint, DWORD nColor, const CString& strText);
	BOOL	DrawText(CPoint ptPoint, DWORD nColor, double dScale,const CString& strText);

	// buffer access
	IplImage* GetIplImage(void);
	BOOL				SetImageBufferPtr(char* pPtr);

	BOOL			DIBtoIplImage(HBITMAP& hbmp, HDC hdc = NULL);
	BOOL			DCtoIplImage(CDC* pDC, const CRect& rect);

	// 4Ch Image..
	CvScalar GetPixelScalar(int x, int y);

protected:
	static void		FillBitmapInfo(BITMAPINFO* bmi, int width, int height, int bpp, int origin);

	BOOL		m_bAllocatedData;

public:
	IplImage* m_pImageData;
};

