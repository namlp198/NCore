using NCore.Wpf.BufferViewerSettingPRO;
using NpcCore.Wpf.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Commons;

namespace NVisionInspectGUI.Manager.Class
{
    public class CameraStreamingController : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _previewTask;
        private BufferViewerSettingPRO m_bufferViewerSettingPRO;

        public CameraStreamingController(BufferViewerSettingPRO ucBuffV)
        {
            this.m_bufferViewerSettingPRO = ucBuffV;
        }

        public void SingleGrab(emCameraBrand camBrand)
        {
            int nCamBrand = (int)camBrand;
            int nCamIdx = m_bufferViewerSettingPRO.CameraIndex;
            int nHikCamCount = MainViewModel.Instance.SettingVM.NumberOfCamBrandList.ElementAt(0);

            switch (camBrand)
            {
                case emCameraBrand.CameraBrand_Hik:
                    break;
                case emCameraBrand.CameraBrand_Basler:
                    nCamIdx = nCamIdx - nHikCamCount;
                    break;
                case emCameraBrand.CameraBrand_Jai:
                    break;
                case emCameraBrand.CameraBrand_IRayple:
                    break;
            }

            if (nCamIdx < 0)
                return;

            Task.Factory.StartNew(async () =>
            {
                m_bufferViewerSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetImageBuffer(nCamBrand, nCamIdx);
                await m_bufferViewerSettingPRO.UpdateImage();
            });
        }

        public async Task ContinuousGrab(emCameraBrand camBrand)
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            MainViewModel.Instance.SettingVM.IsStreamming = true;
            MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.IsStreamming = true;

            int nCamBrand = (int)camBrand;
            int nCamIdx = m_bufferViewerSettingPRO.CameraIndex;
            int nHikCamCount = MainViewModel.Instance.SettingVM.NumberOfCamBrandList.ElementAt(0);

            switch (camBrand)
            {
                case emCameraBrand.CameraBrand_Hik:
                    break;
                case emCameraBrand.CameraBrand_Basler:
                    nCamIdx = nCamIdx - nHikCamCount;
                    break;
                case emCameraBrand.CameraBrand_Jai:
                    break;
                case emCameraBrand.CameraBrand_IRayple:
                    break;
            }

            if (nCamIdx < 0)
                return;

            var initializationSemaphore0 = new SemaphoreSlim(0, 1);

            _cancellationTokenSource = new CancellationTokenSource();

            _previewTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // read hik camera
                    m_bufferViewerSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetImageBuffer(nCamBrand, nCamIdx);
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
        }

        public async Task StopGrab()
        {
            if (_cancellationTokenSource == null) return;
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                MainViewModel.Instance.SettingVM.IsStreamming = false;
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.IsStreamming = false;
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
