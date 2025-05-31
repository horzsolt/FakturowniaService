using FakturowniaService.entity;
using FakturowniaService.util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FakturowniaService.task
{

    [FakturTask]
    class FakturWarehouseImport(MetricService metricsService, ILogger<FakturWarehouseImport> log) : ETLTask
    {
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_WAREHOUSES_API_URL_TEMPLATE");
        public void ExecuteTask()
        {
            List<string> warehouseFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                warehouseFiles = HTTP.DownloadJSON(apiUrlTemplate, log, "warehouses", true);

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
                            DB.DeleteAllRows("Fakturownia_Warehouse", connection, transaction);

                            foreach (string file in warehouseFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter(), new IntegerStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var warehouses = JsonConvert.DeserializeObject<List<Warehouse>>(json, settings);

                                foreach (var warehouse in warehouses)
                                {
                                    DB.InsertWarehouse(warehouse, connection, transaction, log);
                                }
                            }

                            DB.InsertWarehouseImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordWarehouseImportDuration(stopwatch.Elapsed.TotalSeconds);
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
                if (warehouseFiles != null && warehouseFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(warehouseFiles, log);
                }
            }
        }
    }
}


