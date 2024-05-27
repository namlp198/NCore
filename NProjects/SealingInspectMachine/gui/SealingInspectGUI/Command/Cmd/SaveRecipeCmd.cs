using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class SaveRecipeCmd : CommandBase
    {
        public SaveRecipeCmd() { }
        public override void Execute(object parameter)
        {
            string btnName = parameter as string;
            if (btnName != null)
            {
                if(string.Compare(btnName, "btnSaveFrame1_Top") == 0 ||
                   string.Compare(btnName, "btnSaveFrame2_Top") == 0)
                {
                    MessageBox.Show("Save Recipe Top Cam");
                }
                else if(string.Compare(btnName, "btnSaveFrame1_Side") == 0 ||
                        string.Compare(btnName, "btnSaveFrame2_Side") == 0 ||
                        string.Compare(btnName, "btnSaveFrame3_Side") == 0 ||
                        string.Compare(btnName, "btnSaveFrame4_Side") == 0)
                {
                    MessageBox.Show("Save Recipe Side Cam");
                }
            }
        }
    }
}
