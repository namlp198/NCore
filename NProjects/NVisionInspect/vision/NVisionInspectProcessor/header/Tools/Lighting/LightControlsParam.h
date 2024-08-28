#pragma once

#define MAX_CHANNEL_COUNT	16
enum LightControls_StatusType { LightControls_Status_Off = 0, LightControls_Status_On, LightControls_Status_Count };
enum LightControls_ModuleType { LightControls_Module_Simulation = 0, 
	LightControls_Module_NVSystem_AKP6,
	LightControls_Module_NVSystem_AKP_04RA, 
	LightControls_Module_L_LIGHT_50W4CH_LP,
	LightControls_Module_Inoray_TB30,
	LightControls_Module_ONUV_OLC_304A,
	LightControls_Module_Count };

interface ILightControls2Parent
{
	virtual void DisplayMessage(TCHAR* str, ...) = 0;
	virtual void ILC2P_ChangeLightLevel(int nIndex, int nLevel, int nChannel = 0) = 0;
	virtual void ILC2P_ChangeLightStatus(int nIndex, int nLevel, int nChannel = 0) = 0;
};

class CLightControlsParam
{
public:
	CLightControlsParam(void) { Reset(); }
	virtual ~CLightControlsParam(void) { Reset(); }

	void Reset()
	{
		m_nDefaultValue = 50;
		m_nMaxValue = 255;
		m_nMinValue = 0;
		m_nChannelCount = 1;
	}

	// getter
	int	GetParam_DefaultValue() const { return m_nDefaultValue; }
	int	GetParam_MaxValue() const { return m_nMaxValue; }
	int	GetParam_MinValue() const { return m_nMinValue; }
	int	GetParam_ChannelCount() const { return m_nChannelCount; }

	CString GetParam_ConnectAddr()	const {return m_strAddress; }

	// setter
	void SetParam_DefaultValue(int nValue) { m_nDefaultValue = nValue; }
	void SetParam_MaxValue(int nValue) { m_nMaxValue = nValue; }
	void SetParam_MinValue(int nValue) { m_nMinValue = nValue; }
	void SetParam_ChannelCount(int nValue) { m_nChannelCount = nValue; }

	void SetParam_ConnectAddr(CString strAddress) { m_strAddress = strAddress; }

protected:
	CString		m_strAddress;			

	int			m_nDefaultValue;			// 기본 값
	int			m_nMaxValue;				// 최대 값
	int			m_nMinValue;				// 최소 값
	int			m_nChannelCount;			// 채널 수
};


class CLightControlsStatus : public CLightControlsParam
{
public:
	CLightControlsStatus(void) { Reset(); }
	virtual ~CLightControlsStatus(void) { Reset(); }

	void Reset()
	{
		m_nConnected = 0;
		memset(m_pCurrentValue, 0, sizeof(int)*MAX_CHANNEL_COUNT);
		memset(m_pCurrentStatus, 0, sizeof(int)*MAX_CHANNEL_COUNT);
	}

	// getter
	int GetStatus_Connected() const { return m_nConnected; }
	int GetStatus_CurrentValue(int nChannel = 0) const
	{
		if (nChannel < 0 || nChannel >= MAX_CHANNEL_COUNT) return -1;
		return m_pCurrentValue[nChannel];
	}
	int GetStatus_CurrentStatus(int nChannel = 0) const
	{
		if (nChannel < 0 || nChannel >= MAX_CHANNEL_COUNT) return -1;
		return m_pCurrentStatus[nChannel];
	}

	// setter
	void SetStatus_Connected(int nValue) { m_nConnected = nValue; }
	void SetStatus_CurrentValue(int nValue, int nChannel = 0)
	{
		if (nChannel < 0 || nChannel >= MAX_CHANNEL_COUNT) return;
		m_pCurrentValue[nChannel] = nValue;
	}
	void SetStatus_CurrentStatus(int nStatus, int nChannel = 0)
	{
		if (nChannel < 0 || nChannel >= MAX_CHANNEL_COUNT) return;
		m_pCurrentStatus[nChannel] = nStatus;
	}

protected:
	int		m_nConnected;
	int		m_pCurrentValue[MAX_CHANNEL_COUNT];
	int		m_pCurrentStatus[MAX_CHANNEL_COUNT];
};