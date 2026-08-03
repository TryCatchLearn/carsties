using AuctionService.DTOs;
using AuctionService.Entities;
using Mapster;

namespace AuctionService.RequestHelpers;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Auction, AuctionDto>()
            .Map(dest => dest.Make, src => src.Item.Make)
            .Map(dest => dest.Model, src => src.Item.Model)
            .Map(dest => dest.Year, src => src.Item.Year)
            .Map(dest => dest.Color, src => src.Item.Color)
            .Map(dest => dest.Mileage, src => src.Item.Mileage)
            .Map(dest => dest.ImageUrl, src => src.Item.ImageUrl)
            .Map(dest => dest.Description, src => src.Item.Description);

        config.NewConfig<CreateAuctionDto, Auction>()
            .Map(dest => dest.Item, src => src);

        config.NewConfig<UpdateAuctionDto, Item>()
            .IgnoreNullValues(true);
    }
}