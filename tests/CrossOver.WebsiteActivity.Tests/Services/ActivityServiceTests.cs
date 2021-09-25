using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class ActivityServiceTests
    {
        [Fact]
        public void Register_Should_Have_Constant_Response_TimeAsync()
        {
            //Given a website with hundreds of activities recorded and another with millions
            var bigRepository = new ActivityRepository();
            var biggerRepository = new ActivityRepository();
            PrepareRepository(bigRepository, out var activityServiceWithHundreds, 250);
            PrepareRepository(biggerRepository, out var activityServiceWithMillions, 2500000);

            //When registering a new activity on each site
            var timeWithHundreds = checkTimeToRegister(activityServiceWithHundreds);
            var timeWithMillions = checkTimeToRegister(activityServiceWithMillions);

            //Then
            timeWithHundreds.Should().BeCloseTo(timeWithMillions, 5, "time to register is O(1) up to 5 milli");

            long checkTimeToRegister(ActivityRecordingService service)
            {
                var watch = new Stopwatch();
                watch.Start();
                service.Register("any", RandomActivity().Value);
                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }
        [Fact]
        public void Get_Total_Should_Have_Constant_Response_Time()
        {
            //Given a website with hundreds of activities recorded and another with millions
            var bigRepository = new ActivityRepository();
            var biggerRepository = new ActivityRepository();
            var reportingHundredsService = new ReportingService(bigRepository);
            var reportingMillionsService = new ReportingService(biggerRepository);
            PrepareRepository(bigRepository, out var activityServiceWithHundreds, 250);
            PrepareRepository(biggerRepository, out var activityServiceWithMillions, 2500000);

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

        private void PrepareRepository(ActivityRepository repository, out ActivityRecordingService activityServiceWithHundreds, int activitiesToRegister)
        {
            activityServiceWithHundreds = new ActivityRecordingService(repository);
            RegisterSeveralActivities(activityServiceWithHundreds, activitiesToRegister);
        }

        static Random _random = new();
        private void RegisterSeveralActivities(ActivityRecordingService service, int activitiesToRegister, string key = "any")
        {
            Parallel.For(0, activitiesToRegister, async (ind, state) =>
            {
                var randomActivity = RandomActivity();
                service.Register(key, randomActivity.Value);
            });
        }

        private static ActivityValue RandomActivity()
        {
            return new ActivityValue()
            {
                Value = _random.Next(2000)
            };
        }
    }
}