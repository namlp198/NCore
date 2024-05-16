#pragma once

#include "LightControls.h"

class AFX_EXT_CLASS CLightControls_CCS_PJ2_3005_4CA_PE : public CLightControls
{
public:
	CLightControls_CCS_PJ2_3005_4CA_PE(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls_CCS_PJ2_3005_4CA_PE(void);

public:
	virtual BOOL Connect(const CLightControlsParam& param);
	virtual void Disconnect();

	// setter
	virtual BOOL SetLightLevel(int nValue, int nChannel = 0);
	virtual BOOL SetLightLevel(double dValue, int nChannel = 0);
	virtual BOOL SetLightStatus(int nValue, int nChannel = 0);
	virtual BOOL SetLightStatusAll(int nValue);
	virtual BOOL SetLightLevelAll(int nValue);
};

