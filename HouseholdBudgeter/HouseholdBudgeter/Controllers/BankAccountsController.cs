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

        // PUT: api/Accounts/PutAccounts
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult PutAccounts(int id, Accounts accounts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accounts.Id)
            {
                return BadRequest();
            }

            db.Entry(accounts).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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

        //POST: api/Accounts/Create
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult CreateAccounts(Accounts bankAccount)
        {
            if (ModelState.IsValid)
            {
                var houseId = Convert.ToInt32(User.Identity.GetHouseholdId());
                var hh = db.Households.Find(houseId);

                bankAccount.HouseHoldId = Convert.ToInt32(User.Identity.GetHouseholdId());
                db.Accounts.Add(bankAccount);
                db.SaveChanges();

                Transactions originalTransaction = new Transactions()
                {
                    AccountId = bankAccount.Id,
                    Category = hh.Categories.FirstOrDefault((m => m.Name == "Miscellaneous")),
                    Amount = bankAccount.Balance,
                    IsVoided = false
                    
                };

                db.Transactions.Add(originalTransaction);
                db.SaveChanges();

                return Ok(bankAccount);
            }
            return Ok();

        }

        //POST: api/Accounts/BalanceUpdate
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult Balance(Accounts bankAccount)
        {
            bankAccount.HouseHoldId = Convert.ToInt32(User.Identity.GetHouseholdId());

            var trans = from t in db.Transactions
                        where t.AccountId == bankAccount.Id
                        where t.IsVoided == false
                        select new { t.Amount, t.ReconAmount };
            bankAccount.Balance = trans.ToList().Select(s => s.Amount).Sum();
            bankAccount.ReconBalance = trans.ToList().Select(s => s.ReconAmount).Sum();

            if (ModelState.IsValid)
            {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(bankAccount);
            }

            return Ok(bankAccount);
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
        public IHttpActionResult PostEditAccounts(Accounts bankAccount)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Attach(bankAccount);
                db.Entry(bankAccount).Property("Name").IsModified = true;
                db.SaveChanges();
                return Ok(bankAccount);
            }
            return Ok(bankAccount);
        }

        // POST: api/Accounts/Delete
        [ResponseType(typeof(Accounts))]
        public IHttpActionResult DeleteAccounts(int id)
        {
            Accounts bankAccount = db.Accounts.Find(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(bankAccount);
            db.SaveChanges();

            return Ok(bankAccount);
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