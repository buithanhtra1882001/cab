using AutoMapper;
using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class CreateUserCommandHandler : INotificationHandler<CreateUserDetailCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, ILogger<CreateUserCommandHandler> logger, IMapper mapper)
        {
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Handle(CreateUserDetailCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                var user = _mapper.Map<User>(notification.UserDetail);
                user.SequenceId = notification.SequenceId;
                user.Status = 1;
                await _userRepository.CreateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on CreateUserCommandHandler.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
