using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promotion.Models
{
    [Table("episodes")]
    public class Episode
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required]
        public string Thumb { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string VidelUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}