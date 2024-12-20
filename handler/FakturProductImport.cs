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
    class FakturProductImport
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_PRODUCT_API_URL_TEMPLATE");
        public void ExecuteTask(object state)
        {
            List<string> productFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                productFiles = HTTP.DownloadAllProducts(apiUrlTemplate);

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
                            DB.DeleteAllRows("Fakturownia_Product", connection, transaction);

                            foreach (string file in productFiles)
                            {
                                log.Info($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var products = JsonConvert.DeserializeObject<List<Product>>(json, settings);

                                foreach (var product in products)
                                {
                                    DB.InsertProduct(product, connection, transaction);
                                }
                            }

                            stopwatch.Stop();
                            DB.InsertProductImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));

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
                if (productFiles != null && productFiles.Count > 0)
                {
                    log.Info("Cleaning up...");
                    File.DeleteFiles(productFiles);
                }
            }
        }
    }
}
