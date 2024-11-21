using NCore.Wpf.BufferViewerSetting;
using NCore.Wpf.BufferViewerSimple;
using NpcCore.Wpf.MVVM;
using ReadCodeGUI.Commons;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadCodeGUI.Manager.Class
{
    public enum CameraType { Hik, iRayple, Basler }
    public class CameraStreamingController : IDisposable
    {
        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private BufferViewerSimple m_bufferViewerSimple;
        private BufferViewerSetting m_bufferViewerSetting;

        public CameraStreamingController(int frameWidth, int frameHeight, BufferViewerSimple ucBuffV, NCore.Wpf.BufferViewerSimple.emModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_bufferViewerSimple = ucBuffV;

            this.m_bufferViewerSimple.FrameWidth = frameWidth;
            this.m_bufferViewerSimple.FrameHeight = frameHeight;
            this.m_bufferViewerSimple.ModeView = modeView;
            this.m_bufferViewerSimple.SetParamsModeColor(frameWidth, frameHeight);
        }
        public CameraStreamingController(int frameWidth, int frameHeight, BufferViewerSetting ucBuffViewerSetting, NCore.Wpf.BufferViewerSetting.EnModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_bufferViewerSetting = ucBuffViewerSetting;

            this.m_bufferViewerSetting.FrameWidth = frameWidth;
            this.m_bufferViewerSetting.FrameHeight = frameHeight;
            this.m_bufferViewerSetting.ModeView = modeView;
            this.m_bufferViewerSetting.SetParamsModeColor(frameWidth, frameHeight);
        }

        public void SingleGrab()
        {
            Task.Factory.StartNew(async () =>
            {
                if (!InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SingleGrabBaslerCam(m_bufferViewerSetting.CameraIndex)) return;

                m_bufferViewerSetting.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetImageBufferBaslerCam(m_bufferViewerSetting.CameraIndex);

                await m_bufferViewerSetting.UpdateImage();
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
                    if (!InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.ContinuousGrabBaslerCam(m_bufferViewerSetting.CameraIndex)) return;

                    MainViewModel.Instance.SettingVM.IsStreamming = true;
                    MainViewModel.Instance.SettingVM.SettingView.buffSetting.IsStreamming = true;

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            m_bufferViewerSetting.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetImageBufferBaslerCam(m_bufferViewerSetting.CameraIndex);
                            await m_bufferViewerSetting.UpdateImage();

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
                    InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.StopGrabBaslerCam(m_bufferViewerSetting.CameraIndex);
                    MainViewModel.Instance.SettingVM.SettingView.buffSetting.IsStreamming = false;
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
                        if (!InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.StopGrabBaslerCam(m_bufferViewerSetting.CameraIndex)) return;

                        MainViewModel.Instance.SettingVM.SettingView.buffSetting.IsStreamming = false;
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
