using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class ParticipantRepository : Repository<Participant>, IParticipantRepository
    {
        public ParticipantRepository(PromotionContext context) : base(context) {}

        public Participant FindUniqueByEmail(string email)
        {
            return DbSet.Where(p => p.Email == email).FirstOrDefault();
        }
    }
}