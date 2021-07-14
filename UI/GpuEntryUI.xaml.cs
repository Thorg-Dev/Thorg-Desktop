﻿using System;
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
    /// Interaction logic for GpuEntryUI.xaml
    /// </summary>
    public partial class GpuEntryUI : UserControl
    {
        bool _supressEvent = true;

        public GpuEntryUI()
        {
            InitializeComponent();
            //this.cbEnableMining.IsEnabled = false;
            _supressEvent = false;

            GlobalApplicationState.Instance.ApplicationStateChanged += OnGlobalApplicationStateChanged;
        }
        public void OnGlobalApplicationStateChanged(object sender, GlobalApplicationStateEventArgs? args)
        {
            if (args != null)
            {
                switch (args.action)
                {
                    case GlobalApplicationStateAction.benchmarkStopped:
                        _supressEvent = true;
                        //this.cbEnableMining.IsEnabled = true;
                        _supressEvent = false;
                        break;
                }
            }
        }

        public void SetInfo(string info)
        {
            this.lblInfo.Content = info;
        }
        public void SetFinished(string? error)
        {
            if (!String.IsNullOrEmpty(error))
            {
                //this.Background = Brushes.Red;
                this.pbProgress.Foreground = Brushes.Red;
                this.tbProgress.Text = "Unable to mine: " + error;
                this.pbProgress.Value = 100;
                //this.cbEnableMining.IsChecked = false;
                //this.cbEnableMining.IsEnabled = false;
            }
            else
            {
                this.tbProgress.Text = "Ready for mining";
                this.pbProgress.Foreground = Brushes.Green;
                this.pbProgress.Value = 100;
                //this.Background = Brushes.Blue;
            }
        }
        public void SetDagProgress(float progr)
        {
            //this.lblProgress.Content = progr.ToString();
            this.pbProgress.Value = progr * 100;
            this.tbProgress.Text = "Mining initialization";
        }
        public void SetMiningProgress(float progr)
        {
            //this.lblProgress.Content = progr.ToString();
            this.pbProgress.Value = progr * 100;
            this.tbProgress.Text = "Measuring performance";
        }

        public void SetMiningSpeed(float? miningSpeed)
        {
            if (miningSpeed == null)
            {
                this.lblPower.Content = "";
            }
            else
            {
                this.lblPower.Content = String.Format("{0:0.00}MH/s", miningSpeed);
            }
        }
        public void SetEnableByUser(bool enable)
        {
            _supressEvent = true;
            //this.cbEnableMining.IsChecked = enable;
            _supressEvent = false;
            if (!enable)
            {
                this.pbProgress.Foreground = Brushes.Gray;
                this.tbProgress.Text = "Not used in mining";
                this.pbProgress.Value = 100;
            }
            else
            {
                this.pbProgress.Foreground = Brushes.Blue;
            }
        }

        private void cbEnableMining_Changed(object sender, RoutedEventArgs e)
        {
            if (!_supressEvent)
            {
                GlobalApplicationState.Instance.NotifyApplicationStateChanged(this, GlobalApplicationStateAction.benchmarkSettingsChanged);
            }
        }
    }
}
