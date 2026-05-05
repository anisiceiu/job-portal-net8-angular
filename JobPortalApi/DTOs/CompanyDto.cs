using System.ComponentModel.DataAnnotations;

namespace JobPortalApi.DTOs
{
    public class CreateCompanyDto
    {
        [Required]
        [StringLength(180)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }
    }

    public class UpdateCompanyDto : CreateCompanyDto
    {
        [Required]
        public int CompanyId { get; set; }
    }
}
