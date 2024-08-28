#pragma once

#include "LightControlsParam.h"

class AFX_EXT_CLASS CLightControls
{
public:
	CLightControls(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls(void);

public:
	virtual BOOL Connect(const CLightControlsParam& param) = 0;
	virtual void Disconnect() = 0;

	// setter
	virtual BOOL SetLightLevel(int nValue, int nChannel = 0) = 0;
	virtual BOOL SetLightLevel(double dValue, int nChannel = 0) = 0;
	virtual BOOL SetLightStatus(int nValue, int nChannel = 0) = 0;
	virtual BOOL SetLightStatusAll(int nValue) = 0;
	virtual BOOL SetLightLevelAll(int nValue) = 0;

	// getter
	virtual BOOL GetLightLevel(int &nValue, int nChannel = 0);
	virtual BOOL GetLightLevel(double &dValue, int nChannel = 0);
	virtual BOOL GetLightStatus(int &nValue, int nChannel = 0);
	virtual BOOL GetConnected();
	virtual const CLightControlsStatus* GetStatus() const;

protected:
	int						m_nIndex;
	ILightControls2Parent*	m_pILC2P;
	CLightControlsStatus	m_ControlStatus;
	CCriticalSection m_csLightControl;
};

