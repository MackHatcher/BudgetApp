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
    public class CategoriesController : ApiController
    {
        [Authorize]
        [RoutePrefix("api/category")]
        public class CategoryController : ApiController
        {
            private ApplicationDbContext _db = new ApplicationDbContext();

            [HttpPost]
            [Route("create")]
            public IHttpActionResult Create(CreateCategoryBindingModel model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.Identity.GetUserId();

                var houseHold = _db.Households
                    .FirstOrDefault(p => p.Id == model.HouseHoldId);

                if (houseHold == null)
                {
                    return BadRequest("HouseHold doesn't exist");
                }

                if (houseHold.CreatorId == userId ||
                    houseHold.Members.Any(p => p.Id == userId))
                {
                    var category = new Categories();
                    category.Name = model.Name;
                    category.HouseholdId = model.HouseHoldId;

                    _db.Categories.Add(category);
                    _db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return BadRequest("Not authorized");
                }

            }

            [HttpPost]
            [Route("edit")]
            public IHttpActionResult Edit(EditCategoryBindingModel model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.Identity.GetUserId();

                var category = _db.Categories
                    .FirstOrDefault(p => p.Id == model.CategoryId);

                if (category == null)
                {
                    return BadRequest("Category doesn't exist");
                }

                var houseHold = category.Household;

                if (houseHold.CreatorId == userId ||
                    houseHold.Members.Any(p => p.Id == userId))
                {
                    category.Name = model.Name;

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

                var category = _db.Categories
                    .FirstOrDefault(p => p.Id == id);

                if (category == null)
                {
                    return BadRequest("Category doesn't exist");
                }

                var houseHold = category.Household;

                if (houseHold.CreatorId == userId ||
                    houseHold.Members.Any(p => p.Id == userId))
                {
                    _db.Categories.Remove(category);

                    _db.SaveChanges();

                    return Ok();
                }
                else
                {
                    return BadRequest("Not authorized");
                }
            }

            [HttpGet]
            [Route("view/{id}")]
            public IHttpActionResult View(int id)
            {
                var userId = User.Identity.GetUserId();

                var houseHold = _db.Households
                    .FirstOrDefault(p => p.Id == id);

                if (houseHold.CreatorId == userId ||
                    houseHold.Members.Any(p => p.Id == userId))
                {
                    var categories = houseHold.Categories;

                    var categoryViewModel = categories
                        .Select(p => new CategoryViewModel
                        {
                            Id = p.Id,
                            Name = p.Name
                        }).ToList();

                    return Ok(categoryViewModel);
                }
                else
                {
                    return BadRequest("Not authorized");
                }
            }
        }
    }
}
    