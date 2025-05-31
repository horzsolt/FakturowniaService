using FakturowniaService.task;
using FakturowniaService.util;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FakturowniaService
{
    [FakturTask]
    class FakturWarehouseDocImport(MetricService metricsService, ILogger<FakturWarehouseDocImport> log) : ETLTask
    {
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_WAREHOUSEDOC_API_URL_TEMPLATE");

        public void ExecuteTask()
        {
            List<string> warehouseDocFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                warehouseDocFiles = HTTP.DownloadJSON(apiUrlTemplate, log, "warehousedoc");

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
                            DB.DeleteAllRows("Fakturownia_WarehouseDocument", connection, transaction);

                            foreach (string file in warehouseDocFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter(), new IntegerStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var warehouseDocs = JsonConvert.DeserializeObject<List<WarehouseDocument>>(json, settings);

                                foreach (var warehouseDoc in warehouseDocs)
                                {
                                    DB.InsertWarehouseDocument(warehouseDoc, connection, transaction, log);
                                }
                            }

                            DB.InsertWarehouseDocumentImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordWarehouseDocumentImportDuration(stopwatch.Elapsed.TotalSeconds);
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
                if (warehouseDocFiles != null && warehouseDocFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(warehouseDocFiles, log);
                }
            }
        }
    }
}
