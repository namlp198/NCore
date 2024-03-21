#pragma once
#include "TempInspectHikCam.h"
#include "TempInspectCore.h"
#include "TempInspectSystemConfig.h"
#include "TempInspectStatus.h"
#include "TempInspectResult.h"

class AFX_EXT_CLASS CTempInspectProcessor : public ITempInspectHikCamToParent,
	                                        public ITempInspectCoreToParent
{
public:
	CTempInspectProcessor();
	~CTempInspectProcessor();

public:
	BOOL Initialize();
	BOOL Destroy();
	BOOL LoadSystemConfig();
	BOOL LoadRecipe();
	BOOL CreateBuffer();

public:
	BOOL InspectStart(int nThreadCount, int nCamIdx);
	BOOL InspectStop(int nCamIdx);

	static void ReceivedImageCallback(LPVOID pBuffer, int nGrabberIdx,int nNextFrameIdx, LPVOID param);
	void ReceivedImageCallbackEx(int nGrabberIdx, int nNextFrameIdx, LPVOID pBuffer);

public:
	virtual void							InspectComplete(int nCamIdx);
	virtual LPBYTE							GetFrameImage(int nCamIdx, UINT nFrameIndex);
	virtual LPBYTE							GetBufferImage(int nCamIdx, UINT nY);
	virtual CTempInspectRecipe*             GetRecipe(int nIdx) { return m_pTempInspRecipe[nIdx]; }
	virtual CTempInspectSystemConfig*       GetSystemConfig() { return m_pTempInspSysConfig; }
	virtual CTempInspectStatus*             GetEdgeInspectStatus(int nCamIdx) { return m_pTempInspStatus[nCamIdx]; }
	virtual int                             PopInspectWaitFrame(int nCamIdx);

public:
	CTempInspectHikCam* GetHikCamControl() { return m_pHikCamera; }

private:

	// Hik cam
	CTempInspectHikCam*                 m_pHikCamera;

	// Inspect Core
	CTempInspectCore*                   m_pTempInspCore[MAX_CAMERA_INSP_COUNT];

	// config
	CTempInspectSystemConfig*           m_pTempInspSysConfig;

	// recipe
	CTempInspectRecipe*                 m_pTempInspRecipe[MAX_CAMERA_INSP_COUNT];
						                
	// status			                
	CTempInspectStatus*                 m_pTempInspStatus[MAX_CAMERA_INSP_COUNT];
						                
	// result			                
	CTempInspectResult*                 m_pTempInspResult[MAX_CAMERA_INSP_COUNT];

	// Inspect Wait Frame List
	CCriticalSection					m_csInspectWaitList[MAX_CAMERA_INSP_COUNT];
	std::queue<int>						m_queueInspectWaitList[MAX_CAMERA_INSP_COUNT];

	// Image Buffer..
	CSharedMemoryBuffer*                 m_pImageBuffer[MAX_CAMERA_INSP_COUNT];
};