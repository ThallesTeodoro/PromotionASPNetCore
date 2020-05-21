using System.Linq;
using Promotion.Interfaces;
using Promotion.Models;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(PromotionContext context) : base(context) {}

        public IQueryable<Product> Search(string search)
        {
            return DbSet.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        }
    }
}