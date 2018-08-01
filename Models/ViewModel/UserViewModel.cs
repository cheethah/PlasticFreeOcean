using System;
using System.Collections.Generic;

namespace PlasticFreeOcean.Models
{
    public class UserViewModel
    {
        public IList<string> RoleName { get; set; }
        public User user { get; set; }
    }
}
