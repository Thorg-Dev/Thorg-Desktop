﻿using GolemUI.Command;
using GolemUI.Interfaces;

using GolemUI.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GolemUI
{
    /// <summary>
    /// Interaction logic for DashboardMain.xaml
    /// </summary>
    public partial class DashboardMain : UserControl
    {
        public DashboardMainViewModel Model => (DataContext as DashboardMainViewModel)!;

        IProcessController _processController;

        IBenchmarkResultsProvider _benchmarkResultsProvider;

        IHistoryDataProvider _historyDataProvider;

        public DashboardMain(DashboardMainViewModel viewModel, IHistoryDataProvider dataProv, IBenchmarkResultsProvider benchmarkResultsProvider, IProcessController processController)
        {
            _benchmarkResultsProvider = benchmarkResultsProvider;
            DataContext = viewModel;
            InitializeComponent();
            _processController = processController;
            _historyDataProvider = dataProv;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Model!.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Model!.Stop();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            Model.SwitchToSettings();
        }

        private void BtnGolemLogo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(GolemUI.Properties.Settings.Default.GolemWebPage);
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            Model.SwitchToStatistics();
        }
        private void BtnTRexInfo_Click(object sender, RoutedEventArgs e)
        {
            Model.SwitchToTRexInfo();
        }

        private void lblPaymentStateMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Model.ShouldPaymentMessageTooltipBeAccessible)
                tooltip.IsOpen = true;
        }

        private void lblPaymentStateMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            tooltip.IsOpen = false;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Model.PolygonLink);
        }
    }
}
