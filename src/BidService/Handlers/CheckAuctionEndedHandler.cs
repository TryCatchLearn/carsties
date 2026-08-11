using BidService.Data;
using BidService.Messages;
using Contracts;
using Wolverine;

namespace BidService.Handlers;

public class CheckAuctionEndedHandler(IBidRepository repository, IMessageBus bus)
{
    public async Task Handle(CheckAuctionEnded message)
    {
        var auction = await repository.GetAuctionAsync(message.AuctionId);

        if (auction == null || auction.Finished) return;

        await repository.MarkAuctionFinishedAsync(auction.Id);

        var winningBid = await repository.GetWinningBidAsync(auction.Id);

        await bus.PublishAsync(new AuctionFinished
        {
            ItemSold = winningBid != null,
            AuctionId = auction.Id,
            Winner = winningBid?.Bidder,
            Amount = winningBid?.Amount ?? 0,
            Seller = auction.Seller,
        });
    }
}