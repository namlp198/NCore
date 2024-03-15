#include "pch.h"
#include "IMvCamera.h"

CIMVCamera::CIMVCamera()
{
	m_hDevHandle = MV_NULL;
}

CIMVCamera::~CIMVCamera()
{
	if (m_hDevHandle)
	{
		IMV_DestroyHandle(m_hDevHandle);
		m_hDevHandle = MV_NULL;
	}
}

int CIMVCamera::EnumDevices(IMV_DeviceList* pstDevList, unsigned int nTLayerType)
{
	return IMV_EnumDevices(pstDevList, nTLayerType);
}

int CIMVCamera::Open(IMV_ECreateHandleMode mode, IMV_DeviceInfo deviceInfo, unsigned int idx)
{
	if (m_hDevHandle)
		return IMV_INVALID_HANDLE;

	int nRet = IMV_OK;
	switch (mode)
	{
	case modeByIndex:
		nRet = IMV_CreateHandle(&m_hDevHandle, modeByIndex, (void*)&idx);
		break;
	case modeByCameraKey:
		nRet = IMV_CreateHandle(&m_hDevHandle, modeByCameraKey, deviceInfo.cameraKey);
		break;
	case modeByDeviceUserID:
		break;
	case modeByIPAddress:
		break;
	default:
		break;
	}

	if (IMV_OK != nRet)
	{
		return nRet;
	}

	nRet = IMV_Open(m_hDevHandle);
	if (IMV_OK != nRet)
	{
		IMV_DestroyHandle(m_hDevHandle);
		m_hDevHandle = MV_NULL;
	}
	return nRet;
}

int CIMVCamera::Close()
{
	if (MV_NULL == m_hDevHandle)
	{
		return IMV_INVALID_HANDLE;
	}

	if (!IMV_IsOpen(m_hDevHandle))
		return 0;
	// Close camera 
	int nRet = IMV_Close(m_hDevHandle);
	if (IMV_OK != nRet)
	{
		printf("Close camera failed! ErrorCode[%d]\n", nRet);
		return nRet;
	}
	nRet = IMV_DestroyHandle(m_hDevHandle);
	m_hDevHandle = MV_NULL;

	return nRet;
}

bool CIMVCamera::IsDeviceConnected()
{
	return IMV_IsOpen(m_hDevHandle);
}

int CIMVCamera::RegisterImageCallBack(void(*cbOutput)(IMV_Frame* pFrame, void* pUser), void* pUser)
{
	return IMV_AttachGrabbing(m_hDevHandle, cbOutput, pUser);
}

int CIMVCamera::StartGrabbing()
{
	if (!IsDeviceConnected())
		return 0;
	if (IMV_IsGrabbing(m_hDevHandle))
		return 1;
	return IMV_StartGrabbing(m_hDevHandle);
}

int CIMVCamera::StopGrabbing()
{
	if (!IsDeviceConnected())
		return 0;
	if (!IMV_IsGrabbing(m_hDevHandle))
		return 1;
	return IMV_StopGrabbing(m_hDevHandle);
}

int CIMVCamera::GetDeviceInfo(IMV_DeviceInfo* pDevInfo)
{
	return IMV_GetDeviceInfo(m_hDevHandle, pDevInfo);
}

int CIMVCamera::ConvertPixelType(IMV_PixelConvertParam* pCvtParam)
{
	return IMV_PixelConvert(m_hDevHandle, pCvtParam);
}

int CIMVCamera::CommandExecute(IN const char* strKey)
{
	if (!IsDeviceConnected())
		return 0;

	if (IMV_IsGrabbing(m_hDevHandle))
		return 0;

	int ret = IMV_ExecuteCommandFeature(m_hDevHandle, strKey);
	if (IMV_OK != ret)
	{
		printf("Execute command [%s] failed! ErrorCode[%d]\n", strKey, ret);
		return 0;
	}
	return 1;
}

int CIMVCamera::SetEnumFeature(const char* pFeature, const char* pSymbol)
{
	return IMV_SetEnumFeatureSymbol(m_hDevHandle, pFeature, pSymbol);
}

