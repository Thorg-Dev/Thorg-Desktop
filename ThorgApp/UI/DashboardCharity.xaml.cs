﻿
using GolemUI.ViewModel;
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
using GolemUI.Interfaces;
using GolemUI.Model;
using GolemUI.Src.AppNotificationService;
using GolemUI.ViewModel.Dialogs;

namespace GolemUI
{
    public partial class DashboardCharity : UserControl
    {
        public CharityViewModel ViewModel;
        INotificationService _notificationService;

        public DashboardCharity(CharityViewModel viewModel, INotificationService notificationService)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.DataContext = this.ViewModel;
            _notificationService = notificationService;
        }

        private void BtnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommitChanges();
        }

        private void BtnCopyWalletAddress_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CharityWallet != null)
            {
                _notificationService.PushNotification(new SimpleNotificationObject(Src.AppNotificationService.Tag.Clipboard, "eth address has been copied to clipboard", expirationTimeInMs: 5000));
                Clipboard.SetText(ViewModel.CharityWallet);
            }
        }

        private void BtnEditWalletAddress_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new UI.Dialogs.DlgEditAddress(ViewModel.EditModel);
            dlg.Owner = Window.GetWindow(this);
            ViewModel.RequestDarkBackgroundVisibilityChange(true);
            if (dlg != null && dlg.Model != null && (dlg.ShowDialog() ?? false))
            {
                ViewModel.UpdateAddress(dlg.Model.ChangeAction, dlg.Model.NewAddress);
            }
            ViewModel.RequestDarkBackgroundVisibilityChange(false);
        }
    }
}
