﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GolemUI.Miners;
using Newtonsoft.Json;

namespace GolemUI.Model
{
    public class UserSettings : INotifyPropertyChanged
    {
        public int SettingsVersion { get; set; }

        public bool SetupFinished { get; set; }
        public string? BenchmarkLength { get; set; }
        public string? CustomPool { get; set; }
        public string? OptionalEmail { get; set; }

        public bool EnableDebugLogs { get; set; }
        public bool StartYagnaCommandLine { get; set; }
        public bool StartProviderCommandLine { get; set; }

        private bool _disableNotificationsWhenMinimized;
        public bool DisableNotificationsWhenMinimized
        {
            get => _disableNotificationsWhenMinimized;
            set
            {
                _disableNotificationsWhenMinimized = value;
                NotifyChanged("DisableNotificationsWhenMinimized");
            }
        }

        private bool _notificationsEnabled = true;
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                _notificationsEnabled = value;
                NotifyChanged(nameof(NotificationsEnabled));
            }
        }
        private bool _shouldDisplayNotificationsIfMiningIsActive = true;
        public bool ShouldDisplayNotificationsIfMiningIsActive
        {
            get => _shouldDisplayNotificationsIfMiningIsActive;
            set
            {
                _shouldDisplayNotificationsIfMiningIsActive = value;
                NotifyChanged(nameof(ShouldDisplayNotificationsIfMiningIsActive));
            }
        }
        private bool _shouldAutoRestartMiningAfterBenchmark = true;
        public bool ShouldAutoRestartMiningAfterBenchmark
        {
            get => _shouldAutoRestartMiningAfterBenchmark;
            set
            {
                _shouldAutoRestartMiningAfterBenchmark = value;
                NotifyChanged(nameof(ShouldAutoRestartMiningAfterBenchmark));
            }
        }

        private bool _minimizeToTrayOnMinimize;
        public bool MinimizeToTrayOnMinimize
        {
            get => _minimizeToTrayOnMinimize;
            set
            {
                _minimizeToTrayOnMinimize = value;
                NotifyChanged("MinimizeToTrayOnMinimize");
            }
        }

        private bool _closeOnExit;
        public bool CloseOnExit
        {
            get => _closeOnExit;
            set
            {
                _closeOnExit = value;
                NotifyChanged("CloseOnExit");
            }
        }

        private bool _startWithWindows;
        public bool StartWithWindows
        {
            get => _startWithWindows;
            set
            {
                _startWithWindows = value;
                NotifyChanged("StartWithWindows");
            }
        }

        private bool _useHourScheduling = false;
        public bool UseHourScheduling
        {
            get
            {
                return _useHourScheduling;
            }
            set
            {
                _useHourScheduling = value;
                NotifyChanged("UseHourScheduling");
            }
        }
        
        private TimeSpan _workHourBegin;
        public TimeSpan WorkHourBegin
        {
            get
            {
                return _workHourBegin;
            }
            set
            {
                _workHourBegin = value;
                NotifyChanged("WorkHourBegin");
            }
        }

        private TimeSpan _workHourEnd;
        public TimeSpan WorkHourEnd
        {
            get
            {
                return _workHourEnd;
            }
            set
            {
                _workHourEnd = value;
                NotifyChanged("WorkHourEnd");
            }
        }


        public bool EnableWASMUnit { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _opacity = 80;
        public int Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
                NotifyChanged("Opacity");
            }
        }

        private bool _sendDebugInformation = true;
        public bool SendDebugInformation
        {
            get => _sendDebugInformation;
            set
            {
                _sendDebugInformation = value;
                NotifyChanged();
            }
        }
        private bool _forceLowMemoryMode = false;
        public bool ForceLowMemoryMode
        {
            get => _forceLowMemoryMode;
            set
            {
                _forceLowMemoryMode = value;
                NotifyChanged();
            }
        }

        private int _minerType = 0;
        public int SelectedMinerType
        {
            get => _minerType;
            set
            {
                _minerType = value;
                NotifyChanged();
                NotifyChanged("SelectedMinerName");
                NotifyChanged("IsTRexMiner");
            }
        }

        [JsonIgnore]
        public MinerAppName SelectedMinerName
        {
            get
            {
                switch (SelectedMinerType)
                {
                    case 0:
                        return new MinerAppName(MinerAppName.MinerAppEnum.Phoenix);
                    case 1:
                        return new MinerAppName(MinerAppName.MinerAppEnum.TRex);
                    default:
                        return new MinerAppName(MinerAppName.MinerAppEnum.Phoenix);
                }
            }
            set
            {
                switch (value.NameEnum)
                {
                    case MinerAppName.MinerAppEnum.Phoenix:
                        SelectedMinerType = 0;
                        break;
                    case MinerAppName.MinerAppEnum.TRex:
                        SelectedMinerType = 1;
                        break;
                    default:
                        throw new Exception("Unkown enum");
                }
            }
        }

        [JsonIgnore]
        public bool IsTRexMiner
        {
            get
            {
                return _minerType == 1;
            }
            set
            {
                if (value)
                {
                    SelectedMinerType = 1;
                }
                else
                {
                    SelectedMinerType = 0;
                }
                NotifyChanged();
            }
        }
    }
}
