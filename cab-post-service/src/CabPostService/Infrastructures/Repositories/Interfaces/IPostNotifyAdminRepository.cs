using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostNotifyAdminRepository
    {
        void InsertOne(PostNotifyAdmin model);
        void UpdateIsAcceptHide(bool isAcceptHide, string idNotify);
    }
}
