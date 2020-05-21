using System.Linq;
using Promotion.Models;

namespace Promotion.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        IQueryable<Product> Search(string search);
    }
}