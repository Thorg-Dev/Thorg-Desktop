using GolemUI.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace GolemUI.Src
{
    class SchedulingProvider : ISchedulingProvider
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        DispatcherTimer _schedulingTimer;

        IUserSettingsProvider _settingsProvider;

        bool _isMiningSchedulingEnabled = false;
        bool _isMiningScheduled = false;

        bool ISchedulingProvider.isMiningScheduled { get => _isMiningSchedulingEnabled; set => _isMiningSchedulingEnabled = value; }
        bool ISchedulingProvider.isItTimeForMining { get => _isMiningScheduled; set => _isMiningScheduled = value; }

        public SchedulingProvider(IUserSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            _schedulingTimer.Interval = TimeSpan.FromMinutes(1);
            _schedulingTimer.Tick += _schedulingTimer_Tick;
            _schedulingTimer.Start();
        }

        private void _schedulingTimer_Tick(object sender, EventArgs e)
        {
            var settings = _settingsProvider.LoadUserSettings();
            if (settings._useHourScheduling == false) return;

            var currentTime = DateTime.Now.TimeOfDay;
            

            var miningBegin = settings._workHourBegin;
            var miningEnd = settings._workHourEnd;

            if (miningBegin < miningEnd)
            {
                bool newMiningScheduled = miningBegin <= currentTime && currentTime >= miningEnd;
                if (newMiningScheduled != _isMiningScheduled)
                {
                    _isMiningScheduled = newMiningScheduled;
                    OnPropertyChanged("IsMiningScheduled");
                }
            }
            else
            {
                bool newMiningScheduled = miningBegin >= currentTime && currentTime <= miningEnd;
                if (newMiningScheduled != _isMiningScheduled)
                {
                    _isMiningScheduled = newMiningScheduled;
                    OnPropertyChanged("IsMiningScheduled");
                }
            }
        }
    }
}