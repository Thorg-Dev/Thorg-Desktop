﻿using GolemUI.Interfaces;
using GolemUI.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace GolemUI
{
    public class SettingsViewModel : INotifyPropertyChanged, ISavableLoadableDashboardPage
    {
        private Command.Provider _provider;
        private BenchmarkResults? _benchmarkSettings;
        private IProviderConfig? _providerConfig;
        private Interfaces.IEstimatedProfitProvider _profitEstimator;
        private Src.BenchmarkService _benchmarkService;
        public Src.BenchmarkService BenchmarkService => _benchmarkService;
        public ObservableCollection<SingleGpuDescriptor>? GpuList { get; set; }

        private IPriceProvider? _priceProvider;
        private int _activeCpusCount=0;

        private decimal _glmPerDay = 0.0m;

        public void StartBenchmark()
        {
            BenchmarkService.StartBenchmark();
        }

        private int _totalCpusCount=0;
   

        public bool IsMiningActive
        {
            get => _providerConfig.IsMiningActive;
            set
            {
                _providerConfig.IsMiningActive = value;
            }
        }

        public bool IsCpuActive
        {
            get => _providerConfig.IsCpuActive;
            set
            {
                _providerConfig.IsCpuActive = value;
            }
        }
        public SettingsViewModel(IPriceProvider priceProvider, Src.BenchmarkService benchmarkService, Command.Provider provider, IProviderConfig providerConfig, Interfaces.IEstimatedProfitProvider profitEstimator)
        {
            GpuList = new ObservableCollection<SingleGpuDescriptor>();
            _priceProvider = priceProvider;
            _provider = provider;
            _providerConfig = providerConfig;
            _benchmarkService = benchmarkService;
            _providerConfig.PropertyChanged += OnProviderCofigChanged;
            _benchmarkService.PropertyChanged += OnBenchmarkChanged;
            _profitEstimator = profitEstimator;
      
            ActiveCpusCount = 3;
            TotalCpusCount = GetCpuCount();

        }
        public void LoadData()
        {
            GpuList?.Clear();
            _benchmarkSettings = SettingsLoader.LoadBenchmarkFromFileOrDefault();
            if (IsBenchmarkSettingsCorrupted()) return;
            _benchmarkSettings?.liveStatus?.GPUs.ToList().Where(gpu => gpu.Value != null && gpu.Value.IsReadyForMining).ToList().ForEach(gpu =>
            {
                var val = gpu.Value;
                GpuList?.Add(new SingleGpuDescriptor(val));
            });
            NodeName = _providerConfig?.Config?.NodeName;
            TotalCpusCount = GetCpuCount();
        }

        private bool IsBenchmarkSettingsCorrupted()
        {
            return (_benchmarkSettings == null || _benchmarkSettings.liveStatus == null || _benchmarkSettings.liveStatus.GPUs == null);
        }
        public void SaveData()
        {
            GpuList?.ToList().ForEach(gpu =>
            {
                if (IsBenchmarkSettingsCorrupted()) return;
                var res = _benchmarkSettings?.liveStatus?.GPUs.ToList().Find(x => x.Value.gpuNo == gpu.Id);
                if (res != null && res.HasValue && !res.Equals(default(KeyValuePair<int, Claymore.ClaymoreGpuStatus>)))
                {
                    KeyValuePair<int, Claymore.ClaymoreGpuStatus> keyVal = res.Value;
                    keyVal.Value.IsEnabledByUser = gpu.IsActive;
                    keyVal.Value.ClaymorePerformanceThrottling = gpu.ClaymorePerformanceThrottling;
                }
            });

            SettingsLoader.SaveBenchmarkToFile(_benchmarkSettings);
        }
       

        private void OnBenchmarkChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {

                var _newGpus = _benchmarkService.Status?.GPUs.Values?.ToArray();
                //_benchmarkService.Status?.GPUInfosParsed
                if (_newGpus.Length == 0) return;
                if (_newGpus != null)
                {
                    for (var i = 0; i < _newGpus.Length; ++i)
                    {
                        if (i < GpuList.Count)
                        {
                            GpuList[i] = new SingleGpuDescriptor(_newGpus[i], GpuList[i].IsActive, GpuList[i].ClaymorePerformanceThrottling);
                        }
                        else
                        {
                            GpuList.Add(new SingleGpuDescriptor(_newGpus[i]));
                        }
                    }
                    while (_newGpus.Length < GpuList.Count)
                    {
                        GpuList.RemoveAt(GpuList.Count - 1);
                    }
                }

                NotifyChange("GpuList"); // ok 
                NotifyChange("HashRate");
                NotifyChange("ExpectedProfit");
                NotifyChange("BenchmarkIsRunning");
                NotifyChange("BenchmarkReadyToRun");
            }
            if (e.PropertyName == "IsRunning")
            {
                NotifyChange("BenchmarkReadyToRun");
                NotifyChange("BenchmarkIsRunning");
                NotifyChange("ExpectedProfit");
            }
        }
        public bool BenchmarkIsRunning => _benchmarkService.IsRunning;
        public bool BenchmarkReadyToRun => !(_benchmarkService.IsRunning);
        private void OnProviderCofigChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Config")
            {
                NotifyChange("NodeName");
            }
            if (e.PropertyName == "IsMiningActive" || e.PropertyName == "IsCpuActive")
            {
                NotifyChange(e.PropertyName);
            }
        }

      
      
        public int ActiveCpusCount
        {
            get { return _activeCpusCount; }
            set
            {
                _activeCpusCount = value;
                NotifyChange("ActiveCpusCount");
            }
        }
        public int TotalCpusCount
        {
            get { return _totalCpusCount; }
            set
            {
                _totalCpusCount = value;
                NotifyChange("TotalCpusCount");
            }
        }
        public float? Hashrate
        {
            get { return _benchmarkService.TotalMhs; ; }
        }

        public string? NodeName
        {
            get => _providerConfig?.Config?.NodeName;
            set
            {
                _providerConfig?.UpdateNodeName(value);
            }
        }

        public double? ExpectedProfit
        {
            get
            {
                var totalHr = Hashrate;
                if (totalHr != null)
                {
                    return _profitEstimator.EthHashRateToDailyEarnigns((double)totalHr, Interfaces.MiningType.MiningTypeEth, Interfaces.MiningEarningsPeriod.MiningEarningsPeriodDay);
                }
                return null;
            }
        }
        public decimal GlmPerDay
        {
            get
            {
                return _glmPerDay;
            }
        }

        public decimal UsdPerDay
        {
            get
            {
                if (_priceProvider == null)
                {
                    return new Decimal(0.0);
                }
                return _priceProvider.glmToUsd(_glmPerDay);
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyChange([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private int GetCpuCount()
        {
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }

            return coreCount;
        }
    }
}
