using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.Models
{
    public class Category
    {
        public int Id { get; set; }


        [Required][StringLength(100, ErrorMessage = "Maximum description limit reached.")]
        public string CategoryName { get; set; } = string.Empty;


        [Required] [StringLength(500, ErrorMessage ="Maximum description limit reached.")]
        [MinLength(10, ErrorMessage ="Mimimum of 10 characters is needed in description.")]
        public string CategoryDescription { get; set; } =string.Empty;

        public bool IsActive { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
