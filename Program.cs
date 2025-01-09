using FakturowniaService;
using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Microsoft.Extensions.DependencyInjection;
using FakturowniaService.task;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using FakturowniaService.util;
using System.Configuration.Install;
using Microsoft.Extensions.Hosting;

namespace FakturExport
{
    internal class Program
    {
        private static readonly String serviceName = "FakturService";
        private static readonly String serviceVersion = "1.0.0";

        private static void ConfigureServices(HostApplicationBuilder appBuilder)
        {
            appBuilder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "VIR Faktur ETL service.";
            });

            appBuilder.Services.AddOpenTelemetry()
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
                        //.AddConsoleExporter();
                });

            appBuilder.Services.AddLogging(builder =>
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

            //appBuilder.Services.AddMetrics();

            appBuilder.Services.AddSingleton<MetricsService>();

            appBuilder.Services.AddTransient<ETLTask, FakturProductImport>();
            appBuilder.Services.AddTransient<ETLTask, FakturClientImport>();
            appBuilder.Services.AddTransient<ETLTask, FakturInvoiceImport>();
            appBuilder.Services.AddTransient<ETLTask, FakturPaymentImport>();
            appBuilder.Services.AddSingleton<FakturService>();
            appBuilder.Services.AddHostedService<FakturService>();
            appBuilder.Services.AddHostedService<JobStatusService>();
        }

        static void Main(string[] args)
        {

            //var services = new ServiceCollection();
            HostApplicationBuilder appBuilder = Host.CreateApplicationBuilder(args);

            ConfigureServices(appBuilder);

            var serviceProvider = appBuilder.Services.BuildServiceProvider();
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
                            var fakturService = serviceProvider.GetRequiredService<FakturService>();
                            fakturService.StartAsConsole(null);

                            /*var jobStatusService = serviceProvider.GetRequiredService<JobStatusService>();
                            jobStatusService.StartAsConsole(null);
                            */
                        }
                        break;
                }
            }
            else
            {
                logger.LogInformation("Starting the service as a Windows Service...");
                //ServiceBase[] servicesToRun = new ServiceBase[] { serviceProvider.GetRequiredService<FakturService>() };
                //ServiceBase.Run(servicesToRun);
                IHost host = appBuilder.Build();
                host.Run();
            }

            serviceProvider.Dispose();
        }
    }
}
