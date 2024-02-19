#pragma once

#include "ActMulti.h"	

class CMxComponent
{
public:
	CMxComponent();
	virtual ~CMxComponent();

public:
	BOOL	Open(int nStationNum);
	BOOL	Close();
	BOOL	IsOpen() const;

	HRESULT STDMETHODCALLTYPE ReadDeviceBlock2(
		/* [string][in] */ BSTR szDevice,
		/* [in] */ LONG lSize,
		/* [size_is][out] */ SHORT __RPC_FAR *lpsData,
		/* [retval][out] */ LONG __RPC_FAR *lplReturnCode);

	HRESULT STDMETHODCALLTYPE WriteDeviceBlock2(
		/* [string][in] */ BSTR szDevice,
		/* [in] */ LONG lSize,
		/* [size_is][in] */ SHORT __RPC_FAR *lpsData,
		/* [retval][out] */ LONG __RPC_FAR *lplReturnCode);


private:
	CComPtr<IActEasyIF3> m_pComm;
};



