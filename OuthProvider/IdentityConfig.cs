//using Microsoft.AspNet.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;

//namespace PlasticFreeOcean.OuthProvider
//{
//    public class ApplicationUserStore : UserStore<User>
//    {
//        public ApplicationUserStore(AspNetIdentityContext context)
//               : base(context)
//        {
//        }
//    }

//    public class ApplicationRoleStore : RoleStore<IdentityRole>
//    {
//        public ApplicationRoleStore(AspNetIdentityContext context)
//            : base(context)
//        {
//        }
//    }

//    public class ApplicationUserManager : UserManager<GsIdentityUser>
//    {
//        private readonly IAspNetIdentityRepository _aspIdentityRepository;


//        public ApplicationUserManager(IUserStore<GsIdentityUser> store, IDataProtectionProvider dataProtectionProvider, IAspNetIdentityRepository aspIdentityRepository)
//         : base(store)
//        {
//            UserValidator = new UserValidator<GsIdentityUser>(this)
//            {
//                AllowOnlyAlphanumericUserNames = false,
//                RequireUniqueEmail = true
//            };

//            // Configure validation logic for passwords
//            PasswordValidator = new PasswordValidator
//            {
//                RequiredLength = 6,
//                RequireNonLetterOrDigit = false,
//                RequireDigit = false,
//                RequireLowercase = false,
//                RequireUppercase = false,
//            };

//            // Configure user lockout defaults
//            UserLockoutEnabledByDefault = Convert.ToBoolean(ConfigurationManager.AppSettings["UserLockoutEnabledByDefault"].ToString());
//            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(Double.Parse(ConfigurationManager.AppSettings["DefaultAccountLockoutTimeSpan"].ToString()));
//            MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString());

//            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
//            // You can write your own provider and plug it in here.
//            //RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
//            //{
//            //    MessageFormat = "Your security code is {0}"
//            //});

//            //RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
//            //{
//            //    Subject = "Security Code",
//            //    BodyFormat = "Your security code is {0}"
//            //});

//            EmailService = new EmailService();
//            //SmsService = new SmsService();

//            UserTokenProvider = new DataProtectorTokenProvider<GsIdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));

//            _aspIdentityRepository = aspIdentityRepository;
//        }

//        public void SetLockoutManually(string userId, bool lockedOut)
//        {
//            _aspIdentityRepository.SetLockout(userId, lockedOut);
//        }

//        public void SuggestResetPasswordByEmailEnabled(string email)
//        {
//            _aspIdentityRepository.UpdateResetPasswordSuggestion(email, true);
//        }

//        public void DeleteUser(string Id)
//        {
//            _aspIdentityRepository.UpdateFlagIsDeleted(Id, true);
//        }

//        public void SuggestResetPasswordByEmailDisabled(string email)
//        {
//            _aspIdentityRepository.UpdateResetPasswordSuggestion(email, false);
//        }
//    }

//    // Configure the application sign-in manager which is used in this application.
//    public class ApplicationSignInManager : SignInManager<GsIdentityUser, string>
//    {
//        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
//            : base(userManager, authenticationManager)
//        {
//        }

//        public override Task<ClaimsIdentity> CreateUserIdentityAsync(GsIdentityUser user)
//        {
//            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
//        }

//        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
//        {
//            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
//        }
//    }

//    public class ApplicationRoleManager : RoleManager<IdentityRole>
//    {
//        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore)
//        {
//        }
//    }
//}