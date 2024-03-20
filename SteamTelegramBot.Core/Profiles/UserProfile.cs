using AutoMapper;
using SteamTelegramBot.Data.Entities;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Profiles;

/// <summary>
/// Represents a profile for mapping User models.
/// </summary>
internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserEntity>()
            .ForMember(destination => destination.TelegramId, options => options.MapFrom(exp => exp.Id))
            .ForMember(destination => destination.Id, options => options.Ignore())
            ;
    }
}