using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Dealers;

namespace OrderFlow.Infrastructure.Persistence
{
    public class OrderFlowDbContext : DbContext
    {
        public DbSet<Dealer> Dealers => Set<Dealer>();

        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : base(options)
        {
        }
    }
}