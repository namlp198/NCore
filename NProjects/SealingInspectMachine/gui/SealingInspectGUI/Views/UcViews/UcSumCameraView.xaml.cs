﻿using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SealingInspectGUI.Views.UcViews
{
    /// <summary>
    /// Interaction logic for UcAllCameraView.xaml
    /// </summary>
    public partial class UcSumCameraView : UserControl
    {
        public UcSumCameraView()
        {
            InitializeComponent();
        }

        private void btnTestTcpSocket_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.TestTCPSocket();

            //MainViewModel.Instance.RunVM.SumCamVM.LightController_PD3.Set_4_Light_255();
        }
    }
}
