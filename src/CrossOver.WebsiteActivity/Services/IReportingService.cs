namespace CrossOver.WebsiteActivity.Services
{
    /// <summary>
    /// Responsible for reporting total values for a given key
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Gets the total value for <paramref name="key"/>
        /// </summary>
        /// <param name="key">Activity key to report</param>
        /// <returns>The total value</returns>
        long GetTotal(string key);
    }
}