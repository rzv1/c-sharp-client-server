using System.Configuration;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Reflection;
using Avalonia.Markup.Xaml;
using CsharpMPP.ViewModels;
using CsharpMPP.Views;
using log4net;
using log4net.Config;
using Networking;
using Persistence;
using Server;

namespace CsharpMPP;

public partial class App : Application
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(App));
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            Log.Info("Application started");
            
            desktop.MainWindow = new Main
            {
                DataContext = new MainViewModel(new ServerProxy(ConfigurationManager.AppSettings["ServerHost"]!, int.Parse(ConfigurationManager.AppSettings["ClientPort"]!)))
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}