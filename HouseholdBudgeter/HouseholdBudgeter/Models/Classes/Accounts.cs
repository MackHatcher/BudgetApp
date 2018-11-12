using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Accounts
    {
        public Accounts()
        {
            Transactions = new HashSet<Transactions>();
        }

        public int Id { get; set; }

        public int HouseHoldId { get; set; }
        public virtual HouseHold HouseHold { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }
        public decimal ReconBalance { get; set; }

        public ICollection<Transactions> Transactions { get; set; }
    }
}
