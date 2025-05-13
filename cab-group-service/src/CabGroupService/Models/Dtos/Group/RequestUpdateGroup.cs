using CabGroupService.Constants;
using FluentValidation;

namespace CabGroupService.Models.Dtos
{
    public class RequestUpdateGroup
    {
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public GroupType GroupType { get; set; }
        public string? GroupTagline { get; set; }
        public Guid CreatedByUser { get; set; }
        public string? PrivacySettings { get; set; }
        public string? Rules { get; set; }
        public string? Location { get; set; }
        public string? WebsiteURL { get; set; }
        public string? ContactEmail { get; set; }
        public string? TagList { get; set; }
        public bool ApprovalRequired { get; set; } = false;
    }

    public class GroupUpdateRequestValidator : AbstractValidator<RequestUpdateGroup>
    {
        public GroupUpdateRequestValidator()
        {
            RuleFor(p => p.GroupName).NotEmpty();
            RuleFor(p => p.ContactEmail).EmailAddress().When(p => !string.IsNullOrWhiteSpace(p.ContactEmail));
            RuleFor(p => p.GroupType).IsInEnum();
            RuleFor(p => p.CreatedByUser).NotEmpty();
        }
    }
}
