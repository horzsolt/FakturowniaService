using FakturowniaService.task;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace FakturowniaService
{
    //TODO: Each backgroundservice has to derive from a parent, that implements the task execution logic
    public class JobStatusService : BackgroundService
    {
        private DateTime lastExecutionDate;
        private readonly ILogger<JobStatusService> log;
        private List<ETLTask> tasks;

        public JobStatusService(ILogger<JobStatusService> logger, IEnumerable<ETLTask> taskList)
        {
            lastExecutionDate = DateTime.MinValue;
            log = logger;
            tasks = taskList.ToList();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                /*
                // Calculate the next 6 AM
                var now = DateTime.Now;
                var nextRun = now.Date.AddDays(now.Hour >= 6 ? 1 : 0).AddHours(6);
                var delay = nextRun - now;
                */

                var now = DateTime.Now;

                var runTimes = new[]
                {
                    now.Date.AddHours(6),
                    now.Date.AddHours(13).AddMinutes(30)
                };

                DateTime nextRun;
                if (now < runTimes[0])
                {
                    nextRun = runTimes[0]; // Next run is 6:00 AM today
                }
                else if (now < runTimes[1])
                {
                    nextRun = runTimes[1]; // Next run is 1:30 PM today
                }
                else
                {
                    nextRun = runTimes[0].AddDays(1); // Both times passed; schedule 6:00 AM tomorrow
                }

                var delay = nextRun - now;

                var readableDelay = $"{delay.Days} days, {delay.Hours} hours, {delay.Minutes} minutes, and {delay.Seconds} seconds";
                log.LogInformation($"Next task scheduled to run in: {readableDelay}");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                    await DailyTask(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "An error occurred while scheduling the task.");
                }
            }
        }

        public void StartAsConsole(string[] args)
        {
            try
            {
                foreach (var task in tasks)
                {
                    task.ExecuteTask();
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
            }
        }

        private async Task DailyTask(CancellationToken stoppingToken)
        {
            log.LogInformation($"It is {DateTime.Now:yyyy-MM-dd HH:mm:ss}. Start the JobStatus task as a DailyTask.");
            foreach (var task in tasks)
            {
                task.ExecuteTask();
            }

            await Task.CompletedTask;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            // Check if the time is 6:00 AM and if the task has not been executed today
            if (now.Hour == 6 && now.Minute == 0 && lastExecutionDate.Date != now.Date)
            {
                log.LogInformation("It is 6am. Start the JobStatus tasks as an OnTimedEvent.");
                foreach (var task in tasks)
                {
                    task.ExecuteTask();
                }

                lastExecutionDate = now.Date;
            }
        }
    }
}
