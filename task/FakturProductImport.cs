using FakturowniaService.task;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FakturowniaService
{
    class FakturProductImport(MetricsService metricsService, ILogger<FakturPaymentImport> log) : ImportTask
    {
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_PRODUCT_API_URL_TEMPLATE");

        public void ExecuteTask()
        {
            List<string> productFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                productFiles = HTTP.DownloadAllProducts(apiUrlTemplate, log);

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
                            DB.DeleteAllRows("Fakturownia_Product", connection, transaction);

                            foreach (string file in productFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var products = JsonConvert.DeserializeObject<List<Product>>(json, settings);

                                foreach (var product in products)
                                {
                                    DB.InsertProduct(product, connection, transaction, log);
                                }
                            }

                            DB.InsertProductImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordProductImportDuration(stopwatch.Elapsed.TotalSeconds);
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
                if (productFiles != null && productFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(productFiles);
                }
            }
        }
    }
}
