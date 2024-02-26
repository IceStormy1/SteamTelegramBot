using Quartz;

namespace SteamTelegramBot.Jobs.Jobs;

[DisallowConcurrentExecution]
internal class CheckingSteamAppsJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}