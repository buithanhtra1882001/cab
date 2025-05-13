using FluentValidation;

namespace CabGroupService.Models.Dtos.GroupMembers
{
    public class RequestApproval
    {
        public Guid GroupID { get; set; }
        public Guid UserRequest { get; set; }
        public Guid UserApproval { get; set; }
        public bool Approval { get; set; }
    }

    public class RequestApprovalValidator : AbstractValidator<RequestApproval>
    {
        public RequestApprovalValidator()
        {
            RuleFor(p => p.GroupID).NotEmpty();
            RuleFor(p => p.UserRequest).NotEmpty();
            RuleFor(p => p.UserApproval).NotEmpty();
            RuleFor(p => p.Approval).NotEmpty();
        }
    }
}
