using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.DTOs.CategoryDTOs
{
    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category Id is required.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Category Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;
    }
}
