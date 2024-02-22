#include "pch.h"
#include "FrameGrabber_BaslerCam.h"

CFrameGrabber_BaslerCam::CFrameGrabber_BaslerCam() :
    m_cntGrabbedImages(0)
    , m_cntSkippedImages(0)
    , m_cntGrabErrors(0)
    , m_cntLastGrabbedImages(0)
    , m_userHint(-1)
    , m_invertImage(false)
    , m_pCameraImageBuffer(NULL)
{
    // Register this object as an image event handler in order to get notified of new images.
    // See Pylon::CImageEventHandler for details.
    m_camera.RegisterImageEventHandler(this, Pylon::RegistrationMode_ReplaceAll, Pylon::Cleanup_None);

    // Register this object as a configuration event handler in order to get notified of camera state changes.
    // See Pylon::CConfigurationEventHandler for details.
    m_camera.RegisterConfiguration(new Pylon::CAcquireContinuousConfiguration(), Pylon::RegistrationMode_ReplaceAll, Pylon::Cleanup_Delete);
    m_camera.RegisterConfiguration(this, Pylon::RegistrationMode_Append, Pylon::Cleanup_None);
}

CFrameGrabber_BaslerCam::~CFrameGrabber_BaslerCam()
{
    //Close();
    Destroy();
}

void CFrameGrabber_BaslerCam::SetUserHint(int userHint)
{
    CSingleLock lock(&m_MemberLock, TRUE);
    m_userHint = userHint;
}

int CFrameGrabber_BaslerCam::GetUserHint() const
{
    return m_userHint;
}

// Turn our sample image processing on or off.
void CFrameGrabber_BaslerCam::SetInvertImage(bool enable)
{
    CSingleLock lock(&m_MemberLock, TRUE);
    m_invertImage = enable;
}

// Return the converted bitmap.
// Called by the GUI to display the image on the screen.
const Pylon::CPylonBitmapImage& CFrameGrabber_BaslerCam::GetBitmapImage() const
{
    // No need to protect this member as it will only be accessed from the GUI thread.
    return m_bitmapImage;
}

// Returns true if the device has been removed/disconnected.
bool CFrameGrabber_BaslerCam::IsCameraDeviceRemoved() const
{
    return m_camera.IsCameraDeviceRemoved();
}

// Returns true if the device is currently opened.
bool CFrameGrabber_BaslerCam::IsOpen() const
{
    return m_camera.IsOpen();
}

// Returns true if the device is currently grabbing.
bool CFrameGrabber_BaslerCam::IsGrabbing() const
{
    return m_camera.IsGrabbing();
}

// Returns true if the device supports SingleShot acquisition.
bool CFrameGrabber_BaslerCam::IsSingleShotSupported() const
{
    if (!m_camera.IsOpen())
    {
        return false;
    }

    Pylon::StringList_t modeEntries;
    m_camera.AcquisitionMode.GetSettableValues(modeEntries);

    for (Pylon::StringList_t::iterator it = modeEntries.begin(), end = modeEntries.end(); it != end; ++it)
    {
        const Pylon::String_t& entry = *it;
        if (entry.compare("SingleFrame") == 0)
        {
            return true;
        }
    }

    return false;
}

// Return a camera parameter.
// This function is called by the GUI to update controls.
Pylon::IIntegerEx& CFrameGrabber_BaslerCam::GetExposureTime()
{
    return m_exposureTime;
}

Pylon::IIntegerEx& CFrameGrabber_BaslerCam::GetGain()
{
    return m_gain;
}

// Return a camera parameter.
// This function is called by the GUI to update controls.
Pylon::IEnumParameterT<Basler_UniversalCameraParams::PixelFormatEnums>& CFrameGrabber_BaslerCam::GetPixelFormat()
{
    return m_camera.PixelFormat;
}

// Return a camera parameter.
// This function is called by the GUI to update controls.
Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerModeEnums>& CFrameGrabber_BaslerCam::GetTriggerMode()
{
    return m_camera.TriggerMode;
}

Pylon::IEnumParameterT<Basler_UniversalCameraParams::TriggerSourceEnums>& CFrameGrabber_BaslerCam::GetTriggerSource()
{
    return m_camera.TriggerSource;
}

// This GUI needs to lock the bitmap image while painting it to the screen.
CSyncObject* CFrameGrabber_BaslerCam::GetBmpLock() const
{
    return &m_bmpLock;
}

// Returns statistical values for the GUI.
uint64_t CFrameGrabber_BaslerCam::GetGrabbedImages() const
{
    // We must protect this member as it will be accessed from the grab thread and the GUI thread.
    CSingleLock lock(&m_MemberLock, TRUE);
    return m_cntGrabbedImages;
}

// Returns the number of images grabbed since the last call to this function.
// This is used to calculate the FPS received.
uint64_t CFrameGrabber_BaslerCam::GetGrabbedImageDiff()
{
    // We must protect these members as they will be accessed from the grab thread and the GUI thread.
    CSingleLock lock(&m_MemberLock, TRUE);
    uint64_t delta = m_cntGrabbedImages - m_cntLastGrabbedImages;
    m_cntLastGrabbedImages = m_cntGrabbedImages;
    return delta;
}

// Returns statistical values for the GUI.
uint64_t CFrameGrabber_BaslerCam::GetGrabErrors() const
{
    // We must protect this member as it will be accessed from the grab thread and the GUI thread.
    CSingleLock lock(&m_MemberLock, TRUE);
    return m_cntGrabErrors;
}


void CFrameGrabber_BaslerCam::Open(const Pylon::CDeviceInfo& deviceInfo)
{
    CSingleLock lock(&m_MemberLock, TRUE);

    try
    {
        // Add the AutoPacketSizeConfiguration and let pylon delete it when not needed anymore.
        // m_camera.RegisterConfiguration(new CAutoPacketSizeConfiguration(), Pylon::RegistrationMode_Append, Pylon::Cleanup_Delete);

        // Create the device and attach it to CInstantCamera.
        // Let CInstantCamera take care of destroying the device.
        Pylon::IPylonDevice* pDevice = Pylon::CTlFactory::GetInstance().CreateDevice(deviceInfo);
        m_camera.Attach(pDevice, Pylon::Cleanup_Delete);

        // Open camera.
        m_camera.Open();

        // Get the ExposureTime feature.
        // On GigE cameras, the feature is called 'ExposureTimeRaw'.
        // On USB cameras, it is called 'ExposureTime'.
        if (m_camera.ExposureTime.IsValid())
        {
            // We need the integer representation because the GUI controls can only use integer values.
            // If it doesn't exist, return an empty parameter.
            m_camera.ExposureTime.GetAlternativeIntegerRepresentation(m_exposureTime);
        }
        else if (m_camera.ExposureTimeRaw.IsValid())
        {
            m_exposureTime.Attach(m_camera.ExposureTimeRaw.GetNode());
        }

        // Get the Gain feature.
        // On GigE cameras, the feature is called 'GainRaw'.
        // On USB cameras, it is called 'Gain'.
        if (m_camera.Gain.IsValid())
        {
            // We need the integer representation for the this sample
            // If it doesn't exist, return an empty parameter.
            m_camera.Gain.GetAlternativeIntegerRepresentation(m_gain);
        }
        else if (m_camera.GainRaw.IsValid())
        {
            m_gain.Attach(m_camera.GainRaw.GetNode());
        }

        // Add event handlers that will be called when the feature changes.

        if (m_exposureTime.IsValid())
        {
            // If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
            m_camera.RegisterCameraEventHandler(this, m_exposureTime.GetNode()->GetName(), 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);
        }

        if (m_gain.IsValid())
        {
            // If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
            m_camera.RegisterCameraEventHandler(this, m_gain.GetNode()->GetName(), 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);
        }

        m_camera.RegisterCameraEventHandler(this, "PixelFormat", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);

        m_camera.RegisterCameraEventHandler(this, "TriggerMode", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);

        m_camera.RegisterCameraEventHandler(this, "TriggerSource", 0, Pylon::RegistrationMode_Append, Pylon::Cleanup_None, Pylon::CameraEventAvailability_Optional);


    }
    catch (const Pylon::GenericException& e)
    {
        UNUSED(e);
        TRACE(CUtf82W(e.GetDescription()));

        // Undo everthing.
        Close();

        throw;
    }
}

void CFrameGrabber_BaslerCam::Close()
{
    CSingleLock lock(&m_MemberLock, TRUE);

    // Stop the grab, so the grab thread will not set new m_ptrGrabResult.
    StopGrab();

    // Free the converted bitmap, if present.
    CSingleLock lockBmp(&m_bmpLock, TRUE);
    m_bitmapImage.Release();
    lockBmp.Unlock();

    // Free the grab result, if present.
    m_ptrGrabResult.Release();

    // Remove the event handlers that will be called when the feature changes.
    m_camera.DeregisterCameraEventHandler(this, "TriggerSource");

    m_camera.DeregisterCameraEventHandler(this, "TriggerMode");

    m_camera.DeregisterCameraEventHandler(this, "PixelFormat");

    if (m_gain.IsValid())
    {
        // If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
        m_camera.DeregisterCameraEventHandler(this, m_gain.GetNode()->GetName());
    }

    if (m_exposureTime.IsValid())
    {
        // If we must use the alternative integer representation, we don't know the name of the node as it defined by the camera
        m_camera.DeregisterCameraEventHandler(this, m_exposureTime.GetNode()->GetName());
    }

    // Clear the pointers to the features we set manually in Open().
    m_exposureTime.Release();
    m_gain.Release();

    // Close the camera and free all ressources.
    // This will also unregister all 
    m_camera.DestroyDevice();
}

// Grab a single image.
void CFrameGrabber_BaslerCam::SingleGrab()
{
    // Camera may have been disconnected.
    if (!m_camera.IsOpen() || m_camera.IsGrabbing())
    {
        return;
    }

    // Try set single frame mode if available
    m_camera.AcquisitionMode.TrySetValue(Basler_UniversalCameraParams::AcquisitionMode_SingleFrame);

    // Grab one image.
    // When the image is received, pylon will call the OnImageGrab() handler.
    m_camera.StartGrabbing(1, Pylon::GrabStrategy_OneByOne, Pylon::GrabLoop_ProvidedByInstantCamera);
}

// Start a continuous grab on the camera.
void CFrameGrabber_BaslerCam::ContinuousGrab()
{
    // Camera may have been disconnected.
    if (!m_camera.IsOpen() || m_camera.IsGrabbing())
    {
        return;
    }

    // Try set continuous frame mode if available
    m_camera.AcquisitionMode.TrySetValue(Basler_UniversalCameraParams::AcquisitionMode_Continuous);

    // Start grabbing until StopGrabbing() is called.
    m_camera.StartGrabbing(Pylon::GrabStrategy_OneByOne, Pylon::GrabLoop_ProvidedByInstantCamera);
}

// Stop the continuous grab on the camera.
void CFrameGrabber_BaslerCam::StopGrab()
{
    m_camera.StopGrabbing();
}

void CFrameGrabber_BaslerCam::ExecuteSoftwareTrigger()
{
    if (!IsGrabbing())
    {
        return;
    }

    // Only wait if software trigger is currently turned on.
    if (m_camera.TriggerSource.GetValue() == Basler_UniversalCameraParams::TriggerSource_Software
        && m_camera.TriggerMode.GetValue() == Basler_UniversalCameraParams::TriggerMode_On)
    {
        // If the camera is currently processing a previous trigger command,
        // it will silently discard trigger commands.
        // We wait until the camera is ready to process the next trigger.
        m_camera.WaitForFrameTriggerReady(3000, Pylon::TimeoutHandling_ThrowException);
    }
    // Send trigger
    m_camera.ExecuteSoftwareTrigger();
}

void CFrameGrabber_BaslerCam::Initialize()
{
    CreateBuffer();
}

void CFrameGrabber_BaslerCam::Destroy()
{
    Close();

    if (m_pCameraImageBuffer != NULL)
    {
        delete m_pCameraImageBuffer;
        m_pCameraImageBuffer = NULL;
    }
}

BOOL CFrameGrabber_BaslerCam::CreateBuffer()
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

    CString mapFileName = GetMapFileName();
    
    bRetValue = m_pCameraImageBuffer->CreateSharedMemory(mapFileName, dw64Size);

    CString strLogMessage;
    strLogMessage.Format(_T("Total Create Memory : %.2f MB"), (((double)(m_dwFrameSize * m_dwFrameCount)) / 1000000.0));

    return TRUE;
}

LPBYTE CFrameGrabber_BaslerCam::GetBufferImage()
{
    if (m_pCameraImageBuffer == NULL)
        return NULL;

    return m_pCameraImageBuffer->GetBufferImage(0);
}

void CFrameGrabber_BaslerCam::OnImagesSkipped(Pylon::CInstantCamera& camera, size_t countOfSkippedImages)
{

    TRACE(_T("%s\n"), __FUNCTIONW__);

    CSingleLock lock(&m_MemberLock, TRUE);
    m_cntSkippedImages += countOfSkippedImages;

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnImageGrabbed(Pylon::CInstantCamera& camera, const Pylon::CGrabResultPtr& grabResult)
{

    // When overwriting the current CGrabResultPtr, the previous result will automatically be
    // released and reused by CInstantCamera.
    CSingleLock lockGrabResult(&m_MemberLock, TRUE);
    m_ptrGrabResult = grabResult;
    lockGrabResult.Unlock();

    // First check whether the smart pointer is valid.
    // Then call GrabSucceeded() on the CGrabResultData to test whether the grab resulut conatains
    // an sucessfully grabbed image.
    // In case of i.e. transmission errors the result may be invalid
    if (grabResult.IsValid() && grabResult->GrabSucceeded())
    {
        CSingleLock lockBuffer(&m_MemberLock, TRUE);
        m_pCameraImageBuffer->SetFrameImage(0, reinterpret_cast<uint8_t*>(grabResult->GetBuffer()));
        lockBuffer.Unlock();

        // This is where you would do image processing
        // and do other tasks.
        // --- Start of sample image processing --- (only works for 8-bit formats)
        if (m_invertImage && Pylon::BitDepth(grabResult->GetPixelType()) == 8)
        {
            size_t imageSize = Pylon::ComputeBufferSize(grabResult->GetPixelType(), grabResult->GetWidth(), grabResult->GetHeight(), grabResult->GetPaddingX());

            uint8_t* p = reinterpret_cast<uint8_t*>(grabResult->GetBuffer());
            uint8_t* const pEnd = p + (imageSize / sizeof(uint8_t));
            for (; p < pEnd; ++p)
            {
                *p = 255 - *p; //For demonstration purposes only, invert the image.
            }
        }
        //--- End of sample image processing ---

        // Convert the processed image to a bmp so we can display it on the screen.
        // We must lock the bitmap, so we don't modify the pixels while the GUI thread is painting.
        CSingleLock lockBmp(&m_bmpLock, TRUE);
        m_bitmapImage.CopyImage(grabResult);
        lockBmp.Unlock();

        CSingleLock lockImageCount(&m_MemberLock, TRUE);
        ++m_cntGrabbedImages;
        lockImageCount.Unlock();
    }
    else
    {
        // If the grab result is invalid, we also mark the bitmap as invalid.
        CSingleLock lockBmp(&m_bmpLock, TRUE);
        m_bitmapImage.Release();
        lockBmp.Unlock();

        // The some TLs provide an error code why the grab failed.
        TRACE(_T("%s Grab failed. Error code. %x\n"), __FUNCTIONW__, (int)grabResult->GetErrorCode());

        CSingleLock lockErrorCount(&m_MemberLock, TRUE);
        ++m_cntGrabErrors;
        lockErrorCount.Unlock();
    }
}

void CFrameGrabber_BaslerCam::OnAttach(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnAttached(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnDetach(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnDetached(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnDestroy(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnDestroyed(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnOpen(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s - '%s'\n"), __FUNCTIONW__, (LPCWSTR)CUtf82W(camera.GetDeviceInfo().GetFriendlyName().c_str()));

    // Prevent unused variable warning.
    UNUSED(camera);
}

void CFrameGrabber_BaslerCam::OnOpened(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // In this sample we configure only the trigger to start the acquisition.
    // Depending on your camera model, it may be called FrameStart or AcquisitionStart.
    if (!m_camera.TriggerSelector.TrySetValue(Basler_UniversalCameraParams::TriggerSelector_FrameStart))
    {
        m_camera.TriggerSelector.TrySetValue(Basler_UniversalCameraParams::TriggerSelector_AcquisitionStart);
    }

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnClose(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnClosed(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s - '%s'\n"), __FUNCTIONW__, (LPCWSTR)CUtf82W(camera.GetDeviceInfo().GetFriendlyName().c_str()));

    // Prevent unused variable warning.
    UNUSED(camera);
}

void CFrameGrabber_BaslerCam::OnGrabStart(Pylon::CInstantCamera& camera)
{

    TRACE(_T("%s\n"), __FUNCTIONW__);

    // This function may be called from another thread by InstantCamera while holding the camera lock.

    CSingleLock lock(&m_MemberLock, TRUE);

    // Reset statistics.
    m_cntGrabbedImages = 0;
    m_cntSkippedImages = 0;
    m_cntGrabErrors = 0;
    m_cntLastGrabbedImages = 0;

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnGrabStarted(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnGrabStop(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // This function may be called from another thread by InstantCamera while holding the camera lock.

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnGrabStopped(Pylon::CInstantCamera& camera)
{
    TRACE(_T("%s Grabbed: %I64u; Errors: %I64u\n"), __FUNCTIONW__, m_cntGrabbedImages, m_cntGrabErrors);

    // This function may be called from another thread by InstantCamera while holding the camera lock.

    // Reset the FPS counter.
    m_cntLastGrabbedImages = m_cntGrabbedImages;

    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnGrabError(Pylon::CInstantCamera& camera, const char* errorMessage)
{
    TRACE(_T("%s\n"), __FUNCTIONW__);

    // This function may be called from another thread by InstantCamera while holding the camera lock.

    // Prevent unused variable warning.
    UNUSED_ALWAYS(errorMessage);
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnCameraDeviceRemoved(Pylon::CInstantCamera& camera)
{

    TRACE(_T("%s\n"), __FUNCTIONW__);
    // Prevent unused variable warning.
    UNUSED_ALWAYS(camera);
}

void CFrameGrabber_BaslerCam::OnCameraEvent(Pylon::CInstantCamera& camera, intptr_t userProvidedId, GenApi::INode* pNode)
{
    if (pNode == NULL)
    {
        return;
    }

    UNUSED_ALWAYS(camera);
    UNUSED_ALWAYS(userProvidedId);
}


