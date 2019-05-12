using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using User.Api.Data;
using Microsoft.EntityFrameworkCore;

/*
 依赖包:mysql.data.entityframeworkcore
*/
namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private DbUserContext _UserContext;
        public ValuesController(DbUserContext userContext)
        {
            _UserContext = userContext;
        }

        //GET api/values
       [HttpGet]
        public async Task<ActionResult> Get()
        {
            return new JsonResult(await _UserContext.User.SingleOrDefaultAsync(u => u.Name == "cnblogs"));
        }

        //[HttpGet]
        //public ActionResult Get()
        //{
        //    return new JsonResult("90");
        //}
    }
}
