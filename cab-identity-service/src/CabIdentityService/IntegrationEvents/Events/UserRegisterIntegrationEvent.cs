using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace WCABNetwork.Cab.IdentityService.IntegrationEvents.Events
{
    public record UserRegisterIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string UserName { get; private set; }
        public bool IsVerifyEmail { get; private set; }

        public UserRegisterIntegrationEvent(string userId, string email, string fullName, string userName, bool isVerifyEmail)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            UserName = userName;
            IsVerifyEmail = isVerifyEmail;
        }
    }
}
