using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Repository;

namespace CrossOver.WebsiteActivity.Services
{
    /// <summary>
    /// Responsible for registering new activity values
    /// </summary>
    public class RecordingService : IRecordingService
    {
        ConcurrentQueue<Activity> _toProcessQueue = new();
        private bool _isProcessCycleRunning;
        private IActivityRepository _repo;

        public RecordingService(IActivityRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Registers a new value in the repository
        /// </summary>
        /// <param name="key">Activity's key</param>
        /// <param name="activityValue">Activity's value</param>
        /// <param name="registrationTime">Registration time</param>
        /// <exception cref="ArgumentException">Thrown when key is invalid</exception>

        public void Register(string key, int activityValue, DateTime? registrationTime = null)
        {
            registrationTime ??= DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' should not be null or whitespace.", nameof(key));
            }

            var activity = new Activity(key, activityValue)
            {
                RegisterDate = registrationTime.Value
            };

            _toProcessQueue.Enqueue(activity);
            EnsureProcessingIsStarted();
        }

        private void EnsureProcessingIsStarted()
        {
            if (!_isProcessCycleRunning)
            {
                _ = Task.Run(() => ProcessingCycle());
            }
        }

        private void ProcessingCycle()
        {
            _isProcessCycleRunning = true;
            while (_toProcessQueue.TryDequeue(out var next))
            {
                _repo.PushActivity(next);
            }
            _isProcessCycleRunning = false;
        }

    }
}