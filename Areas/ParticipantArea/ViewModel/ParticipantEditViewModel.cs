using System;
using System.ComponentModel.DataAnnotations;
using Promotion.Models;

namespace Promotion.Areas.ParticipantArea.ViewModel
{
    public class ParticipantEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
    }
}