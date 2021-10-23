using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using DB;
using ClientsTable;
using LogsTable;
using BANK.Views;
using LogsTable.Views;

namespace BANK
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindowView>();
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
            containerRegistry.RegisterSingleton<IDB, Bank>();
            
            containerRegistry.RegisterForNavigation<NewClientView>();
            containerRegistry.RegisterForNavigation<LogsView>("LogsRegion");
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ClientsTableModule>();
            moduleCatalog.AddModule<LogsTableModule>();
        }
    }
}
