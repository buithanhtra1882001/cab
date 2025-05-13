using CabNotificationService.Constants;
using CabNotificationService.Hubs;
using CabNotificationService.Infrastructures.Repositories.Interfaces;
using CabNotificationService.Models.Commands;
using CabNotificationService.Models.Dtos;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace CabNotificationService.Handlers.Notification
{
    public partial class NotificationHandler : INotificationHandler<CreateNotificationCommand>
    {
        public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notificationRepository = _seviceProvider.GetRequiredService<INotificationRepository>();

                if (!request.UserIds.Any() || request.UserIds is null || request.Actor is null)
                    return;

                var message = GetNotificationMessage(request);

                var newNotifications = request.UserIds.Select(userId => new Models.Entities.Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ActorId = request.Actor.UserId,
                    ReferenceId = request.ReferenceId,
                    NotificationType = request.NotificationType,
                    Message = message,
                    IsRead = false,
                    ReferenceUrl = request.ReferenceUrl,
                    Type = request.Type
                }).ToList();
                await notificationRepository.CreateRangeAsync(newNotifications);

                var notificationResponses = newNotifications.Select(notification => new NotificationResponse
                {
                    Id = notification.Id,
                    Actor = request.Actor,
                    ReferenceId = notification.ReferenceId,
                    NotificationType = notification.NotificationType,
                    Message = message,
                    ReferenceUrl = notification.ReferenceUrl,
                    CreatedAt = notification.CreatedAt,
                });

                if (!notificationResponses.Any())
                    return;

                await SendNotificationAsync(notificationResponses, request.UserIds);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error CreateNotification {ex.Message}");
            }
        }

        private async Task SendNotificationAsync(IEnumerable<NotificationResponse> notificationResponses, List<Guid> userIds)
        {
            try
            {
                var connections = NotificationHub.NotificationConnections.GetConnections(userIds);
                var connectionIds = connections.ToList();
                if (connections.Any())
                {
                    foreach (var notification in notificationResponses)
                    {
                        await _hubContext.Clients.Clients(connectionIds).SendAsync("SendNotification", notification);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error SendNotification {ex.Message}");
            }
        }

        private string GetNotificationMessage(CreateNotificationCommand request)
        {
            return request.NotificationType switch
            {
                NotificationConstant.CreatePost => $" đã tạo một bài viết mới",
                NotificationConstant.CreatePostInGroup => $" đã tạo một bài viết mới trong nhóm",
                NotificationConstant.LikePost => $" đã thích bài viết của bạn",
                NotificationConstant.UnLikePost => $" không thích bài viết của bạn",
                NotificationConstant.Comment => $" đã bình luận về bài viết của bạn",
                NotificationConstant.ReplyComment => $" đã trả lời bình luận của bạn",
                NotificationConstant.LikeComment => $" đã thích bình luận của bạn",
                NotificationConstant.UnLikeComment => $" không thích bình luận của bạn",
                NotificationConstant.Share => $" đã chia sẻ bài viết của bạn",
                NotificationConstant.FriendRequest => $" đã gửi một yêu cầu kết bạn",
                NotificationConstant.AcceptFriend => $" đã đồng ý làm bạn",
                NotificationConstant.DonatePost => $" đã quyên góp {request.DonateAmount} cho bài viết của bạn",
                NotificationConstant.DonateCreator => $" đã quyên góp {request.DonateAmount} cho bạn",
                NotificationConstant.InviteJoinGroup => $" đã mời bạn tham gia nhóm",
                NotificationConstant.BirthdayFriend => $" chúc mừng sinh nhật bạn",
                NotificationConstant.AdminDeletePostInGroup => $" đã xóa bài viết trong nhóm",
                NotificationConstant.AdminDeletePostInFanpage => $" đã xóa bài viết trên trang fanpage",
                NotificationConstant.FollowUser => $" đã theo dõi bạn",
                NotificationConstant.UnFollowUser => $" đã hủy theo dõi bạn",
                NotificationConstant.QualifiedCreator => $"Bạn đã đủ điều kiện trở thành creator",
                NotificationConstant.SystemDonatePost => $"Bạn đã được cộng {request.DonateAmount} và tài khoản của mình",
                NotificationConstant.SystemDonateCreator => $"Bạn đã được cộng {request.DonateAmount} và tài khoản của mình",
                NotificationConstant.CreateWithdrawalRequest => $"Yêu cầu rút tiền {request.DonateAmount} từ tài khoản đang chờ phê duyệt",
                NotificationConstant.ApproveRequestCreateWithdrawal => $"Đã phê duyệt yêu cầu rút tiền {request.DonateAmount} từ tài khoản",


                NotificationConstant.CreateRequestReceiveDonation => $"Yêu cầu nhận tiền quyên góp đang chờ phê duyệt",
                NotificationConstant.ApproveRequestReceiveDonation => $"Đã phê duyệt yêu cần nhận tiền quyên góp",
                _ => "Bạn có một thông báo mới",
            };
        }


    }
}
