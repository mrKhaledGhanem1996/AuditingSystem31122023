using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuditingSystem.Services.Interfaces
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> ListAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includes);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TKey id);

        Task<TEntity> FindByAsync(TKey id);

        Task<TEntity> FindByAsync(Expression<Func<TEntity, bool>> predicate);

        string GetEntityType();


    }
}
