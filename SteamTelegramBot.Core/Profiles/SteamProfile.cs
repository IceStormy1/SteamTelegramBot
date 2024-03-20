using AutoMapper;
using SteamTelegramBot.Abstractions.Models.Applications;
using SteamTelegramBot.Clients.Models;
using SteamTelegramBot.Data.Entities;

namespace SteamTelegramBot.Core.Profiles;

/// <summary>
/// Represents a profile for mapping Steam models.
/// </summary>
internal sealed class SteamProfile : Profile
{
    public SteamProfile()
    {
        CreateMap<AppItem, AppItemDto>();

        CreateMap<SteamSuggestItem, SteamAppEntity>()
            .ForMember(destination => destination.Title, options => options.MapFrom(exp => exp.Name))
            .ForMember(destination => destination.HeaderImage, options => options.MapFrom(exp => exp.ImageLink))
            .ForMember(destination => destination.SteamAppId, options => options.MapFrom(exp => exp.AppId))
            ;
    }
}