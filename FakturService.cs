using log4net;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;

namespace FakturowniaService
{
    public class FakturService : ServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Timer timer;
        private DateTime lastExecutionDate;

        public FakturService()
        {
            ServiceName = "Faktur Export Service";
            lastExecutionDate = DateTime.MinValue;
        }

        // Custom start method for running in console
        public void StartAsConsole(string[] args)
        {
            try
            {
                FakturProductImport fakturProductHandler = new FakturProductImport();
                fakturProductHandler.ExecuteTask(null);

                FakturClientImport fakturClientHandler = new FakturClientImport();
                fakturClientHandler.ExecuteTask(null);

                FakturInvoiceImport fakturInvoiceHandler = new FakturInvoiceImport();
                fakturInvoiceHandler.ExecuteTask(null);

                FakturPaymentImport fakturPaymentHandler = new FakturPaymentImport();
                fakturPaymentHandler.ExecuteTask(null);
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex}");
            }
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service OnStart called.");
            timer = new Timer(60000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
            //timer = new Timer(fakturInvoiceHandler.ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromDays(7));
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            // Check if the time is 3:00 AM and if the task has not been executed today
            if (now.Hour == 3 && now.Minute == 0 && lastExecutionDate.Date != now.Date)
            {
                log.Info("It is 3am. Start the Faktur import tasks.");

                FakturProductImport fakturProductHandler = new FakturProductImport();
                fakturProductHandler.ExecuteTask(null);

                FakturClientImport fakturClientHandler = new FakturClientImport();
                fakturClientHandler.ExecuteTask(null);

                FakturInvoiceImport fakturInvoiceHandler = new FakturInvoiceImport();
                fakturInvoiceHandler.ExecuteTask(null);

                FakturPaymentImport fakturPaymentHandler = new FakturPaymentImport();
                fakturPaymentHandler.ExecuteTask(null);

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
