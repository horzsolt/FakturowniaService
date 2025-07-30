using Microsoft.Extensions.Logging;
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Management;

namespace FakturowniaService.task
{
    [HostCheckTask]
    class HostCheck(MetricService metricsService, ILogger<HostCheck> log) : ETLTask
    {
        public void ExecuteTask()
        {
            CheckSQLClients(metricsService, log);
            CheckDiskSpace(metricsService, log);
        }

        private static void CheckDiskSpace(MetricService metricsService, ILogger<HostCheck> log)
        {
            try
            {
                DriveInfo cDrive = new DriveInfo("C");
                long freeSpaceBytes = cDrive.AvailableFreeSpace;
                long totalSpaceBytes = cDrive.TotalSize;

                log.LogDebug($"C: drive - Free space: {freeSpaceBytes / (1024 * 1024)} MB / Total: {totalSpaceBytes / (1024 * 1024)} MB");
                metricsService.Diskfreebytes = freeSpaceBytes / (1024 * 1024);
                log.LogDebug($"Available space metric {freeSpaceBytes / (1024 * 1024)}");

                long totalAllocatedSizeMB = 0;
                long totalCurrentUsageMB = 0;

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PageFileUsage"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        totalAllocatedSizeMB += Convert.ToInt64(obj["AllocatedBaseSize"]);
                        totalCurrentUsageMB += Convert.ToInt64(obj["CurrentUsage"]);
                    }
                }

                log.LogDebug($"Total pagefile size: {totalAllocatedSizeMB} MB");
                log.LogDebug($"Total pagefile current usage: {totalCurrentUsageMB} MB");

                metricsService.Pagefilesizebytes = totalAllocatedSizeMB;
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
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
