using CommonCore.Entities;
using System.Linq.Expressions;


namespace CommonCore.DataAccess
{
    public interface IEntityQueryableRepository<T> 
        where T: class, IEntity, new() 
    {
        IQueryable<T> Query(Expression<Func<T, bool>> expression);
    }
}
