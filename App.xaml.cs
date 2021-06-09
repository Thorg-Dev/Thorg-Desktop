﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GolemUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Interfaces.IProcessControler, Services.ProcessController>();
            services.AddSingleton<MainWindow>();
#if DEBUG
            services.AddSingleton<DebugWindow>();
#endif
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Left = 50;
            mainWindow.Top = 50;
#if DEBUG
            var debugWindow = _serviceProvider.GetService<DebugWindow>();
            mainWindow.DebugWindow = debugWindow;
#endif

            mainWindow.Show();
#if DEBUG
            debugWindow.Owner = mainWindow;
            debugWindow.Left = mainWindow.Left + mainWindow.Width;
            debugWindow.Top = mainWindow.Top;
            debugWindow.Show();
#endif



        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            _serviceProvider.Dispose();
        }



    }
}
