using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promotion.Models
{
    [Table("participants")]
    public class Participant
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }

    public enum Gender
    {
        M, F
    }
}