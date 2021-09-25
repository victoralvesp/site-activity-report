using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Services
{
    public class ReportingServiceTests : ActivityTests
    {
        [Fact]
        public void Get_Total_Should_Have_Constant_Response_Time()
        {
            //Given a website with hundreds of activities recorded and another with thousands
            var reportingHundredsService = new ReportingService(_bigRepository);
            var reportingThousandsService = new ReportingService(_biggerRepository);

            //When getting the total of a activity on each service
            var timeWithHundreds = checkTimeToGetTotal(reportingHundredsService);
            var timeWithThousands = checkTimeToGetTotal(reportingThousandsService);

            //Then
            timeWithHundreds.Should().BeCloseTo(timeWithThousands, 5, "time to get total is O(1) up to 5 milli");

            long checkTimeToGetTotal(ReportingService service)
            {
                var watch = new Stopwatch();
                watch.Start();
                var total = service.GetTotal(TESTING_KEY);
                watch.Stop();
                return watch.ElapsedMilliseconds;
            }
        }
        [Fact]
        public void Should_Throw_For_Empty_Key()
        {
            // Given any repository
            var reportingService = new ReportingService(_biggerRepository);

            // When requested the total for a empty key
            // Them a exception should be thrown
            reportingService.Invoking((serv) => serv.GetTotal(string.Empty)).Should().Throw<ArgumentException>();
        }
        

        [Fact]
        public void Total_Should_Give_The_Total_For_A_Key()
        {
            //Given a website with a well defined set of activities recorded
            var repository = new ActivityRepository();
            var key = TESTING_KEY;
            var activities = new[]
            {
                new Models.Activity(key, 1),
                new Models.Activity(key, 1),
                new Models.Activity(key, 2),
                new Models.Activity(key, 3),
                new Models.Activity(key, 5),
                new Models.Activity(key, 8),
                new Models.Activity(key, 13),
                new Models.Activity(key, 21),
            };
            RegisterSeveralActivities(repository, activitiesToRegister: activities);
            var reportingService = new ReportingService(repository);

            //When we ask for the total of a activity
            var total = reportingService.GetTotal(key);

            //Then
            total.Should().Be(54);
        }

    }
}