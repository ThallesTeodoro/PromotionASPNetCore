using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class ParticipationRepository : Repository<Participation>, IParticipationRepository
    {
        public ParticipationRepository(PromotionContext context) : base(context) {}

        public int CountParticipations(int participantId)
        {
            return DbSet.Where(p => p.ParticipantId == participantId).Count();
        }
    }
}