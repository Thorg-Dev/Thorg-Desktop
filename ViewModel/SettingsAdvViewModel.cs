﻿using GolemUI.Interfaces;
using GolemUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.ViewModel
{
    public class SettingsAdvViewModel : INotifyPropertyChanged, ISavableLoadableDashboardPage
    {
        private readonly IUserSettingsProvider _userSettingsProvider;
        public event PageChangeRequestedEvent? PageChangeRequested;

        private UserSettings _userSettings;
        public UserSettings UserSettings
        {
            get
            {
                return _userSettings;
            }
            set
            {
                NotifyChanged("UserSettings");
                _userSettings = value;
            }
        }

        public SettingsAdvViewModel(IUserSettingsProvider userSettingsProvider)
        {
            _userSettingsProvider = userSettingsProvider;
            UserSettings = _userSettingsProvider.LoadUserSettings();
            UserSettings.PropertyChanged += OnUserSettingsPropertyChanged;

            PropertyChanged += OnPropertyChanged;

            PageChangeRequested = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void LoadData()
        {
            UserSettings = _userSettingsProvider.LoadUserSettings();
            UserSettings.PropertyChanged += OnUserSettingsPropertyChanged;
        }

        public void SaveData()
        {
            _userSettingsProvider.SaveUserSettings(UserSettings);
        }

        private void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnUserSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveData();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PageChangeRequested != null)
            {

            }
        }


    }
}
