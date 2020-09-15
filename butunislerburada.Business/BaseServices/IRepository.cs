using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace butunislerburada.Business.BaseServices
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Insert(TEntity entity);
        void Delete(int Id);
        TEntity Update(TEntity entity);
        List<TEntity> GetList();
        List<TEntity> GetList(Expression<Func<TEntity, bool>> _lambda);

        IQueryable<TEntity> GetListQuerable();
        TEntity Find(int Id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> _lambda);
        bool Any(Expression<Func<TEntity, bool>> _lambda);
        int Count(Expression<Func<TEntity, bool>> _lambda);
    }
}
