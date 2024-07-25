#include "pch.h"
#include "PriorityThread.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

CPriorityThread::CPriorityThread(IPriorityThread2Parent* pFT2P, int nPriority) : CWorkThreadPool(1), m_pPT2P(pFT2P)
{
	m_nPriority = nPriority;
}

CPriorityThread::~CPriorityThread(void)
{

}

void CPriorityThread::WorkThreadProcess(PVOID pParameter)
{
	if (m_pPT2P==NULL) return;

	CPriorityThreadData *pData = static_cast<CPriorityThreadData*>(pParameter);
	if (pData==NULL) return;

	HANDLE hThread = GetCurrentThread();
	SetThreadPriority(hThread, m_nPriority);

	m_pPT2P->IPT2P_PriorityThread(pData);	
}

BOOL CPriorityThread::AddPriorityThreadData(UINT msg, WPARAM wParam, LPARAM lParam)
{
	CPriorityThreadData *pData = new CPriorityThreadData(this);
	if (pData==NULL) return FALSE;

	pData->m_unMsg = msg;
	pData->m_WParam = wParam;
	pData->m_LParam = lParam;

	return CreateWorkThread(pData);
}