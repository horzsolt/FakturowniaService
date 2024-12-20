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
    class FakturPaymentImport
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_PAYMENT_API_URL_TEMPLATE");
        public void ExecuteTask(object state)
        {
            List<string> paymentFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                paymentFiles = HTTP.DownloadAllPayments(apiUrlTemplate);

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
                            DB.DeleteAllRows("Fakturownia_Payment", connection, transaction);
                            foreach (string file in paymentFiles)
                            {
                                log.Info($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var payments = JsonConvert.DeserializeObject<List<Payment>>(json);

                                foreach (var payment in payments)
                                {
                                    DB.InsertPayment(payment, connection, transaction);
                                }
                            }

                            stopwatch.Stop();
                            DB.InsertPaymentImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));

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
                if (paymentFiles != null && paymentFiles.Count > 0)
                {
                    log.Info("Cleaning up...");
                    File.DeleteFiles(paymentFiles);
                }
            }
        }
    }
}
