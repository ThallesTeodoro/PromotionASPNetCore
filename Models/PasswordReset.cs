using System;
using System.ComponentModel.DataAnnotations;

namespace Promotion.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}