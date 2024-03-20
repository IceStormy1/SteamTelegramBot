using System.ComponentModel.DataAnnotations;

namespace SteamTelegramBot.Common.Enums;

/// <summary>
/// Represents the type of price for an application.
/// </summary>
public enum PriceType
{
    /// <summary>
    /// Indicates the game costs money.
    /// </summary>
    CostsMoney = 1,

    /// <summary>
    /// Indicates the application is free to play.
    /// </summary>
    [Display(Name = "бесплатно")]
    FreeToPlay = 2,

    /// <summary>
    /// Indicates the price availability is not known.
    /// </summary>
    [Display(Name = "Неизвестно")]
    NotAvailable = 3,

    /// <summary>
    /// Indicates the game is a demo version.
    /// </summary>
    [Display(Name = "демо")]
    Demo = 4
}