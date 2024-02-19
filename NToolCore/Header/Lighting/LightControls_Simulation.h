#pragma once

#include "LightControls.h"

class AFX_EXT_CLASS CLightControls_Simulation : public CLightControls
{
public:
	CLightControls_Simulation(int nIndex, ILightControls2Parent* pILC2P);
	virtual ~CLightControls_Simulation();

public:
	virtual BOOL Connect(const CLightControlsParam& param);
	virtual void Disconnect();

	// setter
	virtual BOOL SetLightLevel(int nValue, int nChannel = 0);
	virtual BOOL SetLightLevel(double dValue, int nChannel = 0);
	virtual BOOL SetLightStatus(int nValue, int nChannel = 0);
	virtual BOOL SetLightStatusAll(int nValue);
	virtual BOOL SetLightLevelAll(int nValue);


	BOOL GetConnected();
};

