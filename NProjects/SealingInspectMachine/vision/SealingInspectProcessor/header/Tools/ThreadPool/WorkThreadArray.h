#pragma once
#include "WorkThreadPool.h"

interface IWorkThreadArray2Parent
{
	virtual void WorkThreadProcessArray(PVOID pParameter) = 0;
};

class AFX_EXT_CLASS CWorkThreadArray : public CWorkThreadPool
{
public:
	CWorkThreadArray();
	CWorkThreadArray(IWorkThreadArray2Parent* pInterface, int nThreadPool);
	~CWorkThreadArray(void);

	void			SetInterface(IWorkThreadArray2Parent* pInterface)	{m_pInterface = pInterface;};

	BOOL			CreateWorkThread(PVOID pParameter);

	virtual void	WorkThreadProcess(PVOID pParameter);

	BOOL			GetComplete()	{return m_bComplete;};

private:

	IWorkThreadArray2Parent* m_pInterface;

	BOOL					 m_bComplete;
};