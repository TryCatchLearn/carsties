namespace BidService.Models;

public class Bid
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string AuctionId { get; set; }
    public required string Bidder { get; set; }
    public DateTime BidTime { get; set; } = DateTime.UtcNow;
    public int Amount { get; set; }
    public BidStatus BidStatus { get; set; }
}