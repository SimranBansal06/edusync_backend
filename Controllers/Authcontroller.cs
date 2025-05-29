//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using webapi.Data;
//using webapi.Models;

//namespace webapi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly IConfiguration _configuration;

//        public AuthController(AppDbContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _configuration = configuration;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(LoginDto loginDto)
//        {
//            if (loginDto == null)
//                return BadRequest("Invalid login data");

//            // Note: Use UserModels instead of UserModel to match your DbContext
//            var user = await _context.UserModels.FirstOrDefaultAsync(u =>
//                u.Email == loginDto.Email);

//            if (user == null)
//                return Unauthorized(new { message = "Invalid email or password" });

//            // In production, use a proper password hashing/verification method
//            if (user.PasswordHash != loginDto.Password)
//                return Unauthorized(new { message = "Invalid email or password" });

//            var tokenString = GenerateJwtToken(user);

//            return Ok(new
//            {
//                token = tokenString,
//                userId = user.UserId,
//                name = user.Name,
//                email = user.Email,
//                role = user.Role
//            });
//        }

//        private string GenerateJwtToken(UserModel user)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, user.UserId.ToString()),
//                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
//            };

//            // Only add role claim if role is not null or empty
//            if (!string.IsNullOrEmpty(user.Role))
//            {
//                claims.Add(new Claim(ClaimTypes.Role, user.Role));
//            }

//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(3),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }

//    public class LoginDto
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using webapi.Data;
using webapi.Models;
namespace webapi.Controllers
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
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest("Invalid login data");

            // Note: Use UserModels instead of UserModel to match your DbContext
            var user = await _context.UserModels.FirstOrDefaultAsync(u =>
                u.Email == loginDto.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // In production, use a proper password hashing/verification method
            if (user.PasswordHash != loginDto.Password)
                return Unauthorized(new { message = "Invalid email or password" });

            var tokenString = GenerateJwtToken(user);

            return Ok(new
            {
                token = tokenString,
                userId = user.UserId,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }

        private string GenerateJwtToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Only add role claim if role is not null or empty
            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            // Get JWT key with Azure compatibility
            string jwtKey = GetJwtKey();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Get token expiration from configuration or use default
            int expirationInMinutes = 180; // Default 3 hours
            if (int.TryParse(_configuration["Jwt:DurationInMinutes"], out int configuredDuration))
            {
                expirationInMinutes = configuredDuration;
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetJwtKey()
        {
            // Try to get the key from environment variables first (for Azure deployment)
            string jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");

            // If not found, fall back to configuration
            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = _configuration["Jwt:Key"];
            }

            // Ensure we have a key
            if (string.IsNullOrEmpty(jwtKey))
            {
                // Log this error in production
                jwtKey = "defaultkeyfordevwhichshouldbereplaced";
            }

            return jwtKey;
        }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}