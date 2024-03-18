using SteamTelegramBot.Common.Enums;
using SteamTelegramBot.Common.Extensions;
using System.Globalization;
using System.Text;

namespace SteamTelegramBot.Abstractions.Models.Applications;

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
    public int AppId { get; init; }

    public SteamSuggestItem()
    {

    }

    public SteamSuggestItem(string listingData)
    {
        Name = HandleSpecialCharacters(listingData.Split(GreaterThanSymbol)[2].Split(LessThanSymbol)[0]);
        StoreLink = HandleSpecialCharacters(GetStoreLink(listingData));
        ImageLink = HandleSpecialCharacters(GetImageLink(listingData));

        AppId = GetAppId(listingData);
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

    private static int GetAppId(string input)
    {
        var appIdString = input.Split(EqualSign)[1]
            .Replace(Quote, string.Empty)
            .Split(Space)[0];

        return int.Parse(appIdString);
    }

    private static string GetImageLink(string input)
    {
        try
        {
            if (input.Split(GreaterThanSymbol).Length < ImageStartIndex)
                return null;

            var imageLink = input.Split(GreaterThanSymbol)[ImageStartIndex]
                .Replace(Quote, string.Empty)
                .Split(EqualSign).ElementAtOrDefault(1);

            if (imageLink?.Contains(LinkQueryStringSeparator) ?? false)
                imageLink = imageLink.Split(LinkQueryStringSeparator)[0];

            return imageLink;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static (PriceType PriceType, decimal? Price) GetPricing(string input)
    {
        var test = input.Split("<div class=\"match_subtitle\">")[1].Split(LessThanSymbol)[0].ToLower();
        var priceCandidate = input.Split("<div class=\"match_subtitle\">")[1]
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
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var bytes = Encoding.Default.GetBytes(input);
        return Encoding.UTF8.GetString(bytes);
    }
}
