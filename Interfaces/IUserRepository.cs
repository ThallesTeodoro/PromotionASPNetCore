using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User FindUniqueByEmail(string email);
        IQueryable<User> GetAllWithoutId(int id);
        IQueryable<User> Search(string search, int id);
    }
}