using System;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OpenIdConnect.Primitives;
using System.Threading.Tasks;
using PlasticFreeOcean.Models;
using Microsoft.AspNetCore.Identity;
using PlasticFreeOcean.Services;
using PlasticFreeOcean.Helper;
using Microsoft.Extensions.Configuration;

namespace PlasticFreeOcean.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        private readonly PlasticFreeOceanContext _context;
        private readonly IAccountHelper _accountHelper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthorizationController(PlasticFreeOceanContext context, IConfiguration config, UserManager<User> userManager, IAccountHelper accountHelper)
        {
            _context = context;
            _accountHelper = accountHelper;
            _userManager = userManager;
            _config = config;
        }
        [HttpPost("~/connect/{provider}"), Produces("application/json")]
        public async Task<IActionResult> ExternalGoogleLogin([FromBody]RegisterExternalBindingModel data, [FromRoute]string provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(data.ExternalAccessToken, "provider");

            if (!verifiedAccessToken)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }
            var findAccount = await _userManager.FindByEmailAsync(data.Email);
            User user = await GetOrCreateUser(data);

            var token = _accountHelper.GenerateToken(user);

            return Ok(MessageHelper.Success<object>(new { token }));
        }

            
        //[HttpPost("~/connect/authorize"), Produces("application/json")]
        //public async Task<IActionResult> ExternalLogin([FromBody]RegisterExternalBindingModel data)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var verifiedAccessToken = await VerifyExternalAccessToken(data.ExternalAccessToken);

        //    if (verifiedAccessToken == null)
        //    {
        //        return BadRequest("Invalid Provider or External Access Token");
        //    }
        //    var findAccount = await _userManager.FindByEmailAsync(data.Email);
        //    User user = await GetOrCreateUser(data);

        //    var token = _accountHelper.GenerateToken(user);

        //    return Ok(MessageHelper.Success<object>(new { token }));

        //}
        private async Task<User> GetOrCreateUser([FromBody]RegisterExternalBindingModel data)
        {
            var findAccount = await _userManager.FindByEmailAsync(data.Email);
            User user = null;
            if (findAccount != null)
            {
                user = findAccount;
            }
            else
            {
                var newAccount = new NewUser
                {
                    Email = data.Email,
                    Password = "ZXasqw12",
                    UserName = data.Email.Split("@")[0]
                };
                var newUser = await _accountHelper.Create(newAccount);
                user = newUser;
            }
            return user;
        }
        private async Task<bool> VerifyExternalAccessToken(string accessToken, string provider)
        {
            var verifyTokenEndPoint = "";
            if(provider.ToLower() == "facebook"){
                var fbAppSecredId = _config["FacebookToken:AppID"] + '|' + _config["FacebookToken:AppSecret"];
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/v2.10/debug_token?input_token={0}&access_token={1}", accessToken, fbAppSecredId);
            }
            else{
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            return response.IsSuccessStatusCode;
        }

        public class RegisterExternalBindingModel
        {
            public string ExternalAccessToken { get; set; }
            public string Email { get; set; }

        }
        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return BadRequest();
            }

            if (!result.Succeeded)
            {

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
