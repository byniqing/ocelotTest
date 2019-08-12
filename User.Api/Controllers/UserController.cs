using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DbUserContext _userContext;
        public UserController(DbUserContext dbUserContext)
        {
            _userContext = dbUserContext;
        }

        [HttpPost("check-or-create")]
        public async Task<IActionResult> CheckOrCreate([FromForm]string phone)
        {
            var user = _userContext.User.SingleOrDefault(u => u.phone == phone);
            if (user == null)
            {
                user = new Model.AppUser { phone = phone };
                _userContext.User.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(user.Id);
        }
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(_userContext.User);
        }
    }
}