using System.Collections.Generic;
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

        public MetricsService(IMeterFactory meterFactory)
        {
            meter = meterFactory.Create("FakturService", "1.0.0");

            productImportDuration = meter.CreateHistogram<double>(
              name: "product_import_duration", unit: "ms",
              description: "Product import duration in milliseconds.");

            clientImportDuration = meter.CreateHistogram<double>(
              name: "client_import_duration", unit: "ms",
              description: "Client import duration in milliseconds.");

            paymentImportDuration = meter.CreateHistogram<double>(
              name: "payment_import_duration", unit: "ms",
              description: "Payment import duration in milliseconds.");

            invoiceImportDuration = meter.CreateHistogram<double>(
              name: "invoice_import_duration", unit: "ms",
              description: "Invoice import duration in milliseconds.");
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
