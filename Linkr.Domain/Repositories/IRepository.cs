namespace Linkr.Domain.Repositories;

public interface IRepository
{
	IUnitOfWork UnitOfWork { get; }
}