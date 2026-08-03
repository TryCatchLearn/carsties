using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities;

[Table("Items")]
public class Item
{
    [MaxLength(36)]
    public string Id { get; set; } =  Guid.NewGuid().ToString();
    [MaxLength(100)]
    public required string Make { get; set; }
    [MaxLength(100)]
    public required string Model { get; set; }
    [MaxLength(100)]
    public required string Color { get; set; }
    [MaxLength(2000)]
    public required string Description { get; set; }

    public int Year { get; set; }
    public int Mileage { get; set; }
    
    [MaxLength(200)]
    public required string ImageUrl { get; set; }
    
    // navigation property
    public Auction? Auction { get; set; } = null;
}