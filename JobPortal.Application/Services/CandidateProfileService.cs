using JobPortal.Application.DTOs;
using JobPortal.Application.Interfaces;
using JobPortal.Application.Interfaces.Services;
using JobPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Services
{
    public class CandidateProfileService:ICandidateProfileService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public CandidateProfileService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<CandidateProfile> CreateCandidateProfileAsync(CreateCandidateProfileDto dto)
        {
            var profile = new CandidateProfile
            {
                UserId = dto.UserId,
                Headline = dto.Headline,
                Summary = dto.Summary,
                ExperienceYears = dto.ExperienceYears,
                CurrentSalary = dto.CurrentSalary,
                ExpectedSalary = dto.ExpectedSalary,
                Location = dto.Location,
                PortfolioUrl = dto.PortfolioUrl,
                LinkedInUrl = dto.LinkedInUrl,
                GitHubUrl = dto.GitHubUrl,
                ResumeUrl = dto.ResumeUrl
            };

            _context.CandidateProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return profile;

        }

        public async Task DeleteCandidateProfileAsync(int id)
        {
            var profile = await GetCandidateProfileByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Candidate profile with ID {id} not found.");
            }

            _context.CandidateProfiles.Remove(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CandidateProfile>> GetAllCandidateProfilesAsync()
        {
            var items = await _context.CandidateProfiles
            .Include(cp => cp.User)
            .ToListAsync();
            return items;
        }

        public async Task<CandidateProfile?> GetCandidateProfileByIdAsync(int id)
        {
            var item = await _context.CandidateProfiles
            .Include(cp => cp.User)
            .FirstOrDefaultAsync(cp => cp.CandidateProfileId == id);
            return item;
        }

        public async Task<CandidateProfile?> UpdateCandidateProfileAsync(int id, UpdateCandidateProfileDto dto)
        {
            var profile = await GetCandidateProfileByIdAsync(id);
            

            profile.Headline = dto.Headline ?? profile.Headline;
            profile.Summary = dto.Summary ?? profile.Summary;
            profile.ExperienceYears = dto.ExperienceYears ?? profile.ExperienceYears;
            profile.CurrentSalary = dto.CurrentSalary ?? profile.CurrentSalary;
            profile.ExpectedSalary = dto.ExpectedSalary ?? profile.ExpectedSalary;
            profile.Location = dto.Location ?? profile.Location;
            profile.PortfolioUrl = dto.PortfolioUrl ?? profile.PortfolioUrl;
            profile.LinkedInUrl = dto.LinkedInUrl ?? profile.LinkedInUrl;
            profile.GitHubUrl = dto.GitHubUrl ?? profile.GitHubUrl;
            profile.ResumeUrl = dto.ResumeUrl ?? profile.ResumeUrl;

            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }
    }
}
