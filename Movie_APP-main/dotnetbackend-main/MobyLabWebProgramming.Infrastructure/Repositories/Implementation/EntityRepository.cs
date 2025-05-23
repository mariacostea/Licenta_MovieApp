using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Database;
namespace MobyLabWebProgramming.Infrastructure.Repositories.Implementation;



public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : BaseEntity
{
    private readonly WebAppDatabaseContext _db;

    public EntityRepository(WebAppDatabaseContext db)
    {
        _db = db;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id) =>
        await _db.Set<TEntity>().FindAsync(id);

    public async Task<List<TEntity>> GetAllAsync() =>
        await _db.Set<TEntity>().ToListAsync();

    public async Task AddAsync(TEntity entity)
    {
        await _db.Set<TEntity>().AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _db.Set<TEntity>().Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}
