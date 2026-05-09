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
    public class CompanyService: ICompanyService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _config;

        public CompanyService(IApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<Company> CreateCompanyAsync(CreateCompanyDto dto)
        {
            var company = new Company
            {
                CompanyName = dto.CompanyName,
                Industry = dto.Industry,
                Website = dto.Website,
                LogoUrl = dto.LogoUrl,
                Description = dto.Description,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                CreatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await GetCompanyByIdAsync(id);
            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {id} not found.");
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId)
        {
            return await _context.Jobs
                .Where(j => j.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Company?> UpdateCompanyAsync(int id, UpdateCompanyDto dto)
        {
            var existingCompany = await GetCompanyByIdAsync(id);
 

            // Update properties
            existingCompany.CompanyName = dto.CompanyName;
            existingCompany.Industry = dto.Industry;
            existingCompany.Website = dto.Website;
            existingCompany.LogoUrl = dto.LogoUrl;
            existingCompany.Description = dto.Description;
            existingCompany.Address = dto.Address;
            existingCompany.City = dto.City;
            existingCompany.Country = dto.Country;

            await _context.SaveChangesAsync();

            return existingCompany;
        }
    }
}
