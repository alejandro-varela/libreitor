using System;

namespace auth_jwt_00.Controllers
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
