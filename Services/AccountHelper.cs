using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PlasticFreeOcean.IdentityModel;
using PlasticFreeOcean.Models;


namespace PlasticFreeOcean.Services
{
    public class AccountHelper : IAccountHelper
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountHelper> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _accountRepository;
        private readonly IRoleHelper _roleHelper;
        private readonly IDeviceService _deviceService;

        public AccountHelper(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            IOptions<IdentityOptions> identityOptions,
            ILogger<AccountHelper> logger, 
            IUnitOfWork<PlasticFreeOceanContext> unitOfWork, 
            IRoleHelper roleHelper,
            IDeviceService deviceService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _identityOptions = identityOptions;
            _accountRepository = unitOfWork.GetRepository<User>();
            _roleHelper = roleHelper;
            _deviceService = deviceService;

        }
        public async Task<string> SignIn(LoginUser loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.UserName);
            if (user == null)
                user = await _userManager.FindByEmailAsync(loginUser.UserName);

            if (user != null)
            {
                //if (!user.PhoneNumberConfirmed)
                //{
                //    throw new ApplicationException("No Handphone belum di konfirmasi.");
                //}
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
                _deviceService.Create(loginUser.DeviceId, user.Id);
               
                if (result.Succeeded)
                {

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, loginUser.UserName)
                    };

                    var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["TokenAuthentication:SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_config["TokenAuthentication:Issuer"],
                        _config["TokenAuthentication:Issuer"],
                        claims,
                        expires: DateTime.Now.AddYears(1),
                        signingCredentials: creds);
                    _logger.LogInformation("Account Sign In");
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                else
                {
                    throw new ApplicationException("Username atau password salah.");
                }
            }
            throw new ApplicationException("Username tidak terdaftar.");
        }

        public async Task<User> Create(NewUser newUser)
        {
            var findAccount = await _userManager.FindByEmailAsync(newUser.Email);
            await _roleHelper.CheckAndCreateRoleAsync("Researcher");
            if (findAccount == null)
            {
                var model = new User
                {
                    Email = newUser.Email,
                    Id = Guid.NewGuid(),
                    UserName = newUser.Email.Split("@")[0]
                };
                var createdAccount = await _userManager.CreateAsync(model, newUser.Password);

                if (!createdAccount.Succeeded)
                {

                    var exceptionText = createdAccount.Errors.Aggregate(
                        "User Creation Failed - Identity Exception. Errors were: \n\r\n\r",
                        (current, error) => current + (" - " + error + "\n\r"));
                    throw new ApplicationException(exceptionText);
                }
                await _userManager.AddToRoleAsync(model, "Researcher");
                return model;
            }
            else
            {
                throw new ApplicationException("Alamat email atau username sudah terdaftar");
            }
        }

        public async Task<AuthenticationTicket> CreateTicketAsync(User user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var ticket = new AuthenticationTicket(
                    principal,
                  OpenIdConnectServerDefaults.AuthenticationScheme);
            ticket.SetScopes(
                    /* email: */ OpenIdConnectConstants.Scopes.Email,
                    /* profile: */ OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess
                );
            ticket.SetResources("resource_server");


            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };
                //if ((claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile)) ||
                //    (claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email)) ||
                //    (claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles)))
                //{
                //    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                //}

                claim.SetDestinations(destinations);
            }

            return ticket;
        }

        Task<AuthenticationTicket> IAccountHelper.CreateTicket(User user)
        {
            return CreateTicketAsync(user);
        }

        public string GenerateToken(User user)
        {
            if (user != null)
            {

                var claims = new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["TokenAuthentication:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_config["TokenAuthentication:Issuer"],
                    _config["TokenAuthentication:Issuer"],
                    claims,
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: creds);
                _logger.LogInformation("Account Sign In");
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            throw new ApplicationException("Username tidak terdaftar.");
        }

        public Task<User> GetUser(ClaimsPrincipal User)
        {
            var user = _userManager.GetUserAsync(User);
            return user;
        }

        public Task<IList<string>> GetRole(User user)
        {
            return _userManager.GetRolesAsync(user);
        }
    }
}
