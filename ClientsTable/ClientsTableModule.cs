using ClientsTable.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ClientsTable
{
    public class ClientsTableModule : IModule
    {
        private readonly IRegionManager _regionManager;
        public ClientsTableModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("ClientsRegion", typeof(ClientsView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}