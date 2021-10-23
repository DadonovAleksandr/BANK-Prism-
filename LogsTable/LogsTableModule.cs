using LogsTable.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace LogsTable
{
    public class LogsTableModule : IModule
    {
        private readonly IRegionManager _regionManager;
        public LogsTableModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("LogsRegion", typeof(LogsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}