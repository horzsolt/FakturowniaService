using FakturowniaService;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Microsoft.Extensions.DependencyInjection;
using FakturowniaService.task;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using FakturowniaService.util;
using System.Configuration.Install;

namespace FakturExport
{
    internal class Program
    {
        private static readonly String serviceName = "FakturService";
        private static readonly String serviceVersion = "1.0.0";

        private static void ConfigureServices(ServiceCollection services)
        {

            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .AddSource(serviceName)
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion))
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                })
                .WithMetrics(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion))
                        .AddMeter(serviceName)
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                });

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                        serviceName: serviceName,
                        serviceVersion: serviceVersion));
                    options.AddOtlpExporter();
                });

                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string log4NetConfigFilePath = Path.Combine(exeDirectory, "log4net.config");
                builder.AddLog4Net(log4NetConfigFilePath);
            });

            services.AddMetrics();

            // Register the services
            services.AddSingleton<MetricsService>();

            services.AddTransient<ImportTask, FakturProductImport>();
            services.AddTransient<ImportTask, FakturClientImport>();
            services.AddTransient<ImportTask, FakturInvoiceImport>();
            services.AddTransient<ImportTask, FakturPaymentImport>();
            services.AddSingleton<FakturService>();

        }

        static void Main(string[] args)
        {

            var services = new ServiceCollection();

            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogDebug("Framework: " + FRWK.GetEnvironmentVersion() + " " + FRWK.GetTargetFrameworkName() + " " + FRWK.GetFrameworkDescription());

            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        // Run as a console app for debugging

                        using (logger.BeginScope("Console mode"))
                        {
                            logger.LogInformation("Starting the service in interactive mode.");
                            var service = serviceProvider.GetRequiredService<FakturService>();
                            service.StartAsConsole(null);
                        }
                        break;
                }
            }
            else
            {
                logger.LogInformation("Starting the service as a Windows Service...");
                ServiceBase[] servicesToRun = new ServiceBase[] { serviceProvider.GetRequiredService<FakturService>() };
                ServiceBase.Run(servicesToRun);
            }

            serviceProvider.Dispose();
        }
    }
}
