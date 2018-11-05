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
using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.Classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace HouseholdBudgeter.Controllers
{
    public class HouseholdsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: api/Households
        public IQueryable<Households> GetHouseholds()
        {
            return db.Households;
        }

        // GET: api/Households/5
        [ResponseType(typeof(Households))]
        public IHttpActionResult GetHouseholds(int id)
        {
            Households households = db.Households.Find(id);
            if (households == null)
            {
                return NotFound();
            }

            return Ok(households);
        }

        // PUT: api/Households/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHouseholds(int id, Households households)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != households.Id)
            {
                return BadRequest();
            }

            db.Entry(households).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseholdsExists(id))
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

        // POST: api/Households
        [Authorize]
        [ResponseType(typeof(Households))]
        public IHttpActionResult PostHouseholds(Households households)
        {
            var creatorName = User.Identity.Name;
            var creatorId = User.Identity.GetUserId();

            db.Households.Add(households);
            
            households.CreatorId = creatorId;

            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = households.Id }, households);
        }

        //POST api/Households/ViewUsers
        [HttpGet]
        [ResponseType(typeof(Households))]
        public IHttpActionResult ViewResidents(int Id)
        {
            
            var Household = db.Households.FirstOrDefault(u => u.Id == Id);
            
            return Ok(Household.Users.ToList());
        }

        //POST api/Households/InviteToHousehold
        [ResponseType(typeof(Households))]
        public async System.Threading.Tasks.Task<IHttpActionResult> InviteToHousehold(HouseholdInvites Invite)
        {
            
            var CreatorHousehold = db.Households.FirstOrDefault(u => u.Id == Invite.Id);
            
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            Households household = db.Households.Find(user.HouseholdId);

            if (userId == CreatorHousehold.CreatorId)
            {
            
                var InvitedUser = db.Users.FirstOrDefault(u => u.Email == Invite.Email);
                var userEmail = await UserManager.FindByNameAsync(Invite.Email);
                await UserManager.SendEmailAsync(userEmail.Id, "Household Invite", "You have been invited to a new household. Please log in to join now.");
                db.Invites.Add(Invite);
                db.SaveChanges();
            }
            
            return Ok(household);
        }
        
        //POST api/Households/RemoveInvite
        [Authorize]
        [ResponseType(typeof(Households))]
        public IHttpActionResult RemoveInvite(int inviteId)
        {
            HouseholdInvites invite = db.Invites.Find(inviteId);
            db.Invites.Remove(invite);
            db.SaveChanges();
            return Ok();
        }

        //POST api/Households/JoinHousehold
        [Authorize]
        [ResponseType(typeof(Households))]
        public IHttpActionResult JoinHousehold(int inviteId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            user.HouseholdId = db.Invites.Find(inviteId).HouseHoldId;
            HouseholdInvites invite = db.Invites.Find(inviteId);
            db.Invites.Remove(invite);

            db.SaveChanges();
            return Ok();
        }
        
        //POST api/Households/LeaveHousehold
        [ResponseType(typeof(Households))]
        public IHttpActionResult LeaveHousehold(HouseholdInvites Invite)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            user.HouseholdId = null;
                        
            db.SaveChanges();
            return Ok();
        }

        // DELETE: api/Households/5
        [ResponseType(typeof(Households))]
        public IHttpActionResult DeleteHouseholds(int id)
        {
            Households households = db.Households.Find(id);
            if (households == null)
            {
                return NotFound();
            }

            db.Households.Remove(households);
            db.SaveChanges();

            return Ok(households);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HouseholdsExists(int id)
        {
            return db.Households.Count(e => e.Id == id) > 0;
        }
    }
}