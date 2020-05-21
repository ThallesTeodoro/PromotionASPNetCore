using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Promotion.Areas.Admin.ViewModel
{
    public class EpisodeEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        public IFormFile Thumb { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string VidelUrl { get; set; }
    }
}