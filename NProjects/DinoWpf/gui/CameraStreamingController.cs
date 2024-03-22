using NCore.Wpf.NUcBufferViewer;
using NCore.Wpf.UcZoomBoxViewer;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoWpf
{
    public enum CameraType { Hik, iRayple, Basler}
    public class CameraStreamingController : IDisposable
    {
        private int _camIdx = 0;

        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private NUcBufferViewer m_NUcBufferViewer;

        public CameraStreamingController(int frameWidth, int frameHeight, NUcBufferViewer ucBuffV, int nCamIdx, NCore.Wpf.NUcBufferViewer.ModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_NUcBufferViewer = ucBuffV;
            this._camIdx = nCamIdx;
            this.m_NUcBufferViewer.FrameWidth = frameWidth;
            this.m_NUcBufferViewer.FrameHeight = frameHeight;
            this.m_NUcBufferViewer.ModeView = modeView;
            this.m_NUcBufferViewer.SetParamsModeColor(frameWidth, frameHeight);
        }
        public async Task SingleGrab()
        {
            await Task.Factory.StartNew(async () =>
            {
                m_NUcBufferViewer.BufferView = InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.GetBufferImageHikCam(_camIdx);
                await m_NUcBufferViewer.UpdateImage();
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
                    if (!InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.ContinuousGrabHikCam(_camIdx)) return;

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            // read hik camera
                            m_NUcBufferViewer.BufferView = InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.GetBufferImageHikCam(_camIdx);

                            await m_NUcBufferViewer.UpdateImage();

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

        public async Task StopGrab(CameraType cameraType)
        {
            switch (cameraType)
            {
                case CameraType.Hik:
                    // If "Dispose" gets called before Stop
                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;

                    if (!_previewTask.IsCompleted)
                    {
                        InterfaceManager.Instance.TempInspProcessorManager.TempInspProcessorDll.StopGrabHikCam(_camIdx);
                        _cancellationTokenSource.Cancel();

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
