#pragma once

#include "Config.h"

class CFileIOInterface
{
public:
	CFileIOInterface(void);
	virtual ~CFileIOInterface(void);

#pragma region Interface Function
	virtual BOOL LoadData(CConfig* pConfig) = 0;
	virtual BOOL SaveData(CConfig* pConfig) = 0;
#pragma endregion Interface Function
};

