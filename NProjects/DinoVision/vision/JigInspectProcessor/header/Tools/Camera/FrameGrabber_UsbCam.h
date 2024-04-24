#pragma once

#include <thread>
#include <chrono>
#include "DeviceEnumerator.h"
#include "SharedMemoryBuffer.h"
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/videoio.hpp>

class CFramGrabber_UsbCam
{
public:
	CFramGrabber_UsbCam(int nCamId = 0);
	~CFramGrabber_UsbCam();

public:
	bool         Connect();
	bool         Disconnect();

	void        StartGrab();
	void        StopGrab();
	void        SingleGrab();

public:
	void                Initialize();
	void                Destroy();
	LPBYTE              GetBufferImage();

public:

	// getter

	int GetId() { return m_nId; }
	std::string GetDeviceName() { return m_sDeviceName; }
	std::string GetDevicePath() { return m_sDevicePath; }

	DWORD GetFrameWidth() { return m_dwFrameWidth; }
	DWORD GetFrameHeight() { return m_dwFrameHeight; }
	DWORD GetFrameSize() { return m_dwFrameSize; }
	DWORD GetFramCount() { return m_dwFrameCount; }
	int GetChannels() { return m_nChannels; }

	bool GetConnected() { return m_bConnected; }
	bool GetGrabbing() { return m_bGrabbing; }

	// setter
	void SetId(int nValue) { m_nId = nValue; }
	void SetDeviceName(std::string sValue) { m_sDeviceName = sValue; }
	void SetDevicePath(std::string sValue) { m_sDevicePath = sValue; }

	void SetFrameWidth(DWORD nValue) { m_dwFrameWidth = nValue; }
	void SetFrameHeight(DWORD nValue) { m_dwFrameHeight = nValue; }
	void SetFrameSize(DWORD nValue) { m_dwFrameSize = nValue; }
	void SetFrameCount(DWORD nValue) { m_dwFrameCount = nValue; }
	void SetChannels(int nValue) { m_nChannels = nValue; }

	void SetConnected(bool bValue) { m_bConnected = bValue; }
	void SetGrabbing(bool bValue) { m_bGrabbing = bValue; }

private:
	bool                  Close();
	BOOL                  CreateBuffer();

private:

	int                   m_nId;
	std::string           m_sDeviceName;
	std::string           m_sDevicePath;

	// Resolution
	DWORD                 m_dwFrameWidth;
	DWORD                 m_dwFrameHeight;
	DWORD                 m_dwFrameSize;
	DWORD                 m_dwFrameCount;
	int                   m_nChannels;

	bool                  m_bConnected;
	bool                  m_bGrabbing;

private:
	cv::VideoCapture*          m_pCamera;
	//cv::Mat*                   m_pLastFrame;

	// Protects members.
	mutable CCriticalSection   m_MemberLock;
	
	// buffer image
	CSharedMemoryBuffer*       m_pCameraImageBuffer;
	
	/*typedef void (*ImageGrabbedEventCallBack)(void*);

	typedef struct {
		void* data;
		ImageGrabbedEventCallBack callback;
	}EventImageCallBack;*/
};