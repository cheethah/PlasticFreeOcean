using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using PlasticFreeOcean.Models;
using System;
using System.Threading.Tasks;

namespace PlasticFreeOcean.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PlasticFreeOceanContext _context;

        public UsersController(PlasticFreeOceanContext context)
        {
            _context = context;

            if (_context.Users.Count() == 0)
            {
                _context.Users.Add(new User { Email = "bagas.cita@gmail.com",IsBlocked = false, IsNeedtoReset= false, Name = "bagas", password = "ZXasqw12" });
                _context.SaveChanges();
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> GetById(long id)
        {
            var item = _context.Users.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }       
    }
}