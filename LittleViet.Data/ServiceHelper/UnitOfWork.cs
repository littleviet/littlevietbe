using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Data.Models.Global;

public partial interface IUnitOfWork
{
    T GetService<T>();
    void Save();
    Task<int> SaveAsync();
}

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(IServiceProvider service, LittleVietContext context, IHttpContextAccessor httpContextAccessor)
    {
        this._service = service;
        this._context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    protected readonly IServiceProvider _service;
    protected readonly LittleVietContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public T GetService<T>()
    {
        return _service.GetService<T>();
    }
    public void Save()
    {
        _context.SaveChanges();
    }
    
    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public IDbContextTransaction BeginTransation()
    {
        return _context.Database.BeginTransaction();
    }

    private void SetBaseAuditInfo() //TODO: use this
    {
        var successful = Guid.TryParse(_httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);
        var userId = successful ? (Guid?)tempId : null;
        
        var entries = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = entityEntry.Entity as AuditableEntity;
            entity!.UpdatedDate = DateTime.UtcNow;
            entity.UpdatedBy = userId;

            if (entityEntry.State == EntityState.Added)
            {
                entity.IsDeleted = false;
                entity.CreatedDate = DateTime.UtcNow;
                entity.CreatedBy = userId;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                (entityEntry as EntityEntry<AuditableEntity>)!.Property(e => e.CreatedDate).IsModified = false;
            }
        }
    }
}

