using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Api.Models;

namespace CrossOver.WebsiteActivity.Api.Repository
{
    public class ActivityRepository
    {
        ConcurrentDictionary<string, Activity[]> _activities = new();
        public event EventHandler<Activity>? OnAdded;
        public event EventHandler<Activity>? OnRemoved;
        public IEnumerable<string> Keys => _activities.Keys;

        public void PurgeActivity(Activity activity)
        {
            var key = activity.Key;
            OnRemoved?.Invoke(this, activity);
            _activities.AddOrUpdate(key, (_) => new Activity[] { }, (_, currentValues) => currentValues.Where(act => act != activity).ToArray());
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