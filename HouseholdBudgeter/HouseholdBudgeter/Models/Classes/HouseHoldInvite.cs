using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class HouseHoldInvite
    {
        public int Id { get; set; }

        public int HouseHoldId { get; set; }
        public virtual HouseHold HouseHold { get; set; }

        public string InvitedUserId { get; set; }
        public virtual ApplicationUser InvitedUser { get; set; }

        public DateTime InvitedDate { get; set; }
    }
}