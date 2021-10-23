using DB;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientsTable.ViewModels
{
    public class ClientsViewModel : BindableBase
    {
        private IDB DB;
        #region Commands 
        public DelegateCommand Info_button { get; private set; }
        public DelegateCommand InfoOK { get; private set; }
        public DelegateCommand Credit_button { get; private set; }
        public DelegateCommand<string> CreditOK { get; private set; }
        public DelegateCommand<Client> EditClient { get; private set; }
        public DelegateCommand CRepayment_button { get; private set; }
        public DelegateCommand Transfer_button { get; private set; }
        public DelegateCommand<string> TransferOK { get; private set; }
        public DelegateCommand<string> NeededClientChange { get; private set; }
        public DelegateCommand BAUpdate_button { get; private set; }
        public DelegateCommand<string> BAUpdateOK { get; private set; }
        public DelegateCommand DeleteClient { get; private set; }
        #endregion
        #region prop
        private List<Client> _clients;
        public List<Client> Clients
        {
            get { return _clients; }
            set { SetProperty(ref _clients, value); }
        }
        #endregion
        public ClientsViewModel(IDB DB)
        {
            this.DB = DB;
            Clients = DB.GetClientList();
            DB.ClientListChanged += ClientBaseUpdate;
            Console.WriteLine($"ClientList context: {Thread.CurrentThread.ManagedThreadId}");
            #region Commands
            NeededClientChange = new DelegateCommand<string>(NeededClientSet);

            EditClient = new DelegateCommand<Client>(ChangeClientInfo);
            Info_button = new DelegateCommand(InfoPop);
            InfoOK = new DelegateCommand(Info);

            DeleteClient = new DelegateCommand(DelClient);

            Transfer_button = new DelegateCommand(TransferPop);
            TransferOK = new DelegateCommand<string>(TransferAdd);

            Credit_button = new DelegateCommand(CreditPop);
            CreditOK = new DelegateCommand<string>(CreditAdd);

            CRepayment_button = new DelegateCommand(CRepayment);

            BAUpdate_button = new DelegateCommand(BAUpdatePop);
            BAUpdateOK = new DelegateCommand<string>(BAUpdate);
            #endregion
        }
        private void ClientBaseUpdate()
        {
            Clients = DB.GetClientList();
        }

        #region Command methods
        private void BAUpdatePop()
        {
            if (!BankPupUpIsOpen)
                NeededClient = Clients.FirstOrDefault(i => i.ClientID == SelectedItem.ClientID);
            else NeededClient = null;
            BankPupUpIsOpen = !BankPupUpIsOpen;
        }
        private void BAUpdate(string m)
        {
            if (m != "" || NeededClient != null)
                DB.UpdateBankAccount(NeededClient.ClientID, int.Parse(m), BAout);
            NeededClient = null;
            BankPupUpIsOpen = !BankPupUpIsOpen;
        }
        private void CRepayment()
        {
            DB.Repayment(SelectedItem.ClientID);
        }
        private void NeededClientSet(string c)
        {
            try { NeededClient = Clients.FirstOrDefault(i => i.ClientID == int.Parse(c)); }
            catch { NeededClient = null; }
        } //Поиск клиента по ID
        private async void ChangeClientInfo(Client c)
        {
            await Task.Delay(50);
            if (c != null)
                DB.UpdateClientInfo(c);
        } 
        private async void InfoPop()
        {
            if (InfoPupUpIsOpen) await Task.Delay(500);
            InfoPupUpIsOpen = !InfoPupUpIsOpen;
        }
        private void Info()
        { 
            InfoPupUpIsOpen = !InfoPupUpIsOpen;
        }
        private void DelClient()
        {
            DB.DeleteClient(SelectedItem.ClientID);
        }
        private void TransferPop()
        {
            //if (!TransferPupUpIsOpen) 
            //    NeededClient = Clients.FirstOrDefault(i => i.ClientID == SelectedItem.ClientID);
            //else NeededClient = null;
            TransferPupUpIsOpen = !TransferPupUpIsOpen;
        }// ПопАп
        private void TransferAdd(string m)
        {
            if (m != "" || NeededClient != null)
                DB.Transfer(SelectedItem.ClientID, NeededClient.ClientID, int.Parse(m));
            NeededClient = null;
            TransferPupUpIsOpen = !TransferPupUpIsOpen;
        }
        private void CreditPop()
        {
            if (!TransferPupUpIsOpen)
                NeededClient = Clients.FirstOrDefault(i => i.ClientID == SelectedItem.ClientID);
            else NeededClient = null;
            CreditPupUpIsOpen = !CreditPupUpIsOpen;
        }// ПопАп
        private void CreditAdd(string m)
        {
            if (m != "" || NeededClient != null)
                DB.NewCredit(NeededClient.ClientID, int.Parse(m));
            NeededClient = null;
            CreditPupUpIsOpen = !CreditPupUpIsOpen;
        }

        #endregion
        #region Auto props
        private bool infoPupUpIsOpen;

        public bool InfoPupUpIsOpen { get => infoPupUpIsOpen; set => SetProperty(ref infoPupUpIsOpen, value); }

        private bool transferPupUpIsOpen;

        public bool TransferPupUpIsOpen { get => transferPupUpIsOpen; set => SetProperty(ref transferPupUpIsOpen, value); }

        private bool bankPupUpIsOpen;

        public bool BankPupUpIsOpen { get => bankPupUpIsOpen; set => SetProperty(ref bankPupUpIsOpen, value); }

        private Client neededClient;

        public Client NeededClient { get => neededClient; set => SetProperty(ref neededClient, value); }

        private Client selectedItem;

        public Client SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        private bool creditPupUpIsOpen;

        public bool CreditPupUpIsOpen { get => creditPupUpIsOpen; set => SetProperty(ref creditPupUpIsOpen, value); }

        private bool bAOut;

        public bool BAout { get => bAOut; set => SetProperty(ref bAOut, value); }
        #endregion
    }
}
