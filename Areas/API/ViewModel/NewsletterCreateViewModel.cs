using System;
using System.ComponentModel.DataAnnotations;

namespace Promotion.Areas.API.ViewModel
{
    public class NewsletterCreateViewModel
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}