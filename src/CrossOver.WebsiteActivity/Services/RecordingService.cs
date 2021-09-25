using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Repository;

namespace CrossOver.WebsiteActivity.Services
{
    public class RecordingService
    {
        ConcurrentQueue<Activity> _toProcessQueue = new();
        private bool _isProcessCycleRunning;
        private ActivityRepository _repo;

        public RecordingService(ActivityRepository repo)
        {
            _repo = repo;
        }

        public void Register(string key, int activityValue, CancellationToken cancellationToken = default)
        {
            var activity = new Activity(key, activityValue);

            _toProcessQueue.Enqueue(activity);
            EnsureProcessingIsStarted(cancellationToken);
        }

        private void EnsureProcessingIsStarted(CancellationToken cancellationToken = default)
        {
            if (!_isProcessCycleRunning)
            {
                _ = Task.Run(() => ProcessingCycle(cancellationToken));
            }
        }

        private void ProcessingCycle(CancellationToken cancellationToken)
        {
            _isProcessCycleRunning = true;
            while(!cancellationToken.IsCancellationRequested && _toProcessQueue.TryDequeue(out var next))
            {
                _repo.PushActivity(next);
            }
            _isProcessCycleRunning = false;
        }

    }
}