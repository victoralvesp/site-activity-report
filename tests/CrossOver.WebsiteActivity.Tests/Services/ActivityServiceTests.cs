using System.Diagnostics;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class ActivityServiceTests
    {
        public ActivityServiceTests()
        {
            _bigRepository = new ActivityRepository();
            _biggerRepository = new ActivityRepository();
            PrepareRepository(_bigRepository, 250);
            PrepareRepository(_biggerRepository, 2500);
        }
        [Fact]
        public void Register_Should_Have_Constant_Response_TimeAsync()
        {
            //Given a website with hundreds of activities recorded and another with millions
            var activityServiceWithHundreds = new RecordingService(_bigRepository);
            var activityServiceWithMillions = new RecordingService(_biggerRepository);
            //When registering a new activity on each site
            var timeWithHundreds = checkTimeToRegister(activityServiceWithHundreds);
            var timeWithMillions = checkTimeToRegister(activityServiceWithMillions);

            //Then
            timeWithHundreds.Should().BeCloseTo(timeWithMillions, 5, "time to register is O(1) up to 5 milli");

            long checkTimeToRegister(RecordingService service)
            {
                var watch = new Stopwatch();
                watch.Start();
                var (key, value) = RandomActivity();
                service.Register(key, value);
                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }
        [Fact]
        public void Get_Total_Should_Have_Constant_Response_Time()
        {
            //Given a website with hundreds of activities recorded and another with millions
            var reportingHundredsService = new ReportingService(_bigRepository);
            var reportingMillionsService = new ReportingService(_biggerRepository);

            //When getting the total of a activity on each service
            var timeWithHundreds = checkTimeToGetTotal(reportingHundredsService);
            var timeWithMillions = checkTimeToGetTotal(reportingMillionsService);

            //Then
            timeWithHundreds.Should().BeCloseTo(timeWithMillions, 5, "time to get total is O(1) up to 5 milli");

            long checkTimeToGetTotal(ReportingService service)
            {
                var watch = new Stopwatch();
                watch.Start();
                var total = service.GetTotal("any");
                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }

        private void PrepareRepository(ActivityRepository repository, int activitiesToRegister)
        {
            RegisterSeveralActivities(repository, activitiesToRegister);
        }

        static Random _random = new();
        private ActivityRepository _bigRepository;
        private ActivityRepository _biggerRepository;

        private void RegisterSeveralActivities(ActivityRepository repository, int activitiesToRegister, string key = "any")
        {
            Parallel.For(0, activitiesToRegister, (ind, state) =>
            {
                var randomActivity = RandomActivity(key);
                repository.PushActivity(randomActivity);
            });
        }

        private static Models.Activity RandomActivity(string key = "any")
        {
            return new(
                Key: key,
                Value: _random.Next(2000)
            );
        }
    }
}