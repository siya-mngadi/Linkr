using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Linkr.Domain.Entities;
using Linkr.Infrastructure.Data;

namespace Linkr.Infrastructure.SchemaDefinition;

public class UrlClickSchemaDefinition : IEntityTypeConfiguration<UrlClick>
{
	public void Configure(EntityTypeBuilder<UrlClick> builder)
	{
		builder.ToTable("UrlClicks", ShotiContext.DefaultSchema);
		builder.HasKey(x => x.Id);

		builder.Property(p => p.IpAddress)
			.IsRequired();

		builder.Property(b => b.UrlId)
			.IsRequired();
	}
}