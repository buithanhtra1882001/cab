using FluentValidation;

namespace CabGroupService.Models.Dtos
{
    public class RequestLeaveGroup
    {
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
    }

    public class RequestLeaveGroupValidator : AbstractValidator<RequestLeaveGroup>
    {
        public RequestLeaveGroupValidator()
        {
            RuleFor(p => p.GroupID).NotEmpty();
            RuleFor(p => p.UserID).NotEmpty();
        }
    }
}
