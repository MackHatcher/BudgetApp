using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class HouseholdInvites
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        public int InvitedUserId { get; set; }
        public string Email { get; set; }
        

        [InverseProperty("Households")]
        public virtual Households Household { get; set; }
    }
}