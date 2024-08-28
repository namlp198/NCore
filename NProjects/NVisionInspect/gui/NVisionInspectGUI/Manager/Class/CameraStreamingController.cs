using NCore.Wpf.BufferViewerSettingPRO;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NVisionInspectGUI.ViewModels;

namespace NVisionInspectGUI.Manager.Class
{
    public enum CameraType { Hik, iRayple, Basler }
    public class CameraStreamingController : IDisposable
    {
        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private BufferViewerSettingPRO m_bufferViewerSettingPRO;

        public CameraStreamingController(int frameWidth, int frameHeight, BufferViewerSettingPRO ucBuffV, NCore.Wpf.BufferViewerSettingPRO.EnModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_bufferViewerSettingPRO = ucBuffV;

            this.m_bufferViewerSettingPRO.FrameWidth = frameWidth;
            this.m_bufferViewerSettingPRO.FrameHeight = frameHeight;
            this.m_bufferViewerSettingPRO.ModeView = modeView;
            this.m_bufferViewerSettingPRO.SetParamsModeColor(frameWidth, frameHeight);
        }

        public void SingleGrab()
        {
            Task.Factory.StartNew(async () =>
            {
                if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SingleGrabHikCam(m_bufferViewerSettingPRO.CameraIndex)) return;

                //m_bufferViewerSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetImageBufferBaslerCam(m_bufferViewerSettingPRO.CameraIndex);
                //await m_bufferViewerSettingPRO.UpdateImage();
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
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.ContinuousGrabHikCam(m_bufferViewerSettingPRO.CameraIndex)) return;

                    MainViewModel.Instance.SettingVM.IsStreamming = true;
                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.IsStreamming = true;

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            m_bufferViewerSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetImageBufferHikCam(m_bufferViewerSettingPRO.CameraIndex);
                            await m_bufferViewerSettingPRO.UpdateImage();

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
                default:
                    break;
            }
        }
        public void StopSingleGrab(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.StopGrabHikCam(m_bufferViewerSettingPRO.CameraIndex);
                    MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.IsStreamming = false;
                    break;
            }

        }
        public async Task StopGrab(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    break;
                case CameraType.iRayple:
                    break;
                case CameraType.Basler:
                    if (_cancellationTokenSource == null) return;
                    // If "Dispose" gets called before Stop
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    if (!_previewTask.IsCompleted)
                    {
                        if (!InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.StopGrabHikCam(m_bufferViewerSettingPRO.CameraIndex)) return;

                        MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.IsStreamming = false;
                        _cancellationTokenSource.Cancel();

                        // Wait for it, to avoid conflicts with read/write of _lastFrame
                        await _previewTask;
                    }
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
