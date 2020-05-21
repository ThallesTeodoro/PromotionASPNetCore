using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class PasswordResetRepository : Repository<PasswordReset>, IPasswordResetRepository
    {
        public PasswordResetRepository(PromotionContext context) : base(context) {}

        public PasswordReset FindUniqueByEmail(string email)
        {
            return DbSet.Where(p => p.Email == email).FirstOrDefault();
        }
    }
}