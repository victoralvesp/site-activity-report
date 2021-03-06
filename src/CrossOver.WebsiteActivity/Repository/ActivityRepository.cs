using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;

namespace CrossOver.WebsiteActivity.Repository
{

    public class ActivityRepository : IActivityRepository
    {
        ConcurrentDictionary<string, Activity[]> _activities = new();
        public event EventHandler<Activity>? OnAdded;
        public event EventHandler<Activity>? OnRemoved;
        public IEnumerable<string> Keys => _activities.Keys;

        public void PurgeActivity(Activity activity)
        {
            var key = activity.Key;
            if (_activities.GetValueOrDefault(key)?.Contains(activity) ?? false)
            {
                OnRemoved?.Invoke(this, activity);
                _activities.AddOrUpdate(key, (_) => new Activity[] { }, (_, currentValues) => currentValues.Where(act => act != activity).ToArray());
            }
        }
        public void PushActivity(Activity activity)
        {
            var key = activity.Key;
            OnAdded?.Invoke(this, activity);
            _activities.AddOrUpdate(key, (_) => new[] { activity }, (_, currentValues) => currentValues.Append(activity).ToArray());
        }

        public IEnumerable<Activity> GetActivities(string key) => _activities.GetValueOrDefault(key, Array.Empty<Activity>());
    }
}