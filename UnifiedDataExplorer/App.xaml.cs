using System;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EIADataViewer.Common;
using EIADataViewer.ViewModel;
using EIADataViewer.Startup;

namespace EIADataViewer
{
    public partial class App : Application
    {
        private IConfiguration _config;
        private IServiceProvider _provider;
        private ILogger _logger;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            _config = Bootstrapper.LoadConfiguration();
            _provider = Bootstrapper.BuildServiceProvider(_config);
            _logger = _provider.GetRequiredService<ILogger<App>>();

            RegisterApplicationDataTemplates();

            MainViewModel viewModel = _provider.GetRequiredService<MainViewModel>();
            MainWindow window = new MainWindow();
            window.DataContext = viewModel;
            window.Show();
            await viewModel.LoadAsync();
        }

        private void RegisterApplicationDataTemplates()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            DataTemplateRegistrar templateRegistrar = new DataTemplateRegistrar(executingAssembly, "EIADataViewer.ViewModel", "EIADataViewer.View");
            templateRegistrar.RegisterAllTemplatesByConvention();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
        {
            _logger.LogError($"Unhandled exception occured.", args.Exception);
            MessageBox.Show("Unexpected Error Occurred." + Environment.NewLine + args.Exception.Message, "Unexpected Error");
            args.Handled = true;
        }
    }
}
