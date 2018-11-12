using HouseholdBudgeter.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class HouseHold
    {
        public HouseHold()
        {
            Members = new HashSet<ApplicationUser>();
            Categories = new HashSet<Categories>();
            Accounts = new HashSet<Accounts>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<Categories> Categories { get; set; }
        public virtual ICollection<Accounts> Accounts { get; set; }
    }
}