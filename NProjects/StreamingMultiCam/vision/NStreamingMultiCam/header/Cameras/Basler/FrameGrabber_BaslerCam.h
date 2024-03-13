#pragma once

#include <pylon/PylonIncludes.h>
#include <pylon/BaslerUniversalInstantCamera.h>
#include "SharedMemoryBuffer.h"

class AFX_EXT_CLASS CFrameGrabber_BaslerCam : public CObject
	, public Pylon::CImageEventHandler
	, public Pylon::CConfigurationEventHandler
	, public Pylon::CCameraEventHandler
{
public:
	CFrameGrabber_BaslerCam();
	virtual ~CFrameGrabber_BaslerCam();

public:
	// Attributes
	void SetUserHint(int userHint);
	int GetUserHint() const;

	void SetInvertImage(bool enable);
	const Pylon::CPylonBitmapImage& GetBitmapImage() const;

	bool IsCameraDeviceRemoved() const;
	bool IsOpen() const;
	bool IsGrabbing() const;
	bool IsSingleShotSupported() const;

public:
	Pylon::IIntegerEx& GetExposureTime();
	Pylon::IIntegerEx& GetGain();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::PixelFormatEnums>& GetPixelFormat();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerModeEnums>& GetTriggerMode();
	Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerSourceEnums>& GetTriggerSource();
	CSyncObject* GetBmpLock() const;

public:
	// Statistics
	uint64_t GetGrabbedImages() const;
	uint64_t GetGrabbedImageDiff();
	uint64_t GetGrabErrors() const;

	void          SetFrameWidth(DWORD dwFrameWidth)              { m_dwFrameWidth = dwFrameWidth; }
	DWORD         GetFrameWidth()                                { return m_dwFrameWidth; }
	void          SetFrameHeight(DWORD dwFrameHeight)            { m_dwFrameHeight = dwFrameHeight; }
	DWORD         GetFrameHeight()                               { return m_dwFrameHeight; }
	void          SetFrameSize(DWORD dwFrameSize)                { m_dwFrameSize = dwFrameSize; }
	DWORD         GetFrameSize()                                 { return m_dwFrameSize; }
	void          SetFrameCount(DWORD dwFrameCount)              { m_dwFrameCount = dwFrameCount; }
	DWORD         GetFrameCount()                                { return m_dwFrameCount; }
	void          SetMapFileName(CString csMapFileName)          { m_csMapFileName = csMapFileName; }
	CString       GetMapFileName()                               { return m_csMapFileName; }

public:
	// Operations
	void Open(const Pylon::CDeviceInfo& deviceInfo);
	void Close();

	void SingleGrab();
	void ContinuousGrab();
	void StopGrab();

	void ExecuteSoftwareTrigger();

public:
	void         Initialize();
	void         Destroy();
	LPBYTE       GetBufferImage();

public:
	// Pylon::CImageEventHandler functions
	virtual void OnImagesSkipped(Pylon::CInstantCamera& camera, size_t countOfSkippedImages);
	virtual void OnImageGrabbed(Pylon::CInstantCamera& camera, const Pylon::CGrabResultPtr& grabResult);

	// Pylon::CConfigurationEventHandler functions
	virtual void OnAttach(Pylon::CInstantCamera& camera);
	virtual void OnAttached(Pylon::CInstantCamera& camera);
	virtual void OnDetach(Pylon::CInstantCamera& camera);
	virtual void OnDetached(Pylon::CInstantCamera& camera);
	virtual void OnDestroy(Pylon::CInstantCamera& camera);
	virtual void OnDestroyed(Pylon::CInstantCamera& camera);
	virtual void OnOpen(Pylon::CInstantCamera& camera);
	virtual void OnOpened(Pylon::CInstantCamera& camera);
	virtual void OnClose(Pylon::CInstantCamera& camera);
	virtual void OnClosed(Pylon::CInstantCamera& camera);
	virtual void OnGrabStart(Pylon::CInstantCamera& camera);
	virtual void OnGrabStarted(Pylon::CInstantCamera& camera);
	virtual void OnGrabStop(Pylon::CInstantCamera& camera);
	virtual void OnGrabStopped(Pylon::CInstantCamera& camera);
	virtual void OnGrabError(Pylon::CInstantCamera& camera, const char* errorMessage);
	virtual void OnCameraDeviceRemoved(Pylon::CInstantCamera& camera);

	// Pylon::CCameraEventHandler function
	virtual void OnCameraEvent(Pylon::CInstantCamera& camera, intptr_t userProvidedId, GenApi::INode* pNode);

private:
	BOOL         CreateBuffer();

private:
	// Statistical values
	uint64_t m_cntGrabbedImages;
	uint64_t m_cntSkippedImages;
	uint64_t m_cntGrabErrors;
	uint64_t m_cntLastGrabbedImages;

	// Used to store the index of the camera.
	int m_userHint;
	// Will be set to toggle image processing on or off.
	bool m_invertImage;
	// Protects members.
	mutable CCriticalSection m_MemberLock;
	// Protects the converted bitmap.
	mutable CCriticalSection m_bmpLock;

	// Resolution
	DWORD m_dwFrameWidth;
	DWORD m_dwFrameHeight;
	DWORD m_dwFrameSize;
	DWORD m_dwFrameCount;

	// String Map File name
	CString m_csMapFileName;

private:

	// The pylon camera object.
	Pylon::CBaslerUniversalInstantCamera m_camera;
	// The grab result retrieved from the camera.
	Pylon::CGrabResultPtr m_ptrGrabResult;
	// The grab result as a windows DIB to be displayed on the screen.
	Pylon::CPylonBitmapImage m_bitmapImage;

	// Configurations to apply when starting single or continuous grab.
	Pylon::CAcquireSingleFrameConfiguration m_singleConfiguration;
	Pylon::CAcquireContinuousConfiguration m_continuousConfiguration;

	// Smart pointer to camera features
	Pylon::CIntegerParameter m_exposureTime;
	Pylon::CIntegerParameter m_gain;

	CSharedMemoryBuffer* m_pCameraImageBuffer;
};