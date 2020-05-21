using System.ComponentModel.DataAnnotations;

namespace Promotion.Areas.API.ViewModel
{
    public class ContactCreateViewModel
    {
        [Required]
        public string Profile { get; set; }

        [Required]
        public string Message { get; set; }
    }
}