using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSIS.Driver;
using LSIS.Driver.Core.DataTypes;
using System.IO.Ports;
using NVisionInspectGUI.Models;
using System.Xml.Linq;
using System.Windows;

namespace NVisionInspectGUI.Manager.Class
{
    public class IOManager_PLC_LS
    {
        private XGTProtocol m_xgt = null;
        private Plc_LS_Model m_plcModel = new Plc_LS_Model();
        public IOManager_PLC_LS()
        {
            
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(m_plcModel.PlcCOM))
            {
                m_xgt = new XGTProtocol(m_plcModel.PlcCOM, m_plcModel.BaudRate, Parity.None, 8, StopBits.One);

                m_xgt.Connect();
            }
        }
        public Plc_LS_Model PlcLSModel
        {
            get { return m_plcModel; }
            set { m_plcModel = value; }
        }

        public void ReadData(string deviceName, short numberOfData)
        {
            try
            {
                if(m_xgt != null)
                {
                    INT[] result = m_xgt.Read<INT>(stationNo: 1, deviceName: deviceName, numberOfData: numberOfData);
                    if (result != null)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WriteData(string deviceName, REAL[] data)
        {
            try
            {
                if (m_xgt != null)
                {
                    bool result = m_xgt.Write<REAL>(stationNo: 1, deviceName, data);
                    if (result)
                    {
                        MessageBox.Show("Save param PLC success!");
                    }
                    else
                    {
                        MessageBox.Show("Couldn't save to PLC");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WriteData(string deviceName, short numberOfBlocks, INT data)
        {
            try
            {
                if (m_xgt != null)
                {
                    bool result = m_xgt.Write<INT>(stationNo: 1, numberOfBlocks: 1, deviceName: deviceName, data);
                    if (result)
                    {
                        MessageBox.Show("Save param PLC success!");
                    }
                    else
                    {
                        MessageBox.Show("Couldn't save to PLC");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
