using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using ReadCodeGUI.Models;
using System.Xml.Linq;
using System.Windows;
using IndustrialNetworks.ModbusCore.ASCII;
using IndustrialNetworks.ModbusCore.Comm;
using IndustrialNetworks.ModbusCore.DataTypes;

namespace ReadCodeGUI.Manager.Class
{
    public class IOManager_PLC_Delta_DVP
    {
        private Plc_Delta_Model m_plcDeltaModel = new Plc_Delta_Model();
        private byte m_slaveAddress = 1;
        private uint m_startAddress_Output = 1280; // start to Y0
        private uint m_startAddress_BitM = 2048; // start to M0 = 2048 -> 4095
        private uint m_startAddress_Register = 4096; // start to D0 = 4096 ->

        private IModbusMaster m_modbusMaster = null;
        public IOManager_PLC_Delta_DVP()
        {

        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(m_plcDeltaModel.PlcCOM))
            {
                try
                {
                    m_modbusMaster = new ModbusASCIIMaster(m_plcDeltaModel.PlcCOM, m_plcDeltaModel.BaudRate, 7, StopBits.One, Parity.Even);
                    m_modbusMaster.Connection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public Plc_Delta_Model PlcDeltaModel
        {
            get { return m_plcDeltaModel; }
            set { m_plcDeltaModel = value; }
        }

        public byte SlaveAddress
        {
            get => m_slaveAddress;
            set => m_slaveAddress = value;
        }
        public uint StartAddressOutput
        {
            get => m_startAddress_Output;
            set
            {
                if (value < 1280)
                {
                    m_startAddress_Output = 1280;
                }
                else
                {
                    m_startAddress_Output = value;
                }
            }
        }
        public uint StartAddressBitM
        {
            get => m_startAddress_BitM;
            set
            {
                if (value < 2048)
                {
                    m_startAddress_BitM = 2048;
                }
                else
                {
                    m_startAddress_BitM = value;
                }
            }
        }
        public uint StartAddressRegister
        {
            get => m_startAddress_Register;
            set
            {
                if(value < 4096)
                {
                    m_startAddress_Register = 4096;
                }
                else
                {
                    m_startAddress_Register = value;
                }
            }
        }

        public void ReadData(string deviceName, short numberOfData)
        {

        }

        public void SetParameterToSingleRegister(short valueRegister)
        {
            try
            {
                short[] datas = new short[1];
                datas[0] = valueRegister;
                byte[] values = Int.ToByteArray(datas);
                m_modbusMaster.WriteSingleRegister(m_slaveAddress, m_startAddress_Register, values);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SetOutputPlc(bool status)
        {
            try
            {
                m_modbusMaster.WriteSingleCoil(m_slaveAddress, m_startAddress_BitM, status);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
