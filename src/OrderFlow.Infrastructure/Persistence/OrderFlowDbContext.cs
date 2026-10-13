using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Dealers;

namespace OrderFlow.Infrastructure.Persistence
{
    public class OrderFlowDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Dealer> Dealers => Set<Dealer>();

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