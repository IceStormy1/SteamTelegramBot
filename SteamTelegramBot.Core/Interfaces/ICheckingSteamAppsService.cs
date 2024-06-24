using SteamTelegramBot.Abstractions.Models.Applications;

namespace SteamTelegramBot.Core.Interfaces;

public interface ICheckingSteamAppsService
{
    Task<(int TotalSuccessfulUpdatedApplications, int TotalApplicationsNotFound)> UpdateApplications(IReadOnlyCollection<AppItemDto> allApplications);
    Task<List<int>> UpdateTrackedApplications(IReadOnlyCollection<AppItemDto> allApplications);
}