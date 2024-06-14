#define USE_THREAD

using LModbus;
using SealingInspectGUI.Commons;
using SealingInspectGUI.ViewModels;
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
        public const int TRIGGER_TOPCAM_2 = 4096 + 80 + 100; // M180
        public const int TRIGGER_SIDECAM = 4096 + 81; // M81

        // Bit Write
        public const int INSPECT_TOPCAM_1_COMPLETED = 4096 + 86; // M86
        public const int INSPECT_TOPCAM_2_COMPLETED = 4096 + 86 + 100; // M86

        //public const int GRABIMAGE_SIDECAM1_COMPLETED = 4096 + 87; // M87
        //public const int GRABIMAGE_SIDECAM2_COMPLETED = 4096 + 87; // M87

        public const int INSPECT_OK_1 = 4096 + 88; // M88
        public const int INSPECT_NG_1 = 4096 + 89; // M89
        public const int INSPECT_OK_2 = 4096 + 88 + 100; // M188
        public const int INSPECT_NG_2 = 4096 + 89 + 100; // M189

        private static readonly object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;

        // PLC Wecon LX5V
        private ModbusTCPIPMASTER LX5V;
        private string m_strIPPLC = "";
        private int m_nPort = 502;
        private int m_nIdPlc = 0; // 1: Plc Cavity 1, 2: Plc Cavity 2

        private bool m_bIsConnected;

        private bool m_bJudgement_1_OKNG;
        private bool m_bInspect_1_Completed;
        private bool m_bInspectTopCam_1_Completed;

        private bool m_bJudgement_2_OKNG;
        private bool m_bInspect_2_Completed;
        private bool m_bInspectTopCam_2_Completed;

        private int m_nScanTime = 2;
        private int m_nDelayTime = 100;
        private int m_nDelayTime_2 = 120;
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

        public bool IsJudgement_1_OKNG
        {
            get => m_bJudgement_1_OKNG; set => m_bJudgement_1_OKNG = value;
        }
        public bool IsInspect_1_Completed
        {
            get => m_bInspect_1_Completed; set => m_bInspect_1_Completed = value;
        }
        public bool IsInspectTopCam_1_Completed
        {
            get => m_bInspectTopCam_1_Completed; set => m_bInspectTopCam_1_Completed = value;
        }

        public bool IsJudgement_2_OKNG
        {
            get => m_bJudgement_2_OKNG; set => m_bJudgement_2_OKNG = value;
        }
        public bool IsInspect_2_Completed
        {
            get => m_bInspect_2_Completed; set => m_bInspect_2_Completed = value;
        }
        public bool IsInspectTopCam_2_Completed
        {
            get => m_bInspectTopCam_2_Completed; set => m_bInspectTopCam_2_Completed = value;
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
        public void StartThreadPlcWecon1()
        {
            m_bIsStartThread = true;
            m_threadRunTask = new Thread(new ThreadStart(ThreadTask1));
            m_threadRunTask.IsBackground = true;
            m_threadRunTask.Start();
        }
        public void StartThreadPlcWecon2()
        {
            m_bIsStartThread = true;
            m_threadRunTask = new Thread(new ThreadStart(ThreadTask2));
            m_threadRunTask.IsBackground = true;
            m_threadRunTask.Start();
        }

        private void ThreadTask1()
        {
            while (true)
            {
                int nProcessStatus = 1;
                m_bInspect_1_Completed = false;
                m_bInspectTopCam_1_Completed = false;
                bool bRet = false;

                WriteSingleCoil(INSPECT_TOPCAM_1_COMPLETED, false);
                Thread.Sleep(m_nDelayTime);

                bRet = ReadSingleCoil(LX5V, TRIGGER_TOPCAM_1);

                // 1. read bit trigger top cam
                while (bRet == false)
                {
                    bRet = ReadSingleCoil(LX5V, TRIGGER_TOPCAM_1);

                    Thread.Sleep(m_nDelayTime);
                }

                // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2

                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus1(nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCam_1_Completed)
                {
                    Thread.Sleep(m_nDelayTime);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam

                // turn off light
                MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_255();
                Thread.Sleep(m_nDelayTime);

                WriteSingleCoil(INSPECT_TOPCAM_1_COMPLETED, true);
                Thread.Sleep(m_nDelayTime);
                WriteSingleCoil(INSPECT_TOPCAM_1_COMPLETED, false);


                // 4. wait for inspection done
                while (!IsInspect_1_Completed)
                {
                    Thread.Sleep(m_nDelayTime);
                }

                // turn off light
                MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_0();
                Thread.Sleep(m_nDelayTime);

                // 5. write OK NG
                if (m_bJudgement_1_OKNG == false)
                {
                    WriteSingleCoil(INSPECT_NG_1, true);
                }
                else
                {
                    WriteSingleCoil(INSPECT_OK_1, true);
                }

                Thread.Sleep(m_nDelayTime);
            }
        }

        private void ThreadTask2()
        {
            while (true)
            {
                int nProcessStatus = 1;
                m_bInspect_2_Completed = false;
                m_bInspectTopCam_2_Completed = false;
                bool bRet = false;

                WriteSingleCoil(INSPECT_TOPCAM_2_COMPLETED, false);
                Thread.Sleep(m_nDelayTime_2);

                bRet = ReadSingleCoil(LX5V, TRIGGER_TOPCAM_2);

                // 1. read bit trigger top cam
                while (bRet == false)
                {
                    bRet = ReadSingleCoil(LX5V, TRIGGER_TOPCAM_2);
                    Thread.Sleep(m_nDelayTime_2);
                }

                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus2(nProcessStatus);

                // 2. wait for top cam inspection done
                while (!m_bInspectTopCam_2_Completed)
                {
                    Thread.Sleep(m_nDelayTime_2);
                }

                // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam

                // turn on light
                MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_255();
                Thread.Sleep(m_nDelayTime_2);

                WriteSingleCoil(INSPECT_TOPCAM_2_COMPLETED, true);
                Thread.Sleep(m_nDelayTime_2);
                WriteSingleCoil(INSPECT_TOPCAM_2_COMPLETED, false);

                // 4. wait for inspection done
                while (!m_bInspect_2_Completed)
                {
                    Thread.Sleep(m_nDelayTime_2);
                }

                // turn off light
                MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_0();
                Thread.Sleep(m_nDelayTime_2);

                // 5. write OK NG
                if (m_bJudgement_2_OKNG == false)
                {
                    WriteSingleCoil(INSPECT_NG_2, true);
                }
                else
                {
                    WriteSingleCoil(INSPECT_OK_2, true);
                }

                Thread.Sleep(m_nDelayTime_2);
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

                    int nProcessStatus = 1;
                    m_bInspect_1_Completed = false;
                    m_bInspectTopCam_1_Completed = false;

                    // 1. read bit trigger top cam
                    while (!ReadSingleCoil(LX5V, TRIGGER_TOPCAM_1))
                    {
                        //Thread.Sleep(DelayTime);
                        await Task.Delay(DelayTime);
                    }

                    // Id Plc = 1: PLC Cavity1, Id Plc = 2: PLC Cavity 2

                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetProcessStatus1(nProcessStatus);

                    // 2. wait for top cam inspection done
                    while (!m_bInspectTopCam_1_Completed)
                    {
                        //Thread.Sleep(DelayTime);
                        await Task.Delay(DelayTime);
                    }

                    // 3. write bit PLC, inform top cam inspect done, start rotate for grab image side cam
                    WriteSingleCoil(INSPECT_TOPCAM_1_COMPLETED, true);

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
                    while (!m_bInspect_1_Completed)
                    {
                        //Thread.Sleep(DelayTime);
                        await Task.Delay(DelayTime);
                    }

                    // 5. write OK NG
                    if (m_bJudgement_1_OKNG == false)
                    {
                        WriteSingleCoil(INSPECT_NG_1, true);
                    }
                    else
                    {
                        WriteSingleCoil(INSPECT_OK_1, true);
                    }

                    //Thread.Sleep(DelayTime);
                    await Task.Delay(DelayTime);
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

            lock (_lockObj)
            {
                bool[] arrBool = plc.ReadCoilsTCPIP(nCoil, 1);

                if (arrBool != null)
                {
                    return arrBool[0];
                }
            }

            return false;
        }

        private void WriteSingleCoil(int nAddress, bool bVal)
        {
            lock (_lockObj)
            {
                if (LX5V == null)
                    return;

                LX5V.WriteSingleCoilTCPIP(nAddress, bVal);
            }

        }
        #endregion

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
