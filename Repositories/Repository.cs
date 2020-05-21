using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Promotion.Interfaces;
using Promotion.Data;

namespace Promotion.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly PromotionContext _context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(PromotionContext context)
        {
            _context = context;
            DbSet = _context.Set<TEntity>();
        }

        public virtual void Add(TEntity obj)
        {
            _context.Add(obj);
        }

        public virtual TEntity GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual void Remove(int id)
        {
            DbSet.Remove(DbSet.Find(id));
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}