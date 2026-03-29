namespace TimeTrackerRepo.Services
{
    // FrozenDateWatcher.cs
    using Microsoft.AspNetCore.SignalR;
    using TimeTrackerRepo.Data;
    using TimeTrackerRepo.Functions;

    public class FrozenDateWatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<FrozenDateHub> _hub;
        private string? _lastSeen; // store last broadcasted string

        public FrozenDateWatcher(IServiceScopeFactory scopeFactory, IHubContext<FrozenDateHub> hub)
        {
            _scopeFactory = scopeFactory;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // adjust interval as you like (5–30s is typical)
            var interval = TimeSpan.FromSeconds(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var data = scope.ServiceProvider.GetRequiredService<IDataFunctions>();

                    // Should return "YYYY-MM-DD 00:00:00.000"
                    var current = await data.GetFrozenDateStringAsync();

                    if (!string.IsNullOrWhiteSpace(current) && !string.Equals(current, _lastSeen, StringComparison.Ordinal))
                    {
                        _lastSeen = current;
                        await _hub.Clients.All.SendAsync("FrozenDateChanged", current, cancellationToken: stoppingToken);
                    }
                }
                catch (OperationCanceledException) { /* shutting down */ }
                catch (Exception ex)
                {
                    // log and keep going (don’t crash the service)
                    Console.Error.WriteLine($"FrozenDateWatcher error: {ex}");
                }

                try { await Task.Delay(interval, stoppingToken); }
                catch (OperationCanceledException) { }
            }
        }
    }

}
