using CabNotificationService.Infrastructures.DbContexts;
using CabNotificationService.Infrastructures.Repositories.Base;
using CabNotificationService.Infrastructures.Repositories.Interfaces;
using CabNotificationService.Models.Entities;

namespace CabNotificationService.Infrastructures.Repositories
{
    public class NotificationUserIdMaterializedViewRepository
        : BaseRepository<NotificationUserIdMaterializedView>
        , INotificationUserIdMaterializedViewRepository
    {
        public NotificationUserIdMaterializedViewRepository(ScyllaDbContext context) : base(context)
        {
        }
    }
}
