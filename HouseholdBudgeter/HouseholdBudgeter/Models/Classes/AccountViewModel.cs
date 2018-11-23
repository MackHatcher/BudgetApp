using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string HouseHolds { get; set; }
        public int NumberOfTransactions { get; set; }
    }
}