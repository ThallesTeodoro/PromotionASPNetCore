using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IPasswordResetRepository : IRepository<PasswordReset>
    {
        PasswordReset FindUniqueByEmail(string email);
    }
}