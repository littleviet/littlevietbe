using System.Security.Claims;
using LittleViet.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace LittleViet.Domain.Repositories;

public partial interface IUnitOfWork
{
    T GetService<T>();
    void Save();
    Task<int> SaveAsync();
    IDbContextTransaction BeginTransaction();
}


internal class UnitOfWork : IUnitOfWork
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
        SetBaseAuditInfo();
        _context.SaveChanges();
    }
    
    public async Task<int> SaveAsync()
    {
        SetBaseAuditInfo();
        return await _context.SaveChangesAsync();
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }

    private void SetBaseAuditInfo()
    {
        var successful = Guid.TryParse(_httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid tempId);
        var userId = successful ? (Guid?)tempId : null;
        
        var entries = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity
                        && e.State is EntityState.Added or EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var entity = entityEntry.Entity as AuditableEntity;
            var utcNow = DateTime.UtcNow; 
            entity.UpdatedDate = utcNow;
            entity.UpdatedBy = userId;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entity.IsDeleted = false;
                    entity.CreatedDate = utcNow;
                    entity.CreatedBy = userId;
                    break;
                case EntityState.Modified when !entity.IsDeleted:
                    // (entityEntry as EntityEntry<AuditableEntity>)!.Property(e => e.CreatedDate).IsModified = false;
                    break;
                case EntityState.Modified when entity.IsDeleted && entityEntry.Property(nameof(entity.IsDeleted)).IsModified:
                    entity.DeletedBy = userId;
                    entity.DeletedDate = utcNow;
                    break;
            }
        }
    }
}

