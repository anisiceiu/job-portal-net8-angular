using JobPortal.Application.Interfaces.Services;
using JobPortal.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobPortal.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IJobCategoryService, JobCategoryService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICandidateProfileService, CandidateProfileService>();
            return services;
        }
    }
}
