using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Application.Common.Messaging;
using OrderFlow.Domain.Exceptions;
using OrderFlow.Domain.Users;

namespace OrderFlow.Application.Users.Queries
{
    public class LoginHandler : IQueryHandler<LoginQuery, string>
    {
        // constructor: IUserRepository, IPasswordHasher, ITokenService
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginQuery query, CancellationToken cancellationToken)
        {
            User? user = await _userRepository.GetByEmailAsync(query.Email, cancellationToken);

            if (user is null || !_passwordHasher.Verify(query.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password");

            return _tokenService.CreateToken(user);
        }
    }
}