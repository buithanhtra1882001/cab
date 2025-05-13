using CabUserService.Constants;
using CabUserService.Cqrs.Requests.Commands;
using FluentValidation;

namespace JREWallet.UserInfo.Cqrs.Handlers.CommandValidators;

public class UpdateProfileCommandValidators : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidators()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.UserIdRequired);
    }
}
