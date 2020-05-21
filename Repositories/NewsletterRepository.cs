using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class NewsletterRepository : Repository<Newsletter>, INewsletterRepository
    {
        public NewsletterRepository(PromotionContext context) : base(context) {}

        public Newsletter FindUniqueByEmail(string email)
        {
            return DbSet.Where(p => p.Email == email).FirstOrDefault();
        }

        public IQueryable<Newsletter> Search(string search)
        {
            return DbSet.Where(p => p.Name.Contains(search) || p.Email.Contains(search));
        }
    }
}