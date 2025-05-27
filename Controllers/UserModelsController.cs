
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;
using webapi.DTOs;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserModelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUserModels()
        {
            return await _context.UserModels.ToListAsync();
        }

        // GET: api/UserModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserModel(Guid id)
        {
            var userModel = await _context.UserModels.FindAsync(id);

            if (userModel == null)
            {
                return NotFound();
            }

            return userModel;
        }

        // PUT: api/UserModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserModel(Guid id, UserModel userModel)
        {
            if (id != userModel.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserModel>> PostUserModel(UserCreateDto userDto)
        {
            var userModel = new UserModel
            {
                UserId = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                Role = userDto.Role,
                PasswordHash = userDto.PasswordHash
            };

            _context.UserModels.Add(userModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserModelExists(userModel.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserModel", new { id = userModel.UserId }, userModel);
        }

        // DELETE: api/UserModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserModel(Guid id)
        {
            var userModel = await _context.UserModels.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            _context.UserModels.Remove(userModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/UserModels/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto loginDto)
        {
            try
            {
                // Find user by email
                var user = await _context.UserModels
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // In a real application, you should use proper password hashing
                // This is a simplified version for demonstration purposes
                if (user.PasswordHash != loginDto.Password)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Return user information
                return Ok(new
                {
                    userId = user.UserId,
                    name = user.Name,
                    email = user.Email,
                    role = user.Role,
                    token = "demo-token-" + Guid.NewGuid().ToString() // Replace with real JWT in production
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        // PUT: api/UserModels/resetpassword
        [HttpPut("resetpassword")]
        public async Task<IActionResult> ResetPassword(PasswordResetDto resetDto)
        {
            try
            {
                if (string.IsNullOrEmpty(resetDto.Email) || string.IsNullOrEmpty(resetDto.NewPassword))
                {
                    return BadRequest(new { message = "Email and new password are required" });
                }

                // Find the user by email
                var user = await _context.UserModels
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == resetDto.Email.ToLower());

                if (user == null)
                {
                    // For security reasons, don't reveal if the email exists or not
                    return Ok(new { message = "If the email exists, the password has been reset" });
                }

                // Update the password - in a real app, hash the password
                user.PasswordHash = resetDto.NewPassword;

                // Save changes
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password has been reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        private bool UserModelExists(Guid id)
        {
            return _context.UserModels.Any(e => e.UserId == id);
        }
    }
}