using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Users;

namespace OrderFlow.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (await context.Users.AnyAsync())
                return;

            var email = configuration["Seed:AdminEmail"]!;
            var password = configuration["Seed:AdminPassword"]!;

            context.Users.Add(new User(email, hasher.Hash(password), "Admin", null));
            await context.SaveChangesAsync();
        }
    }
}