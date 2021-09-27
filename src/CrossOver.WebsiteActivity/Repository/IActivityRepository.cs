using System;
using System.Collections.Generic;
using CrossOver.WebsiteActivity.Models;

namespace CrossOver.WebsiteActivity.Repository
{
    /// <summary>
    /// Manages storage for activities
    /// </summary>
    public interface IActivityRepository
    {
        IEnumerable<string> Keys { get; }

        event EventHandler<Activity>? OnAdded;
        event EventHandler<Activity>? OnRemoved;

        IEnumerable<Activity> GetActivities(string key);
        void PurgeActivity(Activity activity);
        void PushActivity(Activity activity);
    }
}