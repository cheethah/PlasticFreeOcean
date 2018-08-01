using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlasticFreeOcean.Models;

namespace PlasticFreeOcean.Services
{
    public class RoleHelper :IRoleHelper
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleHelper(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CheckAndCreateRoleAsync(string roleName)
        {
            var isRoleExist = await _roleManager.RoleExistsAsync(roleName);
            var role = new Role()
            {
                Id = Guid.NewGuid(),
                Name = roleName
            };
            if (!isRoleExist)
            {
                await _roleManager.CreateAsync(role);
            }
        }
    }
}
