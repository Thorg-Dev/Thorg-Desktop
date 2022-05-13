using GolemUI.Command;
using GolemUI.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.ViewModel
{
    public class CharityViewModel : INotifyPropertyChanged, IDisposable, IDialogInvoker, ISavableLoadableDashboardPage
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event RequestDarkBackgroundEventHandler DarkBackgroundRequested;
        public event PageChangeRequestedEvent PageChangeRequested;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void LoadData()
        {
            //throw new NotImplementedException();
        }

        public void SaveData()
        {
            //throw new NotImplementedException();
        }

        public float? CharityPercentage
        {
            get => settingsProvider.Config.CharityAmmount;
            set
            {
                settingsProvider.UpdateCharityPercentage(value);
            }
        }
        public string CharityWallet
        {
            get => settingsProvider.Config.CharityAccount;
            set
            {
                settingsProvider.UpdateCharityWallet(value);
            }
        }

        private IProviderConfig settingsProvider;
        public CharityViewModel(IProviderConfig _settingsProvider)
        {
            settingsProvider = _settingsProvider;
        }
    }
}
