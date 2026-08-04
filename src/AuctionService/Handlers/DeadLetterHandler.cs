using Contracts;

namespace AuctionService.Handlers;

public class DeadLetterHandler(ILogger<DeadLetterHandler> logger)
{
    public Task Handle(AuctionCreated message)
    {
        logger.LogError("Dead letter handler triggered for {AuctionId} {Make} {Model}", 
            message.Id, message.Make, message.Model);
        
        return Task.CompletedTask;
    }
}