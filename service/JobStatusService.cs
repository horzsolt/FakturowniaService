using FakturowniaService.task;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace FakturowniaService
{
    public class JobStatusService : BackgroundService
    {
        private DateTime lastExecutionDate;
        private readonly ILogger<HostMonitorService> log;
        private List<ETLTask> tasks;

        public JobStatusService(ILogger<HostMonitorService> logger, IEnumerable<ETLTask> taskList)
        {
            lastExecutionDate = DateTime.MinValue;
            log = logger;
            tasks = taskList.ToList();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.AddHours(1).Date.AddHours(now.AddHours(1).Hour).AddMinutes(10);
                var delay = nextRun - now;

                var readableDelay = $"{delay.Hours} hours, {delay.Minutes} minutes, and {delay.Seconds} seconds";
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
            log.LogInformation($"It is {DateTime.Now:yyyy-MM-dd HH:mm:ss}. Start the JobStatus tasks as a DailyTask.");
            foreach (var task in tasks)
            {
                task.ExecuteTask();
            }

            await Task.CompletedTask;
        }
    }
}
