using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GolemUI.Interfaces
{
    public class ProjectData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string homepage { get; set; }
        public string developer { get; set; }
        public string[] images { get; set; }
    }

    public interface IImageMetadata : INotifyPropertyChanged
    {
        public ProjectData? ProjectData { get; }
        public BitmapImage? Image { get; }
    }

}
