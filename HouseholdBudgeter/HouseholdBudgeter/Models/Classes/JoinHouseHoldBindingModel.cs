using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class JoinHouseHoldBindingModel
    {
        [Required]
        public int InviteId { get; set; }
    }
}