using System.ComponentModel;

namespace GolemUI.Interfaces
{
    public interface ISchedulingProvider : INotifyPropertyChanged
    {
        bool isMiningScheduled { get; set; }
        bool isItTimeForMining { get; set; }
    }
}