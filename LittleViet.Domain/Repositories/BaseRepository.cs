using System.Linq.Expressions;
using LittleViet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LittleViet.Domain.Repositories;
public interface IRepository
{
}

public interface IBaseRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    IQueryable<TEntity> DbSet();
    IQueryable<TEntity> DbSetWithDeletedEntities();
    TEntity Get<TKey>(TKey id);
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
    TEntity FirstOrDefault();
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    EntityEntry<TEntity> Add(TEntity entity);
    void AddRange(List<TEntity> entityList);
    void Modify(TEntity entity);
    void Deactivate(TEntity entity);
    IQueryable<TEntity> Include(Expression<Func<TEntity, object>> predicate);
}
public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IRepository where TEntity : class, IEntity
{
    protected DbContext _dbContext;

    protected DbSet<TEntity> _dbSet;

    public BaseRepository(DbContext context)
    {
        this._dbContext = context;
        this._dbSet = _dbContext.Set<TEntity>();
    }

    public virtual TEntity Get<TKey>(TKey id)
    {
        return this._dbSet.Find(new object[1] { id });
    }

    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable.Where(DbSet(), predicate);
    }

    public virtual IQueryable<TEntity> Include(Expression<Func<TEntity, object>> predicate)
    {
        return this._dbSet.Include(predicate);
    }

    public virtual IQueryable<TEntity> DbSet()
    {
        return Queryable.AsQueryable(_dbSet).Where(a => a.IsDeleted == false);
    }

    public virtual TEntity FirstOrDefault()
    {
        return Queryable.FirstOrDefault(DbSet());
    }

    public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable.FirstOrDefault(DbSet(), predicate);
    }

    public virtual EntityEntry<TEntity> Add(TEntity entity)
    {
        return this._dbSet.Add(entity);
    }

    public virtual void AddRange(List<TEntity> entityList)
    {
        this._dbSet.AddRange(entityList);
    }

    public virtual void Modify(TEntity entity)
    {
        this._dbSet.Update(entity);
    }

    public virtual IQueryable<TEntity> DbSetWithDeletedEntities()
    {
        return Queryable.AsQueryable(_dbSet);
    }

    public virtual void Deactivate(TEntity entity)
    {
        entity.IsDeleted = entity.IsDeleted == false
            ? true
            : throw new InvalidOperationException("Cannot deactivate already deactivated entity!");
    }
}

