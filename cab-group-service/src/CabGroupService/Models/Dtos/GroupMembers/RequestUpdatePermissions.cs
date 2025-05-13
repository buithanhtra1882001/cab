using CabGroupService.Constants;
using FluentValidation;

namespace CabGroupService.Models.Dtos.GroupMembers
{
    public class RequestUpdatePermissions
    {
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
        public Guid UserDecentralization { get; set; }
        public GroupPermissions Permissions { get; set; }
    }

    public class RequestUpdatePermissionsValidator : AbstractValidator<RequestUpdatePermissions>
    {
        public RequestUpdatePermissionsValidator()
        {
            RuleFor(p => p.GroupID).NotEmpty();
            RuleFor(p => p.UserID).NotEmpty();
            RuleFor(p => p.UserDecentralization).NotEmpty();
            RuleFor(p => p.Permissions).IsInEnum();
        }
    }
}
