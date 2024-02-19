#include "stdafx.h"
#include "Grabber_Matrox_New.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

/**************************************************************************************************************/
/*     생성/소멸                                                                                              */
/**************************************************************************************************************/
CGrabber_Matrox_New::CGrabber_Matrox_New()
{
	m_bInit = false;
	ZeroMemory(m_bGrabbing, sizeof(m_bGrabbing));
	ZeroMemory(m_MilSys, sizeof(m_MilSys));
	ZeroMemory(m_MilDig, sizeof(m_MilDig));
	m_MilApp = NULL;

	for (int i = 0; i < MAX_CAMERA_COUNT; i++)
	{
		m_nCurrentBufferIdx[i] = 0;

		for (int j = 0; j < CIRCULAR_BUFFER_COUNT; j++)
			m_pCurrent[i][j] = NULL;
	}

	m_pParam = NULL;
	m_pGrabCallback = NULL;
}

CGrabber_Matrox_New::~CGrabber_Matrox_New()
{
	CloseDriver();
}

/**************************************************************************************************************/
/*     Driver Open / Close                                                                                    */
/**************************************************************************************************************/

int CGrabber_Matrox_New::OpenDriver(int nBoardIdx, int nBoardPortIdx, int nDigitizerIdx, int yReverse, CString strCamFilePath)
{
	try
	{
		int nBitCount = 8;																															// 8bit, 16bit(12bit 영상 Grab시)		

		if (m_MilApp == NULL)
		{
			MappAlloc(M_DEFAULT, &m_MilApp);																										// Mil Application 할당   
			m_bInit = true;																															// Driver Open 여부
		}

		if(m_MilSys[nBoardIdx] == NULL)
			MsysAlloc(M_SYSTEM_RAPIXOCXP, nBoardIdx, M_DEFAULT, &m_MilSys[nBoardIdx]);																// Mil System 할당

		if (m_MilDig[nDigitizerIdx] == NULL)
			MdigAlloc(m_MilSys[nBoardIdx], nBoardPortIdx, strCamFilePath, M_DEFAULT, &m_MilDig[nDigitizerIdx]);										// Mil Digitizer 할당
		else
			return ERR_BOARD_OK;																													// Mil Digitizer 할당 완료 된 카메라
		
		/*
		if(nDigitizerIdx == 0)
		MdigControl(m_MilDig[0], M_IO_SOURCE + M_TL_TRIGGER, M_AUX_IO0);
		*/
		
		/*
		if(nDigitizerIdx == 2)
		MdigControl(m_MilDig[2], M_IO_SOURCE + M_TL_TRIGGER, M_AUX_IO4);
		*/

		MappControl(M_ERROR, M_PRINT_DISABLE);																										// Application Error

		m_SizeX[nDigitizerIdx] = MdigInquire(m_MilDig[nDigitizerIdx], M_SIZE_X, M_NULL);															// Size X
		m_SizeY[nDigitizerIdx] = MdigInquire(m_MilDig[nDigitizerIdx], M_SIZE_Y, M_NULL);															// Size Y

		m_nCurrentBufferIdx[nDigitizerIdx] = 0;
		for(int i=0; i< CIRCULAR_BUFFER_COUNT; i++)
			m_pCurrent[nDigitizerIdx][i] = new BYTE[m_SizeX[nDigitizerIdx] * m_SizeY[nDigitizerIdx] * (nBitCount / 8)];									// Bit따라 메모리 할당
		
		for (m_nCntGrabBuffer[nDigitizerIdx] = 0; m_nCntGrabBuffer[nDigitizerIdx] < BUFFERING_SIZE_MAX; m_nCntGrabBuffer[nDigitizerIdx]++)
		{
			MbufAlloc2d(m_MilSys[nBoardIdx], m_SizeX[nDigitizerIdx], m_SizeY[nDigitizerIdx], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[nDigitizerIdx][m_nCntGrabBuffer[nDigitizerIdx]]);			//2D Buffer 생성.
			MbufClear(m_MilBufferList[nDigitizerIdx][m_nCntGrabBuffer[nDigitizerIdx]], 0xFF);														// 버퍼 초기화
		}
		MappControl(M_ERROR, M_PRINT_ENABLE);																										// Application Error

		// MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_MODE, M_ASYNCHRONOUS);																		// Digitiger Async 방식으로 Setting
		
		MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_TIMEOUT, M_INFINITE);																			// Grab Timeout Infinite

		MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_DIRECTION_Y, M_FORWARD);																		// 이미지 상하 반전
	}
	catch (...)
	{
		return ERR_BOARD_CONNECT_FAIL;
	}

	return ERR_BOARD_OK;
}

void CGrabber_Matrox_New::CloseDriver()
{
	if (m_bInit == false) 
		return;
	
	//Camera Grab Stop
	for (int nCamIdx = 0; nCamIdx < MAX_CAMERA_COUNT; nCamIdx++)
	{
		//Grab Stop
		Stop(nCamIdx);

		//Buffer Free 1
		for (int i = 0; i < CIRCULAR_BUFFER_COUNT; i++)
		{
			if (m_pCurrent[nCamIdx][i] != NULL)
			{
				delete[](BYTE*)m_pCurrent[nCamIdx][i];
				m_pCurrent[nCamIdx][i] = NULL;
			}
		}

		//Buffer Free 2
		for (int nBufferIdx = 0; nBufferIdx < m_nCntGrabBuffer[nCamIdx]; nBufferIdx++)
			MbufFree(m_MilBufferList[nCamIdx][nBufferIdx]);

		m_nCntGrabBuffer[nCamIdx] = 0;

		//Digitiger Free
		if (m_MilDig[nCamIdx] != 0)
		{
			MdigFree(m_MilDig[nCamIdx]);
			m_MilDig[nCamIdx] = NULL;
		}
	}

	// System Free
	for (int nBoardIdx = 0; nBoardIdx < MAX_BOARD_COUNT; nBoardIdx++)
	{
		if (m_MilSys[nBoardIdx] != NULL)
		{
			MsysFree(m_MilSys[nBoardIdx]);
			m_MilSys[nBoardIdx] = NULL;
		}
	}

	//Application Free
	if (m_MilApp != NULL)
	{
		MappFree(m_MilApp);
		m_MilApp = NULL;
	}

	m_bInit = false;
}
void CGrabber_Matrox_New::SetScanDirection(bool yReverse)
{
	for (int i = 0; i < 4; i++)
	{
		if (yReverse) //단변 검사 일떄
			//MdigControl(m_MilDig[nDigitizer], M_GRAB_DIRECTION_Y, M_REVERSE);                                   //이미지 상하 반전
			MdigControlFeature(m_MilDig[i], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Forward"));

		else if (yReverse == false) //장변 검사 일때
			//MdigControl(m_MilDig[nDigitizer], M_GRAB_DIRECTION_Y, M_FORWARD);                                   //이미지 상하 반전
			MdigControlFeature(m_MilDig[i], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Reverse"));

		Sleep(100);
	}
}

/**************************************************************************************************************/
/*     Start / Stop - Method                                                                                  */
/**************************************************************************************************************/
void CGrabber_Matrox_New::Start(int nDigitizer, bool yReverse)
{
	if (m_bInit == false)
		return;

	// Grabbing 여부
	CSingleLock localLock(&m_csGrabbing[nDigitizer]);
	localLock.Lock();
	if (m_bGrabbing[nDigitizer] == true)
	{
		localLock.Unlock();
		return;
	}
	m_bGrabbing[nDigitizer] = true;
	localLock.Unlock();

	UserHookData[nDigitizer].lParam = this;
	UserHookData[nDigitizer].ProcessedImageCount = 0;
	UserHookData[nDigitizer].DigiNo = nDigitizer;
	UserHookData[nDigitizer].GrabCount = 0;
	
	if (yReverse == true)
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Forward"));	//이미지 상하 반전
	else
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Reverse"));	//이미지 상하 반전
	
	//-- Hooking ---------------------------------------------------------------------------------------------//
	if (nDigitizer == 0)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_START, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR) CallBackHookFunction_1, this);
	}
	else if(nDigitizer == 1)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_START, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_2, this);
	}
	else if (nDigitizer == 2)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_START, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_3, this);
	}
	else if (nDigitizer == 3)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_START, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_4, this);
	}
}

void CGrabber_Matrox_New::Stop(int nDigitizer)
{
	if (m_bInit == false)
		return;

	// Grabbing 여부
	CSingleLock localLock(&m_csGrabbing[nDigitizer]);
	localLock.Lock();
	if (m_bGrabbing[nDigitizer] == false)
	{
		localLock.Unlock();
		return;
	}
	m_bGrabbing[nDigitizer] = false;
	localLock.Unlock();

	if (nDigitizer == 0)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_STOP, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_1, this);
	}
	else if (nDigitizer == 1)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_STOP, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_2, this);
	}
	else if (nDigitizer == 2)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_STOP, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_3, this);
	}
	else if (nDigitizer == 3)
	{
		MdigProcess(m_MilDig[nDigitizer], &m_MilBufferList[nDigitizer][0], BUFFERING_SIZE_MAX, M_STOP, M_DEFAULT, (MIL_BUF_HOOK_FUNCTION_PTR)CallBackHookFunction_4, this);
	}
}

/**************************************************************************************************************/
/*     Callback - 실제 Grab 처리                                                                              */
/**************************************************************************************************************/
long MFTYPE CGrabber_Matrox_New::CallBackHookFunction_1(long HookType, MIL_ID HookId, void MPTYPE* HookDataPtr)
{
	if (GetThreadPriority(GetCurrentThread()) != THREAD_PRIORITY_HIGHEST)
		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);

	CGrabber_Matrox_New* pGrabber = (CGrabber_Matrox_New*) HookDataPtr;

	pGrabber->Processing(HookType, HookId, 0);

	return 0;
}

long MFTYPE CGrabber_Matrox_New::CallBackHookFunction_2(long HookType, MIL_ID HookId, void MPTYPE* HookDataPtr)
{
	if (GetThreadPriority(GetCurrentThread()) != THREAD_PRIORITY_HIGHEST)
		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);

	CGrabber_Matrox_New* pGrabber = (CGrabber_Matrox_New*) HookDataPtr;

	pGrabber->Processing(HookType, HookId, 1);

	return 0;
}

long MFTYPE CGrabber_Matrox_New::CallBackHookFunction_3(long HookType, MIL_ID HookId, void MPTYPE* HookDataPtr)
{
	if (GetThreadPriority(GetCurrentThread()) != THREAD_PRIORITY_HIGHEST)
		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);

	CGrabber_Matrox_New* pGrabber = (CGrabber_Matrox_New*) HookDataPtr;

	pGrabber->Processing(HookType, HookId, 2);

	return 0;
}

long MFTYPE CGrabber_Matrox_New::CallBackHookFunction_4(long HookType, MIL_ID HookId, void MPTYPE* HookDataPtr)
{
	if (GetThreadPriority(GetCurrentThread()) != THREAD_PRIORITY_HIGHEST)
		SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);

	CGrabber_Matrox_New* pGrabber = (CGrabber_Matrox_New*) HookDataPtr;

	pGrabber->Processing(HookType, HookId, 3);

	return 0;
}

BOOL CGrabber_Matrox_New::Processing(long HookType, MIL_ID HookId, int nDigitizer)
{
	if (m_pGrabCallback == NULL)
		return FALSE;

	MIL_ID ModifiedImage = 0;

	MdigGetHookInfo(HookId, M_MODIFIED_BUFFER + M_BUFFER_ID, &ModifiedImage);
	MbufControl(ModifiedImage, M_LOCK, M_DEFAULT);

	CSingleLock localLock(&m_csCurrentBuffer[nDigitizer]);
	localLock.Lock();

	int nCurrentBufferIdx = m_nCurrentBufferIdx[nDigitizer] = (m_nCurrentBufferIdx[nDigitizer] + 1) % CIRCULAR_BUFFER_COUNT;

	MbufGet2d(ModifiedImage, 0, 0, m_SizeX[nDigitizer], m_SizeY[nDigitizer], m_pCurrent[nDigitizer][nCurrentBufferIdx]);

	localLock.Unlock();

	m_pGrabCallback(m_pCurrent[nDigitizer][nCurrentBufferIdx], m_pParam, nDigitizer);

	MbufControl(ModifiedImage, M_UNLOCK, M_DEFAULT);

	return TRUE;
}

/**************************************************************************************************************/
/*     Get / Set - Porperty                                                                                   */
/**************************************************************************************************************/
double CGrabber_Matrox_New::GetFrameRate(int nDigitizer)
{
	double dFrameRate;
	MdigInquire(m_MilDig[nDigitizer], M_PROCESS_FRAME_RATE, &dFrameRate);          //Frame Rate 받아오기

	return dFrameRate;
}

void CGrabber_Matrox_New::GetCurrentBuffer(void* pBuffer, int nDigitizer)
{
	MbufGet(m_LastBufferId[nDigitizer], pBuffer);                                  //현재 즉, 마지막 버퍼를 읽는다.
}

void CGrabber_Matrox_New::SetPageLength(int nDigitizer, int nLength)
{
	if (m_MilDig[nDigitizer] == NULL) 
		return;

	MdigControl(m_MilDig[nDigitizer], M_SOURCE_SIZE_Y, nLength);                        //Page Length Setting
}

void CGrabber_Matrox_New::SetTimeOut(int nDigitizer, int nTimeOut)
{
	if (m_bInit == false) return;

	if (nTimeOut == -1)                                                            //Grab Timeout Setting
	{
		MdigControl(m_MilDig[nDigitizer], M_GRAB_TIMEOUT, M_INFINITE);
	}
	else
	{
		MdigControl(m_MilDig[nDigitizer], M_GRAB_TIMEOUT, (double) nTimeOut);
	}
}

void CGrabber_Matrox_New::SetAreaMode(int nDigitizer, bool bAreaMode)
{
	if (m_MilDig[nDigitizer] == NULL)
		return;

	if (bAreaMode == true)
	{
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("OperationMode"), M_TYPE_STRING, MIL_TEXT("Area"));

		MIL_DOUBLE ExposureTime = 15.0 * 64.0;
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("ExposureTime"), M_TYPE_DOUBLE, &ExposureTime);
	}
	else
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("OperationMode"), M_TYPE_STRING, MIL_TEXT("TDI"));
}

void CGrabber_Matrox_New::SetTriggerMode(int nDigitizer, bool bOn)
{
	if (m_MilDig[nDigitizer] == NULL)
		return;

	MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("TriggerSelector"), M_TYPE_STRING, MIL_TEXT("LineStart"));

	if(bOn == true)
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("TriggerMode"), M_TYPE_STRING, MIL_TEXT("On"));
	else
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("TriggerMode"), M_TYPE_STRING, MIL_TEXT("Off"));
}

void CGrabber_Matrox_New::SetGrabCallBack(void (*pGrabCallBackFunc)(LPVOID pBuffer, LPVOID pParam, int nDigitizer), LPVOID pParam)
{
	m_pParam = pParam;
	m_pGrabCallback = pGrabCallBackFunc;
}
