using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using HouseholdBudgeter.Models.Classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace HouseholdBudgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        
        public ApplicationUser()
        {
            Transactions = new HashSet<Transactions>();
            CreatedHouseholds = new HashSet<HouseHold>();
            Households = new HashSet<HouseHold>();
        }

        [InverseProperty("Members")]
        public virtual ICollection<HouseHold> Households { get; set; }

        [InverseProperty("Creator")]
        public virtual ICollection<HouseHold> CreatedHouseholds { get; set; }

        public virtual ICollection<Transactions> Transactions { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        internal Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager userManager)
        {
            throw new NotImplementedException();
        }
        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Transactions>()
                    .HasRequired(p => p.Category)
                    .WithMany(p => p.Transactions)
                    .WillCascadeOnDelete(false);
            }
        

        public DbSet<HouseHold> Households { get; set; }
        public DbSet<HouseHoldInvite> Invites { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Categories> Categories { get; set; }
    }
}