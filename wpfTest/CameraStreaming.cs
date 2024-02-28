using NCore.Wpf.UcZoomBoxViewer;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wfTestTaskProcessor;

namespace wpfTest
{
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
        }
        public void SingleGrab()
        {
            Task.Factory.StartNew(async () =>
            {
                _ucZb.BufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetUsbCamBufferImage(_camIdx);
                await _ucZb.UpdateImage();
            });
        }
        public async Task Start()
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            var initializationSemaphore = new SemaphoreSlim(0, 1);

            _cancellationTokenSource = new CancellationTokenSource();

            _previewTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _ucZb.BufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetUsbCamBufferImage(_camIdx);

                    await _ucZb.UpdateImage();

                    await Task.Delay(33);
                }
            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            await initializationSemaphore.WaitAsync();
            initializationSemaphore.Dispose();
            initializationSemaphore = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        public async Task Stop()
        {
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                _cancellationTokenSource.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                await _previewTask;
            }
        }
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
