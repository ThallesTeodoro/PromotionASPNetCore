using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        Participant FindUniqueByEmail(string email);
    }
}