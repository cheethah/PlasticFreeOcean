using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticFreeOcean.Models
{
    public class AccountRegisterViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
