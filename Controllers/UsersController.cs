using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using PlasticFreeOcean.Models;
using System;
using System.Threading.Tasks;

namespace PlasticFreeOcean.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PlasticFreeOceanContext _context;

        public UsersController(PlasticFreeOceanContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _context.Users;
        }     
    }
}