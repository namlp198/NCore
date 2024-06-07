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
        public const int TRIGGER_TOPCAM_1 = 4096 + 80; // M80
        public const int TRIGGER_SIDECAM_1 = 4096 + 81; // M81
        public const int TRIGGER_TOPCAM_2 = 4096 + 80 + 100; // M80
        public const int TRIGGER_SIDECAM_2 = 4096 + 81 + 100; // M81

        // Bit Write
        public const int INSPECT_TOPCAM_COMPLETED_1 = 4096 + 86; // M86
        public const int GRABIMAGE_SIDECAM1_COMPLETED = 4096 + 87; // M87
        public const int GRABIMAGE_SIDECAM2_COMPLETED = 4096 + 87 + 100; // M87
        public const int INSPECT_OK_1 = 4096 + 88; // M88
        public const int INSPECT_NG_1 = 4096 + 89; // M89

        public const int INSPECT_TOPCAM_COMPLETED_2 = 4096 + 86 + 100; // M86
        public const int INSPECT_OK_2 = 4096 + 88 + 100; // M88
        public const int INSPECT_NG_2 = 4096 + 89 + 100; // M89

        private static readonly object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;

        // PLC Wecon LX5V
        private ModbusTCPIPMASTER[] LX5V;
        private string m_strIPPLC_1 = "";
        private string m_strIPPLC_2 = "";
        private int m_nPort = 502;
        private int m_nIdPlc = 0; // 1: Plc Cavity 1, 2: Plc Cavity 2

        private bool[] m_bIsConnected;
        private bool[] m_bJudgementOKNG;
        private bool[] m_bInspectCompleted;
        private bool[] m_bInspectTopCamCompleted;
        private int m_nScanTime = 50;
        private int m_nDelayTime = 50;
        private bool[] m_bIsStartThread;
        Thread m_threadRunTask1;
        Thread m_threadRunTask2;

        public IOManager_PLC_Wecon(string strIP1, string strIP2)
        {
            m_strIPPLC_1 = strIP1;
            m_strIPPLC_2 = strIP2;

            m_bIsConnected = new bool[Defines.MAX_PLC_COUNT];
            m_bJudgementOKNG = new bool[Defines.MAX_PLC_COUNT];
            m_bInspectCompleted = new bool[Defines.MAX_PLC_COUNT];
            m_bInspectTopCamCompleted = new bool[Defines.MAX_PLC_COUNT];
            m_bIsStartThread = new bool[Defines.MAX_PLC_COUNT];
        }

        #region Properties
        public string IPPLC_1
        {
            get => m_strIPPLC_1;
            set => m_strIPPLC_1 = value;
        }
        public string IPPLC_2
        {
            get => m_strIPPLC_2;
            set => m_strIPPLC_2 = value;
        }
        public int Port
        {
            get => m_nPort;
            set => m_nPort = value;
        }

        public bool[] IsConnected
        {
            get => m_bIsConnected; set => m_bIsConnected = value;
        }

        public bool[] IsJudgementOKNG
        {
            get => m_bJudgementOKNG; set => m_bJudgementOKNG = value;
        }
        public bool[] IsInspectCompleted
        {
            get => m_bInspectCompleted; set => m_bInspectCompleted = value;
        }
        public bool[] IsInspectTopCamCompleted
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
        public bool[] IsStartThread
        {
            get => m_bIsStartThread; set => m_bIsStartThread = value;
        }
        #endregion

        #region methods
        public void Initialize()
        {
            LX5V = new ModbusTCPIPMASTER[Defines.MAX_PLC_COUNT];
            for (int i = 0; i < Defines.MAX_PLC_COUNT; i++)
            {
                LX5V[i] = new ModbusTCPIPMASTER();
                LX5V[i].ID = i;
                LX5V[i].Mode_TCP_Serial = false;

                if (i == 0)
                {
                   m_bIsConnected[i] = LX5V[i].ConnectTCP(m_strIPPLC_1, m_nPort);
                }
                else if (i == 1)
                {
                   m_bIsConnected[i] = LX5V[i].ConnectTCP(m_strIPPLC_2, m_nPort);
                }
            }
        }
        public void StartThreadWecon1()
        {
            m_bIsStartThread[0] = true;
            m_threadRunTask1 = new Thread(new ThreadStart(ThreadTask1));
            m_threadRunTask1.IsBackground = true;
            m_threadRunTask1.Start();
        }
        public void StartThreadWecon2()
        {
            m_bIsStartThread[1] = true;
            m_threadRunTask2 = new Thread(new ThreadStart(ThreadTask2));
            m_threadRunTask2.IsBackground = true;
            m_threadRunTask2.Start();
        }

        private void ThreadTask1()
        {
            while (true)
            {
                if (m_bIsConnected[0] == false)
                    break;

                int nCoreIdx = 0;
                int nProcessStatus = 1;
                m_bInspectCompleted[0] = false;
                m_bInspectTopCamCompleted[0] = false;

                // 1. read bit trigger top cam
                while (!ReadSingleCoil(LX5V[0], TRIGGER_TOPCAM_1))
                {
                    Thread.Sleep(DelayTime);
                }

                // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2
            
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCamCompleted[0])
                {
                    Thread.Sleep(DelayTime);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                WriteSingleCoil(LX5V[0], INSPECT_TOPCAM_COMPLETED_1, true);

                int nTriggerCount = 0;

                while(nTriggerCount < 10)
                {
                    if(ReadSingleCoil(LX5V[0], TRIGGER_SIDECAM_1))
                    {
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetGrabFrameSideCam(nCoreIdx, 1);
                        nTriggerCount++;
                        Thread.Sleep(DelayTime);
                    }
                }

                // 4. wait for inspection done
                while (!m_bInspectCompleted[0])
                {
                    Thread.Sleep(DelayTime);
                }

                // 5. write OK NG
                if (m_bJudgementOKNG[0] == false)
                {
                    WriteSingleCoil(LX5V[0], INSPECT_NG_1, true);
                }
                else
                {
                    WriteSingleCoil(LX5V[0], INSPECT_OK_1, true);
                }

                Thread.Sleep(DelayTime);
            }
        }
        private void ThreadTask2()
        {
            while (true)
            {
                if (m_bIsConnected[1] == false)
                    break;

                int nCoreIdx = 1;
                int nProcessStatus = 1;
                m_bInspectCompleted[1] = false;
                m_bInspectTopCamCompleted[1] = false;

                // 1. read bit trigger top cam
                while (!ReadSingleCoil(LX5V[1], TRIGGER_TOPCAM_2))
                {
                    Thread.Sleep(DelayTime);
                }

                // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2

                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCamCompleted[1])
                {
                    Thread.Sleep(DelayTime);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                WriteSingleCoil(LX5V[1], INSPECT_TOPCAM_COMPLETED_2, true);

                // 4. wait for inspection done
                while (!m_bInspectCompleted[1])
                {
                    Thread.Sleep(DelayTime);
                }

                // 5. write OK NG
                if (m_bJudgementOKNG[1] == false)
                {
                    WriteSingleCoil(LX5V[1], INSPECT_NG_2, true);
                }
                else
                {
                    WriteSingleCoil(LX5V[1], INSPECT_OK_2, true);
                }

                Thread.Sleep(DelayTime);
            }
        }

        //private void InspectStart()
        //{
        //    // Never run two parallel tasks
        //    if (_previewTask != null && !_previewTask.IsCompleted)
        //        return;

        //    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

        //    _cancellationTokenSource = new CancellationTokenSource();

        //    _previewTask = Task.Run(async () =>
        //    {
        //        while (!_cancellationTokenSource.IsCancellationRequested)
        //        {
        //            int nCoreIdx = 0;
        //            int nProcessStatus = 1;
        //            m_bInspectCompleted = false;
        //            m_bInspectTopCamCompleted = false;

        //            // 1. read bit trigger top cam
        //            while (!ReadSingleCoil(TRIGGER_TOPCAM))
        //            {
        //                Thread.Sleep(DelayTime);
        //            }

        //            // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2
        //            if (m_nIdPlc == 1)
        //                nCoreIdx = 0;
        //            else
        //                nCoreIdx = 1;
        //            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus(nCoreIdx, nProcessStatus);

        //            // 2. wait for top cam inspection done
        //            while (!m_bInspectTopCamCompleted)
        //            {
        //                Thread.Sleep(DelayTime);
        //            }

        //            // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
        //            WriteSingleCoil(INSPECT_TOPCAM_COMPLETED, true);

        //            // 4. wait for inspection done
        //            while (!m_bInspectCompleted)
        //            {
        //                Thread.Sleep(DelayTime);
        //            }

        //            // 5. write OK NG
        //            if (m_bJudgementOKNG == false)
        //            {
        //                WriteSingleCoil(INSPECT_NG, true);
        //            }
        //            else
        //            {
        //                WriteSingleCoil(INSPECT_OK, true);
        //            }

        //            await Task.Delay(ScanTime);
        //        }
        //    }, _cancellationTokenSource.Token);

        //    // Async initialization to have the possibility to show an animated loader without freezing the GUI
        //    // The alternative was the long polling. (while !variable) await Task.Delay
        //    initializationSemaphore0.Wait();
        //    initializationSemaphore0.Dispose();
        //    initializationSemaphore0 = null;

        //    if (_previewTask.IsFaulted)
        //    {
        //        // To let the exceptions exit
        //        _previewTask.Wait(_cancellationTokenSource.Token);
        //    }
        //}
        //private void InspectStop()
        //{
        //    // If "Dispose" gets called before Stop
        //    if (_cancellationTokenSource.IsCancellationRequested)
        //        return;

        //    if (!_previewTask.IsCompleted)
        //    {
        //        //InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.StopGrabHikCam(_camIdx);
        //        _cancellationTokenSource.Cancel();

        //        // Wait for it, to avoid conflicts with read/write of _lastFrame
        //        _previewTask.Wait(_cancellationTokenSource.Token);
        //    }
        //}
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
        private void WriteSingleCoil(ModbusTCPIPMASTER plc, int nAddress, bool bVal)
        {
            if (plc == null)
                return;

            plc.WriteSingleCoilTCPIP(nAddress, bVal);
        }

        public void WriteSingleCoil(int nIndexPLC, int nAddress, bool bVal)
        {
            WriteSingleCoil(LX5V[nIndexPLC], nAddress, bVal);
        }
        #endregion

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
