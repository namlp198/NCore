#pragma once

class CWorkThreadData
{
public:
	CWorkThreadData(PVOID pPtr) 
	{
		pCallerPtr = pPtr;
		bProcessed = false;
	}

	virtual ~CWorkThreadData()
	{
		pCallerPtr = NULL;
		bProcessed = false;
	}
	PVOID	pCallerPtr;
	bool	bProcessed;
};

