using CabGroupService.Constants;
using FluentValidation;

namespace CabGroupService.Models.Dtos
{
    public class RequestJoinGroup
    {
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
        public JoinMethod JoinMethod { get; set; }
        public Guid InvitedBy { get; set; }
        public string? JoinReason { get; set; } = "Text";
    }

    public class RequestJoinGroupValidator : AbstractValidator<RequestJoinGroup>
    {
        public RequestJoinGroupValidator()
        {
            RuleFor(p => p.GroupID).NotEmpty();
            RuleFor(p => p.UserID).NotEmpty();
            RuleFor(p => p.JoinMethod).IsInEnum();
        }
    }
}
