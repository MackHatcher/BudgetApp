using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Classes
{
    public class StandardCategories
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}