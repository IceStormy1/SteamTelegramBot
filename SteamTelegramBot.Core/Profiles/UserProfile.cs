using AutoMapper;
using SteamTelegramBot.Data.Entities;
using Telegram.Bot.Types;

namespace SteamTelegramBot.Core.Profiles;

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