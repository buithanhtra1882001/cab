using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Handlers.UserBehavior
{

    public partial class UserBehaviorHandler :
        ICommandHandler<UserBehaviorRequest, Guid>
    {
        public async Task<Guid> Handle(
            UserBehaviorRequest request,
            CancellationToken cancellationToken)
        {
            var userBehavior = _mapper.Map<Models.Entities.UserBehavior>(request);
            userBehavior.Id = Guid.NewGuid();
            userBehavior.IsHidden = false;

            var userBehaviorRepository = _seviceProvider.GetRequiredService<IUserBahaviorRepository>();
            await userBehaviorRepository.AddUserBahavior(userBehavior);

            return userBehavior.Id;
        }
    }
}
