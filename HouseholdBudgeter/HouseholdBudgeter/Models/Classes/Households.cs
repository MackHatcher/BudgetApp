using HouseholdBudgeter.Models.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Households
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public ApplicationUser Creator { get; set; }

        public Households()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Invites = new HashSet<HouseholdInvites>();
        }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        [InverseProperty("HouseholdInvites")]
        public virtual ICollection<HouseholdInvites> Invites { get; set; }
    }
    
}