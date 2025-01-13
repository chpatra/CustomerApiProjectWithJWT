using CustomerAPI.Model;
using CutomerRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context; 
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")] 
        public IActionResult Login([FromBody] UserLogin userLogin) 
        { 
            var user = Authenticate(userLogin); 
            if (user != null) 
            { 
                var token = GenerateToken(user); 
                return Ok(new { Token = token }); 
            } 
            return Unauthorized("Invalid credentials"); 
        }

        private User Authenticate(UserLogin userLogin) 
        { 
            var login = _context.Logins.SingleOrDefault(l => l.Username == userLogin.Username && l.Password == userLogin.Password); 
            if (login != null)
            { 
                return new User { Name = login.Username, Email = $"{login.Username}@gmail.com" }; 
            } 
            return null; 
        }

        private string GenerateToken(User user) 
        { 
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 
            var token = new JwtSecurityToken
                (
                    claims: new[] 
                    { 
                        new Claim(ClaimTypes.Name, user.Name), 
                        new Claim(ClaimTypes.Email, user.Email) 
                    }, 
                    expires: DateTime.Now.AddMinutes(120), 
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }
}
