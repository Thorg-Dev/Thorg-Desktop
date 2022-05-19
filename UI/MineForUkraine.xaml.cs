using GolemUI.Interfaces;
using GolemUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace GolemUI.UI
{
    /// <summary>
    /// Interaction logic for MineForUkraine.xaml
    /// </summary>
    public partial class MineForUkraine : Window, IDashboard
    {
        public MineForUkraineViewModel Model => (DataContext as MineForUkraineViewModel)!;
        public MineForUkraine(MineForUkraineViewModel viewModel, Interfaces.IUserSettingsProvider userSettingsProvider, Interfaces.IProcessController processController)
        {
            InitializeComponent();
            DataContext = viewModel;
            _userSettingsProvider = userSettingsProvider;
            _processController = processController;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        public void OnAppReactivate(object sender)
        {
            Dispatcher.Invoke(() =>
            {
                WindowState = WindowState.Normal;
                ShowInTaskbar = true;
                Activate();
            });
        }

        private async Task<bool> RequestCloseAsync()
        {
            //Disable logging in debugwindow to prevent problems with invoke 
            DebugWindow.EnableLoggingToDebugWindow = false;

            //Stop provider
            await _processController.Stop();

            //force exit know after trying to gently stop yagna (which may succeeded or failed)
            this._forceExit = true;
            this.Close();
            return false;
        }

        private bool _forceExit = false;
        private readonly IUserSettingsProvider _userSettingsProvider;
        private readonly IProcessController _processController;

        public void RequestClose()
        {
            Task.Run(() => this.Dispatcher.Invoke(async () => await RequestCloseAsync()));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_forceExit)
            {
                return;
            }

            bool closeOnExit = _userSettingsProvider.LoadUserSettings().CloseOnExit;
            if (closeOnExit)
            {
                RequestClose();
            }
            else
            {
                tbNotificationIcon.Visibility = Visibility.Visible;
                if (_userSettingsProvider.LoadUserSettings().NotificationsEnabled)
                    tbNotificationIcon.ShowBalloonTip("Thorg Miner is still running in tray", "To close application use Thorg Miner's tray's context menu.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            e.Cancel = true;
        }

        public void UpdateAppearance()
        {
            
        }

        public void ShowUpdateDialog()
        {
            
        }

        private void StartMining_Click(object sender, RoutedEventArgs e)
        {
            this.Model.Start();
        }
    }
}
