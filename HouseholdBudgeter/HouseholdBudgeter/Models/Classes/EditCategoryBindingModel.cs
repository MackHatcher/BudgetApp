using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class EditCategoryBindingModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}