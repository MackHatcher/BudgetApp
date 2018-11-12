using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace HouseholdBudgeter.Helpers
{
    public static class HelperExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        
        public static string GetHouseholdId(this IIdentity user)
        {
            var ClaimUser = (ClaimsIdentity)user;
            var Claim = ClaimUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (Claim != null)
                return Claim.Value;
            else
                return null;
        }
    }
}