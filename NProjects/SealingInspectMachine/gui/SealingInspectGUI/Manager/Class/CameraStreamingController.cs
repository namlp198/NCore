using NCore.Wpf.BufferViewerSimple;
using NpcCore.Wpf.MVVM;
using SealingInspectGUI.Commons;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager.Class
{
    public enum CameraType { Hik, iRayple, Basler }
    public class CameraStreamingController : IDisposable
    {
        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private BufferViewerSimple m_bufferViewerSimple;

        public CameraStreamingController(int frameWidth, int frameHeight, BufferViewerSimple ucBuffV, NCore.Wpf.BufferViewerSimple.ModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_bufferViewerSimple = ucBuffV;

            this.m_bufferViewerSimple.FrameWidth = frameWidth;
            this.m_bufferViewerSimple.FrameHeight = frameHeight;
            this.m_bufferViewerSimple.ModeView = modeView;
            this.m_bufferViewerSimple.SetParamsModeColor(frameWidth, frameHeight);
        }

        public void SingleGrab()
        {
            //InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerModeHikCam(m_bufferViewerSimple.CameraIndex,
            //                                                                      (int)eTriggerMode.TriggerMode_External);
            //InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerSourceHikCam(m_bufferViewerSimple.CameraIndex,
            //                                                                      (int)eTriggerSource.TriggerSource_Software);

            Task.Factory.StartNew(() =>
            {
                if (!InterfaceManager.Instance.m_sealingInspProcessor.ContinuousGrabHikCam(m_bufferViewerSimple.CameraIndex)) return;
                //if (!InterfaceManager.Instance.m_sealingInspProcessor.SingleGrabHikCam(m_bufferViewerSimple.CameraIndex)) return;

                m_bufferViewerSimple.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImageHikCam(m_bufferViewerSimple.CameraIndex);
            });
        }

        public async Task ContinuousGrab(CameraType cameraType)
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            switch (cameraType)
            {
                case CameraType.Hik:

                    InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerModeHikCam(m_bufferViewerSimple.CameraIndex,
                                                                                  (int)eTriggerMode.TriggerMode_Internal);
                    InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerSourceHikCam(m_bufferViewerSimple.CameraIndex,
                                                                                          (int)eTriggerSource.TriggerSource_Hardware);
                    if (!InterfaceManager.Instance.m_sealingInspProcessor.ContinuousGrabHikCam(m_bufferViewerSimple.CameraIndex)) return;

                    MainViewModel.Instance.SettingVM.IsStreamming = true;

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            m_bufferViewerSimple.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImageHikCam(m_bufferViewerSimple.CameraIndex);

                            await Task.Delay(33);
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
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    break;
                default:
                    break;
            }
        }
        public void StopSingleGrab(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    InterfaceManager.Instance.m_sealingInspProcessor.StopGrabHikCam(m_bufferViewerSimple.CameraIndex);
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    break;
            }
            
        }
        public async Task StopGrab(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    if (_cancellationTokenSource == null) return;
                    // If "Dispose" gets called before Stop
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    if (!_previewTask.IsCompleted)
                    {
                        InterfaceManager.Instance.m_sealingInspProcessor.StopGrabHikCam(m_bufferViewerSimple.CameraIndex);
                        _cancellationTokenSource.Cancel();

                        MainViewModel.Instance.SettingVM.IsStreamming = false;

                        // Wait for it, to avoid conflicts with read/write of _lastFrame
                        await _previewTask;
                    }
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    break;
                default:
                    break;
            }
        }
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
