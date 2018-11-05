using HouseholdBudgeter.Models.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static HouseholdBudgeter.ApplicationUserManager;

namespace HouseholdBudgeter.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public async Task<ActionResult> Contact(Email model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>{ 1})</ p >< p > Message:</ p >< p >{ 2}</ p > ";
                    var from = "<example@email.com>";
                    model.Body = "You have requested a password reset.";

                    var email = new MailMessage(from,
                                ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Recovered Password",
                        Body = string.Format(body, model.FromName, model.FromEmail,
                                             model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return View(new Email());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

    }
}
