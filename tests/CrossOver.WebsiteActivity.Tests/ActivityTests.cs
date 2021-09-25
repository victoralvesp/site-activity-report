using System.Diagnostics;
using CrossOver.WebsiteActivity.HostedServices;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests
{
    public class ActivityTests
    {
        protected const string TESTING_KEY = "any";
        public ActivityTests()
        {
            _bigRepository = new ActivityRepository();
            _biggerRepository = new ActivityRepository();
            PrepareRepository(_bigRepository, 250);
            PrepareRepository(_biggerRepository, 2500);
            var services = new ServiceCollection();
            services.RegisterActivityServices();
            _provider = services.BuildServiceProvider();
        }

        [Theory]
        [MemberData(nameof(ExampleActivities))]
        public async Task Services_Should_Give_Example_ValuesAsync(string key, Models.Activity[] activities, int expectedTotal)
        {
            // Given a set of activities that happened in the site
            // And all activities have been registered
            var recordingService = GetRecordingService();
            RegisterSeveralActivities(recordingService, activities);

            var purgeService = GetPurgeService();
            await purgeService.StartAsync(default);
            await Task.Delay(TimeSpan.FromSeconds(1));

            //When we get the total value for activities
            var reporting = GetReportingService();
            var total = reporting.GetTotal(key);

            //Then we should get the expected value
            total.Should().Be(expectedTotal);

        }

        private ReportingService GetReportingService() => _provider.GetRequiredService<ReportingService>();
        private RecordingService GetRecordingService() => _provider.GetRequiredService<RecordingService>();
        private IHostedService GetPurgeService() => _provider.GetService<IEnumerable<IHostedService>>()!.First(hs => hs is JanitorHostedService);

        protected void PrepareRepository(ActivityRepository repository, int activitiesToRegister)
        {
            RegisterSeveralActivities(repository, activitiesToRegister);
        }

        static Random _random = new();
        protected ActivityRepository _bigRepository;
        protected ActivityRepository _biggerRepository;
        private ServiceProvider _provider;

        protected void RegisterSeveralActivities(ActivityRepository repository, string key = TESTING_KEY, params Models.Activity[] activitiesToRegister)
        {
            Parallel.ForEach(activitiesToRegister, (activity, state) =>
            {
                repository.PushActivity(activity);
            });
        }
        protected void RegisterSeveralActivities(RecordingService service, params Models.Activity[] activitiesToRegister)
        {
            Parallel.ForEach(activitiesToRegister, (activity, state) =>
            {
                service.Register(activity.Key, activity.Value, activity.RegisterDate);
            });
        }
        private void RegisterSeveralActivities(ActivityRepository repository, int activitiesToRegister, string key = TESTING_KEY)
        {
            Parallel.For(0, activitiesToRegister, (ind, state) =>
            {
                var randomActivity = RandomActivity(key);
                repository.PushActivity(randomActivity);
            });
        }

        protected static Models.Activity RandomActivity(string key = TESTING_KEY)
        {
            return new(
                Key: key,
                Value: _random.Next(2000)
            );
        }

        protected static IEnumerable<object[]> ExampleActivities()
        {
            var key = TESTING_KEY;
            var testingExample = new[] {
                pastActivity(16, TimeSpan.FromMinutes(781)),
                pastActivity(5, TimeSpan.FromMinutes(510)),
                pastActivity(32, TimeSpan.FromMinutes(50)),
                pastActivity(4, TimeSpan.FromMinutes(3)),
            };
            yield return new object[] { key, testingExample, 41 };

            testingExample = testingExample.Append(pastActivity(40, TimeSpan.FromHours(20))).ToArray();
            yield return new object[] { key, testingExample, 41 };
            testingExample = testingExample.Append(pastActivity(20, TimeSpan.FromHours(1))).ToArray();
            yield return new object[] { key, testingExample, 61 };
            testingExample = testingExample.Append(pastActivity(13, TimeSpan.FromHours(16))).ToArray();
            yield return new object[] { key, testingExample, 61 };
            testingExample = testingExample.Append(pastActivity(11, TimeSpan.FromHours(.5))).ToArray();
            yield return new object[] { key, testingExample, 72 };

            Models.Activity pastActivity(int value, TimeSpan pastTime)
            => new(key, value)
            {
                RegisterDate = DateTime.UtcNow - pastTime
            };
        }
    }
}