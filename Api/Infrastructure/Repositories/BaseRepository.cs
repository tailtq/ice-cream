using Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Api.Infrastructure.Repositories
{
    public class BaseRepository<T, K> where T : class
    {
        private readonly ICEDbContext _db = new ICEDbContext();

        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _db.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _db.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items.Where(predicate);
        }

        public T FindById(K id)
        {
            return _db.Set<T>().Find(id);
        }

        public T FindSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return GetAll(includeProperties).SingleOrDefault(predicate);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public void Remove(K id)
        {
            Remove(FindById(id));
        }

        public void RemoveMultiple(List<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _db.Set<T>().Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }
    }
}