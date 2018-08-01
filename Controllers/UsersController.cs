using Microsoft.AspNetCore.Mvc;
using PlasticFreeOcean.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PlasticFreeOcean.IdentityModel;
using PlasticFreeOcean.Services;
using PlasticFreeOcean.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace PlasticFreeOcean.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork<PlasticFreeOceanContext> _unitOfWork;
        private readonly PlasticFreeOceanContext _context;
        private readonly IAccountHelper _accountHelper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public UsersController(PlasticFreeOceanContext context,UserManager<User> userManager, IEmailService emailService, IAccountHelper accountHelper, IUnitOfWork<PlasticFreeOceanContext> unitOfWork)
        {
            _context = context;
            _accountHelper = accountHelper;
            _userManager = userManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginUser = new LoginUser
                {
                    UserName = model.UserName,
                    Password = model.Password,

                };  

                var token = await _accountHelper.SignIn(loginUser);
                return Ok(MessageHelper.Success<object>(new { token }));
            }
            return BadRequest(ModelState);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AccountRegisterViewModel model)
        {
           // _accountHelper.CheckAndCreateRole(string roleName)
            if (ModelState.IsValid)
            {
                var newAccount = new NewUser
                {
                    Email = model.Email,
                    Password = model.Password
                };
                var account = await _accountHelper.Create(newAccount);
                if(account != null){
                    string body = _emailService.CreateEmailBodyKonfirmasi(model.Email.Split("@")[0], "this nuts");
                    await _emailService.SendEmail(account.Email, "Account Confirmation", body);
                }
                return Ok(MessageHelper.Success("Akun berhasil dibuat."));

            }
            return BadRequest(ModelState);

        }

        //[Route("Logout")]
        //[HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public IActionResult Logout([FromBody]string token)
        //{
            
        //    return Ok(MessageHelper.Success("Token sudah dihapus"));
        //}
        //[Route("Profile")]
        //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var name = User.Claims.FirstOrDefault(c => c.Type == "role");
        //    return Ok(name);
        //}
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var account = await _accountHelper.GetUser(User);
            var userViewModel = new UserViewModel()
            {
                user = account,
                RoleName = await _accountHelper.GetRole(account)
            };
            if(account == null)
            {
                throw new ApplicationException("Akun tidak ditemukan.");
            }
            return Ok(MessageHelper.Success(userViewModel));
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if(user == null){
                throw new ApplicationException("Akun dengan email tersebut tidak ditemukan.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendEmail(Email, "Forgot Password", "To reset your password please go to this link");

            return Ok(MessageHelper.Success<object>(new { Message= "Password berhasil direset, silahkan cek email anda." }));

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetEmailModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                throw new ApplicationException("Akun dengan email tersebut tidak ditemukan.");
            }
            var resetPassword = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if(resetPassword.Succeeded){
                return Ok(MessageHelper.Success<object>(new { Message = "Password berhasil direset, silahkan cek email anda." }));
            }
            return BadRequest(ModelState);
        }


    }

}
      

