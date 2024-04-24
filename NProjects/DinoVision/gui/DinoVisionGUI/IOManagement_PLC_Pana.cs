using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DinoVisionGUI
{
    public enum EIOMode { IOMode_Read, IOMode_Write }
    public class IOManagement_PLC_Pana : IDisposable
    {
        #region variable
        public delegate void NewImageUpdateHandler(IOManagement_PLC_Pana sender);
        public event NewImageUpdateHandler NewImageUpdate;

        private static readonly object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;

        private SerialPort _serialPort = new SerialPort();
        private string _portName = "COM4";
        private string _dataSendToRead = "%01#RCS";
        private string _dataSendToWrite = "%01#WCS";
        private byte[] _readBuffer = new byte[200];
        private bool SerialPortIsReceiving;

        private EIOMode _ioMode = EIOMode.IOMode_Read;
        private int m_nLevelLogicReadPLCContact = 0;
        private bool m_bStatusWritePLCContact = false;
        private bool m_bPCControlMode = false;
        private bool m_bInspectCompleted = false;
        private bool m_bJudgeOKNG = false;
        private int m_nCamIdx;

        #endregion
        #region Singleton
        //private static IOManagement_PLC_Pana _instance;
        //public static IOManagement_PLC_Pana Instance
        //{
        //    get { return _instance; }
        //    private set { }
        //}
        #endregion
        #region Constructor
        public IOManagement_PLC_Pana(int nCamIdx)
        {
            //if (_instance == null) _instance = this;
            //else return;

            m_nCamIdx = nCamIdx;
            m_bPCControlMode = false;

            // Init serial port
            InitSerialPort();

            // reset all]
            ResetAllInOut();
        }
        #endregion

        #region Properties
        public bool IsInspectCompleted
        {
            get { return m_bInspectCompleted; }
            set { m_bInspectCompleted = value; }
        }
        public bool IsJudgeOKNG
        {
            get { return m_bJudgeOKNG; }
            set { m_bJudgeOKNG = value; }
        }
        #endregion

        #region methods
        bool InitSerialPort()
        {
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.Odd;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.ReadBufferSize = 200;
            _serialPort.DataReceived += _serialPort_DataReceived;
            _serialPort.Open();

            if (!_serialPort.IsOpen)
                return false;

            return true;
        }
        void CloseSerialPort()
        {
            ResetAllInOut();

            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        void ResetAllInOut()
        {
            if (_serialPort.IsOpen)
            {
                // CLOSE clynder 1
                ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                         contactName: "Y", contactIdx: "1", level: "0");
                Thread.Sleep(150);
                // TURN OFF Lighting
                ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                         contactName: "Y", contactIdx: "3", level: "0");
                Thread.Sleep(150);
                // CLOSE clynder 2
                ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                         contactName: "Y", contactIdx: "2", level: "0");
                Thread.Sleep(150);
                // Reset signal NG
                ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                         contactName: "Y", contactIdx: "0", level: "0");
                Thread.Sleep(150);
                // Reset signal OK
                ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                         contactName: "Y", contactIdx: "5", level: "0");
                Thread.Sleep(150);
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int rslength = 0;
            bool loop = true; // Judge whether the data of the receive buffer is all received
            SerialPortIsReceiving = true; // the bool type of judge whether the serial port is processing data
            try
            {
                Thread.Sleep(5);
                while (loop)
                {
                    if (_serialPort.BytesToRead == rslength)
                    {
                        loop = false;
                    }
                    else
                    {
                        rslength = _serialPort.BytesToRead;
                        // Get the number of bytes of data in the receive buffer
                    }
                    Thread.Sleep(5);
                }
                _serialPort.Read(_readBuffer, 0, rslength); // Read the receive buffer data
                _serialPort.DiscardInBuffer();//Clear the receive buffer data
                rslength = 0;

                // process data received
                string strReceived = Encoding.ASCII.GetString(_readBuffer);
                if (!string.IsNullOrEmpty(strReceived))
                {
                    switch (_ioMode)
                    {
                        case EIOMode.IOMode_Read:
                            lock (_lockObj)
                            {
                                // level logic of contact return at position 6
                                if (strReceived[6] == '1')
                                    m_nLevelLogicReadPLCContact = 1;
                                else
                                    m_nLevelLogicReadPLCContact = 0;
                            }
                            break;
                        case EIOMode.IOMode_Write:
                            lock (_lockObj)
                            {
                                // status of manipulate write PLC contact return at position 3
                                if (strReceived[3] == '$')
                                    m_bStatusWritePLCContact = true;
                                else
                                    m_bStatusWritePLCContact = false;
                            }
                            break;
                    }
                }
            }
            catch { }
            finally
            {
                SerialPortIsReceiving = false;
            }
        }

        void ManipulateWithPLCContact(EIOMode eIOMode, string dataSendTo, string contactName, string contactIdx, string level)
        {
            string dataSendToPLC = string.Empty;
            switch (eIOMode)
            {
                case EIOMode.IOMode_Read:
                    // set IO mode is Read
                    _ioMode = EIOMode.IOMode_Read;
                    string extend_read = contactName + "000" + contactIdx;
                    dataSendToPLC = dataSendTo + extend_read;
                    break;
                case EIOMode.IOMode_Write:
                    // set IO mode is Write
                    _ioMode = EIOMode.IOMode_Write;
                    string extend_write = contactName + "000" + contactIdx + level;
                    dataSendToPLC = dataSendTo + extend_write;
                    break;
                default:
                    break;
            }

            byte t = 0;
            byte[] bSend = Encoding.ASCII.GetBytes(dataSendToPLC);
            for (int i = 0; i < dataSendToPLC.Length; i++) //XOR for instruction data, calculate the check code
            {
                t ^= bSend[i];
            }
            string strBCC = t.ToString("X2");
            string strSend = dataSendToPLC + strBCC + (char)13;
            byte[] bytesSend = Encoding.ASCII.GetBytes(strSend);
            _serialPort.Write(bytesSend, 0, bytesSend.Length); // Send instructions
        }

        /*INPUT
        * START button:          X00
        * SENSOR clynder 1:      X01
        * SENSOR clynder 2:      X02
        */

        /*OUTPUT
         * NG:                    Y00
         * OK:                    Y05
         * OPEN/CLOSE clynder 1:  Y01
         * OPEN/CLOSE clynder 2:  Y02
         * TRIGGER LIGHTING:      Y03
         */

        public async Task StartInspect()
        {
            // Never run two parallel tasks
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            var initializationSemaphore0 = new SemaphoreSlim(0, 1);

            _cancellationTokenSource = new CancellationTokenSource();

            _previewTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // wait for read status of START button
                    while (m_nLevelLogicReadPLCContact == 0)
                    {
                        ManipulateWithPLCContact(EIOMode.IOMode_Read, _dataSendToRead,
                                             contactName: "X", contactIdx: "0", level: "");
                        Thread.Sleep(50);
                    }
                    // reset level logic
                    m_nLevelLogicReadPLCContact = 0;

                    if (m_bPCControlMode)
                    {
                        // OPEN clynder 1
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "1", level: "1");
                        Thread.Sleep(100);
                        // TURN ON Lighting
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "3", level: "1");
                        Thread.Sleep(100);
                    }

                    // OPEN dino cam
                    InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.ConnectDinoCam(m_nCamIdx);

                    if (m_bPCControlMode)
                    {
                        // wait for read status of sensor clynder 1
                        while (m_nLevelLogicReadPLCContact == 0)
                        {
                            ManipulateWithPLCContact(EIOMode.IOMode_Read, _dataSendToRead,
                                                 contactName: "X", contactIdx: "1", level: "");
                            Thread.Sleep(50);
                        }

                        // reset level logic
                        m_nLevelLogicReadPLCContact = 0;

                        // OPEN clynder 2
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "2", level: "1");
                        Thread.Sleep(100);
                    }


                    // wait for read status of SENSOR clynder 2
                    while (m_nLevelLogicReadPLCContact == 0)
                    {
                        ManipulateWithPLCContact(EIOMode.IOMode_Read, _dataSendToRead,
                                             contactName: "X", contactIdx: "2", level: "");
                        Thread.Sleep(50);
                    }

                    // reset level logic
                    m_nLevelLogicReadPLCContact = 0;

                    // TRIGGER DINO CAMERA
                    //InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SingleGrabDinoCam(m_nCamIdx);
                    //NewImageUpdate?.Invoke(this);

                    // START INSPECTION
                    InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.InspectStart(0, m_nCamIdx);

                    // wait for inspect complete
                    int count = 1;
                    while (!m_bInspectCompleted)
                    {
                        Thread.Sleep(1000);
                        count++;
                        if (count > 5) // timeout = 5s
                            m_bInspectCompleted = true;
                    }

                    // Send IO the result OK/NG
                    if (m_bJudgeOKNG == true)
                    {
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "5", level: "1");
                        Thread.Sleep(150);

                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "5", level: "0");
                        Thread.Sleep(100);
                    }
                    else
                    {
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "0", level: "1");
                        Thread.Sleep(150);

                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "0", level: "0");
                        Thread.Sleep(100);
                    }

                    // RESET ALL
                    if (m_bPCControlMode)
                    {
                        // CLOSE clynder 1
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "1", level: "0");
                        Thread.Sleep(100);
                        // TURN OFF Lighting
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "3", level: "0");
                        Thread.Sleep(100);
                        // CLOSE clynder 2
                        ManipulateWithPLCContact(EIOMode.IOMode_Write, _dataSendToWrite,
                                                 contactName: "Y", contactIdx: "2", level: "0");
                        Thread.Sleep(100);
                    }

                    // reset level logic
                    m_nLevelLogicReadPLCContact = 0;
            
                    m_bInspectCompleted = false;

                    // CLOSE dino cam
                    InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.DisconnectDinoCam(m_nCamIdx);
                    Thread.Sleep(20);

                    await Task.Delay(1);
                }
            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            await initializationSemaphore0.WaitAsync();
            initializationSemaphore0.Dispose();
            initializationSemaphore0 = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        public async Task StopInspect()
        {
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                //InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.StopGrabHikCam(_camIdx);
                _cancellationTokenSource.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                await _previewTask;
            }
            //CloseSerialPort();
        }
        #endregion

        public void Dispose()
        {
            CloseSerialPort();
            _cancellationTokenSource.Cancel();
        }
    }
}
