﻿using Microsoft.Extensions.Logging;
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
        private readonly Histogram<double> warehouseDocumentImportDuration;
        private readonly Histogram<double> warehouseImportDuration;

        private int jobExecutionStatus;
        private int revenueRecordCount;
        private int revenueRecordCountDelta;
        private double executionDuration;
        private decimal revenueSum;

        private int job2025ExecutionStatus;
        private int revenue2025RecordCount;
        private int revenue2025RecordCountDelta;
        private double execution2025Duration;
        private decimal revenue2025Sum;

        private int job2025_2ExecutionStatus;
        private double execution2025_2Duration;

        private int sqlClientCount;

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

        public double Job2025ExecutionDuration
        {
            get
            {
                return execution2025Duration;
            }
            set
            {
                execution2025Duration = value;
            }
        }
        public int Job2025ExecutionStatus
        {
            get
            {
                return job2025ExecutionStatus;
            }
            set
            {
                job2025ExecutionStatus = value;
            }
        }

        public double Job2025_2ExecutionDuration
        {
            get
            {
                return execution2025_2Duration;
            }
            set
            {
                execution2025_2Duration = value;
            }
        }
        public int Job2025_2ExecutionStatus
        {
            get
            {
                return job2025_2ExecutionStatus;
            }
            set
            {
                job2025_2ExecutionStatus = value;
            }
        }
        public int Revenue2025RecordCount
        {
            get
            {
                return revenue2025RecordCount;
            }
            set
            {
                revenue2025RecordCount = value;
            }
        }

        public int Revenue2025RecordCountDelta
        {
            get
            {
                return revenue2025RecordCountDelta;
            }
            set
            {
                revenue2025RecordCountDelta = value;
            }
        }

        public decimal Revenue2025Sum
        {
            get
            {
                return revenue2025Sum;
            }
            set
            {
                revenue2025Sum = value;
            }
        }

        public int SQLClientCount
        {
            get
            {
                return sqlClientCount;
            }
            set
            {
                sqlClientCount = value;
            }
        }

        public MetricService(IMeterFactory meterFactory, ILogger<MetricService> logger, string serviceName, string serviceVersion)
        {
            JobExecutionStatus = 1;
            RevenueRecordCount = 1;
            RevenueRecordCountDelta = 1;
            RevenueSum = 0;
            JobExecutionDuration = 0;

            Job2025ExecutionStatus = 1;
            Job2025_2ExecutionStatus = 1;
            Revenue2025RecordCount = 1;
            Revenue2025RecordCountDelta = 1;
            Revenue2025Sum = 0;
            Job2025ExecutionDuration = 0;

            SQLClientCount = 0;

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

            warehouseDocumentImportDuration = meter.CreateHistogram<double>(
              name: "faktur_warehouse_document_duration", unit: "seconds",
              description: "WarehouseDocument import duration in seconds.");

            warehouseImportDuration = meter.CreateHistogram<double>(
              name: "faktur_warehouse_duration", unit: "seconds",
              description: "Warehouse import duration in seconds.");

            meter.CreateObservableGauge(
                name: "revenue_job_execution_status",
                unit: "value",
                observeValue: () => new Measurement<int>(JobExecutionStatus),
                description:
                "The result code of the latest MSSQL QAD-VIR refresh job execution (0 = Failed, 1 = Succeeded, 2 = Retry, 3 = Canceled)"
            );

            meter.CreateObservableGauge(
                name: "revenue_job_record",
                unit: "value",
                observeValue: () => new Measurement<int>(RevenueRecordCount),
                description: "VIR Revenue record count."
            );

            meter.CreateObservableGauge(
                name: "revenue_job_record_delta",
                unit: "value",
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

            meter.CreateObservableGauge(
                name: "revenue2025_job_execution_status",
                unit: "value",
                observeValue: () => new Measurement<int>(Job2025_2ExecutionStatus),
                description:
                "The result code of the latest MSSQL QAD-VIR refresh job execution (0 = Failed, 1 = Succeeded, 2 = Retry, 3 = Canceled)"
            );

            meter.CreateObservableGauge(
                name: "revenue2025_job_record",
                unit: "value",
                observeValue: () => new Measurement<int>(Revenue2025RecordCount),
                description: "VIR Revenue2025 record count."
            );

            meter.CreateObservableGauge(
                name: "revenue2025_job_record_delta",
                unit: "value",
                observeValue: () => new Measurement<int>(Revenue2025RecordCountDelta),
                description: "VIR Revenue2025 record count delta."
            );

            meter.CreateObservableGauge(
                name: "revenue2025_job_revenue_sum",
                unit: "money",
                observeValue: () => new Measurement<decimal>(Revenue2025Sum),
                description: "VIR Revenue2025 summary value."
            );

            meter.CreateObservableGauge(
                name: "revenue2025_job_execution_duration",
                unit: "seconds",
                observeValue: () => new Measurement<double>(Job2025ExecutionDuration),
                description: "VIR Revenue2025 job duration."
            );

            meter.CreateObservableGauge(
                name: "sql_client_count",
                unit: "value",
                observeValue: () => new Measurement<int>(SQLClientCount),
                description: "Number of connected SQL clients."
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

        public void RecordWarehouseDocumentImportDuration(double duration)
        {
            warehouseDocumentImportDuration.Record(duration);
        }

        public void RecordWarehouseImportDuration(double duration)
        {
            warehouseImportDuration.Record(duration);
        }
    }
}
