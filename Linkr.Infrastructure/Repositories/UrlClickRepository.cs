using Microsoft.EntityFrameworkCore;
using Linkr.Domain.Entities;
using Linkr.Domain.Repositories;
using Linkr.Infrastructure.Data;

namespace Linkr.Infrastructure.Repositories;

public class UrlClickRepository : IUrlClickRepository
{
	private readonly ShotiContext _context;

	public IUnitOfWork UnitOfWork => _context;

	public UrlClickRepository(ShotiContext context)
	{
		this._context = context;
	}

	public async Task<IEnumerable<UrlClick>> GetByUrl(int urlId)
	{
		return await _context
			.UrlClicks
			.AsNoTracking()
			.Where(x => x.UrlId == urlId)
			.ToListAsync();
	}

	public async Task<UrlClick> GetById(int id)
	{
		return await _context
			.UrlClicks
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<UrlClick> Create(UrlClick click)
	{
		var result = await _context
			.UrlClicks
			.AddAsync(click);
		return result.Entity;
	}

	public UrlClick Update(UrlClick click)
	{
		var result = _context
			.UrlClicks
			.Update(click);
		return result.Entity;
	}

	public bool Delete(int id)
	{
		var entity = _context.UrlClicks.Remove(new UrlClick { Id = id }).Entity;
		return entity.Id == id;
	}
}