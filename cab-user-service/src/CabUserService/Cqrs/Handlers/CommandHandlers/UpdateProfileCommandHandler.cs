using CabUserService.Cqrs.Requests.Commands;
using CabUserService.Services.Interfaces;
using MediatR;

namespace JREWallet.UserInfo.Cqrs.Handlers.CommandHandlers;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
{

    #region Properties

    private readonly IUserService _userService;

    #endregion

    #region Constructor

    public UpdateProfileCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    #endregion

    public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateProfileAsync(request, cancellationToken);
    }
}

