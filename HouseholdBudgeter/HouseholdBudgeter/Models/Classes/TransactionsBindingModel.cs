using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class TransactionsBindingModel
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
       
        public int CategoryId { get; set; }
        public string Description { get; set; }

        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int Income { get; set; }
        
    }
}