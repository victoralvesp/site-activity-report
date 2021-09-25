using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Models;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class PurgeActivityTests : ActivityServiceTests
    {
        [Theory]
        [MemberData(nameof(ExampleActivities))]
        public void Should_Purge_Older_Activities(Activity[] activities, int expectedTotal)
        {
            //Given
        
            //When
        
            //Then
        }


        
    }
}