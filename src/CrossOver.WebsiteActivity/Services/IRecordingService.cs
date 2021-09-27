using System;

namespace CrossOver.WebsiteActivity.Services
{
    /// <summary>
    /// Responsible for registering new activity values
    /// </summary>
    public interface IRecordingService
    {
        /// <summary>
        /// Registers a new value in the repository
        /// </summary>
        /// <param name="key">Activity's key</param>
        /// <param name="activityValue">Activity's value</param>
        /// <param name="registrationTime">Registration time</param>
        /// <exception cref="ArgumentException">Thrown when key is invalid</exception>
        void Register(string key, int activityValue, DateTime? registrationTime = null);
    }
}