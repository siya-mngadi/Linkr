using Linkr.Domain.Entities;

namespace Linkr.Domain.Repositories;

public interface IUrlRepository : IRepository
{
	Task<IEnumerable<Url>> GetAnonymousUrls();
	Task<IEnumerable<Url>> GetUrls();
	Task<IEnumerable<Url>> GetByUser(string userId);
	Task<Url> GetByCode(string code);
	Task<Url> GetById(int id);
	Task<Url> Create(Url url);
	bool Delete(int id);
}