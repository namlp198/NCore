#pragma once

#include "mil.h"

#ifdef _DEBUG
#pragma comment(lib, "mil.lib")
#else
#pragma comment(lib, "mil.lib")
#endif

#define ERR_BOARD_OK					0
#define ERR_BOARD_CONNECT_FAIL			51                  //Board Open�� ���� 

#define MAX_BOARD_COUNT					4
#define MAX_CAMERA_COUNT				4

#define BUFFERING_SIZE_MAX				100					// �ǽð� ���� ���� ���� ����

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

class AFX_EXT_CLASS CGrabber_Matrox
{
public:
	CGrabber_Matrox();
	~CGrabber_Matrox();

private:
	//-- property ���� ----------------------------------------------------------------------------------------//
	UHookDataStruct UserHookData[MAX_CAMERA_COUNT];

	bool m_bInit;																							  //Driver Open ����
	bool m_bGrabbing[MAX_CAMERA_COUNT];                                                                       //Grabbing ����
	MIL_INT m_SizeX[MAX_CAMERA_COUNT];
    MIL_INT m_SizeY[MAX_CAMERA_COUNT];
	MIL_INT m_nCntGrabBuffer[MAX_CAMERA_COUNT];                                                               //Grab Buffer ����

	LPVOID m_pCurrent[MAX_CAMERA_COUNT];
	LPVOID m_pParam;

	MIL_ID m_MilBufferList[MAX_CAMERA_COUNT][BUFFERING_SIZE_MAX];                                             //Mil Buffer List 
	MIL_ID m_MilImage;                                                                                        //Mil Image
	MIL_ID m_MilApp;                                                                                          //Mil Application
	MIL_ID m_MilSys[MAX_BOARD_COUNT];                                                                         //Mil System - Board ����       
	MIL_ID m_MilDig[MAX_CAMERA_COUNT];                                                                        //Mil Digitiger - ī�޶� ����
	MIL_ID m_LastBufferId[MAX_CAMERA_COUNT];                                                                  //������ ���� 
	//--------------------------------------------------------------------------------------------------------//

public:
	//-- method �Լ� ���� --------------------------------------------------------------------------------------//
	int  OpenDriver(int bCount, int yReverse);																   //dcf ���� �������� Driver Setting
	int  OpenDriver(int nBoardIdx, int nBoardPortIdx, int nDigitizerIdx, int yReverse, CString strCamFilePath);
	void CloseDriver();
	
	void SetScanDirection(bool yReverse);
	//void Start();                                                                                            //Grab Start
	void Start(int nDigitizer, bool yReverse);                                                                 //Grab Start
	
	//void Stop();                                                                                             //Grab Stop
	void Stop(int nDigitizer);                                                                                 //Grab Stop

	// is Grabbing
	bool GetGrabbing(int nDigitizer) { return m_bGrabbing[nDigitizer]; };

	MIL_INT GetSizeX(int devNo) { return m_SizeX[devNo]; }
	MIL_INT GetSizeY(int devNo) { return m_SizeY[devNo]; }
																											   //Frame rate 
	double GetFrameRate(int nDigitizer);                                                                       //Frame rate
	void GetCurrentBuffer(void* pBuffer, int nDigitizer);													   //���� Buffer
																											   //Line Period Setting
	void SetLinePeriod(int devNo, int nPeriod);																   //Line Period Setting
	void SetPageLength(int devNo, int nLength);																   //Page Length
	void SetTimeOut(int devNo, int nTimeOut2);																   //Grab Timeout

	void SetCallBackStage1(void (*pEndGrabCall)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam);
	void SetCallBackStage2(void (*pEndGrabCall)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam);
	void SetCallBackStage3(void (*pEndGrabCall)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam);
	void SetCallBackStage4(void (*pEndGrabCall)(LPVOID pCurrent, LPVOID pParam, int nDigitizer), LPVOID pParam);

private:
	void (*m_pCallbackStage1)(LPVOID pBuffer, LPVOID pParam, int nDigitizer);
	void (*m_pCallbackStage2)(LPVOID pBuffer, LPVOID pParam, int nDigitizer);
	void (*m_pCallbackStage3)(LPVOID pBuffer, LPVOID pParam, int nDigitizer);
	void (*m_pCallbackStage4)(LPVOID pBuffer, LPVOID pParam, int nDigitizer);

	void Callback(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);

	static MIL_INT MFTYPE GlobalCallbackStage1(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);
	static MIL_INT MFTYPE StartCallbackStage1(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);

	static MIL_INT MFTYPE GlobalCallbackStage2(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);
	static MIL_INT MFTYPE StartCallbackStage2(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);

	static MIL_INT MFTYPE GlobalCallbackStage3(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);
	static MIL_INT MFTYPE StartCallbackStage3(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);

	static MIL_INT MFTYPE GlobalCallbackStage4(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);
	static MIL_INT MFTYPE StartCallbackStage4(MIL_INT HookType, MIL_ID HookId, void* HookDataPtr);
};

#pragma pack(pop)
