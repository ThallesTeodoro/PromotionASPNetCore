using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface INewsletterRepository : IRepository<Newsletter>
    {
        Newsletter FindUniqueByEmail(string email);
        IQueryable<Newsletter> Search(string search);
    }
}