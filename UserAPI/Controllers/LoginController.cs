using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration configuration)
        {
            _config = configuration;
        }

        private Login AuthenticateUser(Login user)

        {
            Login login = null;
            if(user.Username == "admin" && user.Password == "admin")
            {
                login = new Login { Username = "Faizan" };
            }
            return login;
        }

        private string GenerateJwtToken(Login user)
        {
           
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("LoginUser")]
        public IActionResult Login([FromBody] Login user)
        {
            var user_ = AuthenticateUser(user);
            if(user_ != null)
            {
                // Generate JWT token
                var token = GenerateJwtToken(user_);
                return Ok(new { token });
            }

            return Unauthorized();
        }

    }
}
