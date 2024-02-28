using System.ComponentModel.DataAnnotations;

namespace SteamTelegramBot.Common.Enums;

public enum PriceType
{
    CostsMoney = 1,

    [Display(Name = "бесплатно")]
    FreeToPlay = 2,

    [Display(Name = "Неизвестно")]
    NotAvailable = 3,

    [Display(Name = "демо")]
    Demo = 4
}