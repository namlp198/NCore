using LModbus;
using SealingInspectGUI.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Manager.Class
{
    public class IOManager_PLC_Wecon : IDisposable
    {
        // Bit Read
        private const int TRIGGER_TOPCAM = 4096 + 80; // M80
        private const int TRIGGER_SIDECAM = 4096 + 81; // M81

        // Bit Write
        private const int INSPECT_TOPCAM_COMPLETED = 4096 + 86; // M86
        private const int INSPECT_OK = 4096 + 88; // M88
        private const int INSPECT_NG = 4096 + 89; // M89

        private static readonly object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;

        // PLC Wecon LX5V
        private ModbusTCPIPMASTER LX5V;
        private string m_strIPPLC = "";
        private int m_nPort = 502;
        private int m_nIdPlc = 0; // 1: Plc Cavity 1, 2: Plc Cavity 2
        private bool m_bIsModeTCPSerial = false;

        private bool m_bIsConnected;
        private bool m_bJudgementOKNG;
        private bool m_bInspectCompleted;
        private bool m_bInspectTopCamCompleted;
        private int m_nScanTime = 50;
        private int m_nDelayTime = 50;
        private bool m_bIsStartThread;
        Thread m_threadRun;

        public IOManager_PLC_Wecon(int IdPlc, bool bIsModeTCPSerial = false)
        {
            LX5V = new ModbusTCPIPMASTER()
            {
                ID = IdPlc,
                Mode_TCP_Serial = bIsModeTCPSerial
            };
        }

        public IOManager_PLC_Wecon(string strIPPLC, int IdPlc, bool bIsModeTCPSerial = false)
        {
            m_strIPPLC = strIPPLC;
            m_nIdPlc = IdPlc;
            m_bIsModeTCPSerial = false;
            LX5V = new ModbusTCPIPMASTER()
            {
                ID = IdPlc,
                Mode_TCP_Serial = m_bIsModeTCPSerial
            };
        }

        #region Properties
        public string IPPLC
        {
            get => m_strIPPLC;
            set => m_strIPPLC = value;
        }
        public int Port
        {
            get => m_nPort;
            set => m_nPort = value;
        }
        public int IdPLC
        {
            get => m_nIdPlc;
            set => m_nIdPlc = value;
        }
        public bool IsModeTCPSerial
        {
            get => m_bIsModeTCPSerial; set => m_bIsModeTCPSerial = value;
        }

        public bool IsConnected
        {
            get => m_bIsConnected; set => m_bIsConnected = value;
        }

        public bool IsJudgementOKNG
        {
            get => m_bJudgementOKNG; set => m_bJudgementOKNG = value;
        }
        public bool IsInspectCompleted
        {
            get => m_bInspectCompleted; set => m_bInspectCompleted = value;
        }
        public bool IsInspectTopCamCompleted
        {
            get => m_bInspectTopCamCompleted; set => m_bInspectTopCamCompleted = value;
        }
        public int ScanTime
        {
            get => m_nScanTime; set => m_nScanTime = value;
        }
        public int DelayTime
        {
            get => m_nDelayTime; set => m_nDelayTime = value;
        }
        public bool IsStartThread
        {
            get => m_bIsStartThread; set => m_bIsStartThread = value;
        }
        #endregion

        #region methods
        public void StartThreadWecon()
        {
            m_bIsStartThread = true;
            m_threadRun = new Thread(new ThreadStart(ThreadTask));
            m_threadRun.IsBackground = true;
            m_threadRun.Start();
        }
        public void StopThreadWecon()
        {
            m_bIsStartThread = false;
            Thread.Sleep(2);
            m_threadRun.Abort();
        }

        private void ThreadTask()
        {
            while (m_bIsStartThread) 
            {
                if (m_bIsConnected == false)
                    break;

                int nCoreIdx = 0;
                int nProcessStatus = 1;
                m_bInspectCompleted = false;
                m_bInspectTopCamCompleted = false;

                // 1. read bit trigger top cam
                while (!ReadSingleCoil(TRIGGER_TOPCAM))
                {
                    Thread.Sleep(DelayTime);
                }

                // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2
                if (m_nIdPlc == 1)
                    nCoreIdx = 0;
                else if(m_nIdPlc == 2)
                    nCoreIdx = 1;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCamCompleted)
                {
                    Thread.Sleep(DelayTime);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                WriteSingleCoil(INSPECT_TOPCAM_COMPLETED, true);

                // 4. wait for inspection done
                while (!m_bInspectCompleted)
                {
                    Thread.Sleep(DelayTime);
                }

                // 5. write OK NG
                if (m_bJudgementOKNG == false)
                {
                    WriteSingleCoil(INSPECT_NG, true);
                }
                else
                {
                    WriteSingleCoil(INSPECT_OK, true);
                }

                Thread.Sleep(DelayTime);
            }
        }
        private void InspectStart()
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
                    int nCoreIdx = 0;
                    int nProcessStatus = 1;
                    m_bInspectCompleted = false;
                    m_bInspectTopCamCompleted = false;

                    // 1. read bit trigger top cam
                    while (!ReadSingleCoil(TRIGGER_TOPCAM))
                    {
                        Thread.Sleep(DelayTime);
                    }

                    // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2
                    if (m_nIdPlc == 1)
                        nCoreIdx = 0;
                    else
                        nCoreIdx = 1;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                    // 2. wait for top cam inspection done
                    while (!m_bInspectTopCamCompleted)
                    {
                        Thread.Sleep(DelayTime);
                    }

                    // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                    WriteSingleCoil(INSPECT_TOPCAM_COMPLETED, true);

                    // 4. wait for inspection done
                    while (!m_bInspectCompleted)
                    {
                        Thread.Sleep(DelayTime);
                    }

                    // 5. write OK NG
                    if (m_bJudgementOKNG == false)
                    {
                        WriteSingleCoil(INSPECT_NG, true);
                    }
                    else
                    {
                        WriteSingleCoil(INSPECT_OK, true);
                    }

                    await Task.Delay(ScanTime);
                }
            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            initializationSemaphore0.Wait();
            initializationSemaphore0.Dispose();
            initializationSemaphore0 = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                _previewTask.Wait(_cancellationTokenSource.Token);
            }
        }
        private void InspectStop()
        {
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                //InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.StopGrabHikCam(_camIdx);
                _cancellationTokenSource.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                _previewTask.Wait(_cancellationTokenSource.Token);
            }
        }
        public bool ConnectPLC()
        {
            if (LX5V != null && m_strIPPLC != string.Empty)
            {
                if (LX5V.ConnectTCP(m_strIPPLC, m_nPort))
                    return m_bIsConnected = true;
                else
                {
                    MessageBox.Show("Cannot connect PLC");
                    return m_bIsConnected = false;
                }
            }
            MessageBox.Show("Cannot connect PLC");
            return false;
        }
        private bool ReadSingleCoil(int nCoil)
        {
            if (LX5V == null)
                return false;

            bool[] arrBool = LX5V.ReadCoilsTCPIP(nCoil, 1);
            if (arrBool != null)
            {
                return arrBool[0];
            }

            return false;
        }
        private void WriteSingleCoil(int nAddress, bool bVal)
        {
            if (LX5V == null)
                return;

            LX5V.WriteSingleCoilTCPIP(nAddress, bVal);
        }
        #endregion

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
