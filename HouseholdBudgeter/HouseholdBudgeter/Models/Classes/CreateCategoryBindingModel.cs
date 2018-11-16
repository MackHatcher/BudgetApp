using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class CreateCategoryBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int HouseHoldId { get; set; }
    }
}