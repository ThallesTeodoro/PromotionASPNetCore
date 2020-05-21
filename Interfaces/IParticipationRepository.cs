using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IParticipationRepository : IRepository<Participation>
    {
        int CountParticipations(int participantId);
    }
}