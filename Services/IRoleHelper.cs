using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using PlasticFreeOcean.IdentityModel;
using PlasticFreeOcean.Models;

namespace PlasticFreeOcean.Services
{
    public interface IRoleHelper
    {
        Task CheckAndCreateRoleAsync(string roleName);
    }
}
