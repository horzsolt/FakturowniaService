using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace FakturowniaService
{
    class FakturInvoiceExportHandler
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string apiUrlTemplate = Environment.GetEnvironmentVariable("VIR_FAKTUR_API_URL_TEMPLATE");
        public void ExecuteTask(object state)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string dateFrom = "2023-01-01";
                string dateTo = DateTime.Today.ToString("yyyy-MM-dd");
                DownloadAllInvoices(dateFrom, dateTo);

                stopwatch.Stop();
                log.Info($"Elapsed Time: {stopwatch.Elapsed.Hours} hours, {stopwatch.Elapsed.Minutes} minutes, {stopwatch.Elapsed.Seconds} seconds");
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex}");
            }
        }

        private void DownloadAllInvoices(string dateFrom, string dateTo)
        {
            string tempDirectory = Path.GetTempPath();
            int maxRetries = 5;
            int page = 1;

            log.Info($"Start invoice download between {dateFrom} and {dateTo}");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                while (true)
                {
                    string apiUrl = string.Format(apiUrlTemplate, dateFrom, dateTo, page);
                    string filePath = Path.Combine(tempDirectory, $"invoices_page_{page}.json");

                    log.Debug($"Downloading page {page} to {filePath}...");
                    log.Debug($"API URL: {apiUrl}");

                    bool success = false;
                    for (int attempt = 0; attempt < maxRetries; attempt++)
                    {
                        try
                        {
                            HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                            response.EnsureSuccessStatusCode();

                            string content = response.Content.ReadAsStringAsync().Result;

                            // Check if the content is empty JSON
                            if (IsEmptyJson(content))
                            {
                                log.Info($"Page {page} is empty. Stopping iteration.");
                                return;
                            }

                            File.WriteAllText(filePath, content, Encoding.UTF8);

                            success = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Error: {ex}, retry: {attempt + 1} failed for page {page}: {ex}");
                            Thread.Sleep(2000);
                        }
                    }

                    if (!success)
                    {
                        throw new Exception($"Failed to download page {page} after {maxRetries} attempts.");
                    }

                    page++;
                    Thread.Sleep(1000);
                }
            }
        }

        private bool IsEmptyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return true;

            try
            {
                var doc = JsonDocument.Parse(json);

                // Check if it's an empty array
                if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() == 0)
                    return true;

                // Check if it's an empty object
                if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.EnumerateObject().MoveNext() == false)
                    return true;

                return false; // JSON is not empty
            }
            catch (JsonException)
            {
                return false; // Invalid JSON
            }
        }
    }
}
