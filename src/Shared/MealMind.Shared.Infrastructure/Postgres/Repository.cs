using MealMind.Shared.Abstractions.Kernel.Database;
using MealMind.Shared.Abstractions.Kernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace MealMind.Shared.Infrastructure.Postgres;

public class Repository<TEntity, TDbContext> : IRepository<TEntity>
    where TEntity : class, IEntity
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    //in derived class constructor should be public
    protected Repository(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

    public void Remove(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);
}