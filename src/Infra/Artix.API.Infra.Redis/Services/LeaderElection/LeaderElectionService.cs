namespace Artix.API.Infra.Redis.Services.LeaderElection;

using Microsoft.Extensions.Hosting;
using RedLockNet;

public class LeaderElectionService : BackgroundService
{
    private readonly LeaderState _leaderState;
    private readonly IDistributedLockFactory _lockFactory;

    public LeaderElectionService(LeaderState leaderState, IDistributedLockFactory lockFactory)
    {
        this._leaderState = leaderState;
        this._lockFactory = lockFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var lockHandle = await this._lockFactory.CreateLockAsync("leader-lock", TimeSpan.FromSeconds(10));
            if (lockHandle.IsAcquired)
            {
                this._leaderState.IsLeader = true;
                Console.WriteLine("🔵 I am the leader!");
                await Task.Delay(5000, stoppingToken);
            }
            else
            {
                this._leaderState.IsLeader = false;
                Console.WriteLine("🟡 Standby...");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
public record LeaderState
{
    public volatile bool IsLeader;
}
