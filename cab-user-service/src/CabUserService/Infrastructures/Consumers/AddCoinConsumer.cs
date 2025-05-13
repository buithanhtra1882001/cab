using CabCommon.Infrastructures.Messages.Commands;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MassTransit;

namespace CabUserService.Infrastructures.Consumers;

public class AddCoinConsumer : IConsumer<AddCoin>
{
    private readonly ILogger<AddCoinConsumer> _logger;
    private readonly IUserRepository _userRepository;

    public AddCoinConsumer(ILogger<AddCoinConsumer> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }
    public async Task Consume(ConsumeContext<AddCoin> context)
    {
        try
        {
            _logger.LogDebug("Begin Consume method. Input {@Context}", context);
            
            var message = context.Message;
            var userId = message.UserId;
            var amount = message.CoinAmount;

            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new Exception("User Id Not Exist");
            }

            var newAmount = user.Coin + amount;

            user.Coin = newAmount;

            user.UpdatedAt = DateTime.Now;

            var rowAffected = await _userRepository.UpdateAsync(user);

            if (rowAffected != 1)
            {
                var e = new Exception(
                    $"Unexpected number of affected row on updating user balance from TopUpPayment event. Expected: 1. Actual: {rowAffected}");

                e.Data.Add("PaymentTopUpEvent", message);

                throw e;
            }

            _logger.LogInformation("Updated user {UserId} balance to {Balance} from PaymentTopUpEvent {message}", userId,
                newAmount, message);
            _logger.LogDebug("Finished Consume method");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Something went wrong");

            throw;
        }
    }
}