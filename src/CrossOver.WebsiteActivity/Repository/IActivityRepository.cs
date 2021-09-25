using CrossOver.WebsiteActivity.Models;

namespace CrossOver.WebsiteActivity.Repository
{
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