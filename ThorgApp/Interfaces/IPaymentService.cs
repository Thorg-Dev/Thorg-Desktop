using GolemUI.Model;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.Interfaces
{
    public interface IPaymentService : INotifyPropertyChanged
    {
        public class GaslessTicket
        {
            public string Driver { get; set; }
            public BigInteger AmountWei { get; set; }
            public string BlockHash { get; set; }
            public string DestinationAddress { get; set; }

            public decimal Amount => Web3.Convert.FromWei(this.AmountWei);
        }

        WalletState? State { get; }
        WalletState? InternalWalletState { get; }

        string? LastError { get; }

        DateTime? LastSuccessfullRefresh { get; }
        string? Address { get; }

        string InternalAddress { get; }

        Task<bool> TransferOutTo(string address);

        Task<decimal> ExitFee(decimal? amount = null, string? to = null);

        Task<decimal> TransferFee(decimal? amount, string? to = null);

        Task Refresh();

        Task<string> ExitTo(string driver, decimal amount, string destinationAddress, decimal? txFee);


        Task<GaslessTicket> RequestGaslessTransferTo(string driver, string destinationAddress);

        Task<string> ExecuteGaslessTransferTo(GaslessTicket ticket);

        Task<string> TransferTo(string driver, decimal amount, string destinationAddress, decimal? txFee);

    }
}
