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

    [Authorize]
    [RoutePrefix("api/household")]
    public class HouseHoldController : ApiController
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        [Route("create")]
        [HttpPost]
        public IHttpActionResult Create(CreateHouseHoldBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = new HouseHold();
            houseHold.CreatorId = User.Identity.GetUserId();
            houseHold.Name = model.Name;

            _db.Households.Add(houseHold);
            _db.SaveChanges();

            var houseHoldViewModel = new CreateHouseHoldViewModel();
            houseHoldViewModel.Id = houseHold.Id;
            houseHoldViewModel.Name = houseHold.Name;

            return Ok(houseHoldViewModel);
        }

        [HttpPost]
        [Route("invite")]
        public IHttpActionResult Invite(InviteHouseHoldBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            if (!_db.Households.Any(p => p.Id == model.HouseHoldId && p.CreatorId == userId))
            {
                return BadRequest("Household not found");
            }

            var invitedUser = _db.Users.FirstOrDefault(p => p.Email == model.Email);

            if (invitedUser == null)
            {
                return BadRequest("User not found");
            }

            //TODO: Validations
            // Validate if the invite has already been sent.
            // Validate if the user already belongs to the household.
            // Validate if we are not inviting ourself.

            var invite = new HouseHoldInvite();
            invite.InvitedDate = DateTime.Now;
            invite.HouseHoldId = model.HouseHoldId;
            invite.InvitedUserId = invitedUser.Id;

            _db.Invites.Add(invite);
            _db.SaveChanges();

            var emailService = new PersonalEmailService();
            var emailMessage = new System.Net.Mail.MailMessage("mywebapi@budget.com", invitedUser.Email);
            emailMessage.Subject = "Hey, you got a new invite :)";
            emailMessage.Body = "New invite pending, please check the website to accept";
            emailService.Send(emailMessage);

            return Ok("Invite created sucessfully");
        }

        [HttpPost]
        [Route("join")]
        public IHttpActionResult Join(JoinHouseHoldBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invite = _db.Invites.FirstOrDefault(p => p.Id == model.InviteId);

            if (invite == null)
            {
                return BadRequest("Invite not found");
            }

            var userId = User.Identity.GetUserId();

            if (invite.InvitedUserId != userId)
            {
                return BadRequest("Invite not found");
            }

            var houseHold = _db.Households.FirstOrDefault(p => p.Id == invite.HouseHoldId);
            var user = _db.Users.FirstOrDefault(p => p.Id == userId);

            houseHold.Members.Add(user);

            _db.Invites.Remove(invite);

            _db.SaveChanges();

            return Ok("Invite processed sucessfully");
        }

        [HttpGet]
        [Route("view")]
        public IHttpActionResult ViewMembers(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = _db.Households
                .Include(p => p.Members)
                .Include(p => p.Creator)
                .FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();

            if (houseHold.CreatorId != userId && !houseHold.Members.Any(p => p.Id == userId))
            {
                return NotFound();
            }

            var houseHoldViewModel = new HouseHoldViewModel();
            houseHoldViewModel.Name = houseHold.Name;

            houseHoldViewModel.Members.Add(new HouseHoldMembersViewModel
            {
                Name = houseHold.Creator.UserName,
                Email = houseHold.Creator.Email,
                Id = houseHold.Creator.Id,
                IsCreator = true
            });

            foreach (var member in houseHold.Members)
            {
                houseHoldViewModel.Members.Add(new HouseHoldMembersViewModel
                {
                    IsCreator = false,
                    Email = member.Email,
                    Id = member.Id,
                    Name = member.UserName
                });
            }

            return Ok(houseHoldViewModel);
        }

        [HttpPost]
        [Route("leave")]
        public IHttpActionResult Leave(LeaveHouseHoldBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = _db.Households.FirstOrDefault(p => p.Id == model.HouseHoldId);

            if (houseHold == null)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();
            var user = _db.Users.FirstOrDefault(p => p.Id == userId);

            houseHold.Members.Remove(user);
            _db.SaveChanges();

            return Ok("User has been removed from the household");
        }
    }
}