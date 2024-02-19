#pragma once

#include "mil.h"

#ifdef _DEBUG
#pragma comment(lib, "mil.lib")
#else
#pragma comment(lib, "mil.lib")
#endif

#pragma warning(disable : 4995)

#define ERR_BOARD_OK					0
#define ERR_BOARD_CONNECT_FAIL			51                  //Board Open�� ���� 

#define MAX_BOARD_COUNT					4
#define MAX_CAMERA_COUNT				4

#define BUFFERING_SIZE_MAX				100					// �ǽð� ���� ���� ���� ����
#define CIRCULAR_BUFFER_COUNT			10					// Callback �� ��� �� ���� ����

#ifndef M_SYSTEM_RAPIXOCXP
#define M_SYSTEM_RAPIXOCXP MIL_TEXT("M_SYSTEM_RAPIXOCXP")
#endif

using namespace std;

#pragma pack(push)
#pragma pack(1)

typedef struct                        //User's processing function hook data structure.
{
	long  GrabCount;
	long  ProcessedImageCount;
	long  DigiNo;
	void* lParam;
} UHookDataStruct;

class AFX_EXT_CLASS CGrabber_Matrox_New
{
public:
	CGrabber_Matrox_New();
	~CGrabber_Matrox_New();

private:
	//-- property ���� ----------------------------------------------------------------------------------------//
	UHookDataStruct UserHookData[MAX_CAMERA_COUNT];

	bool m_bInit;																							  //Driver Open ����

	CCriticalSection m_csGrabbing[MAX_CAMERA_COUNT];
	bool m_bGrabbing[MAX_CAMERA_COUNT];                                                                       //Grabbing ����
	
	MIL_INT m_SizeX[MAX_CAMERA_COUNT];
    MIL_INT m_SizeY[MAX_CAMERA_COUNT];
	MIL_INT m_nCntGrabBuffer[MAX_CAMERA_COUNT];                                                               //Grab Buffer ����

	CCriticalSection m_csCurrentBuffer[MAX_CAMERA_COUNT];
	int m_nCurrentBufferIdx[MAX_CAMERA_COUNT];
	LPVOID m_pCurrent[MAX_CAMERA_COUNT][CIRCULAR_BUFFER_COUNT];

	MIL_ID m_MilBufferList[MAX_CAMERA_COUNT][BUFFERING_SIZE_MAX];                                             //Mil Buffer List 
	MIL_ID m_MilImage;                                                                                        //Mil Image
	MIL_ID m_MilApp;                                                                                          //Mil Application
	MIL_ID m_MilSys[MAX_BOARD_COUNT];                                                                         //Mil System - Board ����       
	MIL_ID m_MilDig[MAX_CAMERA_COUNT];                                                                        //Mil Digitiger - ī�޶� ����
	MIL_ID m_LastBufferId[MAX_CAMERA_COUNT];                                                                  //������ ���� 
	//--------------------------------------------------------------------------------------------------------//

public:
	//-- method �Լ� ���� --------------------------------------------------------------------------------------//
	int  OpenDriver(int nBoardIdx, int nBoardPortIdx, int nDigitizerIdx, int yReverse, CString strCamFilePath);
	void CloseDriver();
	
	void SetScanDirection(bool yReverse);
	void Start(int nDigitizer, bool yReverse);																   //Grab Start
	void Stop(int nDigitizer);                                                                                 //Grab Stop

	// is Grabbing
	MIL_INT GetSizeX(int devNo) { return m_SizeX[devNo]; }
	MIL_INT GetSizeY(int devNo) { return m_SizeY[devNo]; }
																											   //Frame rate 
	double GetFrameRate(int nDigitizer);                                                                       //Frame rate
	void GetCurrentBuffer(void* pBuffer, int nDigitizer);													   //���� Buffer
																											   //Line Period Setting
	void SetPageLength(int nDigitizer, int nLength);																   //Page Length
	void SetTimeOut(int nDigitizer, int nTimeOut2);																   //Grab Timeout

	void SetAreaMode(int nDigitizer, bool bAreaMode);
	void SetTriggerMode(int nDigitizer, bool bOn);

public:
	void SetGrabCallBack(void (*pGrabCallBackFunc)(LPVOID pBuffer, LPVOID pParam, int nDigitizer), LPVOID pParam);

private:
	static long MFTYPE CallBackHookFunction_1(long HookType, MIL_ID HookId, void MPTYPE *HookDataPtr);
	static long MFTYPE CallBackHookFunction_2(long HookType, MIL_ID HookId, void MPTYPE *HookDataPtr);
	static long MFTYPE CallBackHookFunction_3(long HookType, MIL_ID HookId, void MPTYPE *HookDataPtr);
	static long MFTYPE CallBackHookFunction_4(long HookType, MIL_ID HookId, void MPTYPE *HookDataPtr);

	BOOL Processing(long HookType, MIL_ID HookId, int nDigitizer);
private:
	LPVOID m_pParam;
	void (*m_pGrabCallback) (LPVOID pBuffer, LPVOID pParam, int nDigitizer);
};

#pragma pack(pop)
