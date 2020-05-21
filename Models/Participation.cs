using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Promotion.Models
{
    [Table("participations")]
    public class Participation
    {
        public int Id { get; set; }
        
        [Required]
        public int ParticipantId { get; set; }
        
        [Required]
        public int EpisodeId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Participant Participant { get; set; }

        public Episode Episode { get; set; }
    }
}