using System.ComponentModel.DataAnnotations;

namespace AuctionService.DTOs;

public class CreateAuctionDto
{
    [Required] public string Make { get; set; } = "";
    [Required] public string Model { get; set; } = "";   
    public int Year { get; set; }
    [Required] public string Color { get; set; } = "";
    public int Mileage { get; set; }
    [Required] public string ImageUrl { get; set; } = "";
    [Required] public string Description { get; set; } = "";
    public int? ReservePrice { get; set; }
    public DateTime AuctionEnd { get; set; }
}