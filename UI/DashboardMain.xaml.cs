﻿using GolemUI.Interfaces;
using GolemUI.Settings;
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

namespace GolemUI
{
    /// <summary>
    /// Interaction logic for DashboardMain.xaml
    /// </summary>
    public partial class DashboardMain : UserControl
    {
        public DashboardMain()
        {
            InitializeComponent();

            //_processController = processController;

            //this.Title = GlobalSettings.AppTitle;
            btnStop.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            GlobalApplicationState.Instance.NotifyApplicationStateChanged(this, GlobalApplicationStateAction.yagnaAppStarting);

            lblStatus.Content = "Starting";
            //lblStatus.Background = Brushes.Yellow;
            btnStart.IsEnabled = false;

            var settings = SettingsLoader.LoadSettingsFromFileOrDefault();

            GlobalApplicationState.Instance.ProcessController.Subnet = settings.Subnet;

            await GlobalApplicationState.Instance.ProcessController.Init();

            lblStatus.Content = "Started";
            //lblStatus.Background = Brushes.Green;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            
            GlobalApplicationState.Instance.NotifyApplicationStateChanged(this, GlobalApplicationStateAction.yagnaAppStarted);
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            bool killProviderInsteadOfStopping = true;
            if (killProviderInsteadOfStopping)
            {
                GlobalApplicationState.Instance.ProcessController.KillProvider();
                GlobalApplicationState.Instance.ProcessController.KillYagna();
            }
            else
            {
                //insta kill provider and gracefully shutdown yagna
                GlobalApplicationState.Instance.ProcessController.KillProvider();
                
                bool providerEndedSuccessfully = await GlobalApplicationState.Instance.ProcessController.StopProvider();
                if (!providerEndedSuccessfully)
                {
                    MessageBox.Show("Provider process failed to shutdown gracefully, killing...");
                    GlobalApplicationState.Instance.ProcessController.KillProvider();
                }
                bool yagnaEndedSuccessfully = await GlobalApplicationState.Instance.ProcessController.StopYagna();
                if (!yagnaEndedSuccessfully)
                {
                    MessageBox.Show("Yagna process failed to shutdown gracefully, killing...");
                    GlobalApplicationState.Instance.ProcessController.KillYagna();
                }
            }


            lblStatus.Content = "Stopped";
            //lblStatus.Background = Brushes.Gray;
            GlobalApplicationState.Instance.NotifyApplicationStateChanged(this, GlobalApplicationStateAction.yagnaAppStopped);
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
    }
}
