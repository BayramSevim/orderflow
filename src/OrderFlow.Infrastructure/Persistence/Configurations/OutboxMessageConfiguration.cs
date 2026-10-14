using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Infrastructure.Outbox;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Type)
                    .HasMaxLength(200);

            builder.Property(d => d.Payload)
                    .HasColumnType("jsonb");

            builder.Property(d => d.OccurredAt);

            builder.Property(d => d.ProcessedAt);

            builder.Property(d => d.Error);
        }
    }
}