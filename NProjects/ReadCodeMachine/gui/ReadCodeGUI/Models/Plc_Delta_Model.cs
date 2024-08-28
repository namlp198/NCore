using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npc.Foundation.Base;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ReadCodeGUI.Models
{
    [DisplayName("PLC Settings")]
    public class Plc_Delta_Model : ModelBase
    {
        public Plc_Delta_Model() 
        {
            m_strPlcCOM = "COM11";
            m_nBaudRate = 9600;
        }

        private string m_strPlcCOM;
        private int m_nBaudRate;
        private int m_nTriggerDelay;
        private int m_nSignalNGDelay;
        private string m_strRegisterTriggerDelay;
        private string m_strRegisterOutput1Delay;

        [PropertyOrder(1)]
        [DisplayName("PLC Comport")]
        public string PlcCOM
        {
            get { return m_strPlcCOM; }
            set { if (SetProperty(ref m_strPlcCOM, value)) { } }
        }
        [PropertyOrder(2)]
        [DisplayName("BaudRate")]
        public int BaudRate
        {
            get { return m_nBaudRate; }
            set { if(SetProperty(ref m_nBaudRate, value)) { } }
        }
        [PropertyOrder(3)]
        [DisplayName("Trigger Delay")]
        public int TriggerDelay
        {
            get => m_nTriggerDelay;
            set { if(SetProperty(ref m_nTriggerDelay, value)) { } }
        }
        [PropertyOrder(4)]
        [DisplayName("Signal NG Delay")]
        public int SignalNGDelay
        {
            get => m_nSignalNGDelay;
            set { if (SetProperty(ref m_nSignalNGDelay, value)) { } }
        }
        [PropertyOrder(5)]
        [DisplayName("Register Trigger Delay")]
        public string RegisterTriggerDelay
        {
            get => m_strRegisterTriggerDelay;
            set { if (SetProperty(ref m_strRegisterTriggerDelay, value)) { } }
        }
        [PropertyOrder(5)]
        [DisplayName("Register Output 1 Delay")]
        public string RegisterOutput1Delay
        {
            get => m_strRegisterOutput1Delay;
            set { if (SetProperty(ref m_strRegisterOutput1Delay, value)) { } }
        }
    }
}
