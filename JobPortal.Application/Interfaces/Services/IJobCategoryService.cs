using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Interfaces.Services
{
    public interface IJobCategoryService
    {
        Task<IEnumerable<JobCategory>> GetAllCategoriesAsync();
        Task<JobCategory?> GetCategoryByIdAsync(int id);
        Task<JobCategory> CreateCategoryAsync(DTOs.CreateCategoryDto dto);
        Task UpdateCategoryAsync(DTOs.UpdateCategoryDto dto);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<Job>> GetCategoryJobsAsync(int id);
    }
}
