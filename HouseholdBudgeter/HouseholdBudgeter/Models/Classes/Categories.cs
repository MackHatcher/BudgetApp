using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HouseHold_Id { get; set; }
    }
}