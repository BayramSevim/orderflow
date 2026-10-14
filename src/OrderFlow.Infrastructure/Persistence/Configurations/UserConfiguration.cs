using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Users;

namespace OrderFlow.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(d => d.Id);

            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.Property(d => d.Email)
               .HasMaxLength(200)
               .IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(d => d.PasswordHash)
               .HasMaxLength(200)
               .IsRequired();

            builder.Property(d => d.Role)
              .HasMaxLength(10)
              .IsRequired();

            builder.Property(d => d.DealerId);


        }
    }
}