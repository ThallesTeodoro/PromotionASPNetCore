using System;
using System.ComponentModel.DataAnnotations;

namespace Promotion.ViewModel
{
    public class VoteViewModel
    {
        [Required]
        public int EpisodeId { get; set; }
    }
}