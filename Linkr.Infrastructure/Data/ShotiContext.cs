using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Linkr.Domain.Entities;
using Linkr.Domain.Repositories;
using Linkr.Infrastructure.SchemaDefinition;

namespace Linkr.Infrastructure.Data;

public class ShotiContext : IdentityDbContext<User>, IUnitOfWork
{
	public const string DefaultSchema = "dbo";

	public ShotiContext(DbContextOptions<ShotiContext> options)
		: base(options)
	{
	}

	public DbSet<Url> Urls { get; set; }
	public DbSet<UrlClick> UrlClicks { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new UrlSchemaDefinition());
		modelBuilder.ApplyConfiguration(new UrlClickSchemaDefinition());
		base.OnModelCreating(modelBuilder);
	}

	public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
	{
		await SaveChangesAsync(cancellationToken);
		return true;
	}
}