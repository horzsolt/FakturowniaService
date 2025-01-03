using FakturowniaService.task;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace FakturowniaService
{
    public class FakturService : ServiceBase
    {
        private Timer timer;
        private DateTime lastExecutionDate;
        private readonly ILogger<FakturService> log;
        private List<ImportTask> tasks;

        public FakturService(ILogger<FakturService> logger, IEnumerable<ImportTask> taskList)
        {
            lastExecutionDate = DateTime.MinValue;
            log = logger;
            tasks = taskList.ToList();
        }

        // Custom start method for running in console
        public void StartAsConsole(string[] args)
        {
            try
            {
                foreach (var task in tasks)
                {
                    task.ExecuteTask(log);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
            }
        }

        protected override void OnStart(string[] args)
        {
            log.LogInformation("Service OnStart called.");
            timer = new Timer(60000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            // Check if the time is 3:00 AM and if the task has not been executed today
            if (now.Hour == 3 && now.Minute == 0 && lastExecutionDate.Date != now.Date)
            {
                log.LogInformation("It is 3am. Start the Faktur import tasks.");
                foreach (var task in tasks)
                {
                    task.ExecuteTask(log);
                }

                lastExecutionDate = now.Date;
            }
        }

        protected override void OnStop()
        {
            timer?.Stop();
            timer?.Dispose();
        }
    }
}
