using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Repository;

namespace CrossOver.WebsiteActivity.Services
{

    public class ReportingService : IReportingService
    {
        private readonly IActivityRepository _repo;
        private readonly ConcurrentDictionary<string, long> _totalValuesIndex = new();
        private readonly ConcurrentDictionary<string, bool> _isProcessCycleRunningForKey = new();

        public ReportingService(IActivityRepository repo)
        {
            _repo = repo;
            _repo.OnAdded += (_, activity) => ProcessActivityAdded(activity);
            _repo.OnRemoved += (_, activity) => ProcessActivityRemoved(activity);
            ProcessCurrentKeys();
        }

        private void ProcessCurrentKeys()
        {
            foreach (var key in _repo.Keys)
            {
                StartingValueForKey(key);
            }
        }

        private void ProcessActivityRemoved(Activity activity)
        {
            var key = activity.Key;
            var value = activity.Value;
            _totalValuesIndex.AddOrUpdate(key, _ => -value, (_, currentTotal) => currentTotal - value);
        }

        private void ProcessActivityAdded(Activity activity)
        {
            var key = activity.Key;
            var value = activity.Value;
            _totalValuesIndex.AddOrUpdate(key, _ => value, (_, currentTotal) => currentTotal + value);
        }

        private void StartingValueForKey(string key)
        {
            int valueForKey = _repo.GetActivities(key).Sum(act => act.Value);
            _totalValuesIndex.AddOrUpdate(key, (_) => valueForKey, (_, currentTotal) => currentTotal + valueForKey);
        }

        public long GetTotal(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' should not be null or whitespace.", nameof(key));
            }

            return _totalValuesIndex.GetValueOrDefault(key, 0);
        }

    }
}