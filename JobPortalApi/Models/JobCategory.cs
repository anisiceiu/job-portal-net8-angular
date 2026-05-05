
using System.ComponentModel.DataAnnotations;

public class JobCategory
{
    [Key]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}