using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FakturowniaService
{
    public static class FileUtil
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void DeleteInvoiceFiles(List<string> invoiceFiles)
        {
            foreach (var filePath in invoiceFiles)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        log.Debug($"File deleted: {filePath}");
                    }
                    else
                    {
                        log.Debug($"File not found: {filePath}");
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
