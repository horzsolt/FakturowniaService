using log4net;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace FakturowniaService
{
    public class FakturService : ServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Timer timer;

        public FakturService()
        {
            ServiceName = "Faktur Export Service";
        }

        // Custom start method for running in console
        public void StartAsConsole(string[] args)
        {
            try
            {
                FakturInvoiceExportHandler fakturInvoiceHandler = new FakturInvoiceExportHandler();
                fakturInvoiceHandler.ExecuteTask(null);
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex}");
            }
        }

        protected override void OnStart(string[] args)
        {
            log.Debug("Service OnStart called.");
            FakturInvoiceExportHandler fakturInvoiceHandler = new FakturInvoiceExportHandler();
            timer = new Timer(fakturInvoiceHandler.ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromDays(7));
        }

        protected override void OnStop()
        {
            timer?.Dispose();
        }
    }
}
