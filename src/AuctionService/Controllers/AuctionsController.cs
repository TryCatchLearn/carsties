using System.Globalization;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")] // http://localhost:7001/api/auctions GET, POST, PUT
public class AuctionsController(AuctionDbContext context, IDbContextOutbox<AuctionDbContext> outbox) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date)
    {
        var query = context.Auctions.AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            if (!DateTime.TryParse(date, CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal
                    | DateTimeStyles.AssumeUniversal, out var parsedDate))
            {
                return BadRequest("Invalid date");
            }
            
            query = query.Where(x => x.UpdatedAt > parsedDate);
        }
        
        var auctions = await query
            .OrderBy(x => x.Item.Make)
            .ThenBy(x => x.Item.Model)
            .ProjectToType<AuctionDto>()
            .ToListAsync();

        return auctions;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(string id)
    {
        var auction = await context.Auctions
            .ProjectToType<AuctionDto>()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (auction == null)
        {
            return NotFound();
        }
        
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = createAuctionDto.Adapt<Auction>();
        
        auction.Seller = User.Identity?.Name ?? throw new Exception("User has not been authenticated");
        
        context.Auctions.Add(auction);

        var newAuction = auction.Adapt<AuctionDto>();
        await outbox.PublishAsync(newAuction.Adapt<AuctionCreated>());
        await outbox.SaveChangesAndFlushMessagesAsync();
        
        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuction(string id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (updateAuctionDto.Make == "foo") throw new Exception("bar");

        if (auction == null)
        {
            return NotFound();
        }

        if (auction.CurrentHighBid > 0)
        {
            return BadRequest("Cannot update an auction that has bids");
        }

        if (auction.Seller != User.Identity?.Name) return Forbid();
        
        auction.UpdatedAt = DateTime.UtcNow;
        
        var updatedAuction = updateAuctionDto.Adapt(auction.Item);

        await outbox.PublishAsync(updatedAuction.Adapt<AuctionUpdated>());
        await outbox.SaveChangesAndFlushMessagesAsync();
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(string id)
    {
        var auction = await context.Auctions.FindAsync(id);

        if (auction == null)
        {
            return NotFound();
        }

        if (auction.CurrentHighBid > 0)
        {
            return BadRequest("Cannot delete auction that has bids");
        }
        
        if (auction.Seller != User.Identity?.Name) return Forbid();
        
        context.Auctions.Remove(auction);
        
        await outbox.PublishAsync(auction.Adapt<AuctionDeleted>());
        await outbox.SaveChangesAndFlushMessagesAsync();
        
        return NoContent();
    }

    [Authorize]
    [HttpPost("test")]
    public ActionResult<string> AuthTest()
    {
        var name = User.Identity?.Name;
        
        return Ok($"{name} has been authenticated");
    }
}