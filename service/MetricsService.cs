using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace FakturowniaService
{
    public class MetricsService
    {
        private readonly Meter meter;
        private readonly Histogram<double> productImportDuration;
        private readonly Histogram<double> clientImportDuration;
        private readonly Histogram<double> invoiceImportDuration;
        private readonly Histogram<double> paymentImportDuration;
        private readonly Histogram<double> jobExecutionDuration;
        private int jobExecutionStatus;
        private int arbevRecordCount;
        private int arbevRecordCountDiff;
        private readonly ILogger<MetricsService> log;
        public int JobExecutionStatus
        {
            get
            {
                return jobExecutionStatus;
            }
            set
            {
                jobExecutionStatus = value;
            }
        }

        public int ArbevRecordCount
        {
            get
            {
                return arbevRecordCount;
            }
            set
            {
                arbevRecordCount = value;
            }
        }

        public int ArbevRecordCountDiff
        {
            get
            {
                return arbevRecordCountDiff;
            }
            set
            {
                arbevRecordCountDiff = value;
            }
        }

        public MetricsService(IMeterFactory meterFactory, ILogger<MetricsService> logger)
        {
            JobExecutionStatus = 1;
            ArbevRecordCount = 1;
            ArbevRecordCountDiff = 1;

            log = logger;
            meter = meterFactory.Create("FakturService", "1.0.0");

            productImportDuration = meter.CreateHistogram<double>(
              name: "product_import_duration", unit: "seconds",
              description: "Product import duration in seconds.");

            clientImportDuration = meter.CreateHistogram<double>(
              name: "client_import_duration", unit: "seconds",
              description: "Client import duration in econds.");

            paymentImportDuration = meter.CreateHistogram<double>(
              name: "payment_import_duration", unit: "seconds",
              description: "Payment import duration in seconds.");

            invoiceImportDuration = meter.CreateHistogram<double>(
              name: "invoice_import_duration", unit: "seconds",
              description: "Invoice import duration in seconds.");

            jobExecutionDuration = meter.CreateHistogram<double>(
              name: "job_execution_duration", unit: "seconds",
              description: "Job execution duration in seconds.");

            meter.CreateObservableGauge(
                "job_execution_status",
                () => new Measurement<int>(JobExecutionStatus),
                "VIR Job Execution Status",
                "The result code of the latest MSSQL QAD-VIR refresh job execution (0 = Failed, 1 = Succeeded, 2 = Retry, 3 = Canceled)"
            );

            meter.CreateObservableGauge(
                "arbev_recordcount",
                () => new Measurement<int>(ArbevRecordCount),
                "VIR ARBEV record count",
                "How many records are currently in the ARBEV view."
            );

            meter.CreateObservableGauge(
                "arbev_recordcount_diff",
                () => new Measurement<int>(ArbevRecordCountDiff),
                "VIR ARBEV record count difference",
                "Current - previous record count in the ARBEV view"
            );
        }

        public void RecordJobExecutionDuration(double duration)
        {
            log.LogDebug($"RecordJobExecutionDuration {duration}");
            jobExecutionDuration.Record(duration);
        }

        public void RecordProductImportDuration(double duration)
        {
            productImportDuration.Record(duration);
        }

        public void RecordClientImportDuration(double duration)
        {
            clientImportDuration.Record(duration);
        }

        public void RecordInvoiceImportDuration(double duration)
        {
            invoiceImportDuration.Record(duration);
        }

        public void RecordPaymentImportDuration(double duration)
        {
            paymentImportDuration.Record(duration);
        }
    }
}
