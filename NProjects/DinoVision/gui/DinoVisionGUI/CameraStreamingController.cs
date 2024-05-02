using NCore.Wpf.UcZoomBoxViewer;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    public enum CameraType { Hik, iRayple, Basler, UsbCam }
    public class CameraStreamingController : IDisposable
    {
        private int _camIdx = 0;

        private readonly int _frameWidth;
        private readonly int _frameHeight;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private UcZoomBoxViewer m_NUcZoomBoxViewer;

        public CameraStreamingController(int frameWidth, int frameHeight, UcZoomBoxViewer ucBuffV, int nCamIdx, ModeView modeView)
        {
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this.m_NUcZoomBoxViewer = ucBuffV;
            this._camIdx = nCamIdx;
            this.m_NUcZoomBoxViewer.FrameWidth = frameWidth;
            this.m_NUcZoomBoxViewer.FrameHeight = frameHeight;
            this.m_NUcZoomBoxViewer.ModeView = modeView;
            this.m_NUcZoomBoxViewer.SetParamsModeColor(frameWidth, frameHeight);
        }
        public async Task SingleGrab()
        {
            //InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.ConnectDinoCam(_camIdx);
            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.SingleGrabDinoCam(_camIdx);
            m_NUcZoomBoxViewer.BufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetBufferDinoCam(_camIdx);
            await m_NUcZoomBoxViewer.UpdateImage();
            //InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.DisconnectDinoCam(_camIdx);
        }

        public async Task ContinuousGrab(CameraType cameraType)
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            switch (cameraType)
            {
                case CameraType.UsbCam:

                    var initializationSemaphore0 = new SemaphoreSlim(0, 1);

                    _cancellationTokenSource = new CancellationTokenSource();

                    _previewTask = Task.Run(async () =>
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.StartGrabDinoCam(_camIdx);
                            m_NUcZoomBoxViewer.BufferView = InterfaceManager.Instance.JigInspProcessorManager.JigInspProcessorDll.GetBufferDinoCam(_camIdx);
                            await m_NUcZoomBoxViewer.UpdateImage();

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
                case CameraType.UsbCam:
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
