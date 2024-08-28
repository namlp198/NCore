using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;

namespace ReadCodeGUI.Models
{
    public class Plc_LS_Model : ModelBase
    {
        public Plc_LS_Model() 
        {
            m_strPlcCOM = "COM11";
            m_nBaudRate = 115200;
        }

        private string m_strPlcCOM;
        private int m_nBaudRate;
        private int m_nTriggerDelay;
        private int m_nSignalNGDelay;
        private string m_strRegisterTriggerDelay;
        private string m_strRegisterOutput1Delay;

        public string PlcCOM
        {
            get { return m_strPlcCOM; }
            set { if (SetProperty(ref m_strPlcCOM, value)) { } }
        }
        public int BaudRate
        {
            get { return m_nBaudRate; }
            set { if(SetProperty(ref m_nBaudRate, value)) { } }
        }
        public int TriggerDelay
        {
            get => m_nTriggerDelay;
            set { if(SetProperty(ref m_nTriggerDelay, value)) { } }
        }
        public int SignalNGDelay
        {
            get => m_nSignalNGDelay;
            set { if (SetProperty(ref m_nSignalNGDelay, value)) { } }
        }
        public string RegisterTriggerDelay
        {
            get => m_strRegisterTriggerDelay;
            set { if (SetProperty(ref m_strRegisterTriggerDelay, value)) { } }
        }
        public string RegisterOutput1Delay
        {
            get => m_strRegisterOutput1Delay;
            set { if (SetProperty(ref m_strRegisterOutput1Delay, value)) { } }
        }
    }
}
