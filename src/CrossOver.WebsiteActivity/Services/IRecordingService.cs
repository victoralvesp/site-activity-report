namespace CrossOver.WebsiteActivity.Services
{
    public interface IRecordingService
    {
        void Register(string key, int activityValue, DateTime? registrationTime = null, CancellationToken cancellationToken = default);
    }
}