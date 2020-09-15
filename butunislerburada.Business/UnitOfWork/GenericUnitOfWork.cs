using butunislerburada.Business.BaseServices;
using butunislerburada.Data.Context;
using butunislerburada.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace butunislerburada.Business.UnitOfWork
{
    public class GenericUnitOfWork: IDisposable 
    {
        private TContext _context;

        public GenericUnitOfWork()
        {
            _context = new TContext();
        }

        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        
        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (repositories.Keys.Contains(typeof(TEntity)) == true)
            {
                return repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            IRepository<TEntity> repository = new GenericRepository<TEntity>(_context);
            repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public int SaveChanges()
        {
            var result = _context.SaveChanges();
            return result;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
