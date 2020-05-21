using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;
using System.Linq;

namespace Promotion.Repositories
{
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        public EpisodeRepository(PromotionContext context) : base(context) {}

        public IQueryable<Episode> Search(string search)
        {
            return DbSet.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
        }
    }
}