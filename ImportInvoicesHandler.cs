using log4net;
using System;
using System.Data.SqlClient;
using System.Reflection;

namespace FakturowniaService
{
    public class ImportInvoicesHandler
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ImportInvoices(string content)
        {
            string connectionString = $"Server={Environment.GetEnvironmentVariable("VIR_SQL_SERVER_NAME")};" +
                          $"Database={Environment.GetEnvironmentVariable("VIR_SQL_DATABASE")};" +
                          $"User Id={Environment.GetEnvironmentVariable("VIR_SQL_USER")};" +
                          $"Password={Environment.GetEnvironmentVariable("VIR_SQL_PASSWORD")};" +
                          "Connection Timeout=500;";

            log.Debug($"Connection string: {connectionString}");


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string query = "TRUNCATE TABLE Fakturownia_InvoiceHead";
                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    query = "TRUNCATE TABLE Fakturownia_InvoiceItem";
                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine("Transaction committed.");
                }
                catch (Exception ex)
                {
                    log.Error($"Error: {ex}");
                    transaction.Rollback();
                    log.Debug("Transaction rolled back.");
                }
            }
        }
    }
}
