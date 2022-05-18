using GolemUI.Command;
using GolemUI.Interfaces;
using GolemUI.ViewModel.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.ViewModel
{
    public class CharityViewModel : INotifyPropertyChanged, IDialogInvoker, ISavableLoadableDashboardPage
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RequestDarkBackgroundEventHandler DarkBackgroundRequested;
        public event PageChangeRequestedEvent PageChangeRequested;

        public void LoadData()
        {
            //throw new NotImplementedException();
        }

        public void SaveData()
        {
            //throw new NotImplementedException();
        }

        double? _charityPercentage;
        public double? CharityPercentage
        {
            get
            {
                if (_charityPercentage == null)
                {
                    _charityPercentage = _settingsProvider.Config.CharityAmmount;
                }
                return _charityPercentage;
            }
            set => _charityPercentage = value;
        }
        string _charityWallet;
        public string CharityWallet
        {
            get
            {
                if (_charityWallet == null)
                {
                    _charityWallet = _settingsProvider.Config.CharityAccount;
                }
                return _charityWallet;
            }
            set => _charityWallet = value;
        }

        public void CommitChanges()
        {
            _settingsProvider.UpdateCharity(CharityWallet, CharityPercentage);
        }

        public void Save()
        {

        }

        public void RequestDarkBackgroundVisibilityChange(bool shouldBackgroundBeVisible)
        {
            DarkBackgroundRequested?.Invoke(shouldBackgroundBeVisible);
        }


        public DlgEditAddressViewModel EditModel => new DlgEditAddressViewModel(_paymentService);

        private IProviderConfig _settingsProvider;
        private IPaymentService _paymentService;
        public CharityViewModel(IProviderConfig settingsProvider, IPaymentService paymentService)
        {
            _settingsProvider = settingsProvider;
            _paymentService = paymentService;
        }

        public async void UpdateAddress(DlgEditAddressViewModel.Action changeAction, string? address)
        {
            if (address == null)
            {
                return;
            }
            CharityWallet = address;
            OnPropertyChanged(nameof(CharityWallet));
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
