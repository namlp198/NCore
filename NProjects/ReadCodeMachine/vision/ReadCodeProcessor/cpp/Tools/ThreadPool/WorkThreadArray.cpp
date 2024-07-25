#include "pch.h"
#include "WorkThreadArray.h"

CWorkThreadArray::CWorkThreadArray() : CWorkThreadPool(1)
{
	m_pInterface = NULL;
}

CWorkThreadArray::CWorkThreadArray(IWorkThreadArray2Parent* pInterface, int nThreadPool) : CWorkThreadPool(nThreadPool)
{
	m_pInterface = pInterface;

	m_bComplete = FALSE;
}

CWorkThreadArray::~CWorkThreadArray(void)
{
}

BOOL CWorkThreadArray::CreateWorkThread(PVOID pParameter)
{
	m_bComplete = FALSE;

	return CWorkThreadPool::CreateWorkThread(pParameter);
}

void CWorkThreadArray::WorkThreadProcess(PVOID pParameter)
{
	if(m_pInterface != NULL)
		m_pInterface->WorkThreadProcessArray(pParameter);

	m_bComplete = TRUE;
}
