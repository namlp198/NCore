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
                string camName = strBtnName.Substring(3, strBtnName.LastIndexOf("_") - 3);
                string strFrame = strBtnName.Substring(strBtnName.IndexOf("_") + 6, 1);
                int.TryParse(strFrame, out nBuffIdx);
                nBuffIdx -= 1;

                if(string.Compare(camName, "SideCam1") == 0)
                {
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam1.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(nBuffIdx, 0);
                }
                else if (string.Compare(camName, "SideCam2") == 0)
                {
                    nBuffIdx += Defines.MAX_IMAGE_BUFFER_SIDE / 2;
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.buffSideCam2.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(nBuffIdx, 0);
                }
            }
        }
    }
}
