using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Helpers
{
    public class HelperMethods
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //get a household
        public HouseHold GetHousehold(string userId, int householdId)
        {
            var household = db.Households.FirstOrDefault(h => h.Id == householdId && userId == h.CreatorId || h.Members.Any(u => u.Id == userId));

            if (household == null)
            {
                return null;
            }

            return household;
        }

        //add categories
        public List<Categories> AddStandardCategories(List<StandardCategories> categories, HouseHold household)
        {
            var CategoryList = new List<Categories>();
            foreach (var cat in categories)
            {
                var newCategory = new Categories()
                {
                    Name = cat.Name,
                    HouseholdId = household.Id
                };
                CategoryList.Add(newCategory);
            }
            return CategoryList;

        }

        //get an account balance
        public decimal GetAccountBalance(Transactions transaction)
        {
            return ModifyAccountBalance(transaction, false);
        }

        //revert an account balance
        public decimal RevertAccountBalance(Transactions transaction)
        {
            return ModifyAccountBalance(transaction, true);
        }

        //modify account balance
        private decimal ModifyAccountBalance(Transactions transaction, bool Delete)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Id == transaction.AccountId);
            bool AddMoney = false;

            if (AddMoney == true)
            {
                account.Balance += transaction.Amount;
            }
            else
            {
                account.Balance -= transaction.Amount;
            }

            return account.Balance;

        }
    }
}