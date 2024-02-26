using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Npc.Foundation.Shell
{
    public interface IShell
    {
        void ShowPopupWindow(Window popup, bool isModaless);
        void ClosePopupWindow(Window popup);
        ShellMessageBoxResults ShowMessageBox(ShellMessageBoxParameters e);

        void ShowCCIInformation();

        IRegionManager RegionManager { get; }
        IUnityContainer UnityContainer { get; }
    }

    public enum ShellMessageBoxResults
    {
        None,
        OK,
        Cancel,
        Yes,
        No,
    }

    public enum ShellMessageBoxTypes
    {
        None,
        OK,
        OKCancel,
        YesNo,
        YesNoCancel,
        Custom,
        MachineError,
        Retry,
        Save,
        SaveDontSaveCancel,
    }
}
