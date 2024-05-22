using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Command.Cmd
{
    public class SelectSideCamFrameCmd : CommandBase
    {
        public SelectSideCamFrameCmd()
        {

        }
        public override void Execute(object parameter)
        {
            string strBtnName = parameter as string;
            if (strBtnName != null)
            {
                int nBuffIdx = 0;
                int nFrame = 0;

                string strCamName = strBtnName.Substring(3, strBtnName.LastIndexOf("_") - 3);
                string strBuff = strBtnName.Substring(10, strBtnName.LastIndexOf("_") - 10);
                string strFrame = strBtnName.Substring(strBtnName.IndexOf("_") + 6, 1);
                int.TryParse(strBuff, out nBuffIdx);
                int.TryParse(strFrame, out nFrame);
                nFrame -= 1;
                nBuffIdx -= 1;

                if (string.Compare(strCamName, "SideCam1") == 0)
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                  m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuffIdx, nFrame);
                else if (string.Compare(strCamName, "SideCam2") == 0)
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                                                                  m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuffIdx, nFrame);
            }
        }
    }
}
