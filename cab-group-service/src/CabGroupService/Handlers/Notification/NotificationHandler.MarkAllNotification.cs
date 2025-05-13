using CabGroupService.Constants;
using CabGroupService.Handlers.Interfaces;
using CabGroupService.Infrastructures.Exceptions;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.Models.Commands;

namespace CabGroupService.Handlers.Notification
{
    public partial class NotificationHandler : ICommandHandler<MarkAllNotificationCommand, bool>
    {
        public async Task<bool> Handle(MarkAllNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notificationRepository = _seviceProvider.GetRequiredService<INotificationRepository>();
                var notificationUserIdRepository = _seviceProvider.GetRequiredService<INotificationUserIdMaterializedViewRepository>();

                var existNotifications = await notificationUserIdRepository
                    .GetListAsync(x => x.UserId == request.UserId);
                existNotifications = existNotifications.ToList();
                if (!existNotifications.Any())
                    return true;

                foreach (var notification in existNotifications)
                {
                    await notificationRepository.UpdateAsync(
                    new Models.Entities.Notification(),
                    x => x.Id == notification.Id && x.CreatedAt == notification.CreatedAt,
                    x => new Models.Entities.Notification
                    {
                        IsRead = request.IsRead
                    });
                }

                return true;
            }
            catch (Exception ex)
            {

                throw new AppException($"An error occurred while marking all notifications, Erorr = {ex}");
            }
        }
    }
}
