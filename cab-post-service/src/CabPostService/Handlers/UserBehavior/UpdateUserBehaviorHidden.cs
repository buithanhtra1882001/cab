using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Models.Commands;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.UserBehavior
{

    public partial class UserBehaviorHandler :
        ICommandHandler<UpdateUserBehaviorHiddenCommand, bool>
    {
        public async Task<bool> Handle(
            UpdateUserBehaviorHiddenCommand request,
            CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            if (!request.HideAll)
            {
                var userBehavior = await db.UserBehaviors
                    .FirstOrDefaultAsync(x =>
                                         x.UserId == request.UserId &&
                                         x.PostId == request.PostId);

                if (userBehavior is null)
                {
                    _logger.LogError("UserBehavior not found");
                    throw new Exception("UserBehavior not found.");
                }

                userBehavior.IsHidden = true;
                db.UserBehaviors.Update(userBehavior);
            }
            else
            {
                var userBehaviors = await db.UserBehaviors.AsNoTracking()
                                                          .Where(x => x.UserId == request.UserId)
                                                          .ToListAsync();
                if (!userBehaviors.Any())
                {
                    _logger.LogError("User behaviors not found");
                    throw new Exception("User behaviors not found.");
                }

                userBehaviors.ForEach(x => x.IsHidden = true);

                db.UserBehaviors.UpdateRange(userBehaviors);
            }

            await db.SaveChangesAsync();

            return true;
        }
    }
}
