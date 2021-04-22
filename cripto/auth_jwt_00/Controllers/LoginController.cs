using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace auth_jwt_00.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        //  curl https://localhost:5001/Login/echoping --ssl-no-revoke
        [HttpGet]
        [Route("echoping")]
        public IActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        // curl https://localhost:5001/Login/authenticate -X POST -d '{\"Username\":\"paula\",\"Password\":\"123456\"}' -H "Content-Type: application/json" --ssl-no-revoke  -v
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody] LoginRequest login)
        {
            if (login == null)
            {
                return BadRequest();
            }
            
            //TODO: Validate credentials Correctly, this code is only for demo !!
            bool isCredentialValid = (login.Password == "123456");
            if (isCredentialValid)
            {
                var token = TokenGenerator.GenerateTokenJwt(login.Username);
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
