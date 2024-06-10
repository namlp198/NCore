#define USE_THREAD

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
        public const int TRIGGER_TOPCAM = 4096 + 80; // M80
        public const int TRIGGER_SIDECAM = 4096 + 81; // M81

        // Bit Write
        public const int INSPECT_TOPCAM_COMPLETED = 4096 + 86; // M86
        public const int GRABIMAGE_SIDECAM1_COMPLETED = 4096 + 87; // M87
        public const int GRABIMAGE_SIDECAM2_COMPLETED = 4096 + 87; // M87
        public const int INSPECT_OK = 4096 + 88; // M88
        public const int INSPECT_NG = 4096 + 89; // M89

        private static readonly object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;

        // PLC Wecon LX5V
        private ModbusTCPIPMASTER LX5V;
        private string m_strIPPLC = "";
        private int m_nPort = 502;
        private int m_nIdPlc = 0; // 1: Plc Cavity 1, 2: Plc Cavity 2

        private bool m_bIsConnected;
        private bool m_bJudgementOKNG;
        private bool m_bInspectCompleted;
        private bool m_bInspectTopCamCompleted;
        private int m_nScanTime = 2;
        private int m_nDelayTime = 100;
        private bool m_bIsStartThread;
        Thread m_threadRunTask;

        public IOManager_PLC_Wecon(string strIP, int nId)
        {
            m_strIPPLC = strIP;
            m_nIdPlc = nId;
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
        public void Initialize()
        {
            LX5V = new ModbusTCPIPMASTER();
            LX5V.ID = m_nIdPlc;
            LX5V.Mode_TCP_Serial = false;
            m_bIsConnected = LX5V.ConnectTCP(m_strIPPLC, m_nPort);
        }
        public void StartThreadPlcWecon()
        {
            m_bIsStartThread = true;
            m_threadRunTask = new Thread(new ThreadStart(ThreadTask));
            m_threadRunTask.IsBackground = true;
            m_threadRunTask.Start();
        }

        private void ThreadTask()
        {
            while (true)
            {
                if (m_bIsConnected == false)
                    break;

                int nCoreIdx = 0;
                int nProcessStatus = 1;
                m_bInspectCompleted = false;
                m_bInspectTopCamCompleted = false;

                // 1. read bit trigger top cam
                while (!ReadSingleCoil(LX5V, TRIGGER_TOPCAM))
                {
                    Thread.Sleep(DelayTime);
                }

                // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2
            
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCamCompleted)
                {
                    Thread.Sleep(DelayTime);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                WriteSingleCoil(INSPECT_TOPCAM_COMPLETED, true);

                //int nTriggerCount = 0;

                //while(nTriggerCount < 10)
                //{
                //    if(ReadSingleCoil(LX5V, TRIGGER_SIDECAM))
                //    {
                //        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetGrabFrameSideCam(nCoreIdx, 1);
                //        nTriggerCount++;
                //        Thread.Sleep(10);
                //    }
                //}

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

        public void InspectStart()
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
                    if (m_bIsConnected == false)
                        break;

                    int nCoreIdx = 0;
                    int nProcessStatus = 1;
                    m_bInspectCompleted = false;
                    m_bInspectTopCamCompleted = false;

                    // 1. read bit trigger top cam
                    while (!ReadSingleCoil(LX5V, TRIGGER_TOPCAM))
                    {
                        Thread.Sleep(DelayTime);
                    }

                    // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2

                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                    // 2. wait for top cam inspection done
                    while (!m_bInspectTopCamCompleted)
                    {
                        Thread.Sleep(DelayTime);
                    }

                    // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                    WriteSingleCoil(INSPECT_TOPCAM_COMPLETED, true);

                    //int nTriggerCount = 0;

                    //while (nTriggerCount < 10)
                    //{
                    //    if (ReadSingleCoil(LX5V, TRIGGER_SIDECAM))
                    //    {
                    //        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetGrabFrameSideCam(nCoreIdx, 1);
                    //        nTriggerCount++;
                    //        Thread.Sleep(10);
                    //    }
                    //}

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

        public void InspectStop()
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

        private bool ReadSingleCoil(ModbusTCPIPMASTER plc, int nCoil)
        {
            if (plc == null)
                return false;

            bool[] arrBool = plc.ReadCoilsTCPIP(nCoil, 1);
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
