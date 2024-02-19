#include "stdafx.h"
#include "Grabber_Matrox.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

/**************************************************************************************************************/
/*     생성/소멸                                                                                              */
/**************************************************************************************************************/
CGrabber_Matrox::CGrabber_Matrox()
{
	m_bInit = false;
	ZeroMemory(m_bGrabbing, sizeof(m_bGrabbing));
	ZeroMemory(m_MilSys, sizeof(m_MilSys));
	ZeroMemory(m_MilDig, sizeof(m_MilDig));
	m_MilApp = NULL;

	for (int i =0; i < MAX_CAMERA_COUNT; i++)
		m_pCurrent[i] = NULL;
}

CGrabber_Matrox::~CGrabber_Matrox()
{
	CloseDriver();
}

/**************************************************************************************************************/
/*     Driver Open / Close                                                                                    */
/**************************************************************************************************************/

// Board 1개에 Camera 4개 붙는 경우
int CGrabber_Matrox::OpenDriver(int bCount, int yReverse)
{
	try
	{
		int nBitCount = bCount;																														//8bit, 16bit(12bit 영상 Grab시)		

		MappAlloc(M_DEFAULT, &m_MilApp);																											//Mil Application 할당   

		MsysAlloc(M_SYSTEM_RAPIXOCXP, M_DEV0, M_DEFAULT, &m_MilSys[0]);																				//Mil System 할당

		MdigAlloc(m_MilSys[0], M_DEV0, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_1.dcf"), M_DEFAULT, &m_MilDig[0]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[0], M_DEV2, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_2.dcf"), M_DEFAULT, &m_MilDig[1]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[1], M_DEV0, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_3.dcf"), M_DEFAULT, &m_MilDig[2]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[1], M_DEV2, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_4.dcf"), M_DEFAULT, &m_MilDig[3]);                       //Mil Digitiger 할당

		MappControl(M_ERROR, M_PRINT_DISABLE);																										//Application Error

		for (int i = 0; i < 4; i++)
		{
			m_SizeX[i] = MdigInquire(m_MilDig[i], M_SIZE_X, M_NULL);																				//Size X
			m_SizeY[i] = MdigInquire(m_MilDig[i], M_SIZE_Y, M_NULL);																				//Size Y
		}

		//m_pCurrent = new BYTE[m_SizeX * m_SizeY * (nBitCount / 8)];																				//Bit따라 메모리 할당

		m_pCurrent[0] = new BYTE[m_SizeX[0] * m_SizeY[0] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[1] = new BYTE[m_SizeX[1] * m_SizeY[1] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[2] = new BYTE[m_SizeX[2] * m_SizeY[2] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[3] = new BYTE[m_SizeX[3] * m_SizeY[3] * (nBitCount / 8)];																		//Bit따라 메모리 할당

		m_nCntGrabBuffer[0] = 0;
		m_nCntGrabBuffer[1] = 0;
		m_nCntGrabBuffer[2] = 0;
		m_nCntGrabBuffer[3] = 0;

		for (int i=0; i < BUFFERING_SIZE_MAX; i++)
		{
			MbufAlloc2d(m_MilSys[0], m_SizeX[0], m_SizeY[0], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[0][i]);							//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[0], m_SizeX[1], m_SizeY[1], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[1][i]);							//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[0], m_SizeX[2], m_SizeY[2], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[2][i]);							//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[0], m_SizeX[3], m_SizeY[3], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[3][i]);							//2D Buffer 생성.

			MbufClear(m_MilBufferList[0][i], 0xFF);																									//버퍼 초기화
			MbufClear(m_MilBufferList[1][i], 0xFF);																									//버퍼 초기화
			MbufClear(m_MilBufferList[2][i], 0xFF);																									//버퍼 초기화
			MbufClear(m_MilBufferList[3][i], 0xFF);																									//버퍼 초기화

			m_nCntGrabBuffer[0]++;
			m_nCntGrabBuffer[1]++;
			m_nCntGrabBuffer[2]++;
			m_nCntGrabBuffer[3]++;
		}
		MappControl(M_ERROR, M_PRINT_ENABLE);																										//Application Error

		// 이거 왜??
		m_nCntGrabBuffer[0]--;
		m_nCntGrabBuffer[1]--;
		m_nCntGrabBuffer[2]--;
		m_nCntGrabBuffer[3]--;
		MbufFree(m_MilBufferList[0][m_nCntGrabBuffer[0]]);																							//마지막 버퍼 해제
		MbufFree(m_MilBufferList[1][m_nCntGrabBuffer[1]]);																							//마지막 버퍼 해제
		MbufFree(m_MilBufferList[2][m_nCntGrabBuffer[2]]);																							//마지막 버퍼 해제
		MbufFree(m_MilBufferList[3][m_nCntGrabBuffer[3]]);																							//마지막 버퍼 해제

		for (int i = 0; i < 4; i++)
		{
			MdigControl(m_MilDig[i], M_GRAB_MODE, M_ASYNCHRONOUS);																					//Digitiger Async 방식으로 Setting
		}

		if (yReverse)
		{
			for (int i = 0; i < 4; i++)
			{
				MdigControl(m_MilDig[i], M_GRAB_DIRECTION_Y, M_REVERSE);																			//이미지 상하 반전
			}
		}
		m_bInit = true;																																//Driver Open 여부
	}
	catch (...)
	{
		return ERR_BOARD_CONNECT_FAIL;
	}

	return ERR_BOARD_OK;
}

int CGrabber_Matrox::OpenDriver(int nBoardIdx, int nBoardPortIdx, int nDigitizerIdx, int yReverse, CString strCamFilePath)
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
		
		if(nDigitizerIdx == 0)
		MdigControl(m_MilDig[0], M_IO_SOURCE + M_TL_TRIGGER, M_AUX_IO0);
		if(nDigitizerIdx == 2)
		MdigControl(m_MilDig[2], M_IO_SOURCE + M_TL_TRIGGER, M_AUX_IO4);

		MappControl(M_ERROR, M_PRINT_DISABLE);																										// Application Error

		m_SizeX[nDigitizerIdx] = MdigInquire(m_MilDig[nDigitizerIdx], M_SIZE_X, M_NULL);															// Size X
		m_SizeY[nDigitizerIdx] = MdigInquire(m_MilDig[nDigitizerIdx], M_SIZE_Y, M_NULL);															// Size Y

		m_pCurrent[nDigitizerIdx] = new BYTE[m_SizeX[nDigitizerIdx] * m_SizeY[nDigitizerIdx] * (nBitCount / 8)];									// Bit따라 메모리 할당
		
		for (m_nCntGrabBuffer[nDigitizerIdx] = 0; m_nCntGrabBuffer[nDigitizerIdx] < BUFFERING_SIZE_MAX; m_nCntGrabBuffer[nDigitizerIdx]++)
		{
			MbufAlloc2d(m_MilSys[nBoardIdx], m_SizeX[nDigitizerIdx], m_SizeY[nDigitizerIdx], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[nDigitizerIdx][m_nCntGrabBuffer[nDigitizerIdx]]);			//2D Buffer 생성.
			MbufClear(m_MilBufferList[nDigitizerIdx][m_nCntGrabBuffer[nDigitizerIdx]], 0xFF);														// 버퍼 초기화
		}
		MappControl(M_ERROR, M_PRINT_ENABLE);																										// Application Error


		m_nCntGrabBuffer[nDigitizerIdx]--;
		MbufFree(m_MilBufferList[nDigitizerIdx][m_nCntGrabBuffer[nDigitizerIdx]]);																	// 마지막 버퍼 해제
		MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_MODE, M_ASYNCHRONOUS);																			// Digitiger Async 방식으로 Setting
		
		MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_TIMEOUT, M_INFINITE);																			// Grab Timeout Infinite

		MdigControl(m_MilDig[nDigitizerIdx], M_GRAB_DIRECTION_Y, M_FORWARD);																		// 이미지 상하 반전
	}
	catch (...)
	{
		return ERR_BOARD_CONNECT_FAIL;
	}

	return ERR_BOARD_OK;
}

/*
// 보드 한개에 카메라 2개 씩 보드 2개인 경우 - LCD 이형 그라인더 후 검사기
int CGrabber_Matrox::OpenDriver(int bCount, int yReverse)
{
	try
	{
		int nBitCount = bCount;																														//8bit, 16bit(12bit 영상 Grab시)		

		MappAlloc(M_DEFAULT, &m_MilApp);																											//Mil Application 할당   

		MsysAlloc(M_SYSTEM_RAPIXOCXP, M_DEV0, M_DEFAULT, &m_MilSys[0]);																				//Mil System 할당
		MsysAlloc(M_SYSTEM_RAPIXOCXP, M_DEV1, M_DEFAULT, &m_MilSys[1]);																				//Mil System 할당

		MdigAlloc(m_MilSys[0], M_DEV0, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_1.dcf"), M_DEFAULT, &m_MilDig[0]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[0], M_DEV1, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_2.dcf"), M_DEFAULT, &m_MilDig[1]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[1], M_DEV2, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_3.dcf"), M_DEFAULT, &m_MilDig[2]);                       //Mil Digitiger 할당
		MdigAlloc(m_MilSys[1], M_DEV3, _T("D:\\UTO\\UTO_EI\\Debug\\setting\\Camfile\\FreeRun_VT-3K7X_TTL_AUX04_1024Line_4.dcf"), M_DEFAULT, &m_MilDig[3]);                       //Mil Digitiger 할당

		MappControl(M_ERROR, M_PRINT_DISABLE);																										//Application Error

		for (int i = 0; i < 4; i++)
		{
			m_SizeX[i] = MdigInquire(m_MilDig[i], M_SIZE_X, M_NULL);																				//Size X
			m_SizeY[i] = MdigInquire(m_MilDig[i], M_SIZE_Y, M_NULL);																				//Size Y
		}

		//m_pCurrent = new BYTE[m_SizeX * m_SizeY * (nBitCount / 8)];																				//Bit따라 메모리 할당

		m_pCurrent[0] = new BYTE[m_SizeX[0] * m_SizeY[0] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[1] = new BYTE[m_SizeX[1] * m_SizeY[1] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[2] = new BYTE[m_SizeX[2] * m_SizeY[2] * (nBitCount / 8)];																		//Bit따라 메모리 할당
		m_pCurrent[3] = new BYTE[m_SizeX[3] * m_SizeY[3] * (nBitCount / 8)];																		//Bit따라 메모리 할당


		for (m_nCntGrabBuffer = 0; m_nCntGrabBuffer < BUFFERING_SIZE_MAX; m_nCntGrabBuffer++)
		{
			MbufAlloc2d(m_MilSys[0], m_SizeX[0], m_SizeY[0], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[0][m_nCntGrabBuffer]);			//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[0], m_SizeX[1], m_SizeY[1], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[1][m_nCntGrabBuffer]);			//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[1], m_SizeX[2], m_SizeY[2], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[2][m_nCntGrabBuffer]);			//2D Buffer 생성.
			MbufAlloc2d(m_MilSys[1], m_SizeX[3], m_SizeY[3], nBitCount, M_IMAGE + M_GRAB + M_PROC, &m_MilBufferList[3][m_nCntGrabBuffer]);			//2D Buffer 생성.

			MbufClear(m_MilBufferList[0][m_nCntGrabBuffer], 0xFF);																					//버퍼 초기화
			MbufClear(m_MilBufferList[1][m_nCntGrabBuffer], 0xFF);																					//버퍼 초기화
			MbufClear(m_MilBufferList[2][m_nCntGrabBuffer], 0xFF);																					//버퍼 초기화
			MbufClear(m_MilBufferList[3][m_nCntGrabBuffer], 0xFF);																					//버퍼 초기화
		}
		MappControl(M_ERROR, M_PRINT_ENABLE);																										//Application Error


		m_nCntGrabBuffer--;
		MbufFree(m_MilBufferList[0][m_nCntGrabBuffer]);																								//마지막 버퍼 해제
		MbufFree(m_MilBufferList[1][m_nCntGrabBuffer]);																								//마지막 버퍼 해제
		MbufFree(m_MilBufferList[2][m_nCntGrabBuffer]);																								//마지막 버퍼 해제
		MbufFree(m_MilBufferList[3][m_nCntGrabBuffer]);																								//마지막 버퍼 해제

		for (int i = 0; i < 4; i++)
		{
			MdigControl(m_MilDig[i], M_GRAB_MODE, M_ASYNCHRONOUS);																					//Digitiger Async 방식으로 Setting
		}

		if (yReverse)
		{
			for (int i = 0; i < 4; i++)
			{
				MdigControl(m_MilDig[i], M_GRAB_DIRECTION_Y, M_REVERSE);																			//이미지 상하 반전
			}
		}
		m_bInit = true;																																//Driver Open 여부
	}
	catch (...)
	{
		return ERR_BOARD_CONNECT_FAIL;
	}

	return ERR_BOARD_OK;
}
*/

void CGrabber_Matrox::CloseDriver()
{
	if (m_bInit == false) 
		return;
	
	//Camera Grab Stop
	for (int nCamIdx = 0; nCamIdx < MAX_CAMERA_COUNT; nCamIdx++)
	{
		//Grab Stop
		Stop(nCamIdx);

		//Buffer Free 1
		if (m_pCurrent[nCamIdx] != NULL)
		{
			delete[](BYTE*)m_pCurrent[nCamIdx];
			m_pCurrent[nCamIdx] = NULL;
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
void CGrabber_Matrox::SetScanDirection(bool yReverse)
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
void CGrabber_Matrox::Start(int nDigitizer, bool yReverse)
{

	if (m_bInit == false || m_bGrabbing[nDigitizer] == true) return;
	
	if (yReverse == true)
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Forward"));
		//MdigControl(m_MilDig[nDigitizer], M_GRAB_DIRECTION_Y, M_REVERSE);											//이미지 상하 반전
	else
		MdigControlFeature(m_MilDig[nDigitizer], M_FEATURE_VALUE, MIL_TEXT("ScanDirection"), M_TYPE_STRING, MIL_TEXT("Reverse"));
		//MdigControl(m_MilDig[nDigitizer], M_GRAB_DIRECTION_Y, M_FORWARD);											//이미지 상하 반전
	
	m_bGrabbing[nDigitizer]							= true;															//Grabbing 여부
	UserHookData[nDigitizer].lParam					= this;
	UserHookData[nDigitizer].ProcessedImageCount	= 0;
	UserHookData[nDigitizer].DigiNo					= nDigitizer;
	UserHookData[nDigitizer].GrabCount				= 0;
	
	//-- Hooking ---------------------------------------------------------------------------------------------//
	if (nDigitizer == 0)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START, StartCallbackStage1, &UserHookData[nDigitizer]);        //Hook 후 Grab Start 
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END, GlobalCallbackStage1, &UserHookData[nDigitizer]);

		MdigGrab(m_MilDig[nDigitizer], m_MilBufferList[nDigitizer][0]);												 //버퍼에 담기
	}
	else if(nDigitizer == 1)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START, StartCallbackStage2, &UserHookData[nDigitizer]);        //Hook 후 Grab Start 
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END, GlobalCallbackStage2, &UserHookData[nDigitizer]);

		MdigGrab(m_MilDig[nDigitizer], m_MilBufferList[nDigitizer][0]);												 //버퍼에 담기
	}
	else if (nDigitizer == 2)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START, StartCallbackStage3, &UserHookData[nDigitizer]);        //Hook 후 Grab Start 
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END, GlobalCallbackStage3, &UserHookData[nDigitizer]);

		MdigGrab(m_MilDig[nDigitizer], m_MilBufferList[nDigitizer][0]);												 //버퍼에 담기
	}
	else if (nDigitizer == 3)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START, StartCallbackStage4, &UserHookData[nDigitizer]);        //Hook 후 Grab Start 
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END, GlobalCallbackStage4, &UserHookData[nDigitizer]);

		MdigGrab(m_MilDig[nDigitizer], m_MilBufferList[nDigitizer][0]);												 //버퍼에 담기
	}
}

void CGrabber_Matrox::Stop(int nDigitizer)
{
	if (m_bInit == false || m_bGrabbing[nDigitizer] == false)
		return;

	MdigControl(m_MilDig[nDigitizer], M_GRAB_ABORT, M_DEFAULT/*M_NULL*/);                                             //Abort
	MappControl(M_ERROR, M_PRINT_ENABLE);

	//-- UnHook -----------------------------------------------------------------------------//
	if (nDigitizer == 0)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START + M_UNHOOK, StartCallbackStage1, &UserHookData[nDigitizer]);
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END + M_UNHOOK, GlobalCallbackStage1, &UserHookData[nDigitizer]);
	}
	else if(nDigitizer == 1)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START + M_UNHOOK, StartCallbackStage2, &UserHookData[nDigitizer]);
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END + M_UNHOOK, GlobalCallbackStage2, &UserHookData[nDigitizer]);
	}
	else if (nDigitizer == 2)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START + M_UNHOOK, StartCallbackStage3, &UserHookData[nDigitizer]);
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END + M_UNHOOK, GlobalCallbackStage3, &UserHookData[nDigitizer]);
	}
	else if (nDigitizer == 3)
	{
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_START + M_UNHOOK, StartCallbackStage4, &UserHookData[nDigitizer]);
		MdigHookFunction(m_MilDig[nDigitizer], M_GRAB_END + M_UNHOOK, GlobalCallbackStage4, &UserHookData[nDigitizer]);
	}

	m_bGrabbing[nDigitizer] = false;
}

/**************************************************************************************************************/
/*     Callback - 실제 Grab 처리                                                                              */
/**************************************************************************************************************/
void CGrabber_Matrox::SetCallBackStage1(void (*pEndGrabCallbackStage1)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam)
{
	m_pCallbackStage1 = pEndGrabCallbackStage1;
	m_pParam = pParam;
}

void CGrabber_Matrox::SetCallBackStage2(void (*pEndGrabCallbackStage2)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam)
{
	m_pCallbackStage2 = pEndGrabCallbackStage2;
	m_pParam = pParam;
}

void CGrabber_Matrox::SetCallBackStage3(void (*pEndGrabCallbackStage3)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam)
{
	m_pCallbackStage3 = pEndGrabCallbackStage3;
	m_pParam = pParam;
}

void CGrabber_Matrox::SetCallBackStage4(void (*pEndGrabCallbackStage4)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam)
{
	m_pCallbackStage4 = pEndGrabCallbackStage4;
	m_pParam = pParam;
}

MIL_INT MFTYPE CGrabber_Matrox::StartCallbackStage1(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	if (pHelios->m_bGrabbing[UserHookDataPtr->DigiNo])
	{
		UserHookDataPtr->GrabCount++;
		MdigGrab(pHelios->m_MilDig[UserHookDataPtr->DigiNo], pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->GrabCount % 2]);
	}

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::GlobalCallbackStage1(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	MbufGet(pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->ProcessedImageCount % 2], pHelios->m_pCurrent[UserHookDataPtr->DigiNo]);
	UserHookDataPtr->ProcessedImageCount++;

	pHelios->Callback(HookType, HookId, HookDataPtr);

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::StartCallbackStage2(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	if (pHelios->m_bGrabbing[UserHookDataPtr->DigiNo])
	{
		UserHookDataPtr->GrabCount++;
		MdigGrab(pHelios->m_MilDig[UserHookDataPtr->DigiNo], pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->GrabCount % 2]);
	}

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::GlobalCallbackStage2(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	MbufGet(pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->ProcessedImageCount % 2], pHelios->m_pCurrent[UserHookDataPtr->DigiNo]);
	UserHookDataPtr->ProcessedImageCount++;

	pHelios->Callback(HookType, HookId, HookDataPtr);

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::StartCallbackStage3(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	if (pHelios->m_bGrabbing[UserHookDataPtr->DigiNo])
	{
		UserHookDataPtr->GrabCount++;
		MdigGrab(pHelios->m_MilDig[UserHookDataPtr->DigiNo], pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->GrabCount % 2]);
	}

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::GlobalCallbackStage3(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	MbufGet(pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->ProcessedImageCount % 2], pHelios->m_pCurrent[UserHookDataPtr->DigiNo]);
	UserHookDataPtr->ProcessedImageCount++;

	pHelios->Callback(HookType, HookId, HookDataPtr);

	return 0;
}
MIL_INT MFTYPE CGrabber_Matrox::StartCallbackStage4(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	if (pHelios->m_bGrabbing[UserHookDataPtr->DigiNo])
	{
		UserHookDataPtr->GrabCount++;
		MdigGrab(pHelios->m_MilDig[UserHookDataPtr->DigiNo], pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->GrabCount % 2]);
	}

	return 0;
}

MIL_INT MFTYPE CGrabber_Matrox::GlobalCallbackStage4(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;
	CGrabber_Matrox* pHelios = (CGrabber_Matrox*)UserHookDataPtr->lParam;

	MbufGet(pHelios->m_MilBufferList[UserHookDataPtr->DigiNo][UserHookDataPtr->ProcessedImageCount % 2], pHelios->m_pCurrent[UserHookDataPtr->DigiNo]);
	UserHookDataPtr->ProcessedImageCount++;

	pHelios->Callback(HookType, HookId, HookDataPtr);

	return 0;
}

void CGrabber_Matrox::Callback(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr)
{
	UHookDataStruct* UserHookDataPtr = (UHookDataStruct*)HookDataPtr;

	MdigGetHookInfo(HookId, M_MODIFIED_BUFFER + M_BUFFER_ID, &m_LastBufferId[UserHookDataPtr->DigiNo]);

	if (UserHookDataPtr->DigiNo == 0)
	{
		if (m_bGrabbing[UserHookDataPtr->DigiNo] && m_pCallbackStage1 != NULL) m_pCallbackStage1(m_pCurrent[UserHookDataPtr->DigiNo], m_pParam, UserHookDataPtr->DigiNo);
	}
	else if(UserHookDataPtr->DigiNo == 1)
	{
		if (m_bGrabbing[UserHookDataPtr->DigiNo] && m_pCallbackStage2 != NULL) m_pCallbackStage2(m_pCurrent[UserHookDataPtr->DigiNo], m_pParam, UserHookDataPtr->DigiNo);
	}
	else if (UserHookDataPtr->DigiNo == 2)
	{
		if (m_bGrabbing[UserHookDataPtr->DigiNo] && m_pCallbackStage3 != NULL) m_pCallbackStage3(m_pCurrent[UserHookDataPtr->DigiNo], m_pParam, UserHookDataPtr->DigiNo);
	}
	else if (UserHookDataPtr->DigiNo == 3)
	{
		if (m_bGrabbing[UserHookDataPtr->DigiNo] && m_pCallbackStage4 != NULL) m_pCallbackStage4(m_pCurrent[UserHookDataPtr->DigiNo], m_pParam, UserHookDataPtr->DigiNo);
	}
}

/**************************************************************************************************************/
/*     Get / Set - Porperty                                                                                   */
/**************************************************************************************************************/
double CGrabber_Matrox::GetFrameRate(int nDigitizer)
{
	double dFrameRate;
	MdigInquire(m_MilDig[nDigitizer], M_PROCESS_FRAME_RATE, &dFrameRate);          //Frame Rate 받아오기

	return dFrameRate;
}

void CGrabber_Matrox::GetCurrentBuffer(void* pBuffer, int nDigitizer)
{
	MbufGet(m_LastBufferId[nDigitizer], pBuffer);                                  //현재 즉, 마지막 버퍼를 읽는다.
}

void CGrabber_Matrox::SetPageLength(int devNo, int nLength)
{
	if (m_bInit == false) return;

	MdigControl(m_MilDig[devNo], M_SOURCE_SIZE_Y, nLength);                        //Page Length Setting
}

void CGrabber_Matrox::SetTimeOut(int devNo, int nTimeOut)
{
	if (m_bInit == false) return;

	if (nTimeOut == -1)                                                            //Grab Timeout Setting
	{
		MdigControl(m_MilDig[devNo], M_GRAB_TIMEOUT, M_INFINITE);
	}
	else
	{
		MdigControl(m_MilDig[devNo], M_GRAB_TIMEOUT, (double)nTimeOut);
	}
}