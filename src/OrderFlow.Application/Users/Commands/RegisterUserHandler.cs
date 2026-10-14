using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Application.Dealers;
using OrderFlow.Domain.Dealers;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Users;

namespace OrderFlow.Application.Users.Commands
{
    public record RegisterUserCommand(string Email, string Password, string Role, Guid? DealerId);
    public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDealerRepository _dealerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(IUserRepository userRepository, IDealerRepository dealerRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _dealerRepository = dealerRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            User? findUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (findUser is not null)
                throw new ConflictException("User is alread defined");

            if (command.Role == "Dealer")
            {
                if (command.DealerId is null)
                    throw new DomainException("Dealer users must belong to a dealer");

                Dealer? dealer = await _dealerRepository.GetByIdAsync(command.DealerId.Value, cancellationToken);
                if (dealer is null)
                    throw new NotFoundException("Dealer is not defined");
            }

            var hash = _passwordHasher.Hash(command.Password);

            User user = new User(command.Email, hash, command.Role, command.DealerId);
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id;

        }
    }
}