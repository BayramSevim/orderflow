using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Products;

namespace OrderFlow.Infrastructure.Persistence
{
    public class OrderFlowDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Dealer> Dealers => Set<Dealer>();
        public DbSet<Product> Products => Set<Product>();

        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderFlowDbContext).Assembly);
        }

        Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) => base.SaveChangesAsync(cancellationToken);
    }
}