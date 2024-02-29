#pragma once
#include <iostream>
#include <vector>

#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/imgcodecs.hpp>
#include <FindLineTool.h>
#include "SharedMemoryBuffer.h"
#include "InspectResult.h"
#include "InspectCore.h"
#include "InspectionHikCam.h"
#include "InspectionBaslerCam.h"
#include "InspectionBaslerCam_New.h"
#include "InspectionUsbCam.h"
#include "LogView.h"

#define MAX_BUFF 1
#define FRAME_WIDTH 1280
#define FRAME_HEIGHT 1024
#define FRAME_COUNT 1

typedef void _stdcall CallbackLogFunc(char* strLogMsg);
typedef void _stdcall CallbackAlarm(char* strAlarmMessage);
typedef void _stdcall CallbackInspectComplete();

interface IProcessor2Parent
{
	virtual void	ShowLogView(int emCommand) = 0;
	virtual void	DisplayMessage(CString strLogMessage) = 0;
	virtual void	DisplayMessage(TCHAR* str, ...) = 0;
	// virtual void	IProcessor2P_DisplayMessage(int nLogType, TCHAR* str, ...) = 0;
};

class AFX_EXT_CLASS ImageProcessor : public IInspectCoreToParent
{
public:
	ImageProcessor();
	~ImageProcessor();
	BOOL                              FindLineWithHoughLine_Simul(int top, int left, int width, int height, int nBuff);
	virtual LPBYTE                    GetBufferImage(int nBuff, UINT nY);
	BOOL                              LoadImageBuffer(int nBuff, CString strFilePath);
	BOOL                              CreateBuffer();
	BOOL                              ClearBufferImage(int nBuff);
	BOOL							  Initialize();
	BOOL                              Destroy();

	BOOL                              GetInspectData(InspectResult* pInspectData);
	InspectionHikCam*                 GetHikCamControl() { return m_pInspHikCam; }
	CInspectionBaslerCam*             GetBaslerCamControl() { return m_pInspBaslerCam; }
	CInspectionBaslerCam_New*         GetBaslerCamControl_New() { return m_pInspBaslerCam_New; }
	CInspectionUsbCam*                GetUsbCamControl() { return m_pInspUsbCam; }

public:
	void								LogMessage(char* strMessage);
	void								LogMessage(CString strMessage);
	void								ShowLogView(BOOL bShow);

public:
	void SetInterface(IProcessor2Parent* pInterface) { m_pInterface = pInterface; };
	static void GrabCallBack(LPVOID pBuffer, LPVOID pParam, int nDigitizer);
	void GrabCallbackCam(int nDigitizer, LPVOID pBuffer);	// Set Grab Buffer

	// CallBack
	void RegCallbackLogFunc(CallbackLogFunc* pFunc);
	void RegCallbackAlarm(CallbackAlarm* pFunc);
	void RegCallbackInscompleteFunc(CallbackInspectComplete* pFunc);

public:
	// Cam Image Insert to Buffer and Inspect Process
	BOOL InspectFrame(LPBYTE pBuffer = NULL);
	BOOL GrabFrame(int nBuff, LPBYTE pBuffer);

private:
	virtual void						SystemMessage(const TCHAR* lpstrFormat, ...);
	virtual void						AlarmMessage(CString strAlarmMessage);
	void								SystemMessage(CString strMessage);
private:

	// Interface..
	IProcessor2Parent*                       m_pInterface;
	CallbackLogFunc*                         m_pCallbackLogFunc;
	CallbackAlarm*                           m_pCallbackAlarm;
	CallbackInspectComplete*                 m_pCallbackInsCompleteFunc;

	// Image Buffer
	CSharedMemoryBuffer*                     m_pImageBuffer[MAX_BUFF];
	std::vector<cv::Point2f>                 vPoints;

	InspectCore*                             m_pInspectCore;
	InspectionHikCam*                        m_pInspHikCam;
	CInspectionBaslerCam*                    m_pInspBaslerCam;
	CInspectionBaslerCam_New*                m_pInspBaslerCam_New;
	CInspectionUsbCam*                       m_pInspUsbCam;

	// Live Mode
	BOOL                                     m_bLiveMode;

	// UI
	CLogView*                                m_pLogView;
	
	//cv::Mat* pMat;
};