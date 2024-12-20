using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FakturowniaService
{
    public static class File
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void DeleteFiles(List<string> files)
        {
            foreach (var filePath in files)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
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
