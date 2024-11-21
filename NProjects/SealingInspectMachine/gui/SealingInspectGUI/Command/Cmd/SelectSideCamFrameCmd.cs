using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCore.Wpf.BufferViewerSimple;

namespace SealingInspectGUI.Command.Cmd
{
    public class SelectSideCamFrameCmd : CommandBase
    {
        public SelectSideCamFrameCmd()
        {

        }
        public override async void Execute(object parameter)
        {
            string strBtnName = parameter as string;
            if (strBtnName != null)
            {
                int nBuffIdx = 0;
                int nFrame = 0;
                string strFrame = string.Empty;

                string strCamName = strBtnName.Substring(3, strBtnName.LastIndexOf("_") - 3);
                string strBuff = strBtnName.Substring(10, strBtnName.LastIndexOf("_") - 10);
                if (strBtnName.CompareTo("btnSideCam1_Frame10") == 0 || strBtnName.CompareTo("btnSideCam2_Frame10") == 0)
                {
                    strFrame = strBtnName.Substring(strBtnName.IndexOf("_") + 6, 2);
                }
                else
                {
                    strFrame = strBtnName.Substring(strBtnName.IndexOf("_") + 6, 1);
                }
                int.TryParse(strBuff, out nBuffIdx);
                int.TryParse(strFrame, out nFrame);
                nFrame -= 1;
                nBuffIdx -= 1;

                if (string.Compare(strCamName, "SideCam1") == 0)
                {
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                  m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuffIdx, nFrame);
                    await MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.UpdateImage();

                    switch (nFrame)
                    {
                        case 0:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame1 == 1) ? 
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 1:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame2 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 2:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame3 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 3:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame4 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break ;
                        case 4:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame5 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 5:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame6 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break ;
                        case 6:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame7 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 7:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame8 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 8:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame9 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 9:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[0].m_sealingInspResult_SideCam.m_bStatusFrame10 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                    }
                }
                else if (string.Compare(strCamName, "SideCam2") == 0)
                {
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                  m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuffIdx, nFrame);
                    await MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.UpdateImage();

                    switch (nFrame)
                    {
                        case 0:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame1 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 1:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame2 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 2:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame3 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 3:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame4 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 4:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame5 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 5:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame6 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 6:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame7 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 7:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame8 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 8:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame9 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                        case 9:
                            MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.InspectResult = (InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                             m_sealingInspectResult[1].m_sealingInspResult_SideCam.m_bStatusFrame10 == 1) ?
                                                                                                             emInspectResult.InspectResult_OK : emInspectResult.InspectResult_NG;
                            break;
                    }
                }
            }
        }
    }
}
