using DB;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsTable.ViewModels
{
    public class LogsViewModel : BindableBase
    {
        IDB DB;
        #region Commands
        public DelegateCommand Info { get; private set; }
        public DelegateCommand InfoOK { get; private set; }
        #endregion
        public LogsViewModel(IDB DB)
        {
            this.DB = DB;
            Logs = DB.GetLogList();
            Info = new DelegateCommand(InfoUp);
            InfoOK = new DelegateCommand(InfoOKk);
            DB.LogsListChanged += LogBaseUpdate;
        }

        private void LogBaseUpdate()
        {
            Logs = DB.GetLogList();
        }

        private void InfoUp()
        {
            InfoPop = !InfoPop;
            Client1 = DB.GetClient(selectedItem.SenderID);
            Client2 = DB.GetClient(selectedItem.RecipientID);
        }
        private void InfoOKk()
        {
            InfoPop = !InfoPop;
        }
        #region Props
        private Client client1;
        public Client Client1 { get => client1; set => SetProperty(ref client1, value); }
        private Client client2;
        public Client Client2 { get => client2; set => SetProperty(ref client2, value); }
        private List<Log> _logs;
        public List<Log> Logs
        {
            get { return _logs; }
            set { SetProperty(ref _logs, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        private Log selectedItem;
        public Log SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        private bool infoPop;
        public bool InfoPop { get => infoPop; set => SetProperty(ref infoPop, value); }
        #endregion
    }
}
