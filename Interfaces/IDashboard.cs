using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GolemUI.Interfaces
{
    public interface IDashboard
    {
        void OnAppReactivate(object sender);

        void RequestClose();

        void UpdateAppearance();

        void ShowUpdateDialog();

        void Show();

    }
}
