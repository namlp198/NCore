#include "pch.h"
#include "FrameGrabber_UsbCam.h"

CFramGrabber_UsbCam::CFramGrabber_UsbCam(int nCamId)
{
	m_pCamera = new cv::VideoCapture;
}

CFramGrabber_UsbCam::~CFramGrabber_UsbCam()
{
	Destroy();
}

bool CFramGrabber_UsbCam::Connect()
{
	if (m_pCamera != NULL)
		m_pCamera->open(m_nId);

	if (m_pCamera->isOpened())
	{
		m_bConnected = true;
		return true;
	}
}

bool CFramGrabber_UsbCam::Disconnect()
{
	if (m_pCamera != NULL)
		m_pCamera->release();
	m_bConnected = false;
	return true;
}

bool CFramGrabber_UsbCam::Close()
{
	if (m_pCamera != NULL)
		m_pCamera->release();
	m_bConnected = false;

	delete m_pCamera;
	m_pCamera = NULL;
	return true;
}

void CFramGrabber_UsbCam::StartGrab()
{
	if (!m_bConnected)
		return;
	cv::Mat lastFrame;
	m_pCamera->read(lastFrame);
	CSingleLock lockBuffer(&m_MemberLock, TRUE);
	m_pCameraImageBuffer->SetFrameImage(0, lastFrame.data);
	lockBuffer.Unlock();

	lastFrame.release();
}

void CFramGrabber_UsbCam::StopGrab()
{
	Disconnect();
}

void CFramGrabber_UsbCam::SingleGrab()
{
	if (!m_bConnected)
		return;
	cv::Mat lastFrame;
	m_pCamera->read(lastFrame);
	//cv::imshow("dino", lastFrame);
	// call interface func
	CSingleLock lockBuffer(&m_MemberLock, TRUE);
	//cv::imwrite("test1.jpg", lastFrame);
	m_pCameraImageBuffer->SetFrameImage(0, lastFrame.data);
	//LPBYTE pBuffer2 = m_pCameraImageBuffer->GetBufferImage(0);
	//cv::Mat matCopy(m_dwFrameHeight, m_dwFrameWidth, CV_8UC3, pBuffer2);
	//cv::imshow("test", matCopy);
	//cv::imwrite("test2.jpg", matCopy);
	lockBuffer.Unlock();
	lastFrame.release();

	//std::this_thread::sleep_for(std::chrono::milliseconds(100));
	//Disconnect();
}


void CFramGrabber_UsbCam::Initialize()
{
	CreateBuffer();
}

void CFramGrabber_UsbCam::Destroy()
{
	Close();
	if (m_pCameraImageBuffer != NULL)
	{
		m_pCameraImageBuffer->DeleteSharedMemory();
		delete m_pCameraImageBuffer;
		m_pCameraImageBuffer = NULL;
	}
}

BOOL CFramGrabber_UsbCam::CreateBuffer()
{
	BOOL bRetValue = FALSE;

	if (m_pCameraImageBuffer != NULL)
	{
		m_pCameraImageBuffer->DeleteSharedMemory();
		delete m_pCameraImageBuffer;
		m_pCameraImageBuffer = NULL;
	}

	m_pCameraImageBuffer = new CSharedMemoryBuffer;
	m_pCameraImageBuffer->SetFrameWidth(m_dwFrameWidth);
	m_pCameraImageBuffer->SetFrameHeight(m_dwFrameHeight);
	m_pCameraImageBuffer->SetFrameCount(m_dwFrameCount);
	m_pCameraImageBuffer->SetFrameSize(m_dwFrameSize);

	DWORD64 dw64Size = (DWORD64)m_dwFrameCount * m_dwFrameSize;

	CString strMemory;
	strMemory.Format(_T("%s_%d"), "Buffer_UsbCam");

	bRetValue = m_pCameraImageBuffer->CreateSharedMemory(strMemory, dw64Size);

	CString strLogMessage;
	strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(m_dwFrameSize * m_dwFrameCount)) / 1000000.0));

	return TRUE;
}

LPBYTE CFramGrabber_UsbCam::GetBufferImage()
{
	if (m_pCameraImageBuffer == NULL)
		return NULL;

	return m_pCameraImageBuffer->GetBufferImage(0);
}

