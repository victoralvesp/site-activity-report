using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.HostedServices;
using CrossOver.WebsiteActivity.Models;
using CrossOver.WebsiteActivity.Repository;
using FluentAssertions;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class PurgeActivityTests : ActivityTests
    {
        [Theory]
        [MemberData(nameof(ExampleActivities))]
        public async Task Should_Purge_Older_ActivitiesAsync(string key, Activity[] activities, int expectedTotal)
        {
            //Given a set of activities
            var repository = new ActivityRepository();
            RegisterSeveralActivities(repository, activitiesToRegister: activities);
            var janitor = new JanitorHostedService(repository);

            //When we start the janitor service
            await janitor.StartAsync(default);
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            //Then we should not have activities older than expected
            
            repository.GetActivities(key).Select(act => act.RegisterDate).Should().NotContain(date => DateTime.UtcNow - date >= janitor.ActivityHoldingTime);
        }


        
    }
}