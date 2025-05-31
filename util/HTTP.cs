using FakturowniaService.task;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace FakturowniaService
{
    public static class HTTP
    {
        public static List<string> DownloadAllInvoices(string apiUrlTemplate, string dateFrom, string dateTo, ILogger<ETLTask> log)
        {
            string tempDirectory = Path.GetTempPath();
            int maxRetries = 5;
            int page = 1;
            List<string> invoiceFiles = new List<string>();

            log.LogInformation($"Start invoice download between {dateFrom} and {dateTo}");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                while (true)
                {
                    string apiUrl = string.Format(apiUrlTemplate, dateFrom, dateTo, page);
                    string filePath = Path.Combine(tempDirectory, $"invoices_page_{page}.json");

                    log.LogDebug($"Downloading page {page} to {filePath}...");
                    log.LogDebug($"API URL: {apiUrl}");

                    bool success = false;
                    for (int attempt = 0; attempt < maxRetries; attempt++)
                    {
                        try
                        {
                            HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                            response.EnsureSuccessStatusCode();

                            string content = response.Content.ReadAsStringAsync().Result;

                            if (IsEmptyJson(content))
                            {
                                log.LogInformation($"Page {page} is empty. Stopping iteration.");
                                return invoiceFiles;
                            }

                            System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                            invoiceFiles.Add(filePath);

                            success = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            log.LogError($"Error: {ex}, retry: {attempt + 1} failed for page {page}: {ex}");
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

        public static List<string> DownloadJSON(string apiUrlTemplate, ILogger<ETLTask> log, string entityName, bool singlePage = false)
        {
            string tempDirectory = Path.GetTempPath();
            int maxRetries = 5;
            int page = 1;
            List<string> jsonFiles = new List<string>();

            log.LogInformation($"Start {entityName} download.");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                while (true)
                {
                    string apiUrl = string.Format(apiUrlTemplate, page);
                    string filePath = Path.Combine(tempDirectory, $"{entityName}_page_{page}.json");

                    log.LogDebug($"Downloading page {page} to {filePath}...");
                    log.LogDebug($"API URL: {apiUrl}");

                    bool success = false;
                    for (int attempt = 0; attempt < maxRetries; attempt++)
                    {
                        try
                        {
                            HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                            response.EnsureSuccessStatusCode();

                            string content = response.Content.ReadAsStringAsync().Result;

                            if (IsEmptyJson(content))
                            {
                                log.LogInformation($"Page {page} is empty. Stopping iteration.");
                                return jsonFiles;
                            }

                            System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
                            jsonFiles.Add(filePath);

                            success = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            log.LogError($"Error: {ex}, retry: {attempt + 1} failed for page {page}: {ex}");
                            Thread.Sleep(2000);
                        }
                    }

                    if (!success)
                    {
                        throw new Exception($"Failed to download page {page} after {maxRetries} attempts.");
                    }

                    if (singlePage)
                    {
                        log.LogInformation($"Single page download completed for {entityName}.");
                        return jsonFiles;
                    }

                    page++;
                    Thread.Sleep(1000);
                }
            }
        }
        
        private static bool IsEmptyJson(string json)
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
