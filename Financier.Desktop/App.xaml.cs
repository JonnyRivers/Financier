using Financier.Entities;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Financier.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IViewService viewService = IoC.ServiceProvider.Instance.GetRequiredService<IViewService>();
            viewService.OpenMainView();
        }
    }
}
