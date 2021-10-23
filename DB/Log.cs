using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Log
    {
        public Log() { }
        public Log(DateTime Date, decimal MoneyAmount, int SenderID, int RecipientID, bool Successful)
        {
            this.Date = Date;
            this.MoneyAmount = MoneyAmount;
            this.SenderID = SenderID;
            this.RecipientID = RecipientID;
            this.Successful = Successful;
        }
        public int LogID { get; set; }
        public DateTime Date { get; set; }
        public decimal MoneyAmount { get; set; }
        public int SenderID { get; set; }
        public int RecipientID { get; set; }
        public bool Successful { get; set; }
        
    }
}
