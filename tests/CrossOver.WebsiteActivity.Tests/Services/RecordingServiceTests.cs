using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class RecordingServiceTests : ActivityTests
    {
        
        [Fact]
        public void Register_Should_Have_Constant_Response_TimeAsync()
        {
            //Given a website with hundreds of activities recorded and another with thousands
            var activityServiceWithHundreds = new RecordingService(_bigRepository);
            var activityServiceWithThousands = new RecordingService(_biggerRepository);
            //When registering a new activity on each site
            var timeWithHundreds = checkTimeToRegister(activityServiceWithHundreds);
            var timeWithThousands = checkTimeToRegister(activityServiceWithThousands);

            //Then
            timeWithHundreds.Should().BeCloseTo(timeWithThousands, 5, "time to register is O(1) up to 5 milli");

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
        public async Task Register_Should_Eventually_Add_Activity_Value_To_RepositoryAsync()
        {
            //Given a website with a well defined set of activities recorded
            var repository = new ActivityRepository();
            var key = TESTING_KEY;
            
            var reportingService = new RecordingService(repository);

            //When register an activity
            var activity = RandomActivity(key);
            reportingService.Register(activity.Key, activity.Value, activity.RegisterDate);
            await Task.Delay(100);

            //Then
            repository.GetActivities(key).Should().ContainEquivalentOf(activity);
        }

        [Fact]
        public void Should_Throw_For_Empty_Key()
        {
            // Given any repository
            var service = new RecordingService(_biggerRepository);

            // When requested the total for a empty key
            // Them a exception should be thrown
            service.Invoking((serv) => serv.Register(string.Empty, 10)).Should().Throw<ArgumentException>();
        }
    }
}