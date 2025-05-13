using CabNotificationService.Constants;
using CabNotificationService.Handlers.Interfaces;
using CabNotificationService.Infrastructures.Exceptions;
using CabNotificationService.Infrastructures.Repositories.Interfaces;
using CabNotificationService.Models.Commands;

namespace CabNotificationService.Handlers.Notification
{
    public partial class NotificationHandler : ICommandHandler<ToggleNotificationReadStatusCommand, bool>
    {
        public async Task<bool> Handle(ToggleNotificationReadStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notificationRepository = _seviceProvider.GetRequiredService<INotificationRepository>();

                var existNotification = await notificationRepository
                    .GetOneAsync(x => x.Id == request.Id);
                existNotification.IsRead = !existNotification.IsRead;

                if (existNotification is null)
                    throw new AppException(AppError.INVALID_PARAMETERS, "Notification does not exist");

                var result = await notificationRepository.UpdateAsync(
                    new Models.Entities.Notification(),
                    x => x.Id == existNotification.Id && x.CreatedAt == existNotification.CreatedAt,
                    x => new Models.Entities.Notification { IsRead = existNotification.IsRead });

                return result;
            }
            catch (Exception ex)
            {
                throw new AppException($"An error occurred while toggling notification status Erorr = {ex}");
            }
        }
    }
}
