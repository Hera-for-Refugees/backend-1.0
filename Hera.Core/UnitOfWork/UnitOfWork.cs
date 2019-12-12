using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace Hera.Core.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private Data.Entity.HeraEntities _dbContext;
        public UnitOfWork()
        {
            _dbContext = new Data.Entity.HeraEntities();
        }
        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        public Repository.IGenericRepository<T> Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)] as Repository.IGenericRepository<T>;
            }
            Core.Repository.IGenericRepository<T> repo = new Core.Repository.GenericRepository<T>(_dbContext);
            repositories.Add(typeof(T), repo);
            return repo;
        }
        public int Commit()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                if (ex is DbEntityValidationException)
                {
                    var dbValidationError = ex as DbEntityValidationException;
                    throw dbValidationError;
                }
                //throw ex;
                return 0;
            }
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
