using System.Linq.Expressions;
using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceBookStore.DataAccess.Repository {
    public class Repository<T> : IRepository<T> where T : class {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
            this.dbSet = _dbContext.Set<T>(); // dbSet == _dbContext.Categories

            _dbContext.Products.Include(x => x.Category).Include(x => x.CategoryId);
        }

        public void Add(T entity) {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false) {
            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();
            //if (tracked) {
            //    query = dbSet;
            //} else {
            //    query = dbSet.AsNoTracking();
            //}

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties)) {
                foreach (var includeProperty in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null) {
            IQueryable<T> query = dbSet;
            if (filter != null) {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties)) {
                foreach (var includeProperty in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToList();
        }

        public void Remove(T entity) {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities) {
            dbSet.RemoveRange(entities);
        }
    }
}
