using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Promotion.Areas.Admin.ViewModel
{
    public class ProductCreateViewModel
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        public IFormFile ImageMobile { get; set; }
    }
}