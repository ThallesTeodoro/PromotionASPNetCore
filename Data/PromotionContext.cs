using Microsoft.EntityFrameworkCore;
using Promotion.Models;

namespace Promotion.Data
{
    public class PromotionContext : DbContext
    {
        public PromotionContext(DbContextOptions<PromotionContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Participation> Participantions { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
    }
}