using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Promotion.Areas.Admin.ViewModel
{
    public class ProductEditViewModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public IFormFile ImageMobile { get; set; }
    }
}