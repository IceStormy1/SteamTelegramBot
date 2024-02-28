using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Common.Extensions;
using System.Globalization;
using System.Text;

namespace SteamTelegramBot.Abstractions.Models;

public class SteamSuggestItem
{
    private const string CurrencySplitSymbol = "pуб.";
    private const string MatchNameTag = "\"><div class=\"match_name\"";
    private const string ItemKeyStartTag = "data-ds-itemkey=\"";
    private const string LinkStartTag = "href=\"";
    private const string LinkQueryStringSeparator = "?";
    private const string FreeToPlay = "free to play";

    private const char EqualSign = '=';
    private const string Quote = "\"";
    private const char Space = ' ';
    private const char GreaterThanSymbol = '>';
    private const char LessThanSymbol = '<';

    private const byte ImageStartIndex = 4;

    public string Name { get; init; }
    public string StoreLink { get; init; }
    public string ImageLink { get; init; }
    public decimal? Price { get; init; }
    public PriceType PriceType { get; init; }
    public string AppId { get; init; }

    public SteamSuggestItem(string listingData)
    {
        Name = HandleSpecialCharacters(listingData.Split(GreaterThanSymbol)[2].Split(LessThanSymbol)[0]);
        StoreLink = HandleSpecialCharacters(GetStoreLink(listingData));
        AppId = HandleSpecialCharacters(GetAppId(listingData));
        ImageLink = HandleSpecialCharacters(GetImageLink(listingData));

        (PriceType, Price) = GetPricing(listingData);
    }

    private static string GetStoreLink(string input)
    {
        var itemKeyParts = input.Split(ItemKeyStartTag)[1];
        var matchNameParts = itemKeyParts.Split(MatchNameTag)[0];
        var resultLink = matchNameParts.Split(LinkStartTag)[1];

        if (resultLink.Contains(LinkQueryStringSeparator))
            resultLink = resultLink.Split(LinkQueryStringSeparator)[0];

        return resultLink;
    }

    private static string GetAppId(string input)
        => input.Split(EqualSign)[1]
            .Replace(Quote, string.Empty)
            .Split(Space)[0];

    private static string GetImageLink(string input)
    {
        var imageLink = input.Split(GreaterThanSymbol)[ImageStartIndex]
            .Replace(Quote, string.Empty)
            .Split(EqualSign)[1];

        if (imageLink.Contains(LinkQueryStringSeparator))
            imageLink = imageLink.Split(LinkQueryStringSeparator)[0];

        return imageLink;
    }

    private static (PriceType PriceType, decimal? Price) GetPricing(string input)
    {
        var priceCandidate = input.Split(GreaterThanSymbol)[7]
            .Split(LessThanSymbol)[0]
            .ToLower();

        var priceType = GetPriceType(priceCandidate);
        if (priceType is not PriceType.CostsMoney)
            return (PriceType: priceType, Price: null);

        var priceString = priceCandidate.Split(separator: CurrencySplitSymbol)[0].Replace(',', '.');
        decimal? price = string.IsNullOrWhiteSpace(priceString)
            ? null
            : decimal.Parse(priceString, NumberStyles.Number, CultureInfo.InvariantCulture);

        return (priceType, price);
    }

    private static PriceType GetPriceType(string priceCandidate)
    {
        if (priceCandidate.Length < 2)
            return PriceType.NotAvailable;

        if (priceCandidate.Contains(PriceType.FreeToPlay.GetEnumDisplayName()) || priceCandidate.Contains(FreeToPlay))
            return PriceType.FreeToPlay;

        if (priceCandidate.Contains(PriceType.Demo.GetEnumDisplayName()))
            return PriceType.Demo;

        return PriceType.CostsMoney;
    }

    private static string HandleSpecialCharacters(string input)
    {
        var bytes = Encoding.Default.GetBytes(input);
        return Encoding.UTF8.GetString(bytes);
    }
}
