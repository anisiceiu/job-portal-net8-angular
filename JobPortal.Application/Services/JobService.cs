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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JobPortal.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public JobService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> CompanyExistsAsync(int id)
        {
            return await _context.Companies.AnyAsync(c => c.CompanyId == id);
        }

        public async Task<JobApplication> CreateJobApplication(CreateApplicationDto dto)
        {
            string? resumePath = null;

            // Save uploaded resume file
            if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName =
                    $"{Guid.NewGuid()}_{dto.ResumeFile.FileName}";

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ResumeFile.CopyToAsync(stream);
                }

                resumePath = $"/resumes/{uniqueFileName}";
            }

            var application = new JobApplication
            {
                JobId = dto.JobId,
                CandidateProfileId = dto.CandidateProfileId,
                CoverLetter = dto.CoverLetter,
                ResumeUrl = resumePath,
                Status = "Submitted",
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);

            await _context.SaveChangesAsync();


            return application;
        }

        public async Task<Job> CreateJobAsync(CreateJobDto jobDto)
        {
            var company = await _context.Companies.FindAsync(jobDto.CompanyId); 

            var job = new Job
            {
                CompanyId = jobDto.CompanyId,
                PostedByUserId = jobDto.PostedByUserId,
                CategoryId = jobDto.CategoryId,
                Title = jobDto.Title,
                JobType = jobDto.JobType,
                WorkMode = jobDto.WorkMode,
                Location = jobDto.Location,
                SalaryMin = jobDto.SalaryMin,
                SalaryMax = jobDto.SalaryMax,
                Description = jobDto.Description,
                Responsibilities = jobDto.Responsibilities,
                Requirements = jobDto.Requirements,
                Benefits = jobDto.Benefits,
                Deadline = jobDto.Deadline,
                Status = jobDto.Status,
                Company = company
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return job;
        }

        public async Task DeleteJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {id} not found.");
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }

        public async Task<Job> GetJobAsync(int id)
        {
            var job = await _context.Jobs
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.JobId == id);

            return job;
        }

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Jobs.FindAsync(id);
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            return await _context.Jobs.Include(j=> j.Company).ToListAsync();
        }

        public async Task<PaginatedResult<Job>> GetJobsPaginated(int page = 1, int pageSize = 10)
        {
            var totalJobs = await _context.Jobs.CountAsync();
            var jobs = await _context.Jobs
                 .AsNoTracking()
                .Include(j => j.Company)
                .OrderByDescending(j => j.JobId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<Job>
            {
                Items = jobs,
                TotalCount = totalJobs,
                Page = page,
                PageSize = pageSize
            };
          
            return result;
        }

        public async Task<PaginatedResult<Job>> SearchJobs(string? type, int? categoryId, string? workmode, string? experience)
        {
            var query = _context.Jobs.Include(j => j.Company).AsQueryable();

            if (!string.IsNullOrEmpty(workmode))
            {
                query = query.Where(j => j.WorkMode!.Contains(workmode));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(j => type.ToLower().Contains(j.JobType.ToLower()));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(j => j.CategoryId == categoryId.Value);
            }

            var totalJobs = await query.CountAsync();
            var jobs = await query.AsNoTracking()
                .Include(j => j.Company)
                .OrderByDescending(j => j.JobId)
                .Skip(0 * 10)
                .Take(10)
                .ToListAsync();

            var result = new PaginatedResult<Job>
            {
                Items = jobs,
                TotalCount = totalJobs,
                Page = 1,
                PageSize = 10
            };


            return result;
        }

        public async Task<Job?> UpdateJobAsync(int id, UpdateJobDto dto)
        {
            var existingJob = await _context.Jobs.FindAsync(id);
            if (existingJob == null)
            {
                return null;
            }

            existingJob.CompanyId = dto.CompanyId;
            existingJob.PostedByUserId = dto.PostedByUserId;
            existingJob.CategoryId = dto.CategoryId;
            existingJob.Title = dto.Title;
            existingJob.JobType = dto.JobType;
            existingJob.WorkMode = dto.WorkMode;
            existingJob.Location = dto.Location;
            existingJob.SalaryMin = dto.SalaryMin;
            existingJob.SalaryMax = dto.SalaryMax;
            existingJob.Description = dto.Description;
            existingJob.Responsibilities = dto.Responsibilities;
            existingJob.Requirements = dto.Requirements;
            existingJob.Benefits = dto.Benefits;
            existingJob.Deadline = dto.Deadline;
            existingJob.Status = dto.Status;
            existingJob.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingJob;
        }
    }
}
