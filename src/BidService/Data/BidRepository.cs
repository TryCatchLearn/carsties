using BidService.DTOs;
using BidService.Models;
using Dapper;
using Mapster;

namespace BidService.Data;

public class BidRepository(BidDbContext dbContext) : IBidRepository
{
    public async Task<Auction?> GetAuctionAsync(string auctionId)
    {
        return await dbContext.DbConnection.QuerySingleOrDefaultAsync<Auction>(
            "select * from auctions where id = @auctionId", new { auctionId }
        );
    }

    public async Task<Bid?> GetHighestBidAsync(string auctionId)
    {
        return await dbContext.DbConnection.QuerySingleOrDefaultAsync<Bid>(
            """
            select * from bids
            where auctionid = @auctionId and bidstatus in (@Accepted, @AcceptedBelowReserve)
            order by amount desc
            limit 1
            """, new
            {
                auctionId,
                Accepted = (int)BidStatus.Accepted,
                AcceptedBelowReserve = (int)BidStatus.AcceptedBelowReserve
            }
        );
    }

    public async Task<BidDto> InsertBidAsync(Bid bid)
    {
        await dbContext.DbConnection.ExecuteAsync(
            """
            insert into bids (id, auctionid, bidder, bidtime, amount, bidstatus)
            values (@Id, @AuctionId, @Bidder, @BidTime, @Amount, @BidStatus)
            """, bid
        );
        return bid.Adapt<BidDto>();
    }

    public async Task<IEnumerable<BidDto>> GetBidsForAuctionAsync(string auctionId)
    {
        var bids = await dbContext.DbConnection.QueryAsync<Bid>(
            """
            select * from bids
            where auctionid = @auctionId
            order by bidtime desc
            """, new { auctionId }
        );
        
        return bids.Adapt<IEnumerable<BidDto>>();
    }

    public async Task<Auction> CreateAuctionAsync(Auction auction)
    {
        await dbContext.DbConnection.ExecuteAsync(
            """
            insert into auctions (id, auctionend, seller, reserveprice)
            values (@Id, @AuctionEnd, @Seller, @ReservePrice)
            """, auction
        );
        
        return auction;
    }

    public async Task MarkAuctionFinishedAsync(string auctionId)
    {
        await dbContext.DbConnection.ExecuteAsync(
            """
            update auctions set finished = true where id = @auctionId
            """, new { auctionId }
        );
    }

    public async Task<Bid?> GetWinningBidAsync(string auctionId)
    {
        return await dbContext.DbConnection.QuerySingleOrDefaultAsync<Bid>(
            """
            select * from bids
            where auctionid = @auctionId and bidstatus = @Accepted
            order by amount desc
            limit 1
            """, new
            {
                auctionId,
                Accepted = (int)BidStatus.Accepted
            }
        );
    }
}