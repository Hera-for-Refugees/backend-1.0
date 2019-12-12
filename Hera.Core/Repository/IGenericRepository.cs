using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hera.Core.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, object>> orderBy, bool desc = false);
        IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool desc = false);
        T Add(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        void Update(T entity);
        void RunQuery(string sql, params object[] parameters);
        IEnumerable<T> ExecWtihSP(string spName, params object[] parameters);
        void Save();
    }
}
