﻿using GolemUI.Command;
using GolemUI.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.Src
{
    public class ProviderConfigService : IProviderConfig
    {
        private readonly Command.Provider _provider;
        public Network Network { get; private set; }
        public bool IsLowMemoryModeActive { get; set; }

        public ProviderConfigService(Provider provider, Network network)
        {
            _provider = provider;
            Config = _provider.Config;
            Network = network;

            string? name = Config?.NodeName;
            int count = ActiveCpuCount;
            IsLowMemoryModeActive = false;
        }

        public Config? Config { get; }

        private IList<string>? _activePresets;



        private bool _isPresetActive(string presetName)
        {
            if (_activePresets == null)
            {
                _activePresets = _provider.ActivePresets;
            }
            return _activePresets.Contains(presetName);
        }
        private void _setPreset(string presetName, bool value)
        {
            if (value)
            {
                _provider.ActivatePreset(presetName);
            }
            else
            {
                _provider.DeactivatePreset(presetName);
            }
            _activePresets = _provider.ActivePresets;
            OnPropertyChanged("IsMiningActive");
            OnPropertyChanged("IsCpuActive");
        }

        public bool IsMiningActive
        {
            get
            {
                return _isPresetActive("hminer") || _isPresetActive("gminer");
            }
        }

        public void SetMiningActive(bool active, bool isLowMemoryMode)
        {
            IsLowMemoryModeActive = isLowMemoryMode;
            if (IsLowMemoryModeActive)
            {
                _setPreset("hminer", active);
                _setPreset("gminer", false);
            }
            else
            {
                _setPreset("gminer", active);
                _setPreset("hminer", false);
            }
        }

        public void SwitchMiningMode(bool isLowMemoryMode)
        {
            if (_isPresetActive("gminer") || _isPresetActive("hminer"))
            {
                SetMiningActive(true, isLowMemoryMode);
            }
            else
            {
                SetMiningActive(false, isLowMemoryMode);
            }
        }

        public bool IsCpuActive
        {
            get => _isPresetActive("vm");
            set { _setPreset("vm", value); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ActiveCpuCount => _provider?.DefaultProfile?.CpuThreads ?? 0;

        public void UpdateActiveCpuThreadsCount(int threadsCount)
        {
            _provider.UpdateDefaultProfile("--cpu-threads", threadsCount.ToString());
        }
        public void UpdateNodeName(string? nodeName)
        {
            var config = Config ?? _provider.Config;
            if (config != null)
            {
                config.NodeName = nodeName;
                _provider.Config = Config;
            }
            OnPropertyChanged("Config");
            OnPropertyChanged("NodeName");

        }

        public void UpdateWalletAddress(string? walletAddress = null)
        {
            var config = Config ?? _provider.Config;
            if (config != null)
            {
                config.Account = walletAddress;
                _provider.Config = Config;
            }
            OnPropertyChanged("Config");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task Prepare(bool isGpuCapable, bool isLowMemoryMode)
        {
            IsLowMemoryModeActive = isLowMemoryMode;
            var changedProps = await Task.Run(() =>
            {
                var changedProperties = new List<string>();
                var config = Config ?? _provider.Config;
                if (config!.Subnet == null || config!.Subnet != GolemUI.Properties.Settings.Default.Subnet)
                {
                    config.Subnet = GolemUI.Properties.Settings.Default.Subnet;
                    _provider.Config = Config;
                }

                var presets = _provider.Presets.Select(x => x.Name).ToList();
                string _info, _args;


                if (!presets.Contains("hminer"))
                {
                    _provider.AddPreset(new GolemUI.Command.Preset("hminer", "hminer", new Dictionary<string, decimal>()
                    {
                        { "share", 0.00001m },
                        { "duration", 0m },
                        { "raw-share", 0m },
                        { "stale-share", 0m },
                        { "invalid-share", 0m },
                        { "hash-rate", 0m }
                    }), out _args, out _info);
/*                    if (isGpuCapable)
                    {
                        _provider.ActivatePreset("hminer");
                        changedProperties.Add("IsMiningActive");
                    }*/
                }
                else
                {
                    _provider.Preset["hminer"].UpdatePrices(new Dictionary<string, decimal>() {
                        { "share", 0.00001m },
                        { "duration", 0m },
                        { "raw-share", 0m },
                        { "stale-share", 0m },
                        { "invalid-share", 0m },
                        { "hash-rate", 0m }
                    });

                }

                if (!presets.Contains("gminer"))
                {

                    _provider.AddPreset(new GolemUI.Command.Preset("gminer", "gminer", new Dictionary<string, decimal>()
                    {
                        { "share", 0.00001m },
                        { "duration", 0m },
                        { "raw-share", 0m },
                        { "stale-share", 0m },
                        { "invalid-share", 0m },
                        { "hash-rate", 0m }
                    }), out _args, out _info);
            /*        if (isGpuCapable)
                    {
                        _provider.ActivatePreset("gminer");
                        changedProperties.Add("IsMiningActive");
                    }*/
                }
                else
                {
                    _provider.Preset["gminer"].UpdatePrices(new Dictionary<string, decimal>() {
                        { "share", 0.00001m },
                        { "duration", 0m },
                        { "raw-share", 0m },
                        { "stale-share", 0m },
                        { "invalid-share", 0m },
                        { "hash-rate", 0m }
                    });
                }

                if (isGpuCapable && IsMiningActive)
                {
                    if (presets.Contains("gminer") || presets.Contains("hminer"))
                    {
                        if (isLowMemoryMode)
                        {
                            _provider.ActivatePreset("hminer");
                            _provider.DeactivatePreset("gminer");
                        }
                        else
                        {
                            _provider.DeactivatePreset("hminer");
                            _provider.ActivatePreset("gminer");
                        }
                    }
                }

                if (!presets.Contains("vm"))
                {
                    _provider.AddPreset(new GolemUI.Command.Preset("vm", "vm", new Dictionary<string, decimal>()
                    {
                        { "cpu", 0.001m },
                        { "duration", 0m }
                    }), out _args, out _info);
                    changedProperties.Add("IsCpuActive");
                }

                if (IsCpuActive)
                {
                    _provider.ActivatePreset("vm");
                }
                else
                {
                    _provider.DeactivatePreset("vm");
                }

                /*
                if (!presets.Contains("wasmtime"))
                {
                    _provider.AddPreset(new GolemUI.Command.Preset("wasmtime", "wasmtime", new Dictionary<string, decimal>()
                    {
                        { "cpu", 0.001m },
                        { "duration", 0m }
                    }), out _args, out _info);
                    _provider.ActivatePreset("wasmtime");
                    changedProperties.Add("IsCpuActive");
                }

                if (IsCpuActive)
                {
                    _provider.ActivatePreset("wasmtime");
                }*/

                if (presets.Contains("default"))
                {
                    _provider.DeactivatePreset("default");
                }

                return changedProperties;
            });
            if (changedProps.Count > 0)
            {
                _activePresets = _provider.ActivePresets;
                foreach (var propName in changedProps)
                {
                    OnPropertyChanged(propName);
                }
            }
        }
    }
}
