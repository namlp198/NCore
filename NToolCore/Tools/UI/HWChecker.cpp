#include "pch.h"
#include "HWChecker.h"

CHWChecker::CHWChecker()
{
	m_hQuery = 0;
	m_hCounter = 0;
}

CHWChecker::~CHWChecker()
{
	DestroyStatus_CPU();
}


static float CalculateCPULoad(unsigned long long idleTicks, unsigned long long totalTicks)
{

	static unsigned long long _previousTotalTicks = 0;

	static unsigned long long _previousIdleTicks = 0;

	unsigned long long totalTicksSinceLastTime = totalTicks - _previousTotalTicks;

	unsigned long long idleTicksSinceLastTime = idleTicks - _previousIdleTicks;

	float ret = 1.0f - ((totalTicksSinceLastTime > 0) ? ((float)idleTicksSinceLastTime) / totalTicksSinceLastTime : 0);

	_previousTotalTicks = totalTicks;

	_previousIdleTicks = idleTicks;

	return ret;

}

static unsigned long long FileTimeToInt64(const FILETIME& ft)
{
	return (((unsigned long long)(ft.dwHighDateTime)) << 32) | ((unsigned long long)ft.dwLowDateTime);
}

float GetCPULoad()

{

	FILETIME idleTime, kernelTime, userTime;

	return GetSystemTimes(&idleTime, &kernelTime, &userTime) ? CalculateCPULoad(FileTimeToInt64(idleTime), FileTimeToInt64(kernelTime) + FileTimeToInt64(userTime)) : -1.0f;

}



BOOL CHWChecker::GetStatus_CPU(double& dUseRate)
{

	dUseRate = GetCPULoad();

	return TRUE;
	dUseRate = -1.0;

	if (InitStatus_CPU() == FALSE)
		return FALSE;

	PDH_STATUS      status = PdhCollectQueryData(m_hQuery);

	if (status != ERROR_SUCCESS)
		return FALSE;

	PDH_FMT_COUNTERVALUE    value;

	status = PdhGetFormattedCounterValue(m_hCounter, PDH_FMT_DOUBLE, NULL, &value);

	if (status != ERROR_SUCCESS)
		return FALSE;

	dUseRate = value.doubleValue;

	return TRUE;
}


BOOL CHWChecker::GetStaus_Memory(double& dStatus)
{
	MEMORYSTATUS memoryStatus;
	GlobalMemoryStatus(&memoryStatus);

	dStatus = (double)memoryStatus.dwMemoryLoad;

	return TRUE;
}

BOOL CHWChecker::GetStatus_HDD_C(double& dAvailRate, double& dFree_GB, double& dTotal_GB)
{
	ULARGE_INTEGER avail, total, free;

	GetDiskFreeSpaceEx(TEXT("c:\\"), &avail, &total, &free);

	dAvailRate = ((double)avail.QuadPart / (double)total.QuadPart) * 100.0;

	dFree_GB = ((double)free.QuadPart / 1000000000.0);
	dTotal_GB = ((double)total.QuadPart / 1000000000.0);

	return TRUE;
}

BOOL CHWChecker::GetStatus_HDD_D(double& dAvailRate, double& dFree_GB, double& dTotal_GB)
{
	ULARGE_INTEGER avail, total, free;

	GetDiskFreeSpaceEx(TEXT("d:\\"), &avail, &total, &free);

	dAvailRate = ((double)avail.QuadPart / (double)total.QuadPart) * 100.0;

	dFree_GB = ((double)free.QuadPart / 1000000000.0);
	dTotal_GB = ((double)total.QuadPart / 1000000000.0);

	return TRUE;
}

BOOL CHWChecker::GetStatus_HDD_E(double& dAvailRate, double& dFree_GB, double& dTotal_GB)
{
	ULARGE_INTEGER avail, total, free;

	GetDiskFreeSpaceEx(TEXT("e:\\"), &avail, &total, &free);

	dAvailRate = ((double)avail.QuadPart / (double)total.QuadPart) * 100.0;

	dFree_GB = ((double)free.QuadPart / 1000000000.0);
	dTotal_GB = ((double)total.QuadPart / 1000000000.0);

	return TRUE;
}

BOOL CHWChecker::GetStatus_HDD_F(double& dAvailRate, double& dFree_GB, double& dTotal_GB)
{
	ULARGE_INTEGER avail, total, free;

	GetDiskFreeSpaceEx(TEXT("f:\\"), &avail, &total, &free);

	dAvailRate = ((double)avail.QuadPart / (double)total.QuadPart) * 100.0;

	dFree_GB = ((double)free.QuadPart / 1000000000.0);
	dTotal_GB = ((double)total.QuadPart / 1000000000.0);

	return TRUE;
}

BOOL CHWChecker::InitStatus_CPU()
{
	if (m_hQuery != 0)
		return TRUE;

	PDH_STATUS status = PdhOpenQuery(NULL, NULL, &m_hQuery);

	if (status != ERROR_SUCCESS)
		return FALSE;

	status = PdhAddCounter(m_hQuery, L"\\Processor(_TOTAL)\\% Processor Time", NULL, &m_hCounter);

	if (status != ERROR_SUCCESS)
		return FALSE;

	status = PdhCollectQueryData(m_hQuery);

	if (status != ERROR_SUCCESS)
	{
		return FALSE;
	}

	return TRUE;
}

void CHWChecker::DestroyStatus_CPU()
{
	if (m_hQuery != 0)
		PdhCloseQuery(m_hQuery);

	m_hQuery = 0;
}

BOOL CHWChecker::GetStatus_HDD(CString strDriverName, double& dAvailableMByte, double& dTotalMByte)
{
	LPCWSTR lpDirectoryName = strDriverName;
	ULARGE_INTEGER lpFreeBytesAvailableToCaller;
	ULARGE_INTEGER lpTotalNumberOfBytes;
	ULARGE_INTEGER lpTotalNumberOfFreeBytes;

	UINT nType = ::GetDriveTypeW(lpDirectoryName);

	if (GetDiskFreeSpaceExW(lpDirectoryName, &lpFreeBytesAvailableToCaller, &lpTotalNumberOfBytes, &lpTotalNumberOfFreeBytes) == FALSE)
		return FALSE;

	dAvailableMByte = (double)(lpFreeBytesAvailableToCaller.QuadPart) / 1024000;
	dTotalMByte = (double)(lpTotalNumberOfBytes.QuadPart) / 1024000;

	return TRUE;
}