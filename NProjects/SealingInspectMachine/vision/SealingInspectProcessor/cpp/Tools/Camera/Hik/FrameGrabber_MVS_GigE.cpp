#include "pch.h"
#include "FrameGrabber_MVS_GigE.h"
#include "MvCameraControl.h"
#include "MvCamera.h"

CFrameGrabber_MVS_GigE::CFrameGrabber_MVS_GigE(int nIndex /*= 0*/, IFrameGrabber2Parent* pIFG2P /*= NULL*/) : CFrameGrabber(nIndex, pIFG2P)
{
	m_pCamera = new CMvCamera();
}

CFrameGrabber_MVS_GigE::~CFrameGrabber_MVS_GigE()
{
	if (m_pCamera)
	{
		m_pCamera->Close();
		delete m_pCamera;
	}
	m_pCamera = nullptr;
}

void __stdcall ImageCallBackEx(unsigned char * pData, MV_FRAME_OUT_INFO_EX* pFrameInfo, void* pUser)
{
	if (pFrameInfo && pUser)
	{
		CFrameGrabber_MVS_GigE *pThis = static_cast<CFrameGrabber_MVS_GigE *>(pUser);
		pThis->ImageCallBack(pData, pFrameInfo);
	}
}

void CFrameGrabber_MVS_GigE::ImageCallBack(unsigned char * pData, void* pFrameInfo)
{
	CSingleLock myLock(m_pFrameCriticalSection);
	myLock.Lock();

	MV_FRAME_OUT_INFO_EX* pCurFrameInfo = static_cast<MV_FRAME_OUT_INFO_EX*>(pFrameInfo);
	if (nullptr == pCurFrameInfo) return;

	// get cur frame info
	int nFrameIndex = GetCurFrameIndex();

	// call interface func
	IFG2P_FrameGrabbed(nFrameIndex, pData, pCurFrameInfo->nFrameLen);

	//TRACE(_T("Channel: %d, Frame: %d, Total: %d\n"), nChannelIndex, nFrameIndex, nGrabbedFrameCount);

	SetCurFrameIndex(++nFrameIndex);
	SetCurFrameCount(nFrameIndex);
}

int CFrameGrabber_MVS_GigE::Connect(const CFrameGrabberParam& grabberParam)
{
	m_GrabberStatus.Reset();
	CFrameGrabberParam *pGrabberParam = dynamic_cast<CFrameGrabberParam*>(&m_GrabberStatus);
	*pGrabberParam = grabberParam;

	USES_CONVERSION;
	char str_serialNum[200];
#ifdef _UNICODE
	sprintf_s(str_serialNum, "%s", W2A(m_GrabberStatus.GetParam_ConnectAddr()));
#else
	sprintf_s(str_serialNum, "%s", strFilename);
#endif

	MV_CC_DEVICE_INFO_LIST stDeviceList;
	memset(&stDeviceList, 0, sizeof(MV_CC_DEVICE_INFO_LIST));
	int nRet = MV_CC_EnumDevices(MV_GIGE_DEVICE | MV_USB_DEVICE, &stDeviceList);

	if (MV_OK != nRet)
	{
		//printf("Enum Devices fail! nRet [0x%x]\n", nRet);
		return 0;
	}

	// check device count
	if (stDeviceList.nDeviceNum < 1)
	{
		//printf("Find No Devices!\n");
		return 0;
	}

	// for device 
	MV_CC_DEVICE_INFO* pDeviceInfo = nullptr;
	for (unsigned int i = 0; i < stDeviceList.nDeviceNum; i++)
	{
		if (NULL == stDeviceList.pDeviceInfo[i])
		{
			break;
		}

		int nValue = 1;
		switch (stDeviceList.pDeviceInfo[i]->nTLayerType)
		{
		case MV_GIGE_DEVICE: // gigE
			nValue = strcmp(str_serialNum, (const char*)stDeviceList.pDeviceInfo[i]->SpecialInfo.stGigEInfo.chSerialNumber);
			break;

		case MV_USB_DEVICE: // USB
			nValue = strcmp(str_serialNum, (const char*)stDeviceList.pDeviceInfo[i]->SpecialInfo.stUsb3VInfo.chSerialNumber);
			break;
		}

		if (0 == nValue)
		{
			pDeviceInfo = stDeviceList.pDeviceInfo[i];
			break;
		}
	}

	if (nullptr == pDeviceInfo)
	{
		return 0;
	}
	
	// open?
	if (MV_OK == m_pCamera->Open(pDeviceInfo))
	{
		// if gigE
		if (pDeviceInfo->nTLayerType == MV_GIGE_DEVICE)
		{
			UINT nPacketSize = 0;
			if (MV_OK == m_pCamera->GetOptimalPacketSize(&nPacketSize))
			{
				if (MV_OK == m_pCamera->SetIntValue("GevSCPSPacketSize", nPacketSize))
				{

				}
			}
		}
		
		// set call back
		if (MV_OK != m_pCamera->RegisterImageCallBack(ImageCallBackEx, this))
		{
			m_pCamera->Close();
			return 0;
		}
			
		// open success?
		m_pCamera->SetImageNodeNum(m_GrabberStatus.GetParam_FrameCount());


		MVCC_FLOATVALUE stFloatValue = { 0 };
		if (MV_OK == m_pCamera->GetFloatValue("ExposureTime", &stFloatValue))
		{
			m_GrabberStatus.SetStatus_ExposureTime(stFloatValue.fCurValue);
		}

		stFloatValue = { 0 };
		if (MV_OK == m_pCamera->GetFloatValue("ResultingFrameRate", &stFloatValue))
		{
			m_GrabberStatus.SetStatus_FrameRate(stFloatValue.fCurValue);
		}

		stFloatValue = { 0 };
		if (MV_OK == m_pCamera->GetFloatValue("Gain", &stFloatValue))
		{
			m_GrabberStatus.SetStatus_AnalogGain(stFloatValue.fCurValue);
		}

		SetTriggerMode(TriggerMode_Internal);
		SetTriggerSource(TriggerSource_Hardware);

		m_GrabberStatus.SetStatus_Connected(FrameGrabber_Connected);
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);

		return 1;
	}

	// not open?
	return 0;
	
}

int CFrameGrabber_MVS_GigE::Disconnect()
{
	if (nullptr == m_pCamera) return 0;

	// close
	int nRet = m_pCamera->Close();
	if (MV_OK != nRet)
	{
		return 0;
	}

	return 1;
}

int CFrameGrabber_MVS_GigE::StartGrab()
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (m_GrabberStatus.GetStatus_Grabbing()) // grabbing
	{
		return GrabReturn_Grabbing;
	}

	SetCurFrameIndex(0);

	int nRet = m_pCamera->StartGrabbing();
	if (MV_OK != nRet)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);
		return 0;
	}

	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_Grabbing);
	return 1;
}

int CFrameGrabber_MVS_GigE::StopGrab()
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}
	
	if (GrabReturn_NotGrab == m_GrabberStatus.GetStatus_Grabbing()) // grabbing?
	{
		return 1;
	}

	int nRet = m_pCamera->StopGrabbing();
	if (MV_OK != nRet)
	{
		return 0;
	}

	m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotGrab);

	return 1;
}

int CFrameGrabber_MVS_GigE::SendTrigger(int nTriggerCount /*= 1*/)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (MV_OK != m_pCamera->CommandExecute("TriggerSoftware"))
	{
		return 0;
	}

	return 1;
}

int CFrameGrabber_MVS_GigE::SetTriggerMode(int nMode)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	int nRet = MV_E_UNKNOW;
	switch (nMode)
	{
	case TriggerMode_Internal:
		nRet = m_pCamera->SetEnumValue("TriggerMode", MV_TRIGGER_MODE_OFF);
		break;

	case TriggerMode_External:
		nRet = m_pCamera->SetEnumValue("TriggerMode", MV_TRIGGER_MODE_ON);
		break;

	default:
		return 0;
		break;
	}

	if (nRet == MV_OK)
	{
		m_GrabberStatus.SetStatus_TriggerMode(nMode);
		return 1;
	}
	return 0;
}

int CFrameGrabber_MVS_GigE::SetTriggerSource(int nSource)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	int nRet = MV_E_UNKNOW;
	switch (nSource)
	{
	case TriggerSource_Software:
		nRet = m_pCamera->SetEnumValue("TriggerSource", MV_TRIGGER_SOURCE_SOFTWARE);
		break;

	case TriggerSource_Hardware:
		nRet = m_pCamera->SetEnumValue("TriggerSource", MV_TRIGGER_SOURCE_LINE2);
		break;

	default:
		return 0;
		break;
	}

	if (nRet == MV_OK)
	{
		m_GrabberStatus.SetStatus_TriggerSource(nSource);
		return 1;
	}
	return 0;
}

int CFrameGrabber_MVS_GigE::SetExposureTime(double dTime)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

   // en:Adjust these two exposure mode to allow exposure time effective
	if (MV_OK != m_pCamera->SetEnumValue("ExposureMode", MV_EXPOSURE_MODE_TIMED))
	{
		return 0;
	}

	if (MV_OK != m_pCamera->SetEnumValue("ExposureAuto", MV_EXPOSURE_AUTO_MODE_OFF))
	{
		return 0;
	}
	
	if (MV_OK != m_pCamera->SetFloatValue("ExposureTime", (float)dTime))
	{
		return 0;
	}

	m_GrabberStatus.SetStatus_ExposureTime(dTime);
	return 1;
}

int CFrameGrabber_MVS_GigE::SetAnalogGain(double dGain)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (MV_OK != m_pCamera->SetEnumValue("GainAuto", 0))
	{
		return 0;
	}
	
	if (MV_OK != m_pCamera->SetFloatValue("Gain", (float)dGain))
	{
		return 0;
	}

	m_GrabberStatus.SetStatus_ExposureTime(dGain);
	return 1;
}

int CFrameGrabber_MVS_GigE::SetFrameRate(double dRate)
{
	if (GetConnected() != FrameGrabber_Connected)
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return 0;
	}

	if (MV_OK != m_pCamera->SetBoolValue("AcquisitionFrameRateEnable", true))
	{
		return 0;
	}
	
	if (MV_OK != m_pCamera->SetFloatValue("AcquisitionFrameRate", (float)dRate))
	{
		return 0;
	}

	m_GrabberStatus.SetStatus_ExposureTime(dRate);
	return 1;
}

int CFrameGrabber_MVS_GigE::GetConnected()
{
	if (nullptr == m_pCamera)
	{
		m_GrabberStatus.SetStatus_Connected(FrameGrabber_NotConnect);
		return FrameGrabber_NotConnect;
	}

	bool bConnect = m_pCamera->IsDeviceConnected();
	m_GrabberStatus.SetStatus_Connected(bConnect);

	return m_GrabberStatus.GetStatus_Connected();
}

int CFrameGrabber_MVS_GigE::GetGrabbing()
{
	if (GetConnected() != FrameGrabber_Connected) // not connect
	{
		m_GrabberStatus.SetStatus_Grabbing(GrabReturn_NotConnect);
		return GrabReturn_NotConnect;
	}

	return m_GrabberStatus.GetStatus_Grabbing();
}

