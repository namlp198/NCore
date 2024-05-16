#include "pch.h"
#include "MxComponent.h"

#ifdef __IActEasyIF_FWD_DEFINED__
#include <initguid.h>
/***************************************************/
#include "ActDefine.h"    // ACT Common Macro Header 
#include "ActMulti_i.c"    // For CustomInterface
//#include "ActEther_i.c"    // For CustomInterface
/***************************************************/
#endif

CMxComponent::CMxComponent()
{
	HRESULT hr = CoInitialize(NULL);

	m_pComm = NULL;
	hr = m_pComm.CoCreateInstance(CLSID_ActEasyIF);
	if (!SUCCEEDED(hr))
	{
		return;
	}
}

CMxComponent::~CMxComponent()
{
	if (m_pComm)
	{
		m_pComm.Release();
	}
	m_pComm = NULL;

	CoUninitialize();
}

BOOL CMxComponent::Open(int nStationNum)
{
/*	if (IsOpen()) return FALSE;*/
	if (NULL == m_pComm)
	{
		return FALSE;
	}

	HRESULT hr = NULL;

	// set station number
	hr = m_pComm->put_ActLogicalStationNumber(nStationNum);
	if (!SUCCEEDED(hr))
	{
		return FALSE;
	}

	if (NULL == m_pComm)
	{
		return FALSE;
	}

	long lRet = 0;
	hr = m_pComm->Open(&lRet);
	if (0 != lRet)
	{
		m_pComm.Release();
		m_pComm = NULL;
		return FALSE;
	}

	return TRUE;
}

BOOL CMxComponent::Close()
{
	if (FALSE == IsOpen()) return TRUE;

	HRESULT hr = NULL;
	long lRet = 0;
	hr = m_pComm->Close(&lRet);
	if (0 != lRet)
	{
		return FALSE;
	}
	
	return TRUE;
}

BOOL CMxComponent::IsOpen() const
{
	if (NULL == m_pComm) return FALSE;
	return TRUE;
}

HRESULT STDMETHODCALLTYPE CMxComponent::ReadDeviceBlock2(/* [string][in] */ BSTR szDevice, /* [in] */ LONG lSize, /* [size_is][out] */ SHORT __RPC_FAR *lpsData, /* [retval][out] */ LONG __RPC_FAR *lplReturnCode)
{
	if (FALSE == IsOpen()) return NULL;
	return m_pComm->ReadDeviceBlock2(szDevice, lSize, lpsData, lplReturnCode);
}

HRESULT STDMETHODCALLTYPE CMxComponent::WriteDeviceBlock2(/* [string][in] */ BSTR szDevice, /* [in] */ LONG lSize, /* [size_is][in] */ SHORT __RPC_FAR *lpsData, /* [retval][out] */ LONG __RPC_FAR *lplReturnCode)
{
	if (FALSE == IsOpen()) return NULL;
	return m_pComm->WriteDeviceBlock2(szDevice, lSize, lpsData, lplReturnCode);
}
