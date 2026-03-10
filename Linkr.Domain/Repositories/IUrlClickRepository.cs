using Linkr.Domain.Entities;

namespace Linkr.Domain.Repositories;

public interface IUrlClickRepository : IRepository
{
	Task<IEnumerable<UrlClick>> GetByUrl(int urlId);
	Task<UrlClick> GetById(int id);
	Task<UrlClick> Create(UrlClick click);
	UrlClick Update(UrlClick click);
	bool Delete(int id);
}