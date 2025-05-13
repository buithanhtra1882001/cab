using CabCommon.Infrastructures.Messages.Commands;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MassTransit;

namespace CabUserService.Infrastructures.Consumers
{
    public class RollbackCoinConsumer : IConsumer<RollbackCoin>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RollbackCoinConsumer> _logger;
        public RollbackCoinConsumer(IUserRepository userRepository, ILogger<RollbackCoinConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<RollbackCoin> context)
        {
            try
            {
                _logger.LogDebug("Begin Consume method. Input {@context}", context);

                var message = context.Message;
                var user = await _userRepository.GetByIdAsync(message.UserId);

                user.Coin += message.CoinAmount;

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Successfully rollback coin for Refund Saga {id}. User Id {userId}. Coin rolled back: {coin}. New coin balance: {newCoin}. Refund Saga Id {RefundRequestId}", message.RefundRequestId, user.Id, message.CoinAmount, user.Coin, message.RefundRequestId);

                _logger.LogDebug("Finish Consume method.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong. Refund Saga Id {RefundRequestId}", context.Message.RefundRequestId);

                throw;
            }
        }
    }
}
