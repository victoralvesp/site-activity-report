using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Repository;
using Microsoft.Extensions.Hosting;

namespace CrossOver.WebsiteActivity.HostedServices
{
    /// <summary>
    /// Cleans out old activity values to ensure reporting only gets the right values
    /// </summary>
    public class JanitorHostedService : BackgroundService
    {
        private readonly IActivityRepository _repository;
        private bool _isExecuting;

        public JanitorHostedService(IActivityRepository repository)
        {
            _repository = repository;
        }

        public TimeSpan ActivityHoldingTime { get; private set; } = TimeSpan.FromHours(12);
        public bool IsExecuting { get => _isExecuting; set => _isExecuting = value; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _isExecuting = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = _repository.Keys.Select(key => Task.Run(() => PurgeOlderActivities(key)));

                await Task.WhenAll(tasks);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            _isExecuting = false;
        }

        private void PurgeOlderActivities(string key)
        {
            var activities = _repository.GetActivities(key);
            foreach (var activity in activities.Where(act => IsOldActivity(act)))
            {
                _repository.PurgeActivity(activity);
            }
        }

        private bool IsOldActivity(Models.Activity act)
        {
            var activityAge = DateTime.UtcNow - act.RegisterDate;
            return activityAge >= ActivityHoldingTime;
        }
    }
}