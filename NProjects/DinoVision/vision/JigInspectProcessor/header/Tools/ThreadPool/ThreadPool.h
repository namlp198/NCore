#pragma once

typedef struct _CH_TP_CALLBACK_ENVIRON 
{
	TP_VERSION                         Version;
	PTP_POOL                           Pool;
	PTP_CLEANUP_GROUP                  CleanupGroup;
	PTP_CLEANUP_GROUP_CANCEL_CALLBACK  CleanupGroupCancelCallback;
	PVOID                              RaceDll;
	struct _ACTIVATION_CONTEXT        *ActivationContext;
	PTP_SIMPLE_CALLBACK                FinalizationCallback;
	union {
		DWORD                          Flags;
		struct {
			DWORD                      LongFunction :  1;
			DWORD                      Persistent   :  1;
			DWORD                      Private      : 30;
		} s;
	} u;    
	TP_CALLBACK_PRIORITY               CallbackPriority;
	DWORD                              Size;
} CH_TP_CALLBACK_ENVIRON;

class AFX_EXT_CLASS CThreadPool
{
public:
	CThreadPool(int nThreadCount=1);
	virtual ~CThreadPool(void);
	int	GetThreadCount() const;

private:
	void CreateThreadPools(int nThreadCount=1);
	void CloseThreadPools();

protected:
	int						m_nThreadCount;
	int						m_nRollback;
	PTP_POOL				m_pPool;
	CH_TP_CALLBACK_ENVIRON	m_CallBackEnviron;
	PTP_CLEANUP_GROUP		m_pCleanupGroup;
};

