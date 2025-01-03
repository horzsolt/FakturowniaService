using FakturowniaService.task;
using log4net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FakturowniaService
{
    class FakturInvoiceImport : ImportTask
    {
        private readonly MetricsService metricsService;
        private ILogger<FakturService> log;
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_INVOICE_API_URL_TEMPLATE");

        public FakturInvoiceImport(MetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public void ExecuteTask(ILogger<FakturService> logger)
        {
            log = logger;
            List<string> invoiceFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string dateFrom = "2023-01-01";
                string dateTo = DateTime.Today.ToString("yyyy-MM-dd");

                invoiceFiles = HTTP.DownloadAllInvoices(apiUrlTemplate, dateFrom, dateTo, log);

                string connectionString = $"Server={Environment.GetEnvironmentVariable("VIR_SQL_SERVER_NAME")};" +
                          $"Database={Environment.GetEnvironmentVariable("VIR_SQL_DATABASE")};" +
                          $"User Id={Environment.GetEnvironmentVariable("VIR_SQL_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("VIR_SQL_PASSWORD")};" +
                          "Connection Timeout=500;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            try
                            {
                                DB.DeleteAllRows("Fakturownia_InvoiceItem", connection, transaction);
                                DB.DeleteAllRows("Fakturownia_InvoiceHead", connection, transaction);
                            }
                            finally
                            {
                                DB.EnableForeignKeyCheck(connection, transaction, "Fakturownia_InvoiceItem", "FK_Fakturownia_InvoiceItem_InvoiceHead");
                            }

                            foreach (string file in invoiceFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var jsonContent = System.IO.File.ReadAllText(file);
                                var invoices = JsonConvert.DeserializeObject<List<Invoice>>(jsonContent, settings);

                                foreach (var invoice in invoices)
                                {
                                    DB.InsertInvoiceHeader(invoice, connection, transaction, log);

                                    foreach (var item in invoice.Positions)
                                    {
                                        DB.InsertInvoiceItem(item, connection, transaction);
                                    }
                                }
                            }

                            DB.InsertInvoiceImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordInvoiceImportDuration(stopwatch.Elapsed.TotalSeconds);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            log.LogError($"Error: {ex}");
                        }
                    }
                }
                
                log.LogInformation($"Elapsed Time: {stopwatch.Elapsed.Hours} hours, {stopwatch.Elapsed.Minutes} minutes, {stopwatch.Elapsed.Seconds} seconds");
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
            }
            finally
            {
                if (invoiceFiles != null && invoiceFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(invoiceFiles);
                }
            }
        }
    }
}
