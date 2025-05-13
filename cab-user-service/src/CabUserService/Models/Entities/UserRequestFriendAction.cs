using CabUserService.Constants;
using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    /// <summary>
    /// Bảng đề xuất kết bạn
    /// </summary>
    public class UserRequestFriendAction : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Id user hiện tại
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Id user đề xuất
        /// </summary>
        public Guid RequestUserId { get; set; }
        
        /// <summary>
        /// Trạng thái 
        /// </summary>
        public ACTION_FRIEND_TYPE StatusAction { get; set; } //0 từ chối, 1 gửi lời kết bạn
        
        /// <summary>
        /// Đồng ý kết bạn
        /// </summary>
        public ACCEPTANCE_STATUS AcceptStatus { get; set; }
        
        /// <summary>
        /// Loại gợi ý kết bạn
        /// </summary>
        public REQUEST_TYPE TypeRequest { get; set; }
    }
}
