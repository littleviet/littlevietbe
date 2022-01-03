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
    public UnitOfWork(IServiceProvider service, LittleVietContext context)
    {
        this._service = service;
        this._context = context;
    }

    protected readonly IServiceProvider _service;
    protected readonly LittleVietContext _context;

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
}

