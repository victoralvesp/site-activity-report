using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossOver.WebsiteActivity.HostedServices;
using CrossOver.WebsiteActivity.Repository;
using CrossOver.WebsiteActivity.Services;

namespace CrossOver.WebsiteActivity
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterActivityServices(this IServiceCollection services)
        {
            services.AddSingleton<IActivityRepository, ActivityRepository>();
            services.AddHostedService<JanitorHostedService>();
            services.AddScoped<IRecordingService, RecordingService>();
            services.AddScoped<IReportingService, ReportingService>();

            return services;
        }
    }
}