using System.ComponentModel.DataAnnotations;

namespace Promotion.Areas.ParticipantArea.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}