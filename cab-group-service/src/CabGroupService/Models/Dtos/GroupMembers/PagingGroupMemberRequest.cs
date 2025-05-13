using FluentValidation;

namespace CabGroupService.Models.Dtos.GroupMembers
{
    public class PagingGroupMemberRequest : PagingRequest
    {
        public Guid GroupID { get; set; }
        public Guid UserID { get; set; }
    }

    public class PagingGroupMemberRequestValidator : AbstractValidator<PagingGroupMemberRequest>
    {
        public PagingGroupMemberRequestValidator()
        {
            RuleFor(p => p.GroupID).NotEmpty();
            RuleFor(p => p.UserID).NotEmpty();
        }
    }
}
