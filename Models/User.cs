using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PlasticFreeOcean.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public bool IsNeedToResetPassword { get; set; }
        public bool IsDeleted { get; set; }

    }
}