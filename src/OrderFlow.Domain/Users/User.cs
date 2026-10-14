using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Users
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }
        public Guid? DealerId { get; private set; }

        public User(string email, string passwordHash, string role, Guid? dealerId)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new DomainException("Email is required");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password is required");
            if (role != "Admin" && role != "Dealer")
                throw new DomainException("Role must be Admin or Dealer");

            if (role.Equals("Dealer"))
            {
                if (dealerId is null)
                    throw new DomainException("Dealer users must belong to a dealer");
            }

            if (role == "Admin" && dealerId is not null)
                throw new DomainException("Admin users cannot belong to a dealer");

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            DealerId = dealerId;
        }
    }
}