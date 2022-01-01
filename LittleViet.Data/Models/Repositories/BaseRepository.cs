using LittleViet.Data.ServiceHelper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LittleViet.Data.Models.Repositories
{
    public interface IBaseRepository<TEntity> : IRepository where TEntity : class, IEntity
    {
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

        public virtual IQueryable<TEntity> GetActive()
        {
            if (typeof(IActive).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> node = (TEntity q) => !((IActive)q).IsDeleted;
                node = (Expression<Func<TEntity, bool>>)RemoveCastsVisitor.Visit(node);
                return Queryable.Where<TEntity>(this.Get(), node);
            }
            return this.Get();
        }

        public virtual IQueryable<TEntity> GetActive(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(IActive).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> node = (TEntity q) => !((IActive)q).IsDeleted;
                node = (Expression<Func<TEntity, bool>>)RemoveCastsVisitor.Visit(node);
                return Queryable.Where<TEntity>(Queryable.Where<TEntity>(this.Get(), node), predicate);
            }
            return this.Get(predicate);
        }

        public virtual TEntity FirstOrDefault()
        {
            return Queryable.FirstOrDefault<TEntity>((IQueryable<TEntity>)this._dbSet);
        }

        public virtual TEntity FirstOrDefaultActive()
        {
            if (typeof(IActive).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> node = (TEntity q) => !((IActive)q).IsDeleted;
                node = (Expression<Func<TEntity, bool>>)RemoveCastsVisitor.Visit(node);
                return Queryable.FirstOrDefault<TEntity>((IQueryable<TEntity>)this._dbSet, node);
            }
            return this.FirstOrDefault();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable.FirstOrDefault<TEntity>((IQueryable<TEntity>)this._dbSet, predicate);
        }

        public virtual TEntity FirstOrDefaultActive(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(IActive).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> node = (TEntity q) => !((IActive)q).IsDeleted;
                node = (Expression<Func<TEntity, bool>>)RemoveCastsVisitor.Visit(node);
                return Queryable.FirstOrDefault<TEntity>(Queryable.Where<TEntity>((IQueryable<TEntity>)this._dbSet, predicate), node);
            }
            return this.FirstOrDefault(predicate);
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
}
