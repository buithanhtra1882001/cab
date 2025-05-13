using CabNotificationService.Infrastructures.DbContexts;
using CabNotificationService.Infrastructures.Repositories.Base;
using CabNotificationService.Infrastructures.Repositories.Interfaces;
using CabNotificationService.Models.Entities;

namespace CabNotificationService.Infrastructures.Repositories
{
    public class NotificationRepository
        : BaseRepository<Notification>
        , INotificationRepository
    {
        public NotificationRepository(ScyllaDbContext context) : base(context)
        {
        }
    }
}
