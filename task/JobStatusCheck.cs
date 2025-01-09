using Microsoft.Extensions.Logging;
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace FakturowniaService.task
{
    class JobStatusCheck(MetricsService metricsService, ILogger<FakturPaymentImport> log) : ETLTask
    {
        public void ExecuteTask()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                string connectionString = $"Server={Environment.GetEnvironmentVariable("VIR_SQL_SERVER_NAME")};" +
                          $"Database={Environment.GetEnvironmentVariable("VIR_SQL_DATABASE")};" +
                          $"User Id={Environment.GetEnvironmentVariable("VIR_SQL_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("VIR_SQL_PASSWORD")};" +
                          "Connection Timeout=500;Trust Server Certificate=true";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var query = @"SELECT 
                            j.name AS JobName,
                            h.step_id AS StepID,
                            h.step_name AS StepName,
                            h.run_status AS RunStatus, -- 0 = Failed, 1 = Succeeded, 2 = Retry, 3 = Canceled
                            h.run_date AS RunDate,
                            h.run_time AS RunTime,
                            h.run_duration AS RunDuration,
                            h.sql_message_id AS SqlMessageID,
                            h.sql_severity AS SqlSeverity,
                            h.message AS Message
                        FROM 
                            msdb.dbo.sysjobs AS j
                        INNER JOIN 
                            msdb.dbo.sysjobhistory AS h
                            ON j.job_id = h.job_id
                        WHERE 
                            j.name = 'QAD_VIR_frissites'
                            AND h.instance_id = (
                                SELECT MAX(instance_id) 
                                FROM msdb.dbo.sysjobhistory 
                                WHERE job_id = j.job_id
                            )
                        ORDER BY 
                            h.step_id;";

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

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string status = row["RunStatus"] is DBNull ? "-1" : row.Field<int>("RunStatus").ToString();
                                string executionDate = row["RunDate"] is DBNull ? String.Empty : row.Field<int>("RunDate").ToString();
                                string executionTime = row["RunTime"] is DBNull ? String.Empty : row.Field<int>("RunTime").ToString();
                                string duration = row["RunDuration"] is DBNull ? String.Empty : row.Field<int>("RunDuration").ToString();
                                string message = row["Message"] is DBNull ? String.Empty : row.Field<string>("Message");

                                DateTime date = DateTime.ParseExact(executionDate, "yyyyMMdd", null);
                                DateTime time = DateTime.ParseExact(executionTime, "HHmmss", null);

                                string secondsPart = duration.Substring(duration.Length - 2, 2);
                                string minutesPart = duration.Substring(0, duration.Length - 2);
                                int minutes = string.IsNullOrEmpty(minutesPart) ? 0 : int.Parse(minutesPart);
                                int seconds = int.Parse(secondsPart);
                                string durationFormatted = new TimeSpan(0, minutes, seconds).ToString(@"hh\:mm\:ss");

                                string executedAt = new DateTime(
                                    date.Year, date.Month, date.Day,
                                    time.Hour, time.Minute, time.Second).ToString("yyyy-MM-dd HH:mm:ss");

                                if (status == "1")
                                {
                                    log.LogInformation($"QAD_VIR_frissites ran at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                }
                                else
                                {
                                    log.LogError($"QAD_VIR_frissites ran at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                }

                                metricsService.RecordJobStatusResult(int.Parse(status));
                            }
                        }
                    }

                    stopwatch.Stop();
                    metricsService.RecordProductImportDuration(stopwatch.Elapsed.TotalSeconds);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error: {ex}");
            }
        }
    }
}
