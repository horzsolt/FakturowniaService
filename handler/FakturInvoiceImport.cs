using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FakturowniaService
{
    class FakturInvoiceImport
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_INVOICE_API_URL_TEMPLATE");
        public void ExecuteTask(object state)
        {
            List<string> invoiceFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string dateFrom = "2023-01-01";
                string dateTo = DateTime.Today.ToString("yyyy-MM-dd");

                invoiceFiles = HTTP.DownloadAllInvoices(apiUrlTemplate, dateFrom, dateTo);

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
                                log.Info($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var jsonContent = System.IO.File.ReadAllText(file);
                                var invoices = JsonConvert.DeserializeObject<List<Invoice>>(jsonContent, settings);

                                foreach (var invoice in invoices)
                                {
                                    DB.InsertInvoiceHeader(invoice, connection, transaction);

                                    foreach (var item in invoice.Positions)
                                    {
                                        DB.InsertInvoiceItem(item, connection, transaction);
                                    }
                                }
                            }

                            stopwatch.Stop();
                            DB.InsertInvoiceImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            log.Error($"Error: {ex}");
                        }
                    }
                }
                
                log.Info($"Elapsed Time: {stopwatch.Elapsed.Hours} hours, {stopwatch.Elapsed.Minutes} minutes, {stopwatch.Elapsed.Seconds} seconds");
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex}");
            }
            finally
            {
                if (invoiceFiles != null && invoiceFiles.Count > 0)
                {
                    log.Info("Cleaning up...");
                    File.DeleteFiles(invoiceFiles);
                }
            }
        }
    }
}
