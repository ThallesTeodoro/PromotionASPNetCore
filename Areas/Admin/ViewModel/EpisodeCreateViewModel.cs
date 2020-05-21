using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Promotion.Areas.Admin.ViewModel
{
    public class EpisodeCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required]
        public IFormFile Thumb { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string VidelUrl { get; set; }
    }
}