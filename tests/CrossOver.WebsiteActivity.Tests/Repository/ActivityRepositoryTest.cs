using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Repository;
using FluentAssertions;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Repository
{
    public class ActivityRepositoryTest : ActivityTests
    {
        [Fact]
        public void Should_Send_Event_After_Push()
        {
            //Given a activity
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            int eventCount = 0;
            repository.OnAdded += (_, _) => eventCount++;

            //When we push the activity to the repository
            repository.PushActivity(activity);

            //Then
            eventCount.Should().Be(1);

        }
        [Fact]
        public void Should_Send_Event_After_Purge()
        {
            //Given a activity that is on the repository
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            RegisterSeveralActivities(repository, activitiesToRegister: activity);
            int eventCount = 0;
            repository.OnRemoved += (_, _) => eventCount++;

            //When we purge the activity from the repository
            repository.PurgeActivity(activity);

            //Then
            eventCount.Should().Be(1);

        }
        [Fact]
        public void Should_Not_Send_Event_After_Purge_For_Non_Existing_Activity()
        {
            //Given a activity that is not in the repository
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            int eventCount = 0;
            repository.OnRemoved += (_, _) => eventCount++;

            //When we purge the activity from the repository
            repository.PurgeActivity(activity);

            //Then
            eventCount.Should().Be(0);

        }
        [Fact]
        public void Should_Have_Activity_After_Push()
        {
            //Given a activity
            var activity = RandomActivity();
            var repository = new ActivityRepository();

            //When we push the activity
            repository.PushActivity(activity);

            //Then
            var key = activity.Key;
            repository.GetActivities(key).Should().ContainEquivalentOf(activity);
        }
        [Fact]
        public void Should_Not_Have_Activity_After_Purge()
        {
            //Given a activity in the repository
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            repository.PushActivity(activity);

            //When we purge the activity
            repository.PurgeActivity(activity);

            //Then
            var key = activity.Key;
            repository.GetActivities(key).Should().NotContainEquivalentOf(activity);
        }
    }
}