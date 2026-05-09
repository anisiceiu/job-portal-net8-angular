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
    public class JobCategoryService : IJobCategoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public JobCategoryService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<JobCategory> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var Category = new JobCategory
            {
                CategoryName = dto.CategoryName
            };

            _context.JobCategories.Add(Category);
            await _context.SaveChangesAsync();

            return Category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var Category = await GetCategoryByIdAsync(id);

            _context.JobCategories.Remove(Category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobCategory>> GetAllCategoriesAsync()
        {
            return await _context.JobCategories.ToListAsync();
        }

        public async Task<JobCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.JobCategories.FindAsync(id);
        }

        public async Task<IEnumerable<Job>> GetCategoryJobsAsync(int id)
        {
            var jobs = await _context.Jobs
            .Where(j => j.CategoryId == id)
            .ToListAsync();

            return jobs;
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            var existingCategory = await GetCategoryByIdAsync(dto.CategoryId);

            // Update properties
            existingCategory!.CategoryName = dto.CategoryName;

            await _context.SaveChangesAsync();
        }
    }
}
