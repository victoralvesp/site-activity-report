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
        public void Should_Have_Event_After_Push()
        {
            //Given a activity
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            int eventCount = 0;
            repository.OnAdded += (_,_) => eventCount++;

            //When we push the activity to the repository
            repository.PushActivity(activity);

            //Then
            eventCount.Should().Be(1);

        }
        [Fact]
        public void Should_Not_Have_Event_After_Purge_For_Non_Existing_Activity()
        {
            //Given a activity
            var activity = RandomActivity();
            var repository = new ActivityRepository();
            int eventCount = 0;
            repository.OnRemoved += (_,_) => eventCount++;

            //When we push the activity to the repository
            repository.PurgeActivity(activity);

            //Then
            eventCount.Should().Be(0);

        }
        [Fact]
        public void Should_Send_Event_When_Adding_Activity()
        {
        //Given
        
        //When
        
        //Then
        }
    }
}