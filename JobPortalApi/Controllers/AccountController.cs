using Microsoft.AspNetCore.Mvc;
using JobPortalApi.Data;
using JobPortalApi.DTOs;
using JobPortalApi.Models;
using JobPortalApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces.Services;

namespace JobPortalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest("Missing required fields.");

            var exists = await _service.IsEmailRegisteredAsync(dto.Email);
            if (exists) return Conflict("Email already registered.");

            var allowedRoles = new[] { "Candidate", "Employer", "Admin" };
            if (!allowedRoles.Contains(dto.Role)) return BadRequest("Invalid role.");

            var response = await _service.RegisterAsync(dto);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Missing credentials.");

            var user = await _service.GetUserByEmail(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");
            if (!user.IsActive) return Unauthorized("User is not active.");

            if (!await _service.VerifyPassword(user.PasswordHash, dto.Password))
                return Unauthorized("Invalid credentials.");

            var response = await _service.LoginAsync(dto);

            return Ok(response);
        }

        
    }
}
