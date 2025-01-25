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
        private int jobExecutionStatus;
        private int revenueRecordCount;
        private int revenueRecordCountDelta;
        private double executionDuration;
        private decimal revenueSum;
        private readonly ILogger<MetricService> log;
        public double JobExecutionDuration
        {
            get
            {
                return executionDuration;
            }
            set
            {
                executionDuration = value;
            }
        }
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
            JobExecutionDuration = 0;
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

            meter.CreateObservableGauge(
                name: "revenue_job_execution_duration",
                unit: "seconds",
                observeValue: () => new Measurement<double>(JobExecutionDuration),
                description: "VIR Revenue job duration."
            );
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
