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


        string? CharityAddress;

        public void LoadData()
        {
            //throw new NotImplementedException();
        }

        public void SaveData()
        {
            //throw new NotImplementedException();
        }

        float? _charityPercentage;
        public float? CharityPercentage
        {
            get => _charityPercentage;
            set => _charityPercentage = value;
        }
        string _charityWallet;
        public string CharityWallet
        {
            get => _charityWallet;
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
            if (changeAction == DlgEditAddressViewModel.Action.TransferOut)
            {
                CharityAddress = address;
            }
        }
    }
}
