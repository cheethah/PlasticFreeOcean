using System;
using System.ComponentModel.DataAnnotations;

namespace PlasticFreeOcean.Models
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string DeviceId { get; set; }

    }
}
