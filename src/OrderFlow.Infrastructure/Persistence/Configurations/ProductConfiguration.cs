using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Code)
               .HasMaxLength(20)
               .IsRequired();

            builder.HasIndex(d => d.Code)
                .IsUnique();

            builder.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.Price)
                .HasPrecision(18, 2);

        }
    }
}