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
    class FakturClientImport : ImportTask
    {
        private readonly MetricsService metricsService;
        private ILogger<FakturService> log;
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_CLIENT_API_URL_TEMPLATE");

        public FakturClientImport(MetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public void ExecuteTask(ILogger<FakturService> logger)
        {
            log = logger;
            List<string> clientFiles = null;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                clientFiles = HTTP.DownloadAllClients(apiUrlTemplate, log);

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
                            DB.DeleteAllRows("Fakturownia_Client", connection, transaction);

                            foreach (string file in clientFiles)
                            {
                                log.LogInformation($"Processing file: {file}");

                                var settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new DecimalStringConverter() }
                                };

                                var json = System.IO.File.ReadAllText(file);
                                var clients = JsonConvert.DeserializeObject<List<Client>>(json, settings);

                                foreach (var client in clients)
                                {
                                    DB.InsertClient(client, connection, transaction, log);
                                }
                            }

                            DB.InsertClientImportLog(connection, transaction, Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                            transaction.Commit();

                            stopwatch.Stop();
                            metricsService.RecordClientImportDuration(stopwatch.Elapsed.TotalSeconds);
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
                if (clientFiles != null && clientFiles.Count > 0)
                {
                    log.LogInformation("Cleaning up...");
                    File.DeleteFiles(clientFiles);
                }
            }
        }
    }
}
