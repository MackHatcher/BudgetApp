using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class EditAccountBindingModel
    {

        [Required]
        public int BankAccountId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}