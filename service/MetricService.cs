using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace FakturowniaService
{
    public class MetricService
    {
        private readonly Meter meter;
        private readonly Histogram<double> productImportDuration;
        private readonly Histogram<double> clientImportDuration;
        private readonly Histogram<double> invoiceImportDuration;
        private readonly Histogram<double> paymentImportDuration;
        private readonly Histogram<double> jobExecutionDuration;
        private int jobExecutionStatus;
        private int revenueRecordCount;
        private int revenueRecordCountDelta;
        private decimal revenueSum;
        private readonly ILogger<MetricService> log;
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

        public int RevenueRecordCount
        {
            get
            {
                return revenueRecordCount;
            }
            set
            {
                revenueRecordCount = value;
            }
        }

        public int RevenueRecordCountDelta
        {
            get
            {
                return revenueRecordCountDelta;
            }
            set
            {
                revenueRecordCountDelta = value;
            }
        }

        public decimal RevenueSum
        {
            get
            {
                return revenueSum;
            }
            set
            {
                revenueSum = value;
            }
        }

        public MetricService(IMeterFactory meterFactory, ILogger<MetricService> logger, string serviceName, string serviceVersion)
        {
            JobExecutionStatus = 1;
            RevenueRecordCount = 1;
            RevenueRecordCountDelta = 1;
            RevenueSum = 0;
            log = logger;
            meter = meterFactory.Create(serviceName, serviceVersion);

            productImportDuration = meter.CreateHistogram<double>(
              name: "faktur_product_import_duration", unit: "seconds",
              description: "Product import duration in seconds.");

            clientImportDuration = meter.CreateHistogram<double>(
              name: "faktur_client_import_duration", unit: "seconds",
              description: "Client import duration in econds.");

            paymentImportDuration = meter.CreateHistogram<double>(
              name: "faktur_payment_import_duration", unit: "seconds",
              description: "Payment import duration in seconds.");

            invoiceImportDuration = meter.CreateHistogram<double>(
              name: "faktur_invoice_import_duration", unit: "seconds",
              description: "Invoice import duration in seconds.");

            jobExecutionDuration = meter.CreateHistogram<double>(
              name: "revenue_job_execution_duration", unit: "seconds",
              description: "Job execution duration in seconds.");

            meter.CreateObservableGauge(
                name: "revenue_job_execution_status",
                unit: "value",
                observeValue: () => new Measurement<int>(JobExecutionStatus),
                description:
                "The result code of the latest MSSQL QAD-VIR refresh job execution (0 = Failed, 1 = Succeeded, 2 = Retry, 3 = Canceled)"
            );

            meter.CreateObservableGauge(
                name: "revenue_job_record",
                unit: "count",
                observeValue: () => new Measurement<int>(RevenueRecordCount),
                description: "VIR Revenue record count."
            );

            meter.CreateObservableGauge(
                name: "revenue_job_record_delta",
                unit: "count",
                observeValue : () => new Measurement<int>(RevenueRecordCountDelta),
                description: "VIR Revenue record count delta."
            );

            meter.CreateObservableGauge(
                name: "revenue_job_revenue_sum",
                unit: "money",
                observeValue: () => new Measurement<decimal>(RevenueSum),
                description: "VIR Revenue summary value."
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
