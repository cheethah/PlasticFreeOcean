using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using PlasticFreeOcean.IdentityModel;
using PlasticFreeOcean.Models;

namespace PlasticFreeOcean.Services
{
    public interface IAccountHelper
    {
        Task<string> SignIn(LoginUser loginUser);
        Task<User> Create(NewUser newUser);
        Task<AuthenticationTicket> CreateTicket(User user);
        string GenerateToken(User user);
        Task<User> GetUser(ClaimsPrincipal User);
        Task<IList<string>> GetRole(User user);
    }
}
