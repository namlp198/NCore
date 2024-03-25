#include "pch.h"
#include "TempInspectCore.h"

CTempInspectCore::CTempInspectCore(ITempInspectCoreToParent* pInterface)
{
	m_pInterface = pInterface;

	m_pRecipe = new CTempInspectRecipe;
	m_pVsResult = new CVisionResults;
}

CTempInspectCore::~CTempInspectCore()
{
}

void CTempInspectCore::CreateInspectThread(int nThreadCount)
{
	DeleteInspectThread();

	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	m_vecProcessedFrame.clear();
	m_vecProcessedFrame.resize(MAX_FRAME_COUNT, FALSE);

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;

		m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], emInspectWorkType_Inspect, nThreadIdx));

		localLock.Unlock();
	}
}

void CTempInspectCore::DeleteInspectThread()
{
	for (int nThreadIdx = 0; nThreadIdx < MAX_THREAD_COUNT; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] != NULL)
		{
			delete m_pWorkThreadArray[nThreadIdx];
			m_pWorkThreadArray[nThreadIdx] = NULL;
		}

		localLock.Unlock();
	}
}

void CTempInspectCore::WorkThreadProcessArray(PVOID pParameter)
{
	CTempInspectCoreThreadData* pThreadData = static_cast<CTempInspectCoreThreadData*>(pParameter);

	if (pThreadData == NULL) return;

	UINT nThreadOrder = pThreadData->m_nThreadIdx;

	switch (pThreadData->m_nProcessType)
	{
	case emInspectWorkType_Inspect:
		RunningThread((int)nThreadOrder);
		break;
	}
}

void CTempInspectCore::RunningThread(int nThreadIndex)
{
	if (m_pInterface == NULL)
		return;

	// Load Recipe according cam index
	m_pRecipe = m_pInterface->GetRecipe(m_nCamIndex);

	while (m_bRunningThread[nThreadIndex] == TRUE)
	{
		// for avoid UI Freezing
		Sleep(1);

		// reset result
		m_pVsResult->Reset_Result();

		int nFrameIdx = m_pInterface->PopInspectWaitFrame(m_nCamIndex);

		// Not Grab Image..
		if (nFrameIdx == -1)
		{
			/*CString msg = _T("");
			msg.Format(_T("Cam [%d] no frame, continue..."), m_nCamIndex);
			AfxMessageBox(msg);*/
			//ProcessFrame(m_pRecipe[m_nCamIndex], nThreadIndex, nFrameIdx);

			continue;
		}

		// Process Started..
		m_vecProcessedFrame[nFrameIdx] = TRUE;

		// Process
		ProcessFrame(m_pRecipe, nThreadIndex, nFrameIdx);

		m_pVsResult->Judgement_Result();
		m_pVsResult->Draw_Result();
		m_pResultImageBuffer = m_pVsResult->GetResultImageBuffer();

		// inform inspect completed
		m_pInterface->InspectComplete(m_nCamIndex);
	}

	CSingleLock localLock(&m_csPostProcessing);
	m_csPostProcessing.Lock();

	for (int i = 0; i < MAX_THREAD_COUNT; i++)
	{
		if (nThreadIndex != i && m_bRunningThread[i] == TRUE)
		{
			m_bRunningThread[nThreadIndex] = FALSE;
			m_csPostProcessing.Unlock();
			return;
		}
	}

	m_csPostProcessing.Unlock();

	m_bRunningThread[nThreadIndex] = FALSE;

	m_pInterface->InspectComplete(m_nCamIndex);

	return;
}

void CTempInspectCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++)
		m_bRunningThread[i] = FALSE;
}

void CTempInspectCore::ProcessFrame(CTempInspectRecipe* pRecipe, UINT nThreadIndex, UINT nFrameIndex)
{
	// Get image buffer
	LPBYTE pImageBuffer = m_pInterface->GetFrameImage(m_nCamIndex, nFrameIndex);
	if (pImageBuffer == NULL)
		return;

	// set result image
	int nFrameWidth = pRecipe->GetCameraInfos()->m_nFrameWidth;
	int nFrameHeight = pRecipe->GetCameraInfos()->m_nFrameHeight;
	int chanels = pRecipe->GetCameraInfos()->m_nChannels;
	cv::Mat* matResult = m_pVsResult->GetResultImage();
	matResult = new cv::Mat(nFrameHeight, nFrameWidth, chanels, pImageBuffer);

	QueueLocTools pLocTools = pRecipe->GetQueueLocTools();
	QueueSelROITools pSelROITools = pRecipe->GetQueueSelROITools();

	CSingleLock localLock(&m_csProcessing);
	localLock.Lock();
	while (!pLocTools.empty())
	{
		CLocatorTool locTool = pLocTools.front();
		locTool.SetImageBuffer(pImageBuffer);
		if (locTool.Run())
		{
			m_pVsResult->GetVecLocToolRes()->push_back(locTool.GetLocRes());
		}
		pLocTools.pop();
	}

	while (!pSelROITools.empty())
	{
		CSelectROITool selROITool = pSelROITools.front();

		// set image buffer to VisionAlgorithms object
		selROITool.GetVsAlgorithms().SetImageBuffer(pImageBuffer);

		// get result of locator tool for translate or rotate ROI follow that is result. 
		// *note*: currently, this vector will have only an object that should be always selected first object
		selROITool.GetVsAlgorithms().SetLocResult(m_pVsResult->GetVecLocToolRes()->at(0));

		// get the algorithm of this ROI
		emAlgorithms algorithm = selROITool.GetVsAlgorithms().GetAlgorithm();

		if (selROITool.Run(algorithm))
		{
			// push the result to the vectors follow the algorithm
			switch (algorithm)
			{
			case emCountPixel:
				m_pVsResult->GetVecCntPxlRes()->push_back(selROITool.GetVsAlgorithms().GetCntPxlRes());
				break;
			case emCalculateArea:
				m_pVsResult->GetVecCalAreaRes()->push_back(selROITool.GetVsAlgorithms().GetCalAreaRes());
				break;
			}
		}
		pSelROITools.pop();
	}
	localLock.Unlock();
}

