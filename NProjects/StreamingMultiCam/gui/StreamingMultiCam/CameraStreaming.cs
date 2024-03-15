using NCore.Wpf.UcZoomBoxViewer;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingMultiCam
{
    public enum CameraType { Hik, iRayple, Basler}
    public class CameraStreaming : IDisposable
    {
        private int _camIdx = 0;

        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private UcZoomBoxViewer _ucZb;

        public CameraStreaming(int frameWidth, int frameHeight, UcZoomBoxViewer ucZb, int nCamIdx, ModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this._ucZb = ucZb;
            this._camIdx = nCamIdx;
            this._ucZb.FrameWidth = frameWidth;
            this._ucZb.FrameHeight = frameHeight;
            this._ucZb.ModeView = modeView;
            this._ucZb.SetParamsModeColor(frameWidth, frameHeight);
        }
        public void SingleGrab()
        {
            Task.Factory.StartNew(async () =>
            {
                //_ucZb.BufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetUsbCamBufferImage(_camIdx);
                //await _ucZb.UpdateImage();
            });
        }
        public async Task Start(CameraType cameraType)
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            switch (cameraType)
            {
                case CameraType.Hik:
                    if (!InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.StartGrabHikCam(_camIdx)) return;

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            _ucZb.BufferView = InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.GetHikCamBufferImage(_camIdx);

                            await _ucZb.UpdateImage();

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
                    if (!InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.StartGrabiRaypleCam(_camIdx)) return;

                    var initializationSemaphore1 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            _ucZb.BufferView = InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.GetiRaypleCamBufferImage(_camIdx);

                            await _ucZb.UpdateImage();

                            await Task.Delay(33);
                        }
                    }, _cancellationTokenSource.Token);

                    // Async initialization to have the possibility to show an animated loader without freezing the GUI
                    // The alternative was the long polling. (while !variable) await Task.Delay
                    await initializationSemaphore1.WaitAsync();
                    initializationSemaphore1.Dispose();
                    initializationSemaphore1 = null;

                    if (_previewTask.IsFaulted)
                    {
                        // To let the exceptions exit
                        await _previewTask;
                    }
                    break;
                case CameraType.Basler:
                    break;
                default:
                    break;
            }
        }

        public async Task Stop(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    // If "Dispose" gets called before Stop
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    if (!_previewTask.IsCompleted)
                    {
                        InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.StopGrabHikCam(_camIdx);
                        _cancellationTokenSource.Cancel();

                        // Wait for it, to avoid conflicts with read/write of _lastFrame
                        await _previewTask;
                    }
                    break;
                case CameraType.iRayple:
                    // If "Dispose" gets called before Stop
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    if (!_previewTask.IsCompleted)
                    {
                        InterfaceManager.Instance.m_streamingMultiCamProcessorManager.m_streamingMultiCamProcessor.StopGrabiRaypleCam(_camIdx);
                        _cancellationTokenSource.Cancel();

                        // Wait for it, to avoid conflicts with read/write of _lastFrame
                        await _previewTask;
                    }
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
