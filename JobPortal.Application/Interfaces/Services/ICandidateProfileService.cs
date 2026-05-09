using JobPortal.Application.DTOs;
using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Interfaces.Services
{
    public interface ICandidateProfileService
    {
        Task<IEnumerable<CandidateProfile>> GetAllCandidateProfilesAsync();
        Task<CandidateProfile?> GetCandidateProfileByIdAsync(int id);
        Task<CandidateProfile> CreateCandidateProfileAsync(CreateCandidateProfileDto dto);
        Task<CandidateProfile?> UpdateCandidateProfileAsync(int id, UpdateCandidateProfileDto dto);
        Task DeleteCandidateProfileAsync(int id);
        Task<bool> UserExistsAsync(int userId);
    }
}
