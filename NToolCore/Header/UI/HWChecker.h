#pragma once
#include <windows.h>
#include <pdh.h>
#include <pdhmsg.h>

#pragma comment(lib, "pdh.lib")

class AFX_EXT_CLASS CHWChecker
{
public:
	CHWChecker();
	~CHWChecker();

public:	// CPU Status
	BOOL GetStatus_CPU(double& dUseRate);
	BOOL GetStaus_Memory(double& dStatus);
	BOOL GetStatus_HDD_C(double& dAvailRate, double& dFree_GB, double& dTotal_GB);
	BOOL GetStatus_HDD_D(double& dAvailRate, double& dFree_GB, double& dTotal_GB);
	BOOL GetStatus_HDD_E(double& dAvailRate, double& dFree_GB, double& dTotal_GB);
	BOOL GetStatus_HDD_F(double& dAvailRate, double& dFree_GB, double& dTotal_GB);

private:
	BOOL InitStatus_CPU();
	void DestroyStatus_CPU();

	PDH_HQUERY      m_hQuery;
	PDH_HCOUNTER    m_hCounter;

public:	// HDD Status
	BOOL GetStatus_HDD(CString strDriverName, double& dAvailableMByte, double& dTotalMByte);
};

