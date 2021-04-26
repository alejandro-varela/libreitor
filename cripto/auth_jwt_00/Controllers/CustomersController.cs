using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace auth_jwt_00.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        // [HttpGet]
        // public IActionResult GetId(int id)
        // {
        //     var customerFake = "customer-fake";
        //     return Ok(customerFake);
        // }

        [HttpGet]
        public IActionResult GetAll()
        {
            var customersFake = new string[] { "customer-1", "customer-2", "customer-3" };
            return Ok(customersFake);
        }
    }
}
