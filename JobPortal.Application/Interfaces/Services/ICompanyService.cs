using JobPortal.Application.DTOs;
using JobPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company?> GetCompanyByIdAsync(int id);
        Task<Company> CreateCompanyAsync(CreateCompanyDto dto);
        Task<Company?> UpdateCompanyAsync(int id, UpdateCompanyDto dto);
        Task DeleteCompanyAsync(int id);
        Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId);
    }
}
