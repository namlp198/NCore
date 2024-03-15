#include "pch.h"
#include "FrameGrabber_iRayple.h"

void ImageCallBackEx_iRayple(IMV_Frame* pFrame, void* pUser);

CFrameGrabber_iRayple::CFrameGrabber_iRayple(int nIndex, IFrameGrabber2Parent* pIFG2P) : CFrameGrabber(nIndex, pIFG2P)
{
	cImvCamera = new CIMVCamera;
}

CFrameGrabber_iRayple::~CFrameGrabber_iRayple()
{
	Disconnect();
}

int CFrameGrabber_iRayple::Connect(const CFrameGrabberParam& grabberParam)
{
	m_GrabberStatus.Reset();
	CFrameGrabberParam* pGrabberParam = dynamic_cast<CFrameGrabberParam*>(&m_GrabberStatus);
	*pGrabberParam = grabberParam;

	USES_CONVERSION;
	char str_serialNum[200];
#ifdef _UNICODE
	sprintf_s(str_serialNum, "%s", W2A(m_GrabberStatus.GetParam_ConnectAddr()));
#else
	sprintf_s(str_serialNum, "%s", strFilename);
#endif
	int ret = IMV_OK;

	IMV_DeviceList deviceInfoList;
	ret = IMV_EnumDevices(&deviceInfoList, interfaceTypeAll);

	if (IMV_OK != ret)
	{
		printf("Enumeration devices failed! ErrorCode[%d]\n", ret);
		return -1;
	}

	if (deviceInfoList.nDevNum < 1)
	{
		printf("no camera\n");
		return -1;
	}

	IMV_DeviceInfo pDeviceInfo;
	for (unsigned int i = 0; i < deviceInfoList.nDevNum; i++)
	{
		int nValue = 1;
		nValue = strcmp(str_serialNum, deviceInfoList.pDevInfo[i].serialNumber);
		if (nValue == 0)
		{
			pDeviceInfo = deviceInfoList.pDevInfo[i];
			break;
		}
	}
	
	ret = cImvCamera->Open(modeByCameraKey, pDeviceInfo, 0);
	if (ret != IMV_OK)
	{
		return ret;
	}

	m_GrabberStatus.SetStatus_Connected(FrameGrabber_Connected);
	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);

	ret = cImvCamera->RegisterImageCallBack(ImageCallBackEx_iRayple, this);
	if (IMV_OK != ret)
	{
		printf("Attach grabbing failed! ErrorCode[%d]\n", ret);
		return ret;
	}

	// set params

	SetTriggerMode(TriggerMode_Internal);
	SetTriggerSource(TriggerSource_Hardware);
}

int CFrameGrabber_iRayple::Disconnect()
{
	if (cImvCamera)
	{
		cImvCamera->Close();
		delete cImvCamera;
		cImvCamera = NULL;
		return 1;
	}
	return 0;
}

int CFrameGrabber_iRayple::StartGrab()
{
	int ret = cImvCamera->StartGrabbing();

	if (IMV_OK != ret)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
		return 0;
	}

	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_Grabbing);
	return 1;
}

int CFrameGrabber_iRayple::StopGrab()
{
	int ret = cImvCamera->StopGrabbing();

	if (IMV_OK != ret)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_Grabbing);
		return 0;
	}

	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
	return 1;
}

int CFrameGrabber_iRayple::SendTrigger(int nTriggerCount)
{
	return cImvCamera->CommandExecute("TriggerSoftware");
}

int CFrameGrabber_iRayple::SetTriggerMode(int nMode)
{
	if (!cImvCamera->IsDeviceConnected())
		return 0;
	int ret = IMV_OK;
	switch (nMode)
	{
	case TriggerMode_Internal:
		ret = cImvCamera->SetEnumFeature("TriggerMode", "Off");
		break;

	case TriggerMode_External:
		ret = cImvCamera->SetEnumFeature("TriggerMode", "On");
		break;

	default:
		return 0;
		break;
	}

	if (ret == IMV_OK)
	{
		m_GrabberStatus.SetStatus_TriggerMode(nMode);
		return 1;
	}
	return 0;
}

int CFrameGrabber_iRayple::SetTriggerSource(int nSource)
{
	if (!cImvCamera->IsDeviceConnected())
		return 0;
	int ret = IMV_OK;
	switch (nSource)
	{
	case TriggerSource_Software:
		ret = cImvCamera->SetEnumFeature("TriggerSource", "Software");
		break;

	case TriggerSource_Hardware:
		ret = cImvCamera->SetEnumFeature("TriggerSource", "Line1");
		break;

	default:
		return 0;
		break;
	}

	if (ret == IMV_OK)
	{
		m_GrabberStatus.SetStatus_TriggerSource(nSource);
		return 1;
	}
	return 0;
}

int CFrameGrabber_iRayple::SetExposureTime(double dTime)
{
	return 0;
}

int CFrameGrabber_iRayple::SetAnalogGain(double dGain)
{
	return 0;
}

int CFrameGrabber_iRayple::SetFrameRate(double dRate)
{
	return 0;
}

void ImageCallBackEx_iRayple(IMV_Frame* pFrame, void* pUser)
{
	if (pUser)
	{
		CFrameGrabber_iRayple* pThis = static_cast<CFrameGrabber_iRayple*>(pUser);
		pThis->ImageCallback(pFrame, pUser);
	}
}
void CFrameGrabber_iRayple::ImageCallback(IMV_Frame* pFrame, void* pUser)
{
	CSingleLock myLock(m_pFrameCriticalSection);
	myLock.Lock();
	if (pFrame == NULL)
	{
		printf("Frame is NULL\n");
		return;
	}
	ImageConvert(*pFrame, m_ePixelType);
	myLock.Unlock();
}

void CFrameGrabber_iRayple::ImageConvert(IMV_Frame frame, IMV_EPixelType convertFormat)
{
	int	ret = IMV_OK;
	const char* pConvertFormatStr = NULL;

	switch (convertFormat)
	{
	case gvspPixelRGB8:
		m_nDstBufSize = sizeof(unsigned char) * frame.frameInfo.width * frame.frameInfo.height * 3;
		pConvertFormatStr = (const char*)"RGB8";
		break;

	case gvspPixelBGR8:
		m_nDstBufSize = sizeof(unsigned char) * frame.frameInfo.width * frame.frameInfo.height * 3;
		pConvertFormatStr = (const char*)"BGR8";
		break;
	case gvspPixelBGRA8:
		m_nDstBufSize = sizeof(unsigned char) * frame.frameInfo.width * frame.frameInfo.height * 4;
		pConvertFormatStr = (const char*)"BGRA8";
		break;
	case gvspPixelMono8:
	default:
		m_nDstBufSize = sizeof(unsigned char) * frame.frameInfo.width * frame.frameInfo.height;
		pConvertFormatStr = (const char*)"Mono8";
		break;
	}

	m_pDstBuf = (unsigned char*)malloc(m_nDstBufSize);
	if (NULL == m_pDstBuf)
	{
		printf("malloc pDstBuf failed!\n");
		return;
	}

	// convert image to BGR8
	memset(&m_stPixelConvertParam, 0, sizeof(m_stPixelConvertParam));
	m_stPixelConvertParam.nWidth = frame.frameInfo.width;
	m_stPixelConvertParam.nHeight = frame.frameInfo.height;
	m_stPixelConvertParam.ePixelFormat = frame.frameInfo.pixelFormat;
	m_stPixelConvertParam.pSrcData = frame.pData;
	m_stPixelConvertParam.nSrcDataLen = frame.frameInfo.size;
	m_stPixelConvertParam.nPaddingX = frame.frameInfo.paddingX;
	m_stPixelConvertParam.nPaddingY = frame.frameInfo.paddingY;
	m_stPixelConvertParam.eBayerDemosaic = demosaicNearestNeighbor;
	m_stPixelConvertParam.eDstPixelFormat = convertFormat;
	m_stPixelConvertParam.pDstBuf = m_pDstBuf;
	m_stPixelConvertParam.nDstBufSize = m_nDstBufSize;

	ret = cImvCamera->ConvertPixelType(&m_stPixelConvertParam);
	if (IMV_OK == ret)
	{
		printf("image convert to %s successfully! nDstDataLen (%u)\n",
			pConvertFormatStr, m_stPixelConvertParam.nDstBufSize);

		// call interface func
		IFG2P_FrameGrabbed(0, m_pDstBuf, m_nDstBufSize);
	}
	else
	{
		printf("image convert to %s failed! ErrorCode[%d]\n", pConvertFormatStr, ret);
	}

	if (m_pDstBuf)
	{
		free(m_pDstBuf);
		m_pDstBuf = NULL;
	}

	return;
}

