using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace FakturowniaService.task
{
    [HostCheckTask]
    class HostCheck(MetricService metricsService, ILogger<HostCheck> log) : ETLTask
    {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetDiskFreeSpaceEx(
            string lpDirectoryName,
            out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);

        private static void LogMountedPathSpace(
            string path,
            string metricName,
            MetricService metricsService,
            ILogger log)
        {
            if (!Directory.Exists(path))
            {
                log.LogWarning($"Mounted path does not exist: {path}");
                return;
            }

            bool success = GetDiskFreeSpaceEx(
                path,
                out ulong freeBytesAvailable,
                out ulong totalBytes,
                out ulong totalFreeBytes);

            if (!success)
            {
                log.LogError($"GetDiskFreeSpaceEx failed for: {path}");
                return;
            }

            long freeSpaceMB = (long)(freeBytesAvailable / 1024 / 1024);
            long totalSpaceMB = (long)(totalBytes / 1024 / 1024);

            log.LogDebug(
                $"{path} - Free: {freeSpaceMB} MB / Total: {totalSpaceMB} MB");

            metricsService.UpdateDriveFreeSpace(metricName, freeSpaceMB);
        }
        public void ExecuteTask()
        {
            CheckSQLClients(metricsService, log);
            CheckDiskSpace(metricsService, log);
            CheckCpuLoad(metricsService, log);
            CheckMemory(metricsService, log);
        }

        private static void CheckMemory(MetricService metricsService, ILogger<HostCheck> log)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    long totalMemoryKB = Convert.ToInt64(obj["TotalVisibleMemorySize"]);
                    long freeMemoryKB = Convert.ToInt64(obj["FreePhysicalMemory"]);
                    long usedMemoryKB = totalMemoryKB - freeMemoryKB;

                    long totalMemoryMB = totalMemoryKB / 1024;
                    long freeMemoryMB = freeMemoryKB / 1024;
                    long usedMemoryMB = usedMemoryKB / 1024;

                    log.LogDebug($"Memory - Total: {totalMemoryMB} MB, Used: {usedMemoryMB} MB, Free: {freeMemoryMB} MB");

                    metricsService.MemoryTotalMB = totalMemoryMB;
                    metricsService.MemoryFreeMB = freeMemoryMB;
                    metricsService.MemoryUsedMB = usedMemoryMB;
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error checking memory: {ex}");
            }
        }

        private static void CheckCpuLoad(MetricService metricsService, ILogger<HostCheck> log)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor");
                double totalLoad = 0;
                int coreCount = 0;

                foreach (ManagementObject obj in searcher.Get())
                {
                    totalLoad += Convert.ToDouble(obj["LoadPercentage"]);
                    coreCount++;
                }

                double averageCpuLoad = coreCount > 0 ? totalLoad / coreCount : 0;

                log.LogDebug($"CPU load: {averageCpuLoad:F1}% across {coreCount} processor(s)");

                metricsService.CpuLoadPercent = averageCpuLoad;
            }
            catch (Exception ex)
            {
                log.LogError($"Error checking CPU load: {ex}");
            }
        }
        private static void CheckDiskSpace(MetricService metricsService, ILogger<HostCheck> log)
        {
            try
            {
                long totalFreeSpaceMB = 0;

                //
                // 1. Check normal fixed drives
                //
                var localDrives = DriveInfo.GetDrives()
                    .Where(d => d.DriveType == DriveType.Fixed && d.IsReady)
                    .ToList();

                foreach (var drive in localDrives)
                {
                    long freeSpaceMB = drive.AvailableFreeSpace / (1024 * 1024);
                    long totalSpaceMB = drive.TotalSize / (1024 * 1024);

                    // "C:\" -> "C"
                    string driveLetter = drive.Name
                        .TrimEnd('\\', '/')
                        .TrimEnd(':');

                    log.LogDebug($"{drive.Name} - Free: {freeSpaceMB} MB / Total: {totalSpaceMB} MB");

                    metricsService.UpdateDriveFreeSpace(driveLetter, freeSpaceMB);

                    totalFreeSpaceMB += freeSpaceMB;
                }

                //
                // 2. Check mounted host folders
                //
                LogMountedPathSpace(
                    @"C:\mnt\m",
                    "mnt_m",
                    metricsService,
                    log);

                LogMountedPathSpace(
                    @"C:\mnt\r",
                    "mnt_r",
                    metricsService,
                    log);

                //
                // 4. Pagefile
                //
                long totalAllocatedSizeMB = 0;
                long totalCurrentUsageMB = 0;

                using (var searcher =
                       new ManagementObjectSearcher(
                           "SELECT * FROM Win32_PageFileUsage"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        totalAllocatedSizeMB += Convert.ToInt64(obj["AllocatedBaseSize"]);
                        totalCurrentUsageMB += Convert.ToInt64(obj["CurrentUsage"]);
                    }
                }

                log.LogDebug(
                    $"Pagefile size: {totalAllocatedSizeMB} MB, usage: {totalCurrentUsageMB} MB");

                metricsService.Pagefilesizebytes = totalAllocatedSizeMB;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while checking disk space");
            }
        }
        private static void CheckSQLClients(MetricService metricsService, ILogger<HostCheck> log)
        {
            try
            {
                string connectionString = $"Server={Environment.GetEnvironmentVariable("VIR_SQL_SERVER_NAME")};" +
                          $"Database={Environment.GetEnvironmentVariable("VIR_SQL_DATABASE")};" +
                          $"User Id={Environment.GetEnvironmentVariable("VIR_SQL_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("VIR_SQL_PASSWORD")};" +
                          "Connection Timeout=500;Trust Server Certificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"SELECT session_id, login_name, host_name, program_name, status
                                    FROM sys.dm_exec_sessions
                                    WHERE is_user_process = 1 AND
                                    program_name != 'Microsoft SQL Server VSS Writer' AND
                                    program_name not like 'SQLAgent%';";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 500;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            if (dataTable.Rows.Count == 0)
                            {
                                log.LogError("No records found.");
                                return;
                            }

                            try
                            {
                                int counter = 0;
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    string session_id = row["session_id"] is DBNull ? String.Empty : row["session_id"].ToString();
                                    string login_name = row["login_name"] is DBNull ? String.Empty : row["login_name"].ToString();
                                    string host_name = row["host_name"] is DBNull ? String.Empty : row["host_name"].ToString();
                                    string program_name = row["program_name"] is DBNull ? string.Empty : row["program_name"].ToString();
                                    string status = row["status"] is DBNull ? String.Empty : row["status"].ToString();

                                    log.LogDebug($"Connected clients, session: {session_id}, login: {login_name} host: {host_name}, program_name: {program_name}, status: {status}");
                                    ++counter;
                                }

                                metricsService.SQLClientCount = counter;
                                log.LogDebug($"Client connection count: {counter}");
                            }
                            catch (Exception ex)
                            {
                                log.LogError($"Error: {ex}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
            }
        }
    }
}
