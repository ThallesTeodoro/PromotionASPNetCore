using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>
    {
        IQueryable<Episode> Search(string search);
    }
}