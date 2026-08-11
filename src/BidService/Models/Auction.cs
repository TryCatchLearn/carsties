namespace BidService.Models;

public class Auction
{
    public required string Id { get; set; }
    public DateTime AuctionEnd { get; set; }
    public required string Seller { get; set; }
    public int ReservePrice { get; set; }
    public bool Finished { get; set; }
}