using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int HouseholdId { get; set; }
        public virtual HouseHold Household { get; set; }

        public Categories()
        {
            this.Transactions = new HashSet<Transactions>();
        }

        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}