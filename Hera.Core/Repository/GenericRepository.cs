using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Hera.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected Data.Entity.HeraEntities _entities;
        protected readonly IDbSet<T> _dbSet;

        public GenericRepository(Data.Entity.HeraEntities context)
        {
            _entities = context;
            _dbSet = context.Set<T>();
        }
        public T Add(T entity)
        {
            return _dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.AsEnumerable<T>();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, object>> orderBy, bool desc = false)
        {
            return desc ? _dbSet.OrderByDescending(orderBy).AsEnumerable<T>() : _dbSet.OrderBy(orderBy).AsEnumerable<T>();
        }

        public IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsEnumerable<T>();
        }

        public IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool desc = false)
        {
            var result = _dbSet.Where(predicate);
            return desc ? result.OrderByDescending(orderBy).AsEnumerable<T>() : result.OrderBy(orderBy).AsEnumerable<T>();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(Expression<Func<T, bool>> predicate)
        {
            var entity = _dbSet.Where(predicate).ToList();
            if (entity == null) return;
            entity.ForEach(x => _dbSet.Remove(x));
        }

        public void Update(T entity)
        {
            if (entity == null) return;

            _entities.Entry(entity).State = EntityState.Modified;
        }

        public void RunQuery(string sql, params object[] parameters)
        {
            _entities.Database.ExecuteSqlCommand(sql, parameters);
        }

        public void Save()
        {
            _entities.SaveChanges();
        }

        public IEnumerable<T> ExecWtihSP(string spName, params object[] parameters)
        {
            if (parameters == null)
                parameters = new object[0];
            return _entities.Database.SqlQuery<T>(spName, parameters);
        }
    }
}
