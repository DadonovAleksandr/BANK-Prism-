using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Client
    {
        public Client() { }
        public Client(string Type, string FullName, int MainAccount, string Adress, string PhoneNumber)
        {
            this.Type = Type;
            this.FullName = FullName;
            this.MainAccount = MainAccount;
            this.Adress = Adress;
            this.PhoneNumber = PhoneNumber;
            this.Reliability = Type == "VIP";
        }
        
        public int ClientID { get; set; }
        public string Type { get; set; }
        public string FullName { get; set; }
        public int MainAccount { get; set; }
        public string Adress { get; set; }
        public int BankAccount { get; set; }
        public bool Reliability { get; set; }
        public int Credit { get; set; }
        public string PhoneNumber { get; set; }
        public int Current { get; set; }
    }
}
