#include "pch.h"
#include "ImageBuffer.h"

CImageBuffer::CImageBuffer(void)
{
	m_bAllocatedData = FALSE;
	m_pImageData = NULL;
}

CImageBuffer::CImageBuffer(int nWidth, int nHeight, int nDepth, int nChannels)
{
	/* depth info
	IPL_DEPTH_8U - unsigned 8-bit integers
	IPL_DEPTH_8S - signed 8-bit integers
	IPL_DEPTH_16U - unsigned 16-bit integers
	IPL_DEPTH_16S - signed 16-bit integers
	IPL_DEPTH_32S - signed 32-bit integers
	IPL_DEPTH_32F - single precision floating-point numbers
	IPL_DEPTH_64F - double precision floating-point numbers
	*/
	m_pImageData = cvCreateImage(cvSize(nWidth, nHeight), nDepth, nChannels);
	if (m_pImageData) m_bAllocatedData = TRUE;
}

CImageBuffer::~CImageBuffer(void)
{
	ReleaseImage();
}

BOOL CImageBuffer::GetImageExist(void) const
{
	return m_pImageData ? TRUE : FALSE;
}

BOOL CImageBuffer::GetAllocatedData(void) const
{
	return m_bAllocatedData;
}

int CImageBuffer::GetHeight(void) const
{
	return GetImageExist() ? m_pImageData->height : 0;
}

int CImageBuffer::GetWidth(void) const
{
	return GetImageExist() ? m_pImageData->width : 0;
}

int CImageBuffer::GetWidthStep(void) const
{
	return GetImageExist() ? m_pImageData->widthStep : 0;
}

int CImageBuffer::GetDepth(void) const
{
	return GetImageExist() ? m_pImageData->depth : 0;
}

int CImageBuffer::GetChannels(void) const
{
	return GetImageExist() ? m_pImageData->nChannels : 0;
}

int	CImageBuffer::GetBpp(void) const
{
	return GetImageExist() ? (m_pImageData->depth & 255) * m_pImageData->nChannels : 0;
}

int	CImageBuffer::GetImageSize(void) const
{
	return GetImageExist() ? m_pImageData->imageSize : 0;
}

int	CImageBuffer::GetPixelValue(int x, int y) const
{
	if (!GetImageExist()) return -1;

	if (GetDepth() != IPL_DEPTH_8U) return -1;

	if (GetWidth() <= x || x < 0 || GetHeight() <= y || y < 0)	return  -1;

	CvScalar sValue = cvGet2D(m_pImageData, y, x);

	return int(sValue.val[0]);
}

int CImageBuffer::GetPixelValue(int x, int y, int& nR, int& nG, int& nB) const
{
	if (!GetImageExist()) return -1;

	if (GetDepth() != IPL_DEPTH_8U) return -1;

	if (GetWidth() <= x || x < 0 || GetHeight() <= y || y < 0)	return  -1;

	CvScalar sValue = cvGet2D(m_pImageData, y, x);

	int nSum = 0;

	nSum += nR = sValue.val[0];
	nSum += nG = sValue.val[1];
	nSum += nB = sValue.val[2];

	return int(nSum / 3);
}

int	CImageBuffer::SetPixelValue(int x, int y, int nR, int nG, int nB)
{
	if (!GetImageExist()) return 0;

	if (GetDepth() != IPL_DEPTH_8U) return 0;

	CvScalar sValue;
	sValue.val[0] = nR;
	sValue.val[1] = nG;
	sValue.val[2] = nB;

	cvSet2D(m_pImageData, y, x, sValue);

	return 1;
}

int CImageBuffer::SetPixelValue(int x, int y, int nR, int nG, int nB, int nA)
{
	if (!GetImageExist()) return 0;

	if (GetDepth() != IPL_DEPTH_8U) return 0;

	CvScalar sValue;
	sValue.val[0] = nR;
	sValue.val[1] = nG;
	sValue.val[2] = nB;
	sValue.val[3] = nA;

	cvSet2D(m_pImageData, y, x, sValue);

	return 1;
}


BOOL CImageBuffer::LoadImage(const CString& strFilename)
{
	ReleaseImage();

	USES_CONVERSION;
	char str_filename[1024];
	sprintf_s(str_filename, "%s", W2A(strFilename));

	m_pImageData = cvLoadImage(str_filename, CV_LOAD_IMAGE_ANYCOLOR);

	if (m_pImageData == NULL) return FALSE;
	m_bAllocatedData = TRUE;

	return TRUE;
}

BOOL CImageBuffer::SaveImage(const CString& strFilename, int nQiality) const
{
	if (!GetImageExist()) return FALSE;

	// ext check
	int pos = strFilename.ReverseFind(_T('.'));
	if (pos == -1) return FALSE;

	CString extName = strFilename.Mid(pos + 1, 3);

	USES_CONVERSION;
	char str_filename[1024];
	sprintf_s(str_filename, "%s", W2A(strFilename));

	int nReturnValue = 0;

	if (extName.CompareNoCase(_T("bmp")) == 0)
	{
		nReturnValue = cvSaveImage(str_filename, m_pImageData);
	}
	else if (extName.CompareNoCase(_T("jpg")) == 0)
	{
		int param[3];
		param[0] = CV_IMWRITE_JPEG_QUALITY;
		param[1] = nQiality;
		param[2] = 0;
		nReturnValue = cvSaveImage(str_filename, m_pImageData, param);
	}
	else	// PNG 저장을 위해..
	{
		nReturnValue = cvSaveImage(str_filename, m_pImageData);
	}

	if (nReturnValue == 0) return FALSE;

	return TRUE;
}

void CImageBuffer::ReleaseImage(void)
{
	if (m_pImageData != NULL)
	{
		if (m_bAllocatedData)
			cvReleaseImage(&m_pImageData);
		else
			cvReleaseImageHeader(&m_pImageData);
	}
	m_bAllocatedData = FALSE;
	m_pImageData = NULL;
}

BOOL CImageBuffer::CreateImage(int nWidth, int nHeight, int nDepth, int nChannels)
{
	/* depth info
	IPL_DEPTH_8U - unsigned 8-bit integers
	IPL_DEPTH_8S - signed 8-bit integers
	IPL_DEPTH_16U - unsigned 16-bit integers
	IPL_DEPTH_16S - signed 16-bit integers
	IPL_DEPTH_32S - signed 32-bit integers
	IPL_DEPTH_32F - single precision floating-point numbers
	IPL_DEPTH_64F - double precision floating-point numbers
	*/

	ReleaseImage();
	m_pImageData = cvCreateImage(cvSize(nWidth, nHeight), nDepth, nChannels);
	if (m_pImageData == NULL) return FALSE;
	m_bAllocatedData = TRUE;

	return TRUE;
}

BOOL CImageBuffer::CreateImageHeader(int nWidth, int nHeight, int nDepth/*=8*/, int nChannels/*=1*/)
{
	/* depth info
	IPL_DEPTH_8U - unsigned 8-bit integers
	IPL_DEPTH_8S - signed 8-bit integers
	IPL_DEPTH_16U - unsigned 16-bit integers
	IPL_DEPTH_16S - signed 16-bit integers
	IPL_DEPTH_32S - signed 32-bit integers
	IPL_DEPTH_32F - single precision floating-point numbers
	IPL_DEPTH_64F - double precision floating-point numbers
	*/

	ReleaseImage();
	m_pImageData = cvCreateImageHeader(cvSize(nWidth, nHeight), nDepth, nChannels);
	if (m_pImageData == NULL) return FALSE;
	m_bAllocatedData = FALSE;

	return TRUE;
}

char* CImageBuffer::GetImageBuffer(void)
{
	return (GetImageExist()) ? m_pImageData->imageData : NULL;
}

char* CImageBuffer::GetImageBufferOrigin(void)
{
	return (GetImageExist()) ? m_pImageData->imageDataOrigin : NULL;
}

const char* CImageBuffer::GetImageBuffer(void) const
{
	return (GetImageExist()) ? m_pImageData->imageData : NULL;
}

const char* CImageBuffer::GetImageBufferOrigin(void) const
{
	return (GetImageExist()) ? m_pImageData->imageDataOrigin : NULL;
}


void CImageBuffer::ShowImage(HDC hDC, const CRect& dstRect)
{
	if (!GetImageExist()) return;

	if (GetDepth() != IPL_DEPTH_8U) return;

	if (m_pImageData->imageData == NULL) return;

	uchar buffer[sizeof(BITMAPINFOHEADER) + 1024];
	BITMAPINFO* bmi = (BITMAPINFO*)buffer;
	int bmp_w = m_pImageData->width, bmp_h = m_pImageData->height;

	SetStretchBltMode(hDC, COLORONCOLOR);

	FillBitmapInfo(bmi, bmp_w, bmp_h, GetBpp(), m_pImageData->origin);

	::StretchDIBits(
		hDC,
		dstRect.left,
		dstRect.top,
		dstRect.Width(),
		dstRect.Height(),
		0,
		0,
		bmp_w,
		bmp_h,
		m_pImageData->imageData,
		bmi,
		DIB_RGB_COLORS,
		SRCCOPY);
}

void CImageBuffer::ShowImage(HDC hDC, const CRect& srcRect, const CRect& dstRect)
{
	if (!GetImageExist()) return;

	if (GetDepth() != IPL_DEPTH_8U) return;

	if (m_pImageData->imageData == NULL) return;

	uchar buffer[sizeof(BITMAPINFOHEADER) + 1024];
	BITMAPINFO* bmi = (BITMAPINFO*)buffer;
	int bmp_w = m_pImageData->width, bmp_h = m_pImageData->height;

	SetStretchBltMode(hDC, COLORONCOLOR);

	FillBitmapInfo(bmi, bmp_w, bmp_h, GetBpp(), m_pImageData->origin);

	::StretchDIBits(
		hDC,
		dstRect.left,
		dstRect.top,
		dstRect.Width(),
		dstRect.Height(),
		srcRect.left,
		srcRect.top,
		srcRect.Width(),
		srcRect.Height(),
		m_pImageData->imageData,
		bmi,
		DIB_RGB_COLORS,
		SRCCOPY);
}

void CImageBuffer::ShowImage(HDC hDC, int nSx, int nSy, int nWidth, int nHeight, int nFromX, int nFromY)
{
	if (!GetImageExist()) return;

	if (GetDepth() != IPL_DEPTH_8U) return;

	uchar buffer[sizeof(BITMAPINFOHEADER) + 1024];
	BITMAPINFO* bmi = (BITMAPINFO*)buffer;
	int bmp_w = m_pImageData->width, bmp_h = m_pImageData->height;

	FillBitmapInfo(bmi, bmp_w, bmp_h, GetBpp(), m_pImageData->origin);

	nFromX = MIN(MAX(nFromX, 0), bmp_w - 1);
	nFromY = MIN(MAX(nFromY, 0), bmp_h - 1);

	int sw = MAX(MIN(bmp_w - nFromX, nWidth), 0);
	int sh = MAX(MIN(bmp_h - nFromY, nHeight), 0);

	SetDIBitsToDevice(
		hDC,
		nSx,
		nSy,
		sw,
		sh,
		nFromX,
		nFromY,
		nFromY,
		sh,
		m_pImageData->imageData + nFromY * m_pImageData->widthStep,
		bmi,
		DIB_RGB_COLORS);
}

void CImageBuffer::FillBitmapInfo(BITMAPINFO* bmi, int width, int height, int bpp, int origin)
{
	assert(bmi && width >= 0 && height >= 0 && (bpp == 8 || bpp == 24 || bpp == 32));

	BITMAPINFOHEADER* bmih = &(bmi->bmiHeader);

	memset(bmih, 0, sizeof(*bmih));
	bmih->biSize = sizeof(BITMAPINFOHEADER);
	bmih->biWidth = width;
	bmih->biHeight = origin ? abs(height) : -abs(height);
	bmih->biPlanes = 1;
	bmih->biBitCount = (unsigned short)bpp;
	bmih->biCompression = BI_RGB;

	if (bpp == 8)
	{
		RGBQUAD* palette = bmi->bmiColors;
		int i;
		for (i = 0; i < 256; i++)
		{
			palette[i].rgbBlue = palette[i].rgbGreen = palette[i].rgbRed = (BYTE)i;
			palette[i].rgbReserved = 0;
		}
	}
}

BOOL CImageBuffer::ClearImage(int nValue)
{
	if (!GetImageExist()) return FALSE;

	cvSet(m_pImageData, CV_RGB(nValue, nValue, nValue));

	return TRUE;
}

BOOL CImageBuffer::ClearImage(int nValue, int nA)
{
	if (!GetImageExist()) return FALSE;

	cvSet(m_pImageData, cvScalar(nValue, nValue, nValue, nA));

	return TRUE;
}

BOOL CImageBuffer::CopyImageTo(CImageBuffer* pToImage) const
{
	if (pToImage == NULL) return FALSE;

	if (!GetImageExist()) return FALSE;

	pToImage->ReleaseImage();

	pToImage->m_pImageData = cvCloneImage(m_pImageData);
	pToImage->m_bAllocatedData = TRUE;

	return TRUE;
}

BOOL CImageBuffer::CopyImageTo(IplImage* pToImage) const
{
	if (!GetImageExist()) return FALSE;

	if (pToImage != NULL) cvReleaseImage(&pToImage);

	pToImage = cvCloneImage(m_pImageData);

	return TRUE;
}

BOOL CImageBuffer::CopyImageFrom(const CImageBuffer* pFromImage)
{
	if (pFromImage == NULL) return FALSE;

	if (!pFromImage->GetImageExist()) return FALSE;

	ReleaseImage();

	m_pImageData = cvCloneImage(pFromImage->m_pImageData);
	m_bAllocatedData = TRUE;

	return TRUE;
}

BOOL CImageBuffer::CopyImageFrom(const IplImage* pFromImage)
{
	if (pFromImage == NULL) return FALSE;

	ReleaseImage();

	m_pImageData = cvCloneImage(pFromImage);
	m_bAllocatedData = TRUE;

	return TRUE;
}

BOOL CImageBuffer::GetSubImage(int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pGetImage)
{
	if (pGetImage == NULL) return FALSE;
	if (!GetImageExist()) return FALSE;
	if (nSx < 0 || nSy < 0) return FALSE;
	if ((nSx + nWidth) > GetWidth() || (nSy + nHeight) > GetHeight()) return FALSE;

	// set roi
	CvRect orgROI = cvGetImageROI(m_pImageData);
	cvSetImageROI(m_pImageData, cvRect(nSx, nSy, nWidth, nHeight));

	// create out image
	pGetImage->ReleaseImage();
	pGetImage->CreateImage(nWidth, nHeight, GetDepth(), GetChannels());

	// copy roi image
	cvCopy(m_pImageData, pGetImage->m_pImageData);

	// set origin (mch)
	pGetImage->m_pImageData->origin = m_pImageData->origin;

	// set org roi
	cvSetImageROI(m_pImageData, orgROI);

	return TRUE;
}

BOOL CImageBuffer::GetSubImage(int nSx, int nSy, int nWidth, int nHeight, int nChannels, BYTE* pGetBuffer)
{
	if (pGetBuffer == NULL) return FALSE;
	if (!GetImageExist()) return FALSE;
	if (nChannels != GetChannels()) return FALSE;
	if (IPL_DEPTH_8U != GetDepth()) return FALSE;
	if (nSx < 0 || nSy < 0) return FALSE;
	if ((nSx + nWidth) > GetWidth() || (nSy + nHeight) > GetHeight()) return FALSE;

	// set roi
	CvRect orgROI = cvGetImageROI(m_pImageData);
	cvSetImageROI(m_pImageData, cvRect(nSx, nSy, nWidth, nHeight));

	// create out image
	IplImage* tmpImage;
	tmpImage = cvCreateImage(cvSize(nWidth, nHeight), IPL_DEPTH_8U, nChannels);

	// copy roi image
	cvCopy(m_pImageData, tmpImage);

	int nGetStep = nWidth * nChannels;
	int nTmpStep = tmpImage->widthStep;
	if (nGetStep == nTmpStep)
	{
		memcpy(pGetBuffer, tmpImage->imageData, sizeof(char) * tmpImage->imageSize);
	}
	else
	{
		for (int i = 0; i < tmpImage->height; i++)
		{
			memcpy(&pGetBuffer[i * nGetStep], &tmpImage->imageData[i * nTmpStep], sizeof(char) * nGetStep);
		}
	}

	// set org roi
	cvSetImageROI(m_pImageData, orgROI);
	cvReleaseImage(&tmpImage);

	return TRUE;
}

BOOL CImageBuffer::GetSubImage(const CRect& rtRect, CImageBuffer* pGetImage)
{
	return GetSubImage(rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), pGetImage);
}

BOOL CImageBuffer::GetSubImage(const CRect& rtRect, int nBpp, BYTE* pGetBuffer)
{
	return GetSubImage(rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), nBpp, pGetBuffer);
}

BOOL CImageBuffer::GetBandImage(int nColorBand, CImageBuffer* pGetImage) const
{
	if (pGetImage == NULL) return FALSE;
	if (!GetImageExist()) return FALSE;
	if (GetDepth() != IPL_DEPTH_8U) return FALSE;

	if (nColorBand == ImageBand_Color || m_pImageData->nChannels < 3)
	{
		pGetImage->ReleaseImage();
		pGetImage->m_pImageData = cvCloneImage(m_pImageData);
		pGetImage->m_bAllocatedData = TRUE;
		return TRUE;
	}

	if (!pGetImage->CreateImage(GetWidth(), GetHeight(), IPL_DEPTH_8U, 1)) return FALSE;

	int nChannelIndex = -1;
	char cChannelName = _T('C');

	switch (nColorBand)
	{
	case ImageBand_Blue:
		cChannelName = _T('B');
		break;

	case ImageBand_Green:
		cChannelName = _T('G');
		break;

	case ImageBand_Red:
		cChannelName = _T('R');
		break;

	case ImageBand_Gray:
		cvCvtColor(m_pImageData, pGetImage->m_pImageData, CV_RGB2GRAY);

		// set origin (mch)
		pGetImage->m_pImageData->origin = m_pImageData->origin;
		return TRUE;

	default:
		pGetImage->ReleaseImage();
		return FALSE;
	}

	for (int i = 0; i < m_pImageData->nChannels; i++)
	{
		if (m_pImageData->channelSeq[i] == cChannelName)
		{
			nChannelIndex = i;
			break;
		}
	}

	if (nChannelIndex == -1 || nChannelIndex >= m_pImageData->nChannels) return FALSE;

	switch (nChannelIndex)
	{
	case 0:
		cvSplit(m_pImageData, pGetImage->m_pImageData, NULL, NULL, NULL);
		break;
	case 1:
		cvSplit(m_pImageData, NULL, pGetImage->m_pImageData, NULL, NULL);
		break;
	case 2:
		cvSplit(m_pImageData, NULL, NULL, pGetImage->m_pImageData, NULL);
		break;
	case 3:
		cvSplit(m_pImageData, NULL, NULL, NULL, pGetImage->m_pImageData);
		break;
	}

	// set origin (mch)
	pGetImage->m_pImageData->origin = m_pImageData->origin;

	return TRUE;
}

BOOL CImageBuffer::GetBandImage(int nColorBand, int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pGetImage)
{
	CImageBuffer tmpImage;

	if (!GetSubImage(nSx, nSy, nWidth, nHeight, &tmpImage)) return FALSE;

	if (!tmpImage.GetBandImage(nColorBand, pGetImage)) return FALSE;

	return TRUE;
}

BOOL CImageBuffer::GetBandImage(int nColorBand, int nSx, int nSy, int nWidth, int nHeight, BYTE* pGetBuffer)
{
	CImageBuffer tmpImage;

	if (!GetBandImage(nColorBand, &tmpImage)) return FALSE;

	if (!tmpImage.GetSubImage(nSx, nSy, nWidth, nHeight, 1, pGetBuffer)) return FALSE;

	return TRUE;
}


BOOL CImageBuffer::GetBandImage(int nColorBand, const CRect& rtRect, CImageBuffer* pGetImage)
{
	return GetBandImage(nColorBand, rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), pGetImage);
}

BOOL CImageBuffer::GetBandImage(int nColorBand, const CRect& rtRect, BYTE* pGetBuffer)
{
	return GetBandImage(nColorBand, rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), pGetBuffer);
}

BOOL CImageBuffer::SetSubImage(int nSx, int nSy, int nWidth, int nHeight, CImageBuffer* pSetImage)
{
	if (pSetImage == NULL) return FALSE;
	if (!GetImageExist() || !pSetImage->GetImageExist()) return FALSE;
	if (GetDepth() != pSetImage->GetDepth() || GetChannels() != pSetImage->GetChannels()) return FALSE;
	if (nWidth != pSetImage->GetWidth() || nHeight != pSetImage->GetHeight()) return FALSE;
	if (nSx < 0 || nSy < 0) return FALSE;
	if ((nSx + nWidth) > GetWidth() || (nSy + nHeight) > GetHeight()) return FALSE;

	CvRect orgROI = cvGetImageROI(m_pImageData);
	cvSetImageROI(m_pImageData, cvRect(nSx, nSy, nWidth, nHeight));

	CvRect setROI = cvGetImageROI(pSetImage->m_pImageData);
	cvResetImageROI(pSetImage->m_pImageData);

	cvCopy(pSetImage->m_pImageData, m_pImageData);

	// set origin (mch)
	m_pImageData->origin = pSetImage->m_pImageData->origin;

	cvSetImageROI(m_pImageData, orgROI);
	cvSetImageROI(pSetImage->m_pImageData, setROI);

	return TRUE;
}

BOOL CImageBuffer::SetSubImage(int nSx, int nSy, int nWidth, int nHeight, int nChannels, BYTE* pSetBuffer)
{
	if (pSetBuffer == NULL) return FALSE;
	if (!GetImageExist()) return FALSE;
	if (nChannels != GetChannels()) return FALSE;
	if (GetDepth() != IPL_DEPTH_8U) return FALSE;
	if (nSx < 0 || nSy < 0) return FALSE;
	if ((nSx + nWidth) > GetWidth() || (nSy + nHeight) > GetHeight()) return FALSE;

	IplImage* tmpImage;
	tmpImage = cvCreateImage(cvSize(nWidth, nHeight), IPL_DEPTH_8U, nChannels);

	int nSetStep = nWidth * nChannels;
	int nTmpStep = tmpImage->widthStep;
	if (nSetStep == nTmpStep)
	{
		//memcpy(tmpImage, pSetBuffer, sizeof(char)*tmpImage->imageSize);
		memcpy(tmpImage->imageData, pSetBuffer, sizeof(char) * tmpImage->imageSize);
	}
	else
	{
		for (int i = 0; i < tmpImage->height; i++)
		{
			memcpy(&tmpImage->imageData[i * nTmpStep], &pSetBuffer[i * nSetStep], sizeof(char) * nSetStep);
		}
	}

	// set roi
	CvRect orgROI = cvGetImageROI(m_pImageData);
	cvSetImageROI(m_pImageData, cvRect(nSx, nSy, nWidth, nHeight));

	cvCopy(tmpImage, m_pImageData);

	// set org roi
	cvSetImageROI(m_pImageData, orgROI);
	cvReleaseImage(&tmpImage);

	return TRUE;
}

BOOL CImageBuffer::SetSubImage(const CRect& rtRect, CImageBuffer* pSetImage)
{
	return SetSubImage(rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), pSetImage);
}

BOOL CImageBuffer::SetSubImage(const CRect& rtRect, int nBpp, BYTE* pSetBuffer)
{
	return SetSubImage(rtRect.left, rtRect.top, rtRect.Width(), rtRect.Height(), nBpp, pSetBuffer);
}

IplImage* CImageBuffer::GetIplImage(void)
{
	return m_pImageData;
}

BOOL CImageBuffer::DrawLine(CPoint ptPos1, CPoint ptPos2, DWORD nColor, int nThickness, int nLineType, int nShift)
{
	if (!GetImageExist()) return FALSE;

	CvPoint pt1 = cvPoint(ptPos1.x, ptPos1.y);
	CvPoint pt2 = cvPoint(ptPos2.x, ptPos2.y);
	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);

	cvLine(m_pImageData, pt1, pt2, color, nThickness, nLineType, nShift);

	return TRUE;
}

BOOL CImageBuffer::DrawRectangle(CPoint ptPos1, CPoint ptPos2, DWORD nColor, int nThickness, int nLineType, int nShift)
{
	if (!GetImageExist()) return FALSE;

	CvPoint pt1 = cvPoint(ptPos1.x, ptPos1.y);
	CvPoint pt2 = cvPoint(ptPos2.x, ptPos2.y);
	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);

	cvRectangle(m_pImageData, pt1, pt2, color, nThickness, nLineType, nShift);

	return TRUE;
}

BOOL CImageBuffer::DrawCircle(CPoint ptCenter, int nRadius, DWORD nColor, int nThickness, int nLineType, int nShift)
{
	if (!GetImageExist()) return FALSE;

	if (nRadius < 0) return FALSE;

	CvPoint center = cvPoint(ptCenter.x, ptCenter.y);
	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);

	cvCircle(m_pImageData, center, nRadius, color, nThickness, nLineType, nShift);

	return TRUE;
}

BOOL CImageBuffer::DrawEllipse(CPoint ptCenter, CPoint ptSize, double dAngle, DWORD nColor, int nThickness, int nLineType, int nShift)
{
	if (!GetImageExist()) return FALSE;

	CvBox2D box;
	box.center.x = (float)ptCenter.x;
	box.center.y = (float)ptCenter.y;
	box.size.width = (float)ptSize.x;
	box.size.height = (float)ptSize.y;
	box.angle = (float)dAngle;

	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);
	cvEllipseBox(m_pImageData, box, color, nThickness, nLineType, nShift);

	return TRUE;
}

BOOL CImageBuffer::DrawText(CPoint ptPoint, DWORD nColor, const CString& strText)
{
	if (!GetImageExist()) return FALSE;

	CvPoint point = cvPoint(ptPoint.x, ptPoint.y);
	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);
	CvFont font;

	cvInitFont(&font, CV_FONT_HERSHEY_PLAIN, 1.0, 1.0);// double shear=0, int thickness=1, int line_type=8 );

	USES_CONVERSION;
	char str_text[1024];
	sprintf_s(str_text, "%s", W2A(strText));

	if (str_text == NULL) return FALSE;

	cvPutText(m_pImageData, str_text, point, &font, color);

	return TRUE;
}

BOOL CImageBuffer::DrawText(CPoint ptPoint, DWORD nColor, double dScale,const CString& strText)
{
	if (!GetImageExist()) return FALSE;

	CvPoint point = cvPoint(ptPoint.x, ptPoint.y);
	CvScalar color = cvScalar(GetBValue(nColor), GetGValue(nColor), GetRValue(nColor), 0);
	CvFont font;

	cvInitFont(&font, CV_FONT_HERSHEY_DUPLEX, dScale, dScale);// double shear=0, int thickness=1, int line_type=8 );

	USES_CONVERSION;
	char str_text[1024];
	sprintf_s(str_text, "%s", W2A(strText));

	if (str_text == NULL) return FALSE;

	cvPutText(m_pImageData, str_text, point, &font, color);

	return TRUE;
}

BOOL CImageBuffer::DCtoIplImage(CDC* pDC, const CRect& rect)
{
	if (pDC == NULL) return FALSE;

	int cx = rect.right - rect.left;
	int cy = rect.bottom - rect.top;

	if (cx <= 0 || cy <= 0) return FALSE;

	HDC hMemDC = CreateCompatibleDC(pDC->m_hDC);

	BITMAPINFO			bmi;
	BITMAPINFOHEADER* bmih = &(bmi.bmiHeader);

	ZeroMemory(bmih, sizeof(BITMAPINFOHEADER));
	bmih->biSize = sizeof(BITMAPINFOHEADER);

	bmih->biBitCount = 24;
	bmih->biWidth = cx;
	bmih->biHeight = cy;
	bmih->biPlanes = 1;

	LPVOID				pBits;
	HBITMAP hBitmap = CreateDIBSection(pDC->m_hDC, &bmi, DIB_RGB_COLORS, (LPVOID*)&pBits, NULL, 0);

	SelectObject(hMemDC, hBitmap);

	BitBlt(hMemDC, 0, 0, cx, cy, pDC->m_hDC, rect.left, rect.top, SRCCOPY);

	DeleteDC(hMemDC);

	BOOL bResult = DIBtoIplImage(hBitmap);

	DeleteObject(hBitmap);

	return bResult;
}


BOOL CImageBuffer::DIBtoIplImage(HBITMAP& hbmp, HDC hdc)
{
	ReleaseImage();

	bool mustrelease = false;
	BITMAPINFO bmi;
	BITMAPINFOHEADER* bmih = &(bmi.bmiHeader);

	if (hdc == NULL)
	{
		hdc = GetDC(NULL);
		mustrelease = true;
	}

	ZeroMemory(bmih, sizeof(BITMAPINFOHEADER));
	bmih->biSize = sizeof(BITMAPINFOHEADER);

	if (GetDIBits(hdc, hbmp, 0, 0, NULL, &bmi, DIB_RGB_COLORS))
	{
		int height = (bmih->biHeight > 0) ? bmih->biHeight : -bmih->biHeight;
		int origin = (bmih->biHeight > 0);

		if (origin)
		{
			_IplImage* pTempImage = cvCreateImage(cvSize(bmih->biWidth, height), IPL_DEPTH_8U, 3);
			m_pImageData = cvCreateImage(cvSize(bmih->biWidth, height), IPL_DEPTH_8U, 3);
			m_bAllocatedData = TRUE;
			pTempImage->origin = origin;
			bmih->biBitCount = 24;
			bmih->biCompression = BI_RGB;
			GetDIBits(hdc, hbmp, 0, height, pTempImage->imageData, &bmi, DIB_RGB_COLORS);

			cvFlip(pTempImage, m_pImageData, 0);

			cvReleaseImage(&pTempImage);
		}
		else
		{
			m_pImageData = cvCreateImage(cvSize(bmih->biWidth, height), IPL_DEPTH_8U, 3);
			m_bAllocatedData = TRUE;
			m_pImageData->origin = origin;
			bmih->biBitCount = 24;
			bmih->biCompression = BI_RGB;

			GetDIBits(hdc, hbmp, 0, height, m_pImageData->imageData, &bmi, DIB_RGB_COLORS);
		}
	}

	if (mustrelease)
	{
		ReleaseDC(NULL, hdc);
	}

	return TRUE;
}


BOOL CImageBuffer::SetImageBufferPtr(char* pPtr)
{
	if (m_pImageData == NULL) return FALSE;

	if (m_pImageData->imageData) return FALSE;

	if (m_pImageData->imageDataOrigin) return FALSE;

	m_pImageData->imageData = m_pImageData->imageDataOrigin = pPtr;

	return TRUE;
}

CvScalar CImageBuffer::GetPixelScalar(int x, int y)
{
	if (!GetImageExist()) return -1;

	if (GetDepth() != IPL_DEPTH_8U) return -1;

	CvScalar sValue = cvGet2D(m_pImageData, y, x);

	return sValue;
}