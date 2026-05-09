using System.ComponentModel.DataAnnotations;
namespace JobPortal.Application.DTOs;  

public class CreateCategoryDto
{
    [Required]
    [StringLength(180)]
    public string CategoryName { get; set; } = string.Empty;
    
}

public class UpdateCategoryDto : CreateCategoryDto
{
    [Required]
    public int CategoryId { get; set; }
}