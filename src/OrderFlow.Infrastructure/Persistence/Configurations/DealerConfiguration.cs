using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Dealers;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class DealerConfiguration : IEntityTypeConfiguration<Dealer>
    {
        public void Configure(EntityTypeBuilder<Dealer> builder)
        {
            builder.ToTable("dealers");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Code)
               .HasMaxLength(20)
               .IsRequired();

            builder.HasIndex(d => d.Code)
                .IsUnique();

            builder.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.CreditLimit)
                .HasPrecision(18, 2);
        }
    }
}