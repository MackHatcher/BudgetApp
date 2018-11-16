namespace HouseholdBudgeter.Migrations
{
    using HouseholdBudgeter.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HouseholdBudgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(HouseholdBudgeter.Models.ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            ApplicationUser newUser = null;

            if (!context.Users.Any(p => p.UserName == "user@mybudgetapp.com"))
            {
                newUser = new ApplicationUser();
                newUser.UserName = "user@mybudgetapp.com";
                newUser.Email = "user@mybudgetapp.com";
                userManager.Create(newUser, "Password-1");
            }
            else
            {
                newUser = context.Users.Where(p => p.UserName == "user@mybudgetapp.com")
                    .FirstOrDefault();
            }

            if (!context.Users.Any(p => p.UserName == "admin@mybudgetapp.com"))
            {
                newUser = new ApplicationUser();
                newUser.UserName = "admin@mybudgetapp.com";
                newUser.Email = "admin@mybudgetapp.com";
                userManager.Create(newUser, "Password-1");
            }
            else
            {
                newUser = context.Users.Where(p => p.UserName == "admin@mybudgetapp.com")
                    .FirstOrDefault();
            }

        }
    }
}
