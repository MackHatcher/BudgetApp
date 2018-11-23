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

    [Authorize]
    [RoutePrefix("api/transactions")]
    public class TransactionsController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [HttpPost]
        [Route("create")]
        public IHttpActionResult Create(CreateTransactionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var account = _db.Accounts
                .FirstOrDefault(p => p.Id == model.AccountId);

            if (account == null)
            {
                return BadRequest("Account doesn't exist");
            }

            var houseHold = account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                var transaction = new Transactions();
                transaction.AccountId = model.AccountId;
                transaction.Description = model.Description;
                transaction.Date = model.Date;
                transaction.Amount = model.Amount;
                transaction.CategoryId = model.CategoryId;
                transaction.IsVoided = false;
                transaction.EnteredById = userId;

                account.Balance += transaction.Amount;


                _db.Transactions.Add(transaction);
                _db.SaveChanges();

                return Ok();
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest("Not authorized");
            }

        }

        

        [HttpPost]
        [Route("edit")]
        public IHttpActionResult Edit(EditTransactionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var transaction = _db.Transactions
                .FirstOrDefault(p => p.Id == model.TransactionId);

            if (transaction == null)
            {
                return BadRequest("Transaction doesn't exist");
            }

            var houseHold = transaction.Account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                transaction.Account.Balance -= transaction.Amount;

                transaction.Description = model.Description;
                transaction.Date = model.Date;
                transaction.Amount = model.Amount;
                transaction.CategoryId = model.CategoryId;

                transaction.Account.Balance += transaction.Amount;

                _db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpPost]
        [Route("delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var transaction = _db.Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return BadRequest("Category doesn't exist");
            }

            var houseHold = transaction.Account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                transaction.Account.Balance -= transaction.Amount;

                _db.Transactions.Remove(transaction);

                _db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }
        [HttpGet]
        [Route("view/getaccountlist/{id}")]
        public IHttpActionResult ListAccounts(int id)
        {
            var accountList = _db.Accounts.Where(a => a.HouseHoldId == id)
                .Select(c => new AccountViewModel
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToList();

            return Ok(accountList);
        }
        [HttpGet]
        [Route("view/getcategorylist/{id}")]
        public IHttpActionResult ListCategories(int id)
        {
            var categoryList = _db.Categories.Where(c => c.HouseholdId == id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name

                }).ToList();

            return Ok(categoryList);
        }

        [HttpGet]
        [Route("view/{id}")]
        public IHttpActionResult View(int id)
        {
            var userId = User.Identity.GetUserId();

            var transactions = _db.Transactions.Where(p => p.Account.HouseHoldId == id).ToList();
            
            var houseHold = _db.Households.FirstOrDefault(h => h.Id == id);

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                
                var transactionViewModel = transactions
                    .Select(p => new TransactionViewModel
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        CategoryId = p.Category.Id,
                        CategoryName = p.Category.Name,
                        Date = p.Date,
                        Description = p.Description,
                        EnteredById = p.EnteredById,
                        EnteredByName = p.EnteredBy.UserName,
                        IsVoided = p.IsVoided
                    }).ToList();

                return Ok(transactionViewModel);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpGet]
        [Route("viewaccounts/{id}")]
        public IHttpActionResult ViewCategory(int id, int categoryId)
        {
            var userId = User.Identity.GetUserId();

            var transactions = _db.Transactions.Where(p => p.CategoryId == categoryId).ToList();

            var houseHold = _db.Households.FirstOrDefault(h => h.Id == id);

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {

                var transactionViewModel = transactions
                    .Select(p => new TransactionViewModel
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        CategoryId = p.Category.Id,
                        CategoryName = p.Category.Name,
                        Date = p.Date,
                        Description = p.Description,
                        EnteredById = p.EnteredById,
                        EnteredByName = p.EnteredBy.UserName,
                        IsVoided = p.IsVoided
                    }).ToList();

                return Ok(transactionViewModel);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpGet]
        [Route("viewcategory/{id}")]
        public IHttpActionResult ViewAccounts(int id, int accountId)
        {
            var userId = User.Identity.GetUserId();

            var transactions = _db.Transactions.Where(p => p.AccountId == accountId).ToList();

            var houseHold = _db.Households.FirstOrDefault(h => h.Id == id);

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {

                var transactionViewModel = transactions
                    .Select(p => new TransactionViewModel
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        CategoryId = p.Category.Id,
                        CategoryName = p.Category.Name,
                        Date = p.Date,
                        Description = p.Description,
                        EnteredById = p.EnteredById,
                        EnteredByName = p.EnteredBy.UserName,
                        IsVoided = p.IsVoided
                    }).ToList();

                return Ok(transactionViewModel);
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }

        [HttpPost]
        [Route("void/{id}")]
        public IHttpActionResult Void(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var transaction = _db.Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return BadRequest("Transaction doesn't exist");
            }

            var houseHold = transaction.Account.HouseHold;

            if (houseHold.CreatorId == userId ||
                houseHold.Members.Any(p => p.Id == userId))
            {
                transaction.Account.Balance -= transaction.Amount;
                transaction.IsVoided = true;

                _db.SaveChanges();

                return Ok();
            }
            else
            {
                return BadRequest("Not authorized");
            }
        }
    }

}