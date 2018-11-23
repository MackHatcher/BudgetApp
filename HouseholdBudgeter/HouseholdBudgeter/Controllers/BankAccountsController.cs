using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HouseholdBudgeter.Helpers;
using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Classes;
using Microsoft.AspNet.Identity;

namespace HouseholdBudgeter.Controllers
{
    public class BankAccountsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Accounts
        public IQueryable<Accounts> GetAccounts()
        {
            return db.Accounts;
        }

        // GET: api/Accounts/5
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult GetAccounts(int id)
        {
            Accounts accounts = db.Accounts.Find(id);
            if (accounts == null)
            {
                return NotFound();
            }

            return Ok(accounts);
        }

        //GET: api/Accounts/GetBankAccount
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult GetBankAccount(int Id, Accounts bankAccounts)
        {
            if (bankAccounts == null)
            {
                return NotFound();
            }

            var Household = db.Households.Find(Id);
            var BankAccounts = db.Accounts.Where(a => a.Id == Household.Id);

            return Ok(BankAccounts);
        }

        // POST: api/Accounts/PostAccounts
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult PostAccounts(Accounts accounts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Accounts.Add(accounts);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = accounts.Id }, accounts);
        }

        [HttpGet]
        [Route("api/bankaccounts/view/{id}")]
        public IHttpActionResult View(int id)
        {
            var userId = User.Identity.GetUserId();

            var houseHold = db.Households
                .FirstOrDefault(p => p.Id == id);

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                var accounts = houseHold.Accounts;

                var bankAccountsViewModel = accounts
                    .Select(p => new AccountViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Balance = p.Balance,
                        HouseHolds = p.HouseHold.Name,
                        NumberOfTransactions = p.Transactions.Count()
                    }).ToList();

                return Ok(bankAccountsViewModel);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        //POST: api/Accounts/Create
        
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult Create(CreateAccountBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var houseHold = db.Households
                .FirstOrDefault(p => p.Id == model.HouseHoldId);

            if (houseHold == null)
            {
                return BadRequest("HouseHold doesn't exist");
            }

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                var bankAccount = new Accounts();
                bankAccount.Name = model.Name;
                bankAccount.HouseHoldId = model.HouseHoldId;
                bankAccount.Balance = 0;

                db.Accounts.Add(bankAccount);
                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpGet]
        [Route("view/getcategorylist/{id}")]
        public IHttpActionResult ListCategories(int id)
        {
            var categoryList = db.Categories.Where(c => c.HouseholdId == id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToList();

            return Ok(categoryList);
        }
        
        //POST: api/Accounts/BalanceUpdate
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult Balance(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var bankAccount = db.Accounts
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return BadRequest("Bank account doesn't exist");
            }

            var houseHold = bankAccount.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                var balance = bankAccount.Transactions
                    .Where(p => !p.IsVoided)
                    .Sum(p => p.Amount);

                bankAccount.Balance = balance;

                db.SaveChanges();

                return Ok("The new balance is: " + balance);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }
    

        //GET: api/Accounts/Edit
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult GetEditAccounts(int id)
        {
            Accounts bankAccount = db.Accounts.Find(id);
            return Ok(bankAccount);
        }

        //POST: api/Accounts/Edit
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult PostEditAccounts(EditAccountBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var bankAccount = db.Accounts
                .FirstOrDefault(p => p.Id == model.BankAccountId);

            if (bankAccount == null)
            {
                return BadRequest("Bank account doesn't exist");
            }

            var houseHold = bankAccount.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                bankAccount.Name = model.Name;

                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        // POST: api/Accounts/Delete
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult DeleteAccounts(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var bankAccount = db.Accounts
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return BadRequest("Bank account doesn't exist");
            }

            var houseHold = bankAccount.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                db.Accounts.Remove(bankAccount);

                db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountsExists(int id)
        {
            return db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}