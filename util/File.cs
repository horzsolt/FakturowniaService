using FakturowniaService.task;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FakturowniaService
{
    public static class File
    {
        public static void DeleteFiles(List<string> files, ILogger<ImportTask> log)
        {
            foreach (var filePath in files)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        log.LogDebug($"File deleted: {filePath}");
                    }
                    else
                    {
                        log.LogDebug($"File not found: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file {filePath}: {ex.Message}");
                }
            }
        }
    }
}
