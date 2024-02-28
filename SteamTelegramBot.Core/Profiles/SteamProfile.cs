using AutoMapper;
using SteamTelegramBot.Abstractions.Models;
using SteamTelegramBot.Clients.Models;

namespace SteamTelegramBot.Core.Profiles;

public sealed class SteamProfile : Profile
{
    private const string CurrencySplitSymbol = "pуб.";

    public SteamProfile()
    {
        CreateMap<AppItem, AppItemDto>();

        CreateMap<AppDetail, AppDetailDto>()
            .ForMember(dest => dest.FullGameId, act =>
            {
                act.PreCondition(src => src.FullGame != null);
                act.MapFrom(src => src.FullGame.AppID);
            })
            .ForMember(dest => dest.Price, act =>
            {
                act.PreCondition(src => src.PriceOverview != null && !string.IsNullOrWhiteSpace(src.PriceOverview.FinalFormatted));
                act.MapFrom(src => GetPriceFromString(src.PriceOverview.FinalFormatted));
            })
            .ForMember(dest => dest.ReleaseDate, act =>
            {
                act.PreCondition(src => src.ReleaseDate != null);

                DateOnly releaseDate;
                act.MapFrom(src => DateOnly.TryParse(src.ReleaseDate.Date, out releaseDate) ? releaseDate : (DateOnly?)null);
            })
            ;
    }

    private static decimal GetPriceFromString(string price)
    {
        // "123 руб." => ["123"]
        var splitPrice = price.Split(separator: CurrencySplitSymbol);
        
        return splitPrice.Length == default 
            ? default 
            : decimal.Parse(splitPrice[0]);
    }
}