namespace Linkr.Domain.Repositories;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}