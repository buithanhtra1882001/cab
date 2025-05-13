using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<UpdateNotifyAdminCommand, bool>
    {
        public async Task<bool> Handle(
            UpdateNotifyAdminCommand request,
            CancellationToken cancellationToken)
        {
            return await Handle(request);
        }

        private async Task<bool> Handle(UpdateNotifyAdminCommand request)
        {
            await Task.CompletedTask;

            try
            {
                var postNotifyAdminRepository = _seviceProvider.GetRequiredService<IPostNotifyAdminRepository>();
                postNotifyAdminRepository.UpdateIsAcceptHide(request.IsAcceptHide, request.IdNotify);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Cannot update notify admin {request.IdNotify}, errors: {e.Message}");
                throw new ApiValidationException(e.Message);
            }

            return true;
        }
    }
}