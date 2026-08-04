using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Auction> Auctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Auction>()
            .HasOne(x => x.Item)
            .WithOne(x => x.Auction)
            .HasForeignKey<Item>(x => x.Id)
            .IsRequired();
    }
}