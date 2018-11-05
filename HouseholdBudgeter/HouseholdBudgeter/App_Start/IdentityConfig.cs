using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using HouseholdBudgeter.Models;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net;
using System;
using static HouseholdBudgeter.ApplicationUserManager;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace HouseholdBudgeter
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {

            var personalEmailService = new PersonalEmail();

            var mailMessage = new MailMessage(
                WebConfigurationManager.AppSettings["emailto"],
                message.Destination
                );

            mailMessage.Body = message.Body;
            mailMessage.Subject = message.Subject;
            mailMessage.IsBodyHtml = true;

            return personalEmailService.SendAsync(mailMessage);

        }
    }
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            manager.EmailService = new EmailService();
            return manager;
        }
        public class PersonalEmail
        {
            public async Task SendAsync(MailMessage message)
            {
                var username = WebConfigurationManager.AppSettings["username"];
                var password = WebConfigurationManager.AppSettings["password"];
                var host = WebConfigurationManager.AppSettings["host"];
                int port = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);


                using (var smtp = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                })
                {
                    try
                    {
                        await smtp.SendMailAsync(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await Task.FromResult(0);

                    }
                };
            }
            public void Send(MailMessage message)
            {
                var username = WebConfigurationManager.AppSettings["username"];
                var password = WebConfigurationManager.AppSettings["password"];
                var host = WebConfigurationManager.AppSettings["host"];
                int port = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);


                using (var smtp = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                })
                {
                    try
                    {
                        smtp.Send(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);


                    }
                };
            }
        }
    }
}
