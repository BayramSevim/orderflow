using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");

            builder.HasKey(d => d.Id);

            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.Property(d => d.ProductId)
                    .IsRequired();

            builder.Property(d => d.ProductName)
                    .HasMaxLength(200)
                    .IsRequired();

            builder.Property(d => d.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(d => d.Quantity);

            builder.Ignore(o => o.LineTotal);
        }
    }
}