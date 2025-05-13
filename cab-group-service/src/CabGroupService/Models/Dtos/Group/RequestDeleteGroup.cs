using FluentValidation;

namespace CabGroupService.Models.Dtos.Group
{
    public class RequestDeleteGroup
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }

    public class RequestDeleteGroupValidator : AbstractValidator<RequestDeleteGroup>
    {
        public RequestDeleteGroupValidator()
        {
            RuleFor(p => p.UserId).NotEmpty();
            RuleFor(p => p.GroupId).NotEmpty();
        }
    }
}
