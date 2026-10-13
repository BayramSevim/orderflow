using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.DealerId)
                    .IsRequired();

            builder.Property(d => d.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

            builder.Property(d => d.CreatedAt);

            builder.HasMany(o => o.Items)
                    .WithOne()
                    .HasForeignKey("OrderId")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.Items)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(o => o.Total);
        }
    }
}