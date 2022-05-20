using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GolemUI.Interfaces;
using GolemUI.Miners;
using GolemUI.Miners.Phoenix;

namespace GolemUI.ViewModel
{
    public class MineForUkraineViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string DestinationAddress => "0x165CD37b4C644C2921454429E7F9358d18A45e14";

        public MineForUkraineViewModel(IProcessController processController, GolemUI.Miners.Phoenix.PhoenixMiner minnerApp)
        {
            this._processController = processController;
            this._minerApp = minnerApp;
            this._processController.PropertyChanged += (sender, ev) => {
                if (ev.PropertyName == nameof(_processController.IsStarting))
                {
                    OnPropertyChanged(nameof(IsReady));
                }
            };
        }

        public decimal AmountUSD => 25;

        public decimal AmountETH => 0.3m;

        private bool _running = false;


        public bool IsRunning {
            get => _running;
            set {
                _running = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        private string? _errorMessage;

        public string? ErrorMessage { 
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsReady => !_processController.IsStarting;

        public async void Start()
        {
            IsRunning = true;
            ErrorMessage = null;
            await _processController.Prepare();
            var me = await _processController.Me();
            Console.WriteLine($"me={me.Id}");
            var configuration = new MinerAppConfiguration()
            {
                EthereumAddress = this.DestinationAddress,
                NodeName = me.Id,
                Pool = GolemUI.Properties.Settings.Default.DefaultProxy
            };
            var parser = _minerApp.CreateParserForBenchmark();
            
            ErrorMessage = "Invalid card";
            IsRunning = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

        private readonly IProcessController _processController;
        private readonly IMinerApp _minerApp;
    }
}
