using FakturowniaService.task;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FakturowniaService
{
    class FakturPaymentImport(MetricsService metricsService, ILogger<FakturPaymentImport> log) : ETLTask
    {
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_PAYMENT_API_URL_TEMPLATE");
        public void ExecuteTask()
        {
            List<string> paymentFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                paymentFiles = HTTP.DownloadAllPayments(apiUrlTemplate, log);

                string connectionString = $"Server={Environment.GetEnvironmentVariable("VIR_SQL_SERVER_NAME")};" +
                          $"Database={Environment.GetEnvironmentVariable("VIR_SQL_DATABASE")};" +
                          $"User Id={Environment.GetEnvironmentVariable("VIR_SQL_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("VIR_SQL_PASSWORD")};" +
                          "Connection Timeout=500;Trust Server Certificate=true";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DB.DeleteAllRows("Fakturownia_Payment", connection, transaction);
                            foreach (string file in paymentFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var payments = JsonConvert.DeserializeObject<List<Payment>>(json);

                                foreach (var payment in payments)
                                {
                                    DB.InsertPayment(payment, connection, transaction, log);
                                }
                            }

                            DB.InsertPaymentImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordPaymentImportDuration(stopwatch.Elapsed.TotalSeconds);
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
                if (paymentFiles != null && paymentFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(paymentFiles, log);
                }
            }
        }
    }
}
