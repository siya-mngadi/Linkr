using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Linkr.Domain.Entities;
using Linkr.Infrastructure.Data;

namespace Linkr.Infrastructure.SchemaDefinition;

public class UrlSchemaDefinition : IEntityTypeConfiguration<Url>
{
	public void Configure(EntityTypeBuilder<Url> builder)
	{
		builder.ToTable("Urls", ShotiContext.DefaultSchema);
		builder.HasKey(x => x.Id);

		builder.Property(p => p.OriginalUrl)
			.IsRequired();

		builder.Property(b => b.ShortUrl)
			.IsRequired();

		builder.Property(b => b.Code)
			.IsRequired()
			.HasMaxLength(10);

		builder.HasIndex(b => b.Code)
			.IsUnique();

		builder.Property(b => b.CreatedAtUtc)
			.ValueGeneratedOnAdd();

		builder.HasMany(b => b.Clicks)
			.WithOne(x => x.Url)
			.HasForeignKey(b => b.UrlId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}