using CabPostService.Constants;
using CabPostService.Cqrs.Requests.Commands;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace TecsPjService.Apis.Cqrs.CommandValidators.Delete
{
    public class DonateReceiverCommandValidators : AbstractValidator<DonateReceiverCommand>
    {
        #region Constructor

        public DonateReceiverCommandValidators()
        {
            RuleFor(x => x.PostId).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.PostIdRequired);

            RuleFor(x => x.ReceiverId).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.ReceiverIdRequired);

            RuleFor(x => x.Title).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.TitleRequired);

            RuleFor(x => x.Content).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.ContentRequired);

            RuleFor(x => x.Coin).NotNull().NotEmpty()
                .WithErrorCode(ValidationMessages.CoinRequired);
        }

        #endregion
    }
}
