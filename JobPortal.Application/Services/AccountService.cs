using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Application.Utils;
using JobPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AccountService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == email);

            return exists;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await GetUserByEmail(loginDto.Email);
            var candidateProfileId = user?.Role == "Candidate"
                ? await _context.CandidateProfiles
                    .Where(cp => cp.UserId == user.UserId)
                    .Select(cp => cp.CandidateProfileId)
                    .FirstOrDefaultAsync()
                : 0;
            var token = GenerateJwtToken(user!, candidateProfileId);
            var response = new AuthResponseDto
            {
                Token = token,
                FullName = user!.FullName,
                Email = user!.Email,
                Role = user!.Role,
                CandidateProfileId = candidateProfileId
            };

            return response;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = dto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(null, new { id = user.UserId }, new { user.UserId, user.FullName, user.Email, user.Role });
            var candidateProfileId = user.Role == "Candidate"
                ? await _context.CandidateProfiles
                    .Where(cp => cp.UserId == user.UserId)
                    .Select(cp => cp.CandidateProfileId)
                    .FirstOrDefaultAsync()
                : 0;
            var token = GenerateJwtToken(user, candidateProfileId);
            var response = new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CandidateProfileId = candidateProfileId
            };


            return response;

        }

        public async Task<bool> VerifyPassword(string passwordHash,string password)
        {
            return PasswordHasher.VerifyPassword(passwordHash, password);
        }

        private string GenerateJwtToken(User user, int candidateProfileId)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expireMinutes = jwtSection.GetValue<int>("ExpireMinutes");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
                new Claim("candidateProfileId", candidateProfileId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
