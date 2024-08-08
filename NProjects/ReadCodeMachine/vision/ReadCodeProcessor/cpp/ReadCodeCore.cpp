#include "pch.h"
#include "ReadCodeCore.h"

CReadCodeCore::CReadCodeCore(IReadCodeCoreToParent* pInterface)
{
	m_pInterface = pInterface;
	m_bSimulator = FALSE;
}

CReadCodeCore::~CReadCodeCore()
{
}

void CReadCodeCore::CreateInspectThread(int nThreadCount)
{
	DeleteInspectThread();

	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	m_vecProcessedFrame.clear();
	m_vecProcessedFrame.resize(MAX_FRAME_WAIT, FALSE);

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		if (m_pWorkThreadArray[nThreadIdx] == NULL)
			m_pWorkThreadArray[nThreadIdx] = new CWorkThreadArray(this, 1);

		m_bRunningThread[nThreadIdx] = TRUE;
		m_pWorkThreadArray[nThreadIdx]->CreateWorkThread(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], nThreadIdx));

		localLock.Unlock();
	}
}

void CReadCodeCore::DeleteInspectThread()
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

void CReadCodeCore::WorkThreadProcessArray(PVOID pParameter)
{
	CTempInspectCoreThreadData* pThreadData = static_cast<CTempInspectCoreThreadData*>(pParameter);

	if (pThreadData == NULL) return;

	UINT nThreadOrder = pThreadData->m_nThreadIdx;
	RunningThread_INSPECT((int)nThreadOrder);
}

void CReadCodeCore::RunningThread_INSPECT(int nThreadIndex)
{
	if (m_pInterface == NULL)
		return;

	int nGrabberIdx = 0;
	CReadCodeRecipe* recipe = m_pInterface->GetRecipeControl();
	CReadCodeSystemSetting* sysSetting = m_pInterface->GetSystemSettingControl();

	while (m_bRunningThread[nThreadIndex] == TRUE)
	{
		// for avoid UI Freezing
		Sleep(1);

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

	m_pInterface->InspectComplete(FALSE);
}

void CReadCodeCore::StopThread()
{
	for (int i = 0; i < MAX_THREAD_COUNT; i++) {
		CSingleLock lockLocal(&m_csWorkThreadArray[i]);
		lockLocal.Lock();
		m_bRunningThread[i] = FALSE;
		lockLocal.Unlock();
	}
}

void CReadCodeCore::StartThread(int nThreadCount)
{
	m_nThreadCount = (MAX_THREAD_COUNT < nThreadCount) ? MAX_THREAD_COUNT : nThreadCount;

	for (int nThreadIdx = 0; nThreadIdx < (int)m_nThreadCount; nThreadIdx++)
	{
		CSingleLock localLock(&m_csWorkThreadArray[nThreadIdx]);
		localLock.Lock();

		m_bRunningThread[nThreadIdx] = TRUE;

		m_pWorkThreadArray[nThreadIdx]->WorkThreadProcess(new CTempInspectCoreThreadData(m_pWorkThreadArray[nThreadIdx], nThreadIdx));
		localLock.Unlock();
	}
}

void CReadCodeCore::Inspect_Simulation(int nCoreIdx, int nFrame)
{
	int nBuff = nCoreIdx;
	LPBYTE pBuff = m_pInterface->GetSimulatorBuffer(nBuff, nFrame);

	if (pBuff == NULL)
		return;

	cv::Mat matSrc(FRAME_HEIGHT, FRAME_WIDTH, CV_8UC3);
	cv::Mat matResult(FRAME_HEIGHT, FRAME_WIDTH, CV_8UC3);

	DWORD dFrameSize = FRAME_HEIGHT * FRAME_WIDTH * NUMBER_OF_CHANNEL_BGR;
	memcpy(matSrc.data, pBuff, dFrameSize);

	matSrc.copyTo(matResult);

	cv::cvtColor(matSrc, matSrc, cv::COLOR_BGR2GRAY);

	BOOL bRet = FALSE;
	CString csRet;

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		if (barcodes.size() == m_pInterface->GetRecipeControl()->m_nMaxCodeCount)
		{
			bRet = TRUE;
		}
		else
		{
			bRet = FALSE;
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	m_pInterface->SetResultBuffer(0, 0, matResult.data);

	m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_bInspectCompleted = TRUE;
	m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_bResultStatus = bRet;
	ZeroMemory(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString, sizeof(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString));
	wsprintf(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);

	m_pInterface->InspectComplete(TRUE);
}

void CReadCodeCore::Inspect_Real(int nCoreIdx, LPBYTE pBuffer)
{
	ProcessFrame(nCoreIdx, pBuffer);
}

void CReadCodeCore::ProcessFrame(int nCoreIdx, LPBYTE pBuffer)
{
	if (pBuffer == NULL)
		return;

	// Start read code

	cv::Mat matSrc(m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameHeight, m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameWidth, CV_8UC1);
	cv::Mat matResult(m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameHeight, m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameWidth, CV_8UC3);

	DWORD dFrameSize = m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameHeight * m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nFrameWidth * m_pInterface->GetCameraSettingControl(nCoreIdx)->m_nChannel;
	memcpy(matSrc.data, (LPBYTE)pBuffer, dFrameSize);

	/*char pathSaveImage[200] = { };
	sprintf_s(pathSaveImage, "%s%s_%d.bmp", "D:\\entry\\NCore\\NProjects\\ReadCodeMachine\\bin\\SaveImages\\", "Code_", nNextFrameIdx);
	cv::imwrite(pathSaveImage, matSrc);*/

	cv::cvtColor(matSrc, matResult, cv::COLOR_GRAY2BGR);

	BOOL bRet = FALSE;
	CString csRet;

	auto barcodes = ReadBarcodes(matSrc);

	if (!barcodes.empty())
	{
		if (barcodes.size() == m_pInterface->GetRecipeControl()->m_nMaxCodeCount)
		{
			bRet = TRUE;
		}
		else
		{
			bRet = FALSE;
		}

		std::string sRet;
		const char* const delim = ";";
		for (auto& barcode : barcodes) {
			DrawBarcode(matResult, barcode);
			if (!sRet.empty())
				sRet += delim;

			sRet += barcode.text();
		}
		csRet = (CString)sRet.c_str();
	}

	m_pInterface->SetResultBuffer(nCoreIdx, 0, matResult.data); // cause the buffer just have a frame should be frame index = 0

	m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_bInspectCompleted = TRUE;
	m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_bResultStatus = bRet;
	ZeroMemory(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString, sizeof(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString));
	wsprintf(m_pInterface->GetReadCodeResultControl(nCoreIdx)->m_sResultString, _T("%s"), (TCHAR*)(LPCTSTR)csRet);

	m_pInterface->InspectComplete(FALSE);
}
