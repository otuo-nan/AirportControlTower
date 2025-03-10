using AirportControlTower.API.Application.Jobs;
using Quartz;


namespace AirportControlTower.API.Infrastructure.Configurations
{
    public static class QuartzConfiguration
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey(nameof(AutoParkAirlinesJob));

                q.AddJob<AutoParkAirlinesJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{nameof(AutoParkAirlinesJob)}-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(1)
                        .RepeatForever())
                );

                var weatherUpdateJobKey = new JobKey(nameof(WeatherUpdateJob));

                q.AddJob<WeatherUpdateJob>(opts => opts.WithIdentity(weatherUpdateJobKey));

                q.AddTrigger(opts => opts
                    .ForJob(weatherUpdateJobKey)
                    .WithIdentity($"{nameof(WeatherUpdateJob)}-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(5)
                        //.WithIntervalInSeconds(60)
                        .RepeatForever())
                );
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
