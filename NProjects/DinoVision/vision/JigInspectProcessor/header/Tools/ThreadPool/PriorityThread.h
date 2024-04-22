#pragma once

#include "WorkThreadPool.h"

class CPriorityThreadData : public CWorkThreadData
{
public:
	CPriorityThreadData(PVOID pPtr) : CWorkThreadData(pPtr)
	{
		m_unMsg = 0;
		m_WParam = NULL;
		m_LParam = NULL;
	}
	virtual ~CPriorityThreadData()
	{
	}

	UINT		m_unMsg;
	WPARAM		m_WParam;
	LPARAM		m_LParam;
};

interface IPriorityThread2Parent
{
	virtual void IPT2P_PriorityThread(CPriorityThreadData* pData) = 0;
};

class AFX_EXT_CLASS CPriorityThread : public CWorkThreadPool
{
public:
	CPriorityThread(IPriorityThread2Parent* pFT2P, int nPriority);
	virtual ~CPriorityThread(void);
	BOOL AddPriorityThreadData(UINT msg, WPARAM wParam = NULL, LPARAM lParam = NULL);

protected:
	virtual void WorkThreadProcess(PVOID pParameter);

protected:
	IPriorityThread2Parent*		m_pPT2P;
	int							m_nPriority;	
};
