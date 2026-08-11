using EvoManage.Application.Abstractions.Persistence.Repositories;

namespace EvoManage.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context = context;

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await Context.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}