using JobPortal.Application.DTOs;
using JobPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<Job?> GetJobByIdAsync(int id);
        Task<JobApplication> CreateJobApplication(CreateApplicationDto dto);
        Task<IEnumerable<Job>> GetJobsAsync();
        Task<Job> GetJobAsync(int id);
        Task<Job> CreateJobAsync(CreateJobDto dto);
        Task<bool> CompanyExistsAsync(int id);
        Task<Job?> UpdateJobAsync(int id, UpdateJobDto dto);
        Task DeleteJobAsync(int id);
        Task<PaginatedResult<Job>> SearchJobs(string? type,int? categoryId,string? workmode,string? experience);
        Task<PaginatedResult<Job>> GetJobsPaginated(int page = 1, int pageSize = 10);
    }
}
