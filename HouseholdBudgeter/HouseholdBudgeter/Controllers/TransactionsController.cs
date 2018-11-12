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
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Transactions
        public IQueryable<Transactions> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult GetTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            var Category = db.Categories.Where(c => c.Id == transactions.Id);
            if (transactions == null)
            {
                return NotFound();
            }

            return Ok(Category);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTransactions(int id, Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transactions.Id)
            {
                return BadRequest();
            }

            db.Entry(transactions).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionsExists(id))
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

        // POST: api/Transactions
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult PostTransactions(Transactions transactions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Transactions.Add(transactions);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = transactions.Id }, transactions);
        }

        //POST: api/Transactions/Create
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult CreateTransactions(TransactionsBindingModel transactions)
        {
            var id = User.Identity.GetUserId();
            var newTransaction = new Transactions();
            var test = db.Accounts.FirstOrDefault(e => e.Id == transactions.AccountId);
            var household = newTransaction.Account.HouseHold;
            HelperMethods helpers = new HelperMethods();

            if (ModelState.IsValid)
            {
                var account = db.Accounts.FirstOrDefault(a => a.Id == transactions.AccountId);

                if (newTransaction.CategoryId == null)
                {
                    transactions.CategoryId = household.Categories.FirstOrDefault(c => c.Name == "Miscellaneous").Id;
                }

                account.Balance = helpers.GetAccountBalance(newTransaction);
                db.Entry(transactions).State = EntityState.Modified;
                db.Transactions.Add(newTransaction);
                db.SaveChanges();
                
                return Ok();
            }
            return Ok();
        }

        //POST: api/Transactions/Edit
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult EditTransactions(TransactionsBindingModel transactions)
        {
            var id = User.Identity.GetUserId();
            var newTransaction = new Transactions();
            HelperMethods helpers = new HelperMethods();
            var test = db.Accounts.FirstOrDefault(e => e.Id == transactions.AccountId);
            var household = newTransaction.Account.HouseHold;

            if (ModelState.IsValid)
            {
                var original = db.Transactions.FirstOrDefault(t => t.Id == transactions.Id);
                var account = db.Accounts.FirstOrDefault(a => a.Id == original.AccountId);

                account.Balance = helpers.RevertAccountBalance(original);
                account = db.Accounts.FirstOrDefault(a => a.Id == original.AccountId);
                account.Balance = helpers.GetAccountBalance(newTransaction);

                db.Entry(transactions).State = EntityState.Modified;
                db.SaveChanges();

                return Ok();
            }
            return Ok();
        }

        //POST: api/Transactions/View
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult ViewTransactions(int id)
        {
            var Transaction = db.Transactions.ToList();

            return Ok(Transaction);
        }

        //POST: api/Transactions/Void
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult VoidTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            var userId = User.Identity.GetUserId();
            if (transactions == null)
            {
                return NotFound();
            }
            transactions.IsVoided = true;
            db.SaveChanges();

            return Ok(transactions);
        }


        // POST: api/Transactions/DELETE
        [ResponseType(typeof(Transactions))]
        public IHttpActionResult DeleteTransactions(int id)
        {
            Transactions transactions = db.Transactions.Find(id);
            var userId = User.Identity.GetUserId();
            var account = db.Accounts.FirstOrDefault(a => a.Id == transactions.AccountId);
            HelperMethods helpers = new HelperMethods();
            if (transactions == null)
            {
                return NotFound();
            }
            account.Balance = helpers.RevertAccountBalance(transactions);

            db.Transactions.Remove(transactions);
            db.SaveChanges();

            return Ok(transactions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionsExists(int id)
        {
            return db.Transactions.Count(e => e.Id == id) > 0;
        }
    }
}