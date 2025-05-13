using CabCommon.Infrastructures.Messages.Commands;
using CabCommon.Infrastructures.Messages.Events;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MassTransit;

namespace CabUserService.Infrastructures.Consumers
{
    public class SubtractCoinConsumer : IConsumer<SubtractCoin>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SubtractCoinConsumer> _logger;

        public SubtractCoinConsumer(IUserRepository userRepository, ILogger<SubtractCoinConsumer> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubtractCoin> context)
        {
            try
            {
                _logger.LogDebug("Begin Consume method. Input {@context}", context);

                var message = context.Message;
                var user = await _userRepository.GetByIdAsync(message.UserId);

                if (user.Coin < message.CoinAmount)
                {
                    await context.Publish<CoinSubtractFail>(new { message.RefundRequestId });

                    _logger.LogWarning("User {userId} NOT have enough coin match the refund amount. Current coin balance: {coin}. Coin need for refund: {coinRequest}. Refund Saga Id {RefundRequestId}", user.Id, user.Coin, message.CoinAmount, message.RefundRequestId);
                }
                else
                {
                    user.Coin -= message.CoinAmount;

                    await _userRepository.UpdateAsync(user);

                    await context.Publish<CoinSubtracted>(new { message.RefundRequestId });

                    _logger.LogInformation("Subtract coin success. User Id {id}. Coin subtracted: {coin}. Coin left in balance: {newCoin}. Refund Saga Id {RefundRequestId}", user.Id, message.CoinAmount, user.Coin, message.RefundRequestId);
                }

                _logger.LogDebug("Finish Consume method.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong. Refund Saga Id {Id}", context.Message.RefundRequestId);

                throw;
            }
        }
    }
}
