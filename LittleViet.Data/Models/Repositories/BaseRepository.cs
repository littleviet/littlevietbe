using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LittleViet.Data.Models.Repositories;

public interface IBaseRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    IQueryable<TEntity> Get();
    IQueryable<TEntity> ActiveOnly();
    TEntity Get<TKey>(TKey id);
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
    TEntity FirstOrDefault();
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    void Add(TEntity entity);
    void AddRange(List<TEntity> entityList);
    void Edit(TEntity entity);
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
        this._dbSet = this._dbContext.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Get()
    {
        return Queryable.AsQueryable<TEntity>((IEnumerable<TEntity>)this._dbSet);
    }

    public virtual TEntity Get<TKey>(TKey id)
    {
        return (TEntity)this._dbSet.Find(new object[1] { id });
    }

    public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable.Where<TEntity>((IQueryable<TEntity>)this._dbSet, predicate);
    }

    public virtual IQueryable<TEntity> Include(Expression<Func<TEntity, object>> predicate)
    {
        return this._dbSet.Include(predicate);
    }

    public virtual IQueryable<TEntity> ActiveOnly()
    {
        return Queryable.AsQueryable<TEntity>((IEnumerable<TEntity>)this._dbSet).Where(a => a.IsDeleted == false);
    }

    public virtual TEntity FirstOrDefault()
    {
        return Queryable.FirstOrDefault<TEntity>((IQueryable<TEntity>)this._dbSet);
    }

    public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable.FirstOrDefault<TEntity>((IQueryable<TEntity>)this._dbSet, predicate);
    }

    public virtual void Add(TEntity entity)
    {
        this._dbSet.Add(entity);
    }

    public virtual void AddRange(List<TEntity> entityList)
    {
        this._dbSet.AddRange(entityList);
    }

    public virtual void Edit(TEntity entity)
    {
        this._dbContext.Update<TEntity>(entity);
    }

    public virtual void Deactivate(TEntity entity)
    {
        if (((object)entity) is IActive)
        {
            ((IActive)(object)entity).IsDeleted = true;
            return;
        }
        throw new NotSupportedException("TEntity must implement IActivable to use this method. TEntity: " + typeof(TEntity).FullName);
    }
}

