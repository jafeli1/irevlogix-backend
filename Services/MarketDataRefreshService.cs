using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Services;

namespace irevlogix_backend.Services
{
    public class MarketDataRefreshService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MarketDataRefreshService> _logger;
        private Timer? _timer;

        public MarketDataRefreshService(IServiceProvider serviceProvider, ILogger<MarketDataRefreshService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Market Data Refresh Service started");
            
            var now = DateTime.UtcNow;
            var cstNow = TimeZoneInfo.ConvertTimeFromUtc(now, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var nextRun = new DateTime(cstNow.Year, cstNow.Month, cstNow.Day, 0, 1, 0);
            
            if (cstNow.TimeOfDay > new TimeSpan(0, 1, 0))
            {
                nextRun = nextRun.AddDays(1);
            }
            
            var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRun, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            var timeUntilFirstRun = nextRunUtc - now;
            
            if (timeUntilFirstRun < TimeSpan.Zero)
            {
                timeUntilFirstRun = TimeSpan.FromDays(1) + timeUntilFirstRun;
            }

            _logger.LogInformation("Next market data refresh scheduled at: {NextRun} UTC (in {Minutes} minutes)", 
                nextRunUtc, timeUntilFirstRun.TotalMinutes);

            _timer = new Timer(
                RefreshMarketData, 
                null, 
                timeUntilFirstRun,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Market Data Refresh Service stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void RefreshMarketData(object? state)
        {
            try
            {
                _logger.LogInformation("Starting scheduled market data refresh at {Time} UTC", DateTime.UtcNow);

                using var scope = _serviceProvider.CreateScope();
                var marketIntelligenceService = scope.ServiceProvider.GetRequiredService<MarketIntelligenceService>();

                var refreshedCount = await marketIntelligenceService.RefreshMarketDataAsync();

                _logger.LogInformation("Market data refresh completed. Refreshed {Count} products", refreshedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scheduled market data refresh");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
