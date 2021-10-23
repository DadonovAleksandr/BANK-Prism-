using DB;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace BANK.ViewModels
{
    public class NewClientViewModel : BindableBase
    {
        public static event Action<string> OK;
        public DelegateCommand Do { get; private set; }
        private IDB DB;
        #region props
        private string clientType;
        public string ClientType { get => clientType; set => SetProperty(ref clientType, value); }

        private string fullName;
        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        private string adress;
        public string Adress { get => adress; set => SetProperty(ref adress, value); }

        private string phoneNumber;
        public string PhoneNumber { get => phoneNumber; set => SetProperty(ref phoneNumber, value); }
        #endregion
        public NewClientViewModel(IDB DB)
        {
            this.DB = DB;
            Do = new DelegateCommand(PerformDo);
        }

        private void PerformDo()
        {
            DB.AddNewClient(new Client(ClientType, FullName, 0, Adress, PhoneNumber));
            OK?.Invoke("LogsRegion");
        }
    }
}
