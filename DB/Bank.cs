using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DB
{
    public class BankContext : DbContext
    {
        public BankContext() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Bank;Integrated Security=True; MultipleActiveResultSets=true") { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
    public delegate void BaseChanged();
    public interface IDB
    {
        event Action ClientListChanged;
        event Action LogsListChanged;
        void CreateBank();
        List<Client> GetClientList();
        List<Log> GetLogList();
        void AddNewClient(Client c);
        void UpdateClientInfo(Client c);
        void DeleteClient(int c);
        void Transfer(int c1, int c2, int money);
        void NewCredit(int cID, int money);
        void Repayment(int cID);
        void UpdateBankAccount(int cID, int money, bool outside);
        void Imitation(bool on);
        Client GetClient(int cID);
    }
    /// <summary>
    /// Набор функций взаимодействия с данными БД
    /// </summary>
    public class Bank : BindableBase, IDB
    {
        public event Action ClientListChanged;
        public event Action LogsListChanged;
        private Random r = new Random();
        private BankContext DB;
        Thread imitationThread;

        /// <summary>
        /// Очередь Методов к исполнению во время имитации работы БД
        /// </summary>
        Queue<Client> ActionQueye;

        public Bank()
        {
            ActionQueye = new Queue<Client>();
            DB = new BankContext();
        }
        public List<Client> GetClientList() { return DB.Clients.ToList(); }
        public List<Log> GetLogList() { return DB.Logs.ToList(); }

        /// <summary>
        /// Создание новой ДБ
        /// </summary>
        /// <param name="w"></param>
        public void CreateBank()
        {
            DB.Database.ExecuteSqlCommand("TRUNCATE TABLE [Clients]");
            DB.Database.ExecuteSqlCommand("TRUNCATE TABLE [Logs]");
            DB = new BankContext(); //загрузка "новых" данных из БД

            int n = 2_000;
            for (int d = 0; d < n; d++)
            {
                CBNW();
            }
            DB.SaveChanges();
            ClientListChanged?.Invoke();
            LogsListChanged?.Invoke();
        }
        /// <summary>
        /// Рандомизатор добавления клиентов
        /// </summary>
        private void CBNW()
        {
            int rand = r.Next(500, 800);
            switch (r.Next(3))
            {
                case 0://VIP
                    DB.Clients.Add(new Client("VIP", "Василий vip", rand, "Г. Москва, ул. Советская 63", "+79995554444"));
                    break;
                case 1:// CLIENT
                    DB.Clients.Add(new Client("Client", "Павел Александров", rand*3, "Г. Москва, ул. Пушкина 22", "+79992456585"));
                    break;
                default://Entitie      
                    DB.Clients.Add(new Client("Entitie", "Завод", rand*10, "Г. Москва, ул. Строителей 8", "+79384205543"));
                    break;
            }
        }

        public void AddNewClient(Client c)
        {
            if (ImitationOn) ActionQueye.Enqueue(DB.Clients.Add(c));
            else
            {
                DB.Clients.Add(c);
                DB.SaveChanges();
                ClientListChanged?.Invoke();
            }
        }   
        public void DeleteClient(int c)
        {
            if (ImitationOn) ActionQueye.Enqueue(DB.Clients.Remove(DB.Clients.Local.Single(x => x.ClientID == c)));// Во время режима иммиатции добавлять в очередь на удаление
            else
            {
                DB.Clients.Remove(DB.Clients.Local.Single(x => x.ClientID == c));
                DB.SaveChanges();
                ClientListChanged?.Invoke();
            }
        }

        public void Transfer(int c1, int c2, int money)
        {
            if (ImitationOn) ActionQueye.Enqueue(TransferC(c1, c2, money));
            else
            {
                TransferC(c1, c2, money);
                DB.SaveChanges();
                ClientListChanged?.Invoke();
                LogsListChanged?.Invoke();
            }
        }
        private Client TransferC(int c1, int c2, int money)
        {
            try
            {
                var from = DB.Clients.Single(x => x.ClientID == c1);
                var to = DB.Clients.Single(x => x.ClientID == c2);
                if (from.MainAccount >= money)
                {
                    from.MainAccount -= money;
                    to.MainAccount += money;

                    DB.Logs.Add(new Log(DateTime.Now, money, c1, c2, true));
                }
                else
                    DB.Logs.Add(new Log(DateTime.Now, money, c1, c2, false));
            }
            catch
            {
                DB.Logs.Add(new Log(DateTime.Now, money, c1, c2, false));
            }
            return new Client();
        }

        public void NewCredit(int cID, int money)
        {
            if (ImitationOn) ActionQueye.Enqueue(Credit(cID, money));
            else 
            {
                Credit(cID, money);
                DB.SaveChanges();
                ClientListChanged?.Invoke();
            }
        }
        private Client Credit(int cID, int money)
        {
            float LR;
            var client = DB.Clients.Single(x => x.ClientID == cID);
            switch (client.Type)
            {
                case "VIP": LR = 9; break;
                case "Entitie": LR = 5; break;
                default: LR = 15; break;
            };
            client.MainAccount += money;
            if (!client.Reliability) client.Credit += (int)(money + (money * (LR / 100)));
            else client.Credit += (int)(money + (money * (LR / 125)));//для надёжных клиентов, ставка по кредиту ниже
            return client;
        }

        public void UpdateClientInfo(Client c)
        {
            if (ImitationOn) ActionQueye.Enqueue(UpdateInfo(c));
            else
            {
                UpdateInfo(c);
                DB.SaveChanges();
            }
        }
        private Client UpdateInfo(Client c)
        {
            var sc = DB.Clients.Single(x => x.ClientID == c.ClientID);
            sc.FullName = c.FullName;
            return sc;
        }

        public void Repayment(int cID)
        {
            if (ImitationOn) ActionQueye.Enqueue(Repay(cID));
            else
            {
                Repay(cID);
                DB.SaveChanges();
                ClientListChanged?.Invoke();
            }
        }
        private Client Repay(int cID)
        {
            var client = DB.Clients.Single(x => x.ClientID == cID);
            if (client.MainAccount >= client.Credit)
            {
                client.MainAccount -= client.Credit;
                client.Credit -= client.Credit;
            }
            return client;
        }

        public void UpdateBankAccount(int cID, int money, bool outside)
        {
            if (ImitationOn) ActionQueye.Enqueue(BAUpdate(cID, money, outside));
            else
            {
                BAUpdate(cID, money, outside);
                DB.SaveChanges();
                ClientListChanged?.Invoke();
            }
        }
        private Client BAUpdate(int cID, int money, bool outside)
        {
            var client = DB.Clients.Single(x => x.ClientID == cID);
            if (!outside)
            {
                if (client.MainAccount >= money)
                {
                    client.MainAccount -= money;
                    client.BankAccount += money;
                }
            }
            else
            {
                if (client.BankAccount >= money)
                {
                    client.MainAccount += money;
                    client.BankAccount -= money;
                }
            }
            return client;
        }

        public Client GetClient(int cID)
        {
            return DB.Clients.Single(x => x.ClientID == cID);
        }
        
        private bool ImitationOn = false;
        public void Imitation(bool on)
        {
            if (on)
            {
                ImitationOn = on;
                imitationThread = new Thread(StartImitaion)
                { Priority = ThreadPriority.BelowNormal, IsBackground = true };
                imitationThread.Start();
            }
            else
            {
                imitationThread.Abort();
                ImitationOn = false;
            }
        }
        private void StartImitaion()
        {
            while (true)
            {
                int ClientsCount = DB.Clients.Local.Count;
                if (ClientsCount > 0)
                {
                    int c1 = r.Next(0, ClientsCount - 1);
                    int c2 = r.Next(0, ClientsCount - 1);
                    int tempM = r.Next(100, 1000);

                    TransferC(c1, c2, tempM);
                }
                Task[] tasks = new Task[DB.Clients.Local.Count];
                for (int i = 0; i < DB.Clients.Local.Count; i++)
                    Update(DB.Clients.Local[i]);

                for (int i = 0; i < ActionQueye?.Count; i++)
                    ActionQueye?.Dequeue();
                DB.SaveChanges();
                LogsListChanged?.Invoke();
                ClientListChanged?.Invoke();
            }
        }
        public void Update(Client client)
        {
            float AIR;
            switch (client.Type)
            {
                case "VIP": AIR = 13; break;
                case "Entitie": AIR = 15; break;
                default: AIR = 9; break;
            };
            if (client.Reliability)
            {
                client.BankAccount += (int)(client.BankAccount * (AIR / 100 / 12));
                client.MainAccount = (int)(client.MainAccount * 1.01);//каддый месяц 1% от остатка
            }//Т.К. в теории оно происходит каждый день, нет смысла делать дополнительные проверки

            if (client.Credit > 0)
            {
                if (client.MainAccount > client.Credit / 10)
                {
                    client.MainAccount = client.Credit / 10;//каждый месяц выплачивается 10% от остатка кредита
                    client.Credit -= client.Credit / 10;
                    if (client.Current > 0) client.Current--;
                    else client.Reliability = true;//клиент становится надёжным, если не просрочил хотя бы один месяц
                }
                if (client.Credit < 100 && client.MainAccount >= 100) { client.MainAccount -= client.Credit; client.Credit = 0; } //последние 100 Рублей снимаются сами, выходя из бесконечного цикла
            }
            if (client.Current >= 5) client.Reliability = false;//если просрочил кредит 5 месяцев к ряду, надёжность пропадает
        }
    }

}
