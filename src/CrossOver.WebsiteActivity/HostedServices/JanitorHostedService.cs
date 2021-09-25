using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Repository;

namespace CrossOver.WebsiteActivity.HostedServices
{
    public class JanitorHostedService : BackgroundService
    {
        private readonly IActivityRepository _repository;

        public JanitorHostedService(IActivityRepository repository)
        {
            _repository = repository;
        }

        public TimeSpan ActivityHoldingTime { get; private set; } = TimeSpan.FromHours(12);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var tasks = _repository.Keys.Select(key => Task.Run(() => PurgeOlderActivities(key)));

                await Task.WhenAll(tasks);
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private void PurgeOlderActivities(string key)
        {
            var activities = _repository.GetActivities(key);
            foreach (var activity in activities.Where(act => act.RegisterDate - DateTime.UtcNow > ActivityHoldingTime))
            {
                _repository.PurgeActivity(activity);
            }
        }
    }
}