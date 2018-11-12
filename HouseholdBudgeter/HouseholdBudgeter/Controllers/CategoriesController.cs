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

namespace HouseholdBudgeter.Controllers
{
    public class CategoriesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Categories
        public IQueryable<Categories> GetCategories()
        {
            return db.Categories;
        }

        [ResponseType(typeof(Categories))]
        public IHttpActionResult GetCategories(Categories categories)
        {
            var Category = db.Categories.ToList();

            return Ok(Category);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategories(int id, Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categories.Id)
            {
                return BadRequest();
            }

            db.Entry(categories).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriesExists(id))
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

        //POST: api/Categories/Create
        [ResponseType(typeof(Categories))]
        public IHttpActionResult CreateCategories(Categories categories)
        {
            if (ModelState.IsValid)
            {
                categories.HouseholdId = Convert.ToInt32(User.Identity.GetHouseholdId());

                db.Categories.Add(categories);
                db.SaveChanges();
                return Ok(categories);
            }
            return Ok();
        }


        //POST: api/Categories/Edit
        [ResponseType(typeof(Categories))]
        public IHttpActionResult EditCategories(Categories categories)
        {
            if (ModelState.IsValid)
            {
                var original = db.Categories.AsNoTracking().FirstOrDefault(c => c.Id == categories.Id);

                if (categories.Name != original.Name)
                {
                    var transactions = db.Transactions.Where(t => t.Category.Name == original.Name);
                    foreach (var trans in transactions)
                    {
                        trans.Category.Name = categories.Name;
                    }
                }
            }
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(categories);
        }


        // POST: api/Categories
        [ResponseType(typeof(Categories))]
        public IHttpActionResult PostCategories(Categories categories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Categories.Add(categories);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = categories.Id }, categories);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Categories))]
        public IHttpActionResult DeleteCategories(int id)
        {
            Categories categories = db.Categories.Find(id);
            if (categories == null)
            {
                return NotFound();
            }
            var transactions = db.Transactions.Where(b => b.CategoryId == id);
            var misc = db.Categories.FirstOrDefault(c => c.Name == "Miscellaneous").Id;
            foreach (var transaction in transactions) transaction.CategoryId = misc;

            db.Categories.Remove(categories);
            db.SaveChanges();

            return Ok(categories);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoriesExists(int id)
        {
            return db.Categories.Count(e => e.Id == id) > 0;
        }
    }
}