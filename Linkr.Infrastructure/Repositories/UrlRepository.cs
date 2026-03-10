using Microsoft.EntityFrameworkCore;
using Linkr.Domain.Entities;
using Linkr.Domain.Repositories;
using Linkr.Infrastructure.Data;

namespace Linkr.Infrastructure.Repositories;

public class UrlRepository : IUrlRepository
{
	private readonly ShotiContext _context;

	public IUnitOfWork UnitOfWork => _context;

	public UrlRepository(ShotiContext context)
	{
		this._context = context;
	}

	public async Task<IEnumerable<Url>> GetAnonymousUrls()
	{
		return await _context
			.Urls
			.AsNoTracking()
			.Include(u => u.Clicks)
			.Where(x => x.UserId == null)
			.ToListAsync();
	}

	public async Task<IEnumerable<Url>> GetUrls()
	{
		return await _context
			.Urls
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<IEnumerable<Url>> GetByUser(string userId)
	{
		return await _context
			.Urls
			.AsNoTracking()
			.Include(u => u.Clicks)
			.Where(x => x.UserId == userId)
			.ToListAsync();
	}

	public async Task<Url> GetByCode(string code)
	{
		return await _context
			.Urls
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Code == code);
	}

	public async Task<Url> GetById(int id)
	{
		return await _context
			.Urls
			.AsNoTracking()
			.Include(u => u.Clicks)
			.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<Url> Create(Url url)
	{
		var result = await _context
			.Urls
			.AddAsync(url);
		return result.Entity;
	}

	public bool Delete(int id)
	{
		var entity = _context.Urls.Remove(new Url { Id = id }).Entity;
		return entity.Id == id;
	}
}