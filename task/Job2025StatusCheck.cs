using FakturowniaService.util;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace FakturowniaService.task
{
    [JobStatusTask]
    class Job2025StatusCheck(MetricService metricsService, ILogger<Job2025StatusCheck> log) : ETLTask
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
                            j.name LIKE 'QAD_VIR_2025_ejszakai_frissites%'
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
                                metricsService.Job2025ExecutionStatus = 0;
                                log.LogError("No records found.");
                                return;
                            }

                            try
                            {

                                int _status = 1;
                                TimeSpan _duration = TimeSpan.Zero;

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    string status = row["RunStatus"] is DBNull ? "-1" : row.Field<int>("RunStatus").ToString();
                                    string executionDate = row["RunDate"] is DBNull ? String.Empty : row.Field<int>("RunDate").ToString();
                                    string executionTime = row["RunTime"] is DBNull ? String.Empty : row.Field<int>("RunTime").ToString();
                                    string duration = row["RunDuration"] is DBNull ? String.Empty : row.Field<int>("RunDuration").ToString();
                                    string message = row["Message"] is DBNull ? String.Empty : row.Field<string>("Message");

                                    if (executionTime.Length != 6) executionTime = "0" + executionTime;

                                    log.LogDebug($"Last job execution status {status}, started at {executionDate} {executionTime}, duration: {duration}");

                                    DateTime date = DateTime.ParseExact(executionDate, "yyyyMMdd", null);
                                    DateTime time = DateTime.ParseExact(executionTime, "HHmmss", null);

                                    _duration += Time.GetDurationFromString(duration);
                                    string durationFormatted = _duration.ToString(@"hh\:mm\:ss");

                                    string executedAt = new DateTime(
                                        date.Year, date.Month, date.Day,
                                        time.Hour, time.Minute, time.Second).ToString("yyyy-MM-dd HH:mm:ss");

                                    if (status == "1")
                                    {
                                        log.LogInformation($"QAD_VIR_2025_ejszakai_frissites ran at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                    }
                                    else
                                    {
                                        log.LogError($"QAD_VIR_2025_ejszakai_frissites failed at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                    }

                                    if (_status == 1)
                                    {
                                        _status = (int.Parse(status));
                                    }
                                }

                                metricsService.Job2025ExecutionStatus = _status;
                                metricsService.Job2025ExecutionDuration = _duration.TotalSeconds;
                            }
                            catch (Exception ex)
                            {
                                metricsService.Job2025ExecutionStatus = 0;
                                log.LogError($"Error: {ex}");
                            }
                        }
                    }

                    var query2 = @"SELECT 
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
                            j.name = 'QAD_VIR_2025_ejszakai_frissites_2'
                            AND h.instance_id = (
                                SELECT MAX(instance_id) 
                                FROM msdb.dbo.sysjobhistory 
                                WHERE job_id = j.job_id
                            )
                        ORDER BY 
                            h.step_id;";

                    using (SqlCommand command = new SqlCommand(query2, connection))
                    {
                        command.CommandTimeout = 500;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            if (dataTable.Rows.Count == 0)
                            {
                                metricsService.Job2025_2ExecutionStatus = 0;
                                log.LogError("No records found.");
                                return;
                            }

                            try
                            {

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    string status = row["RunStatus"] is DBNull ? "-1" : row.Field<int>("RunStatus").ToString();
                                    string executionDate = row["RunDate"] is DBNull ? String.Empty : row.Field<int>("RunDate").ToString();
                                    string executionTime = row["RunTime"] is DBNull ? String.Empty : row.Field<int>("RunTime").ToString();
                                    string duration = row["RunDuration"] is DBNull ? String.Empty : row.Field<int>("RunDuration").ToString();
                                    string message = row["Message"] is DBNull ? String.Empty : row.Field<string>("Message");

                                    // Normalize time to HHmmss
                                    executionTime = executionTime.PadLeft(6, '0');

                                    log.LogDebug(
                                        $"Last job_2 execution status {status}, started at {executionDate} {executionTime}, duration: {duration}");

                                    // Parse combined datetime
                                    DateTime executionDateTime = DateTime.ParseExact(
                                        executionDate + executionTime,
                                        "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture);

                                    // Duration
                                    TimeSpan jobDuration = Time.GetDurationFromString(duration);
                                    string durationFormatted = jobDuration.ToString(@"hh\:mm\:ss");

                                    DateTime executedAtDateTime = DateTime.ParseExact(
                                        executionDate + executionTime,
                                        "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture);

                                    string executedAt = executedAtDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                                    if (status == "1")
                                    {
                                        log.LogInformation($"QAD_VIR_2025_ejszakai_frissites_2_frissites ran at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                    }
                                    else
                                    {
                                        log.LogError($"QAD_VIR_2025_2_frissites ran at {executedAt}, status {status}, duration {durationFormatted}, {message}");
                                    }

                                    metricsService.Job2025_2ExecutionStatus = (int.Parse(status));
                                    metricsService.Job2025_2ExecutionDuration = jobDuration.TotalSeconds;
                                }
                            }
                            catch (Exception ex)
                            {
                                metricsService.Job2025_2ExecutionStatus = 0;
                                log.LogError($"Error: {ex}");
                            }
                        }
                    }

                    query = @"
                    SELECT TOP 2 [record_count], sum_arbevetel 
                    FROM [vir].[dbo].[t_qad_arbevetel_import_log_2025]
                    ORDER BY [date] DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = 500;

                        List<(int RecordCount, decimal SumArbevetel)> results = new List<(int, decimal)>();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add((reader.GetInt32(0), reader.GetDecimal(1)));
                            }
                        }

                        int latestRecordCount = results.Count > 0 ? results[0].RecordCount : 0;
                        int previousRecordCount = results.Count > 1 ? results[1].RecordCount : 0;
                        decimal latestArbevSum = results.Count > 0 ? results[0].SumArbevetel : 0;

                        metricsService.Revenue2025RecordCount = latestRecordCount;
                        metricsService.Revenue2025RecordCountDelta = latestRecordCount - previousRecordCount;
                        metricsService.Revenue2025Sum = latestArbevSum;

                        log.LogDebug($"Recorded metrics: RevenueRecordCount {latestRecordCount}, RevenueRecordCountDelta {latestRecordCount - previousRecordCount}, RevenueSum {latestArbevSum}");
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
