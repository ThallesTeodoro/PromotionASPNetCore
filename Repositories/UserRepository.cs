using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class UserReposiotry : Repository<User>, IUserRepository
    {
        public UserReposiotry(PromotionContext context) : base(context) {}

        public User FindUniqueByEmail(string email)
        {
            return DbSet.Where(p => p.Email == email).FirstOrDefault();
        }

        public IQueryable<User> GetAllWithoutId(int id)
        {
            return DbSet.Where(p => p.Id != id);
        }

        public IQueryable<User> Search(string search, int id)
        {
            return DbSet.Where(p => p.Id != id && (p.Name.Contains(search) || p.Email.Contains(search)));
        }
    }
}