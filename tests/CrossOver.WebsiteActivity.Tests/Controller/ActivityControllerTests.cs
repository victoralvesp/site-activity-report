using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.Controllers;
using CrossOver.WebsiteActivity.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CrossOver.WebsiteActivity.Tests.Controller
{
    public class ActivityControllerTests
    {
        private const string TESTING_KEY = "any";
        private const int TESTING_VALUE = 10;
        private IReportingService _reportingService;
        private IRecordingService _recordingService;
        private ServiceProvider _provider;

        public ActivityControllerTests()
        {
            var serviceCollection = new ServiceCollection();
            _reportingService = Mock.Of<IReportingService>(MockBehavior.Loose);
            _recordingService = Mock.Of<IRecordingService>(MockBehavior.Loose);
            serviceCollection.AddScoped<IReportingService>((prov) => _reportingService);
            serviceCollection.AddScoped<IRecordingService>((prov) => _recordingService);
            serviceCollection.AddScoped<ActivityController>();
            serviceCollection.AddLogging();
            _provider = serviceCollection.BuildServiceProvider();
        }



        [Fact]
        public void Register_New_Activity_Should_Call_Recording_Service()
        {
            //Given
            var mock = Mock.Get(_recordingService);
            var controller = GetActivityController();

            //When we register a new activity
            controller.RegisterNewActivity(TESTING_KEY, new() { Value = TESTING_VALUE });

            //Then the service should have been called
            mock.Verify(rec => rec.Register(TESTING_KEY, TESTING_VALUE, It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }
        [Fact]
        public void Register_New_Activity_Should_Return_500_When_Recording_Service_Throws()
        {
            //Given that the recording service is throwing
            var mock = Mock.Get(_recordingService);
            mock.Setup(rec => rec.Register(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception());
            var controller = GetActivityController();

            //When we register a new activity
            var result = controller.RegisterNewActivity(TESTING_KEY, new() { Value = TESTING_VALUE });

            //Then should return 500
            result.Should().BeEquivalentTo(new StatusCodeResult(500), options => options.Including(o => o.StatusCode));
        }
        [Fact]
        public void Get_Total_Should_Call_Reporting_Service()
        {
            //Given
            var mock = Mock.Get(_reportingService);
            var controller = GetActivityController();

            //When we register a new activity
            controller.GetActivityTotal(TESTING_KEY);

            //Then the service should have been called
            mock.Verify(rep => rep.GetTotal(TESTING_KEY), Times.AtLeastOnce());
        }
        [Fact]
        public void Get_Total_Should_Return_500_When_Reporting_Service_Throws()
        {
            //Given that the reporting service is throwing
            var mock = Mock.Get(_reportingService);
            mock.Setup(rep => rep.GetTotal(It.IsAny<string>()))
                .Throws(new Exception());
            var controller = GetActivityController();

            //When we register a new activity
            var result = controller.GetActivityTotal(TESTING_KEY);

            //Then should return 500
            result.Should().BeEquivalentTo(new StatusCodeResult(500), options => options.Including(o => o.StatusCode));
        }
        

        private ActivityController GetActivityController()
        {
            return _provider.GetRequiredService<ActivityController>();
        }
    }
}