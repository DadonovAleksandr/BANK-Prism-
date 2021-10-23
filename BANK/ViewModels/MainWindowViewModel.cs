using Prism.Mvvm;
using Prism.Commands;
using Prism.Regions;
using DB;

namespace BANK.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager region;
        private IDB DB;
        //SynchronizationContext UIContext;
        #region Команды
        public DelegateCommand CreateNewDB { get; private set; }
        public DelegateCommand<string> AddClient { get; private set; }
        public DelegateCommand StartImitation { get; private set; }
        #endregion

        public MainWindowViewModel(IRegionManager region, IDB DB)
        {
            this.region = region;
            this.DB = DB;
            NewClientViewModel.OK += Add;
            //UIContext = SynchronizationContext.Current;
            #region Команды
            CreateNewDB = new DelegateCommand(NewDB);
            AddClient = new DelegateCommand<string>(Add);
            StartImitation = new DelegateCommand(ImitationStart);
            #endregion

        }
        #region Методы команд
        private void NewDB()
        {
            DB.CreateBank();
        }
        private void Add(string uri)
        {
            region.RequestNavigate("LogsRegion", uri);
        }     
        private void ImitationStart()
        {
            if (Imitation == "Включить иммитацию")
            {
                Imitation = "Выключить иммитацию";
                DB.Imitation(true);
            }
            else
            {   
                Imitation = "Включить иммитацию";
                DB.Imitation(false);
            }
        }

        #endregion
        #region Поля и свойства
        private string _title = "Тестовое приложение";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }       
        private string imitation = "Включить иммитацию";
        public string Imitation 
        { 
            get => imitation; 
            set => SetProperty(ref imitation, value); 
        }
        #endregion
    }
}
