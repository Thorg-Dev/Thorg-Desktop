using System.ComponentModel;

namespace GolemUI.Interfaces
{
    interface ISchedulingProvider : INotifyPropertyChanged
    {
        bool isMiningScheduled { get; set; }
        bool isItTimeForMining { get; set; }
    }
}